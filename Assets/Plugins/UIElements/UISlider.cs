using UnityEngine;


public enum UISliderLayout { Horizontal, Vertical }
public delegate void UISliderChanged( UISlider sender, float value );

public class UISlider : UITouchableSprite
{
	public bool continuous = false; // Indicates whether changes in the sliders value generate continuous update events

	private float _knobMinimumXY; // Minimum value for the sliderKnobs position
	private float _knobMaximumXY; // Maximum value for the sliderKnobs position
	private float _value = 0;
	private UISprite _sliderKnob;
	
	private UISliderLayout layout = UISliderLayout.Horizontal;
	public event UISliderChanged onChange;
	
	
	public static UISlider create( string filename, int xPos, int yPos, UISprite sliderKnob, UISliderLayout layout )
	{
		var textureInfo = UI.instance.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, textureInfo.size.x, textureInfo.size.y );
		
		return new UISlider( frame, 5, textureInfo.uvRect, sliderKnob, layout );
	}
	
	
	public UISlider( UITextureInfo textureInfo, int xPos, int yPos, UISprite sliderKnob, UISliderLayout layout ):this( new Rect( xPos, yPos, textureInfo.size.x, textureInfo.size.y ), 5, textureInfo.uvRect, sliderKnob, layout )
	{
		
	}
	

	public UISlider( Rect frame, int depth, UIUVRect uvFrame, UISprite sliderKnob, UISliderLayout layout ):base( frame, depth, uvFrame )
	{
		this.layout = layout;
		
		// save the sliderKnob and make it a child of the slider for organization purposes
		_sliderKnob = sliderKnob;
		_sliderKnob.clientTransform.parent = this.clientTransform;

		// setup the min/max position values for the sliderKnob
		if( layout == UISliderLayout.Horizontal )
		{
			_knobMinimumXY = frame.x;
			_knobMaximumXY = frame.x + width - sliderKnob.width;
		}
		else
		{
			_knobMinimumXY = -frame.y - height + sliderKnob.height;
			_knobMaximumXY = -frame.y;
		}
		
		UI.instance.addTouchableSprite( this );
	}


	public float value
	{
		get { return _value; }
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0, 1 );
				
				// Update the slider position
				this.updateSliderKnobWithNormalizedValue( _value );
			}
		}
	}


	// Takes in a value from 0 - 1 and sets the sliderKnob based on it
	private void updateSliderKnobWithNormalizedValue( float normalizedKnobValue )
	{
		if( layout == UISliderLayout.Horizontal )
		{
			float newKnobPosition = Mathf.Clamp( clientTransform.position.x + normalizedKnobValue * width, _knobMinimumXY, _knobMaximumXY );
			_sliderKnob.clientTransform.position = new Vector3( newKnobPosition, _sliderKnob.clientTransform.position.y, _sliderKnob.clientTransform.position.z );
		}
		else
		{
			float newKnobPosition = Mathf.Clamp( clientTransform.position.y - normalizedKnobValue * height, _knobMinimumXY, _knobMaximumXY );
			_sliderKnob.clientTransform.position = new Vector3( _sliderKnob.clientTransform.position.x, newKnobPosition, _sliderKnob.clientTransform.position.z );
		}
		_sliderKnob.updateTransform();
	}

	
	// Takes in a touch position in world coordinates and takes care of all events and value setting
	private void updateSliderKnobForTouchPosition( Vector2 touchPos )
	{
		Vector2 localTouchPosition = this.inverseTranformPoint( touchPos );

		// Calculate the normalized value (0 - 1) based on the touchPosition
		float normalizedValue = ( layout == UISliderLayout.Horizontal ) ? ( localTouchPosition.x / width ) : ( localTouchPosition.y / height );
		this.value = normalizedValue;

		// If the delegate wants continuous updates, send one along
		if( continuous && onChange != null )
			onChange( this, _value );
	}


	// Touch handlers.  Subclasses should override to get their specific behaviour
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		highlighted = true;

		this.updateSliderKnobForTouchPosition( touchPos );
	}

	
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
	{
		this.updateSliderKnobForTouchPosition( touchPos );
	}
	
	
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		if( touchCount == 0 )
			highlighted = false;
		
		if( onChange != null )
			onChange( this, _value );
	}

}

