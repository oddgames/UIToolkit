using System;
using UnityEngine;

public class UIGhostJoystick : UITouchableSprite
{
	public Vector2 joystickPosition;
	public Vector2 deadZone = Vector2.zero; // Controls when position output occurs
	public bool normalize = true; // Normalize output after the dead-zone?  If true, we start at 0 even though the joystick is moved deadZone pixels already
	public UIUVRect highlightedUVframe = UIUVRect.zero; // Highlighted UV's for the joystick
	
	private UISprite _joystickSprite;
	private UISprite _backgroundSprite;

	private Vector2 _joystickCenter;

	public float maxJoystickMovement = 20.0f; // max distance from _joystickCenter that the joystick will move
	private UIToolkit _manager; // we need this for getting at texture details after the constructor

	private int currentTouchId = -1;
	
	
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
	
	
	public static UIGhostJoystick create( string joystickFilename, Rect hitArea )
	{
		return create( UI.firstToolkit, joystickFilename, hitArea );
	}

	
	public static UIGhostJoystick create( UIToolkit manager, string joystickFilename, Rect hitArea )
	{
		// create the joystrick sprite
		var joystick = manager.addSprite( joystickFilename, 0, 0, 1, true );
		
		return new UIGhostJoystick( manager, hitArea, 1, joystick);
	}

	
	public UIGhostJoystick( UIToolkit manager, Rect frame, int depth, UISprite joystickSprite)
		: base(frame, depth, UIUVRect.zero)
	{
		// Save out the uvFrame for the sprite so we can highlight
		_tempUVframe = joystickSprite.uvFrame;
		
		// Save the joystickSprite and make it a child of the us for organization purposes
		_joystickSprite = joystickSprite;
		_joystickSprite.parentUIObject = this;
		
		resetJoystick();
		
		manager.addTouchableSprite( this );
		_manager = manager;
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
		_backgroundSprite.hidden = true;
	}
	
	
	// Resets the sprite to default position and zeros out the position vector
	private void resetJoystick()
	{
		_joystickSprite.localPosition = _joystickCenter;
		joystickPosition.x = joystickPosition.y = 0.0f;
		
		// If we have a highlightedUVframe, swap the original back in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = _tempUVframe;

		if (_backgroundSprite != null) {
			_backgroundSprite.hidden = true;
		}
		_joystickSprite.localPosition = new Vector3(-1000, -1000, -1000);
		_joystickSprite.hidden = true;
	}

	private void displayJoystick(Vector2 localTouchPos)
	{
		_joystickCenter = localTouchPos;

		_joystickSprite.localPosition = _joystickCenter;
		joystickPosition.x = joystickPosition.y = 0.0f;
		if (_backgroundSprite != null) {
			_backgroundSprite.localPosition = new Vector3( _joystickCenter.x, _joystickCenter.y, 2 );
			_backgroundSprite.hidden = false;
		}
		_joystickSprite.hidden = false;
	}

	private void layoutJoystick( Vector2 localTouchPosition )
	{
		// Clamp the new position based on the boundaries we have set.  Dont forget to reverse the Y axis!
		Vector2 newPosition = localTouchPosition;

		float sqrlen = newPosition.sqrMagnitude;
		if( sqrlen > maxJoystickMovement * maxJoystickMovement * UI.scaleFactor * UI.scaleFactor )
			newPosition = newPosition.normalized * maxJoystickMovement * UI.scaleFactor;
		
		// Set the new position and update the transform
		_joystickSprite.localPosition = new Vector2( newPosition.x + _joystickCenter.x, _joystickCenter.y + newPosition.y );
		
		// Get a value between -1 and 1 for position
		joystickPosition = newPosition / ( maxJoystickMovement * UI.scaleFactor );
		
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
		if (currentTouchId != -1)
			return;

		currentTouchId = touch.fingerId;

		touchPos.y = -touchPos.y;

		highlighted = true;

		// Re-center joystick pad
		displayJoystick(touchPos);

		this.layoutJoystick( this.inverseTranformPoint(touchPos - _joystickCenter));
		
		// If we have a highlightedUVframe, swap it in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = highlightedUVframe;
	}
	

	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		if (touch.fingerId != currentTouchId)
			return;

		touchPos.y = -touchPos.y;

		this.layoutJoystick(this.inverseTranformPoint(touchPos - _joystickCenter));
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		if (touch.fingerId != currentTouchId)
			return;

		// Set highlighted to avoid calling super
		highlighted = false;
		
		// Reset back to default state
		this.resetJoystick();

		currentTouchId = -1;
	}
	
}


