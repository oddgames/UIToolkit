using UnityEngine;
using System.Collections;


public class UIContinuousButton : UIButton
{
	public delegate void UIContinousButtonDelegate( UIButton sender );
	public event UIContinousButtonDelegate onTouchIsDown;
	public event UIContinousButtonDelegate onActivationStarted;
	public event UIContinousButtonDelegate onActivationEnded;
	
	public bool onlyFireStartAndEndEvents = false;
	
	
	public static new UIContinuousButton create( string filename, string highlightedFilename, int xPos, int yPos )
	{
		return create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos );
	}

	
	public static new UIContinuousButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos )
	{
		return create( manager, filename, highlightedFilename, xPos, yPos, 1 );
	}
	
	
	public static new UIContinuousButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIContinuousButton( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}


	public UIContinuousButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( manager, frame, depth, uvFrame, highlightedUVframe )
	{
	}


	// UITouchWrapper handlers
	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		base.onTouchBegan( touch, touchPos );

		if( onlyFireStartAndEndEvents && onActivationStarted != null )
			onActivationStarted( this );
	}

	
	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		// dont fire this continously if we were asked to only fire start and end
		if( !onlyFireStartAndEndEvents && onTouchIsDown != null )
			onTouchIsDown( this );
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		base.onTouchEnded( touch, touchPos, touchWasInsideTouchFrame );
		
		if( onlyFireStartAndEndEvents && onActivationEnded != null )
			onActivationEnded( this );
	}
}
