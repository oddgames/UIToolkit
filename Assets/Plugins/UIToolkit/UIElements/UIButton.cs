using UnityEngine;
using System;


public class UIButton : UITouchableSprite
{
	public event Action<UIButton> onTouchUpInside;
	public event Action<UIButton> onTouchDown;
	public event Action<UIButton> onTouchUp;

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
		get { return _clipped ? _uvFrameClipped : _uvFrame; }
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


	// UITouchWrapper handlers
	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		highlighted = true;
		
		initialTouchPosition = touch.position;
		
		if( touchDownSound != null )
			UI.instance.playSound( touchDownSound );
		
		if( onTouchDown != null )
			onTouchDown( this );
	}



	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		// If someone has un-highlighted us through code we are deactivated 
		// and should not fire the event
		if (!highlighted)
			return;
		
		highlighted = false;

		if (onTouchUp != null)
			onTouchUp(this);
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame && onTouchUpInside != null )
			onTouchUpInside( this );
    }

	
    public override void destroy()
    {
        base.destroy();

        highlighted = false;
    }
}