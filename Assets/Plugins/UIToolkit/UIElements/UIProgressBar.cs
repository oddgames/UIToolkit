using UnityEngine;
using System;


public class UIProgressBar : UISprite
{
	public bool rightToLeft;
	
	private float _value = 0;
	private UISprite _bar;
	private float _barOriginalWidth;
	private UIUVRect _barOriginalUVframe;
	private bool _resizeTextureOnChange = false;
	
	
	public static UIProgressBar create( string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos, int depth = 2, bool barInFront = true )
	{
		return create( UI.firstToolkit, barFilename, borderFilename, barxPos, baryPos, borderxPos, borderyPos, false, depth, barInFront );
	}


	public static UIProgressBar create( string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos, bool rightToLeft, int depth = 2, bool barInFront = true )
	{
		return create( UI.firstToolkit, barFilename, borderFilename, barxPos, baryPos, borderxPos, borderyPos, rightToLeft, depth, barInFront );
	}

	
	// the bars x/y coordinates should be relative to the borders
	public static UIProgressBar create( UIToolkit manager, string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos, bool rightToLeft, int depth, bool barInFront = true )
	{
		var borderTI = manager.textureInfoForFilename( borderFilename );
	
		var borderFrame = new Rect( borderxPos, borderyPos, borderTI.frame.width, borderTI.frame.height );
		
		UISprite bar;
		
		if( rightToLeft )
			bar = manager.addSprite( barFilename, borderxPos - barxPos + ((int)borderTI.frame.width), borderyPos + baryPos, depth );
		else
			bar = manager.addSprite( barFilename, borderxPos + barxPos, borderyPos + baryPos, depth );

		var barDepth = barInFront ? depth - 1 : depth + 1;
		var progressBar = new UIProgressBar( manager, borderFrame, barDepth, borderTI.uvRect, bar );
		progressBar.rightToLeft = rightToLeft;
		
		return progressBar;
	}
	
	
	public UIProgressBar( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UISprite bar ):base( frame, depth, uvFrame )
	{
		// Save the bar and make it a child of the container/border for organization purposes
		_bar = bar;
		_bar.parentUIObject = this;
		
		// Save the bars original size
		_barOriginalWidth = _bar.width;
		_barOriginalUVframe = _bar.uvFrame;

		// Update the bar size based on the value
		if (rightToLeft)
			_bar.setSize(_value * -_barOriginalWidth, _bar.height);
		else
			_bar.setSize(_value * _barOriginalWidth, _bar.height);
		
		manager.addSprite( this );
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
			
			// pass the call down to our bar
			_bar.hidden = value;
        }
    }


    public bool resizeTextureOnChange
    {
        get { return _resizeTextureOnChange; }
        set
        {
            if (_resizeTextureOnChange != value)
            {
                // Update the bar UV's if resizeTextureOnChange is set
                if (value)
                {
                    UIUVRect newUVframe = _barOriginalUVframe;
                    newUVframe.uvDimensions.x *= _value;
                    _bar.uvFrame = newUVframe;
                }
                // Set original uv if not
                else
                {
                    _bar.uvFrame = _barOriginalUVframe;
                }

                // Update the bar size based on the value
                if (rightToLeft)
                    _bar.setSize(_value * -_barOriginalWidth, _bar.height);
                else
                    _bar.setSize(_value * _barOriginalWidth, _bar.height);

                _resizeTextureOnChange = value;
            }
        }
    }

	
	public override void destroy()
	{
		_bar.destroy();
		base.destroy();
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
					_bar.uvFrame = newUVframe;
				}

				// Update the bar size based on the value
				if( rightToLeft )
					_bar.setSize( _value * -_barOriginalWidth, _bar.height );	
				else
					_bar.setSize( _value * _barOriginalWidth, _bar.height );
			}
		}
	}

	
}