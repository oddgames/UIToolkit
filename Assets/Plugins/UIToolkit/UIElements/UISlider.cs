using UnityEngine;


public enum UISliderLayout { Horizontal, Vertical }

public class UISlider : UITouchableSprite
{
	public bool continuous = false; // Indicates whether changes in the sliders value generate continuous update events

	private float _knobMinimumXY; // Minimum value for the sliderKnobs position
	private float _knobMaximumXY; // Maximum value for the sliderKnobs position
	private float _value = 0;
	private UISprite _sliderKnob;
	
	private UISliderLayout layout = UISliderLayout.Horizontal;
	public delegate void UISliderChanged( UISlider sender, float value );
	public event UISliderChanged onChange;
	
	
	
	// The knobs x/y coordinates should be relative to the tracks and it is measured from the center of the knob
	public static UISlider create( string knobFilename, string trackFilename, int trackxPos, int trackyPos, UISliderLayout layout )
	{
		return create( UI.firstToolkit, knobFilename, trackFilename, trackxPos, trackyPos, layout );
	}

	
	public static UISlider create( UIToolkit manager, string knobFilename, string trackFilename, int trackxPos, int trackyPos, UISliderLayout layout )
	{
		// create the track first so we can use its dimensions to position the knob		
		var trackTI = manager.textureInfoForFilename( trackFilename );
		var trackFrame = new Rect( trackxPos, trackyPos, trackTI.frame.width, trackTI.frame.height );
		
		// position the knob based on the knobs size, layout and the track size
		if( layout == UISliderLayout.Horizontal )
			trackyPos += (int)trackTI.frame.height / 2;
		else
			trackxPos += (int)trackTI.frame.width / 2;

		// create a knob using our cacluated position
		var knob = manager.addSprite( knobFilename, trackxPos, trackyPos, 1, true );
		
		return new UISlider( manager, trackFrame, 2, trackTI.uvRect, knob, layout );
	}
	

	public UISlider( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UISprite sliderKnob, UISliderLayout layout ):base( frame, depth, uvFrame )
	{
		this.layout = layout;
		
		// save the sliderKnob and make it a child of the slider for organization purposes
		_sliderKnob = sliderKnob;
		_sliderKnob.parentUIObject = this;
		
		// setup the min/max position values for the sliderKnob
		updateSliderKnobConstraints();
		
		manager.addTouchableSprite( this );
	}

	// Removes the sprite from the mesh and destroys it's client GO
	public override void destroy()
	{
		// pass the call down to our knob
		_sliderKnob.destroy();
		
		base.destroy();
	}
	
    public override bool hidden
    {
        get { return ___hidden; }
        set
        {
            // No need to do anything if we're already in this state:
            if( value == ___hidden )
                return;

			base.hidden = value;
			
			// pass the call down to our knob
			_sliderKnob.hidden = value;
        }
    }


	// Current value of the slier.  Will always be between 0 and 1.
	public float value
	{
		get { return _value; }
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0f, 1f );
				
				// Update the slider position
				this.updateSliderKnobWithNormalizedValue( _value );
			}
		}
	}
	
	
	public override void updateTransform()
	{
		base.updateTransform();
		
		updateSliderKnobConstraints();
	}


	/// <summary>
	/// Updates the min/max constraints for the slider knob
	/// </summary>
	private void updateSliderKnobConstraints()
	{
		// setup the min/max position values for the sliderKnob
		if( layout == UISliderLayout.Horizontal )
		{
			_knobMinimumXY = position.x + _sliderKnob.width / 2;
			_knobMaximumXY = position.x + width - _sliderKnob.width / 2;
		}
		else
		{
			_knobMinimumXY = position.y - height + _sliderKnob.height / 2;
			_knobMaximumXY = position.y - _sliderKnob.height / 2;
		}
	}


	// Takes in a value from 0 - 1 and sets the sliderKnob based on it
	private void updateSliderKnobWithNormalizedValue( float normalizedKnobValue )
	{
		if( layout == UISliderLayout.Horizontal )
		{
			float newKnobPosition = Mathf.Clamp( position.x + normalizedKnobValue * width, _knobMinimumXY, _knobMaximumXY );
			_sliderKnob.position = new Vector3( newKnobPosition, _sliderKnob.position.y, _sliderKnob.position.z );
		}
		else
		{
			// inverse the value because 1 is our peak value but that corresponds to a lower y coordinate due to 0 being on top
			normalizedKnobValue = 1 - normalizedKnobValue;
			float newKnobPosition = Mathf.Clamp( position.y - normalizedKnobValue * height, _knobMinimumXY, _knobMaximumXY );
			_sliderKnob.position = new Vector3( _sliderKnob.position.x, newKnobPosition, _sliderKnob.position.z );
		}
	}

	
	// Takes in a touch position in world coordinates and takes care of all events and value setting
	private void updateSliderKnobForTouchPosition( Vector2 touchPos )
	{
		Vector2 localTouchPosition = this.inverseTranformPoint( touchPos );

		// Calculate the normalized value (0 - 1) based on the touchPosition
		float normalizedValue = ( layout == UISliderLayout.Horizontal ) ? ( localTouchPosition.x / width ) : ( ( height - localTouchPosition.y ) / height );
		this.value = normalizedValue;

		// If the delegate wants continuous updates, send one along
		if( continuous && onChange != null )
			onChange( this, _value );
	}


	// Touch handlers
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		highlighted = true;

		this.updateSliderKnobForTouchPosition( touchPos );
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		this.updateSliderKnobForTouchPosition( touchPos );
	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		if( touchCount == 0 )
			highlighted = false;
		
		if( onChange != null )
			onChange( this, _value );
	}

}

