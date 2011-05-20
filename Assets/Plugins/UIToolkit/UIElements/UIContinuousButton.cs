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


	// Touch handlers
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		base.onTouchBegan( touch, touchPos );

		if( onlyFireStartAndEndEvents && onActivationStarted != null )
			onActivationStarted( this );
	}

	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		// dont fire this continously if we were asked to only fire start and end
		if( !onlyFireStartAndEndEvents && onTouchIsDown != null )
			onTouchIsDown( this );
	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		base.onTouchEnded( touch, touchPos, touchWasInsideTouchFrame );
		
		if( onlyFireStartAndEndEvents && onActivationEnded != null )
			onActivationEnded( this );
	}
}
