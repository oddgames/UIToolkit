/*
	This file can be used as a template for making custom UI controls.  Just rename the file and class and get coding!
*/
using UnityEngine;


#if DONE_COMPILE_ME
public class UIControlTemplate : UITouchableSprite
{
	public UIUVRect highlightedUVframe;

	
	
	#region Constructors/Destructor
	
	/*
	public static UIControlTemplate create( string filename, string highlightedFilename, int xPos, int yPos, int depth = 1 )
	{
		// create and return a new UIControlTemplate
	}
	*/

	public UIControlTemplate( Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame )
	{
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_tempUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
	}

	#endregion;


	// Sets the uvFrame of the original UISprite and resets the _normalUVFrame for reference when highlighting
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


	// Touch handlers
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		highlighted = true;
	}


public virtual void onTouchMoved( Touch touch, Vector2 touchPos )
	{
		highlighted = true;
	}


	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		highlighted = false;
	}


}
#endif