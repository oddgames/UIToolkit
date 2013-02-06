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
	public static UISlider create( string knobFilename, string trackFilename, int trackxPos, int trackyPos, UISliderLayout layout, int depth = 2, bool knobInFront = true )
	{
		return create( UI.firstToolkit, knobFilename, trackFilename, trackxPos, trackyPos, layout, depth, knobInFront );
	}

	
	public static UISlider create( UIToolkit manager, string knobFilename, string trackFilename, int trackxPos, int trackyPos, UISliderLayout layout, int depth = 2, bool knobInFront = true )
	{
		// create the track first so we can use its dimensions to position the knob		
		var trackTI = manager.textureInfoForFilename( trackFilename );
		var trackFrame = new Rect( trackxPos, trackyPos, trackTI.frame.width, trackTI.frame.height );

		// create a knob using our cacluated position
		var knobDepth = knobInFront ? depth - 1 : depth + 1;
		var knob = manager.addSprite( knobFilename, trackxPos, trackyPos, knobDepth, true );
		
		return new UISlider( manager, trackFrame, depth, trackTI.uvRect, knob, layout );
	}
	

	public UISlider( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UISprite sliderKnob, UISliderLayout layout ):base( frame, depth, uvFrame )
	{
		this.layout = layout;
		
		// save the sliderKnob and make it a child of the slider for organization purposes
		_sliderKnob = sliderKnob;
		_sliderKnob.parentUIObject = this;
		
		// setup the min/max position values for the sliderKnob
		updateSliderKnobConstraints();
        updateSliderKnobWithNormalizedValue(_value);
		
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


    public override Color color
    {
        get { return base.color; }
        set
        {
            base.color = value;
            // Cascade color to knob
            _sliderKnob.color = value;
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
            _knobMaximumXY = (width - _sliderKnob.width) / 2f;
            _knobMinimumXY = -_knobMaximumXY;
		}
		else
		{
            _knobMaximumXY = (height - _sliderKnob.height) / 2f;
            _knobMinimumXY = -_knobMaximumXY;
		}

        var hdFactor = 1f / UI.scaleFactor;
        _knobMaximumXY *= hdFactor;
        _knobMinimumXY *= hdFactor;
	}


	// Takes in a value from 0 - 1 and sets the sliderKnob based on it
	private void updateSliderKnobWithNormalizedValue( float normalizedKnobValue )
	{
        float offsetX = 0f, offsetY = 0f;
        float relativeKnobValue = normalizedKnobValue - 0.5f;
        var hdFactor = 1f / UI.scaleFactor;
		
		if( layout == UISliderLayout.Horizontal )
		{
            offsetX = Mathf.Clamp((width - _sliderKnob.width) * hdFactor * relativeKnobValue, _knobMinimumXY, _knobMaximumXY);
		}
		else
		{
            offsetY = -Mathf.Clamp((height - _sliderKnob.height) * hdFactor * relativeKnobValue, _knobMinimumXY, _knobMaximumXY);
		}

        _sliderKnob.pixelsFromCenter((int)offsetY, (int)offsetX);
	}

	
	// Takes in a touch position in world coordinates and takes care of all events and value setting
	private void updateSliderKnobForTouchPosition( Vector2 touchPos )
	{
		Vector2 localTouchPosition = this.inverseTranformPoint( touchPos );

        // Calculate the normalized value (0 - 1) based on the touchPosition
        float normalizedValue = 0f;
        if (layout == UISliderLayout.Horizontal)
        {
            normalizedValue = (localTouchPosition.x - _sliderKnob.width / 2f) / (width - _sliderKnob.width);
        }
        else
        {
            normalizedValue = 1f - (localTouchPosition.y - _sliderKnob.height / 2f) / (height - _sliderKnob.height);
        }

		this.value = normalizedValue;

		// If the delegate wants continuous updates, send one along
		if( continuous && onChange != null )
			onChange( this, _value );
	}


	// UITouchWrapper handlers
	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		highlighted = true;

		this.updateSliderKnobForTouchPosition( touchPos );
	}


	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		this.updateSliderKnobForTouchPosition( touchPos );
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		if( touchCount == 0 )
			highlighted = false;
		
		if( onChange != null )
			onChange( this, _value );
	}

}

