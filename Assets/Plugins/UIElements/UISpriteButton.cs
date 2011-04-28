using UnityEngine;


public delegate void UIButtonTouchUpInside( UISpriteButton sender );

public class UISpriteButton : UITouchableSprite
{
	public event UIButtonTouchUpInside touchUpInside;
	public UIButtonTouchUpInside action = null; // Delegate for when we get a touchUpInside
	
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe;

	
	#region Constructors/Destructor
	
	public UISpriteButton( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
	}


	public UISpriteButton( Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):this( frame, depth, uvFrame )
	{
		this.highlightedUVframe = highlightedUVframe;
	}

	#endregion;


	// Sets the uvFrame of the original GUISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			_uvFrame = value;
			_normalUVframe = value;
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
					base.uvFrame = _normalUVframe;
			}
		}
	}


	// Override transform() so we can mark the touchFrame as dirty
	public override void updateTransform()
	{
		base.updateTransform();
		
		touchFrameIsDirty = true;
	}


	// Touch handlers
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		highlighted = false;
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame && touchUpInside != null )
			touchUpInside( this );
	}


}