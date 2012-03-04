using UnityEngine;
using System;


public class UIProgressBar : UISprite
{
	public bool rightToLeft;
	
	private float _value = 0;
	private float _barOriginalWidth;
	private UIUVRect _barOriginalUVframe;
	private bool _resizeTextureOnChange = false;
	
	
	public static UIProgressBar create( string barFilename, int xPos, int yPos )
	{
		return create( UI.firstToolkit, barFilename, xPos, yPos, false, 1 );
	}


	public static UIProgressBar create( UIToolkit manager, string barFilename, int xPos, int yPos, bool rightToLeft, int depth )
	{
		var textureInfo = manager.textureInfoForFilename( barFilename );
		var frame = new Rect( xPos, yPos, textureInfo.frame.width, textureInfo.frame.height );
		
		if( rightToLeft )
			frame.x = xPos + (int)textureInfo.frame.width;

		var progressBar = new UIProgressBar( manager, frame, depth, textureInfo.uvRect, rightToLeft );
		
		return progressBar;
	}
	
	
	public UIProgressBar( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, bool rightToLeft ):base( frame, depth, uvFrame )
	{
		manager.addSprite( this );
		
		// Save the bars original size
		_barOriginalWidth = frame.width;
		_barOriginalUVframe = uvFrame;
		this.rightToLeft = rightToLeft;

		// Update the bar size based on the value
		if( rightToLeft )
			setSize( _value * -_barOriginalWidth, frame.height );
		else
			setSize( _value * _barOriginalWidth, frame.height );
	}


    public bool resizeTextureOnChange
    {
        get { return _resizeTextureOnChange; }
        set
        {
            if( _resizeTextureOnChange != value )
            {
                // Update the bar UV's if resizeTextureOnChange is set
                if( value )
                {
                    UIUVRect newUVframe = _barOriginalUVframe;
                    newUVframe.uvDimensions.x *= _value;
                    uvFrame = newUVframe;
                }
                else // Set original uv if not
                {
                    uvFrame = _barOriginalUVframe;
                }

                // Update the bar size based on the value
                if( rightToLeft )
                    setSize( _value * -_barOriginalWidth, height );
                else
                    setSize( _value * _barOriginalWidth, height );

                _resizeTextureOnChange = value;
            }
        }
    }
	

	// Current value of the progress bar.  Value is always between 0 and 1.
	public float value
	{
		get { return _value; }
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0, 1 );
				
				// Update the bar UV's if resizeTextureOnChange is set
				if( resizeTextureOnChange )
				{
					// Set the uvFrame's width based on the value
					UIUVRect newUVframe = _barOriginalUVframe;
					newUVframe.uvDimensions.x *= _value;
					uvFrame = newUVframe;
				}

				// Update the bar size based on the value
				if( rightToLeft )
					setSize( _value * -_barOriginalWidth, height );	
				else
					setSize( _value * _barOriginalWidth, height );
			}
		}
	}
}