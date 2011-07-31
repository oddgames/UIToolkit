using UnityEngine;


public class UIButton : UITouchableSprite
{
	public delegate void UIButtonTouchUpInside( UIButton sender );
	public event UIButtonTouchUpInside onTouchUpInside;

	public delegate void UIButtonTouchDown( UIButton sender );
	public event UIButtonTouchDown onTouchDown;

	public UIUVRect highlightedUVframe;
	public AudioClip touchDownSound;
	public Vector2 initialTouchPosition;


	#region Constructors/Destructor
	
	public static UIButton create( string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIButton.create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos );
	}
	
	
	public static UIButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIButton.create( manager, filename, highlightedFilename, xPos, yPos, 1 );
	}

	
	public static UIButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIButton( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}


	public UIButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame )
	{
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
		
		manager.addTouchableSprite( this );
	}

	#endregion;


	// Sets the uvFrame of the original UISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			_uvFrame = value;
			manager.updateUV( this );
		}
	}

	
	public override bool highlighted
	{
		set
		{
			// Only set if it is different than our current value
			if( _highlighted != value )
			{			
				_highlighted = value;
				
				if ( value )
					base.uvFrame = highlightedUVframe;
				else
					base.uvFrame = _tempUVframe;
			}
		}
	}


	// Touch handlers
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		highlighted = true;
		
		initialTouchPosition = touch.position;
		
		if( touchDownSound != null )
			UI.instance.playSound( touchDownSound );
		
		if( onTouchDown != null )
			onTouchDown( this );
	}



#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		highlighted = false;
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame && onTouchUpInside != null )
			onTouchUpInside( this );
	}


}