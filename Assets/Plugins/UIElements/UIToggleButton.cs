using UnityEngine;
using System;


public delegate void UIToggleButtonChanged( UIToggleButton sender, bool selected );

public class UIToggleButton : UITouchableSprite
{
	public UIToggleButtonChanged action = null; // Delegate for when we get a touchUpInside
	
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe;
	public UIUVRect selectedUVframe;
	
	private bool _selected;
	

	#region Constructors
	
	public UIToggleButton( Rect frame, int depth, UIUVRect uvFrame, UIUVRect selectedUVframe ):base( frame, depth, uvFrame )
	{
		this.selectedUVframe = selectedUVframe;
		
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
	}


	public UIToggleButton( Rect frame, int depth, UIUVRect uvFrame, UIUVRect selectedUVframe, UIUVRect highlightedUVframe ):this( frame, depth, uvFrame, selectedUVframe )
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
				else if( _selected )
					base.uvFrame = selectedUVframe;
				else
					base.uvFrame = _normalUVframe;
			}
		}
	}


	public bool selected
	{
		get { return _selected; }
		set
		{
			// Only set if it is different than our current value
			if( _selected != value )
			{			
				_selected = value;
				
				if ( value )
					base.uvFrame = selectedUVframe;
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
		if( touchWasInsideTouchFrame )
		{
			// Toggle our selected state
			this.selected = !_selected;
			
			// Let our delegate know things changed
			if( action != null )
				action( this, _selected );
		}
	}

}

