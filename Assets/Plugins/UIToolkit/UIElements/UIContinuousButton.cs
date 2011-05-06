using UnityEngine;
using System.Collections;


public class UIContinuousButton : UIButton
{
	public delegate void UIContinousButtonDelegate( UIButton sender );
	public event UIContinousButtonDelegate onTouchIsDown;
	public event UIContinousButtonDelegate onTouchDown;
	public event UIContinousButtonDelegate onTouchUp;
	
	public bool onlyFireStartAndEndEvents = false;
	

	public static new UIContinuousButton create( string filename, string highlightedFilename, int xPos, int yPos, int depth = 1 )
	{
		// grab the texture details for the normal state
		var normalTI = UI.instance.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.size.x, normalTI.size.y );
		
		// get the highlighted state
		var highlightedTI = UI.instance.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIContinuousButton( frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}


	public UIContinuousButton( Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame, highlightedUVframe )
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

		if( onlyFireStartAndEndEvents && onTouchDown != null )
			onTouchDown( this );
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
		
		if( onlyFireStartAndEndEvents && onTouchUp != null )
			onTouchUp( this );
	}
}
