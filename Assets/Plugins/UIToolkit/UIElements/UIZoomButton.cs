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
	
	public static new UIZoomButton create( string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIZoomButton.create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos );
	}
	
	
	public static new UIZoomButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIZoomButton.create( manager, filename, highlightedFilename, xPos, yPos, 1 );
	}

	
	public static new UIZoomButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIZoomButton( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}


	public UIZoomButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( manager, frame, depth, uvFrame, highlightedUVframe )
	{
		centerize();
		autoRefreshPositionOnScaling = false;
		_zoomInAnimation = new UIAnimation( this, 0.3f, UIAnimationProperty.Scale, new Vector3( 1, 1, 1 ), new Vector3( 1.3f, 1.3f, 1.3f ), Easing.Quartic.easeInOut );
		_zoomOutAnimation = new UIAnimation( this, 0.3f, UIAnimationProperty.Scale, new Vector3( 1.3f, 1.3f, 1.3f ), new Vector3( 1, 1, 1 ), Easing.Quartic.easeInOut );
	}

	#endregion;



	// UITouchWrapper handlers
	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		base.onTouchBegan( touch, touchPos );
		
		_zoomInAnimation.restartStartToCurrent();
		UI.instance.StartCoroutine( _zoomInAnimation.animate() );
	}



	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		base.onTouchEnded( touch, touchPos, touchWasInsideTouchFrame );
		
		_zoomInAnimation.stop();
		_zoomOutAnimation.restartStartToCurrent();
		UI.instance.StartCoroutine( _zoomOutAnimation.animate() );
	}
	
}
