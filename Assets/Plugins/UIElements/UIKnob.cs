using UnityEngine;


public class UIKnob : UITouchableSprite
{
	// Indicates whether changes in the sliders value generate continuous update events
	public bool continuous = false;
	private float _value = 0;

	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe;
	
	public delegate void UIKnobChanged( UIKnob sender, float value );
	public event UIKnobChanged onKnobChanged;
	
	
	public UIKnob( UITextureInfo textureInfo, int xPos, int yPos ):this( new Rect( xPos, yPos, textureInfo.size.x, textureInfo.size.y ), 1, textureInfo.uvRect )
	{
		
	}
	
	
	public UIKnob( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		// Set our origin in the center
		this.gameObjectOriginInCenter = true;
		touchFrameIsDirty = true;
		
		_normalUVframe = uvFrame;
		
		UI.instance.addTouchableSprite( this );
	}

	
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
					base.uvFrame = _normalUVframe;
			}
		}
	}


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

		_value = ( 360 - clientTransform.rotation.eulerAngles.z ) / 360;
		
		if( continuous && onKnobChanged != null )
			onKnobChanged( this, _value );
	}
	

	#region Touch Handlers

#if UNITY_EDITOR
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		highlighted = true;

		this.updateKnobForTouchPosition( touchPos );
	}


#if UNITY_EDITOR
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		this.updateKnobForTouchPosition( touchPos );
	}
	

#if UNITY_EDITOR
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		highlighted = false;
		
		if( onKnobChanged != null )
			onKnobChanged( this, _value );
	}
	
	#endregion;


}
