using UnityEngine;
using System;
using System.Collections;


public class UIZoomButton : UIButton
{
	private UIAnimation _zoomInAnimation;
	private UIAnimation _zoomOutAnimation;
	
	
	public float animationDuration
	{
		set
		{
			_zoomOutAnimation.setDuration( value );
			_zoomInAnimation.setDuration( value );
		}
	}
	
	
	public Vector3 animationTargetScale
	{
		set
		{
			_zoomInAnimation.setTarget( value );
		}
	}
	

	#region Constructors/Destructor
	
	public static new UIZoomButton create( string filename, string highlightedFilename, int xPos, int yPos, string stringData = "" )
	{
		return UIZoomButton.create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos, stringData );
	}
	
	
	public static new UIZoomButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, string stringData = "" )
	{
		return UIZoomButton.create( manager, filename, highlightedFilename, xPos, yPos, 1, stringData );
	}

	
	public static new UIZoomButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth, string stringData = "" )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIZoomButton( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect, stringData );
	}


	public UIZoomButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe, string stringData = "" ):base( manager, frame, depth, uvFrame, highlightedUVframe, stringData )
	{
		centerize();
		_zoomInAnimation = new UIAnimation( this, 0.3f, UIAnimationProperty.Scale, new Vector3( 1, 1, 1 ), new Vector3( 1.3f, 1.3f, 1.3f ), Easing.Quartic.easeInOut );
		_zoomOutAnimation = new UIAnimation( this, 0.3f, UIAnimationProperty.Scale, new Vector3( 1.3f, 1.3f, 1.3f ), new Vector3( 1, 1, 1 ), Easing.Quartic.easeInOut );
	}

	#endregion;



	// Touch handlers
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		base.onTouchBegan( touch, touchPos );
		
		_zoomInAnimation.restartStartToCurrent();
		UI.instance.StartCoroutine( _zoomInAnimation.animate() );
	}



#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		base.onTouchEnded( touch, touchPos, touchWasInsideTouchFrame );
		
		_zoomInAnimation.stop();
		_zoomOutAnimation.restartStartToCurrent();
		UI.instance.StartCoroutine( _zoomOutAnimation.animate() );
	}
	
}
