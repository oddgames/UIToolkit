using UnityEngine;


public class UIKnob : UITouchableSprite
{
	// Indicates whether changes in the sliders value generate continuous update events
	public bool continuous = false;
	private float _value = 0;

	public UIUVRect highlightedUVframe;
	
	public delegate void UIKnobChanged( UIKnob sender, float value );
	public event UIKnobChanged onKnobChanged;
	
	
	
	public static UIKnob create( string filename, string highlightedFilename, int xPos, int yPos )
	{
		return create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos );
	}
	
	
	public static UIKnob create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos )
	{
		return create( manager, filename, highlightedFilename, xPos, yPos, 1 );
	}


	public static UIKnob create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIKnob( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}

	
	public UIKnob( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame, true )
	{
		_tempUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
		
		manager.addTouchableSprite( this );
	}

	
	// Sets the uvFrame of the original GUISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			_uvFrame = value;
			_tempUVframe = value;
			manager.updateUV( this );
		}
	}


	// Override to have a different UV set for highlighted state
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

	
	// Gets or sets the current value of the UIKnob
	public float value
	{
		get	{ return _value; }
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0, 1 );
				
				// Update the knob rotation
				clientTransform.rotation = Quaternion.Euler( 0, 0, -_value * 360 );
				updateTransform();
			}
		}
	}


	// Updates the rotation of the knob based on the touchPos
	private void updateKnobForTouchPosition( Vector2 touchPos )
	{
		Vector2 localTouchPosition = this.inverseTranformPoint( touchPos );
		
		// Calculate the rotation then apply it
		float x = localTouchPosition.x - ( width / 2 );
		Vector3 newVector = new Vector3( x, ( height / 2 ) - localTouchPosition.y, 0 ) - Vector3.zero;
		float rot = Vector3.Angle( Vector3.up, newVector ) * ( -Mathf.Sign( x ) );
		
		clientTransform.rotation = Quaternion.Euler( 0, 0, rot );
		updateTransform();

		_value = ( 360 - eulerAngles.z ) / 360;
		
		if( continuous && onKnobChanged != null )
			onKnobChanged( this, _value );
	}
	

	#region UITouchWrapper Handlers

	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		highlighted = true;

		this.updateKnobForTouchPosition( touchPos );
	}


	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		this.updateKnobForTouchPosition( touchPos );
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		highlighted = false;
		
		if( onKnobChanged != null )
			onKnobChanged( this, _value );
	}
	
	#endregion;


}
