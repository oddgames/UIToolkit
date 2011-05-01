using UnityEngine;


public class UIButton : UITouchableSprite
{
	public delegate void UIButtonTouchUpInside( UIButton sender );
	public event UIButtonTouchUpInside onTouchUpInside;
	
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe;

	
	#region Constructors/Destructor
	
	public static UIButton create( string filename, string highlightedFilename, int xPos, int yPos, int depth = 1 )
	{
		// grab the texture details for the normal state
		var normalTI = UI.instance.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.size.x, normalTI.size.y );
		
		// get the highlighted state
		var highlightedTI = UI.instance.textureInfoForFilename( highlightedFilename );
		
		// create the button
		var button = new UIButton( frame, depth, normalTI.uvRect, highlightedTI.uvRect );
		UI.instance.addTouchableSprite( button );
		
		return button;
	}


	public UIButton( Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame )
	{
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
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
#if UNITY_EDITOR
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