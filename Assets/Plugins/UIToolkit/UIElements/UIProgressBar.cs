using UnityEngine;
using System;


public class UIProgressBar : UISprite
{
	public bool resizeTextureOnChange = false;
	
	private float _value = 0;
	private UISprite _bar;
	private float _barOriginalWidth;
	private UIUVRect _barOriginalUVframe;
	
	
	
	public static UIProgressBar create( string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos )
	{
		return create( UI.firstToolkit, barFilename, borderFilename, barxPos, baryPos, borderxPos, borderyPos );
	}

	
	// the bars x/y coordinates should be relative to the borders
	public static UIProgressBar create( UIToolkit manager, string barFilename, string borderFilename, int barxPos, int baryPos, int borderxPos, int borderyPos )
	{
		var bar = manager.addSprite( barFilename, borderxPos + barxPos, borderyPos + baryPos, 2 );
		
		var borderTI = manager.textureInfoForFilename( borderFilename );
		var borderFrame = new Rect( borderxPos, borderyPos, borderTI.size.x, borderTI.size.y );
		
		return new UIProgressBar( manager, borderFrame, 1, borderTI.uvRect, bar );
	}
	
	
	public UIProgressBar( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UISprite bar ):base( frame, depth, uvFrame )
	{
		// Save the bar and make it a child of the container/border for organization purposes
		_bar = bar;
		_bar.parentUIObject = this;
		
		// Save the bars original size
		_barOriginalWidth = _bar.width;
		_barOriginalUVframe = _bar.uvFrame;
		
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
				_bar.setSize( _value * _barOriginalWidth, _bar.height );
			}
		}
	}

	
}

