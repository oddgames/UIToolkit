using System;
using UnityEngine;


struct UIBoundary 
{
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	
	public static UIBoundary boundaryFromPoint( Vector2 point, float maxDistance )
	{
		UIBoundary boundary = new UIBoundary();
		
		boundary.minX = point.x - maxDistance;
		boundary.maxX = point.x + maxDistance;
		boundary.minY = point.y - maxDistance;
		boundary.maxY = point.y + maxDistance;
		
		return boundary;
	}
}


public class UIJoystick : UITouchableSprite
{
	public Vector2 joystickPosition;
	public Vector2 deadZone = Vector2.zero; // Controls when position output occurs
	public bool normalize = true; // Normalize output after the dead-zone?  If true, we start at 0 even though the joystick is moved deadZone pixels already
	public UIUVRect highlightedUVframe = UIUVRect.zero; // Highlighted UV's for the joystick
	
	private UISprite _joystickSprite;
	private UISprite _backgroundSprite;
	private Vector3 _joystickOffset;
	private UIBoundary _joystickBoundary;
	private float _maxJoystickMovement = 40.0f; // max distance from _joystickOffset that the joystick will move
	private UIToolkit _manager; // we need this for getting at texture details after the constructor
	
	
	/// <summary>
	/// Hides the all joystick sprites
	/// </summary>
    public override bool hidden
    {
        set
        {
            // No need to do anything if we're already in this state
            if( value == ___hidden )
                return;
			___hidden = value;

			// apply state to the children
			_joystickSprite.hidden = value;
			
			if( _backgroundSprite != null )
				_backgroundSprite.hidden = value;
			base.hidden = value;
        }
    }
	
	
	public static UIJoystick create( string joystickFilename, Rect hitAreaFrame, float xPos, float yPos )
	{
		return create( UI.firstToolkit, joystickFilename, hitAreaFrame, xPos, yPos );
	}

	
	// 
	public static UIJoystick create( UIToolkit manager, string joystickFilename, Rect hitAreaFrame, float xPos, float yPos )
	{
		// create the joystrick sprite
		var joystick = manager.addSprite( joystickFilename, 0, 0, 1, true );
		
		return new UIJoystick( manager, hitAreaFrame, 1, joystick, xPos, yPos );
	}

	
	public UIJoystick( UIToolkit manager, Rect frame, int depth, UISprite joystickSprite, float xPos, float yPos ):base( frame, depth, UIUVRect.zero )
	{
		// Save out the uvFrame for the sprite so we can highlight
		_tempUVframe = joystickSprite.uvFrame;
		
		// Save the joystickSprite and make it a child of the us for organization purposes
		_joystickSprite = joystickSprite;
		_joystickSprite.parentUIObject = this;
		
		// Move the joystick to its default position after converting the offset to a vector3
		_joystickOffset = new Vector3( xPos, yPos );
		
		// Set the maxMovement which will in turn calculate the _joystickBoundary
		this.maxJoystickMovement = _maxJoystickMovement;
		
		resetJoystick();
		
		manager.addTouchableSprite( this );
		_manager = manager;
	}
	

	public float maxJoystickMovement
	{
		get { return _maxJoystickMovement; }
		set
		{
			_maxJoystickMovement = value;
			_joystickBoundary = UIBoundary.boundaryFromPoint( _joystickOffset, _maxJoystickMovement );
		}
	}


	// Sets the image to be displayed when the joystick is highlighted
	public void setJoystickHighlightedFilename( string filename )
	{
		var textureInfo = _manager.textureInfoForFilename( filename );
		highlightedUVframe = textureInfo.uvRect;
	}
	

	// Sets the background image for display behind the joystick sprite
	public void addBackgroundSprite( string filename )
	{
		_backgroundSprite = _manager.addSprite( filename, 0, 0, 2, true );
		_backgroundSprite.parentUIObject = this;
		_backgroundSprite.localPosition = new Vector3( _joystickOffset.x, _joystickOffset.y, 2 );
	}
	
	
	// Resets the sprite to default position and zeros out the position vector
	private void resetJoystick()
	{
		_joystickSprite.localPosition = _joystickOffset;
		joystickPosition.x = joystickPosition.y = 0.0f;
		
		// If we have a highlightedUVframe, swap the original back in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = _tempUVframe;
	}
	
	
	private void layoutJoystick( Vector2 localTouchPosition )
	{
		// Clamp the new position based on the boundaries we have set.  Dont forget to reverse the Y axis!
		Vector3 newPosition = Vector3.zero;
		
		//adjust the touches with the scale
		float X = localTouchPosition.x * _joystickSprite.localScale.x;
		float Y = localTouchPosition.y * _joystickSprite.localScale.y;
		
		//fixed to adjust the touches to the scale of the image
		newPosition.x = Mathf.Clamp( X, _joystickBoundary.minX, _joystickBoundary.maxX );
		newPosition.y = Mathf.Clamp( -Y, _joystickBoundary.minY, _joystickBoundary.maxY );
		
		// Set the new position and update the transform		
		_joystickSprite.localPosition = newPosition;
		
		// Get a value between -1 and 1 for position
		joystickPosition = ( newPosition - _joystickOffset ) / _maxJoystickMovement;
		
		// Adjust for dead zone	
		float absoluteX = Mathf.Abs( joystickPosition.x );
		float absoluteY = Mathf.Abs( joystickPosition.y );
	
		if( absoluteX < deadZone.x )
		{
			// Report the joystick as being at the center if it is within the dead zone
			joystickPosition.x = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			joystickPosition.x = Mathf.Sign( joystickPosition.x ) * ( absoluteX - deadZone.x ) / ( 1 - deadZone.x );
		}
		
		if( absoluteY < deadZone.y )
		{
			// Report the joystick as being at the center if it is within the dead zone
			joystickPosition.y = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			joystickPosition.y = Mathf.Sign( joystickPosition.y ) * ( absoluteY - deadZone.y ) / ( 1 - deadZone.y );
		}
	}
	

	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		highlighted = true;
		
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
		
		// If we have a highlightedUVframe, swap it in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = highlightedUVframe;
	}
	

	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		// Set highlighted to avoid calling super
		highlighted = false;
		
		// Reset back to default state
		this.resetJoystick();
	}
	
}


