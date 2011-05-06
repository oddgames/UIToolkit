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


	public static UIKnob create( string filename, string highlightedFilename, int xPos, int yPos, int depth = 1 )
	{
		// grab the texture details for the normal state
		var normalTI = UI.instance.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.size.x, normalTI.size.y );
		
		// get the highlighted state
		var highlightedTI = UI.instance.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UIKnob( frame, depth, normalTI.uvRect, highlightedTI.uvRect );
	}

	
	public UIKnob( Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame, true )
	{
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
		
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

		_value = ( 360 - eulerAngles.z ) / 360;
		
		if( continuous && onKnobChanged != null )
			onKnobChanged( this, _value );
	}
	

	#region Touch Handlers

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		highlighted = true;

		this.updateKnobForTouchPosition( touchPos );
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		this.updateKnobForTouchPosition( touchPos );
	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
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
