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
	public Vector2 position;
	public Vector2 deadZone = Vector2.zero; // Controls when position output occurs
	public bool normalize; // Normalize output after the dead-zone?
	
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe = UIUVRect.zero; // Highlighted UV's for the joystick
	
	private UISprite _joystickSprite;
	private Vector3 _joystickOffset;
	private UIBoundary _joystickBoundary;
	private float _maxJoystickMovement = 50.0f; // max distance from _joystickOffset that the joystick will move
	
	
	public UIJoystick( Rect frame, int depth, UIUVRect uvFrame, UISprite joystickSprite, Vector2 joystickOffset ):base( frame, depth, uvFrame )
	{
		// Add the joystickSprite to the manager
		UI.instance.addSprite( joystickSprite );
		
		// Save out the uvFrame for the sprite so we can highlight
		_normalUVframe = joystickSprite.uvFrame;
		
		// Save the joystickSprite and make it a child of the us for organization purposes
		_joystickSprite = joystickSprite;
		_joystickSprite.clientTransform.parent = this.clientTransform;
		
		// Move the joystick to its default position after converting the offset to a vector3
		_joystickOffset = new Vector3( joystickOffset.x, joystickOffset.y );
		
		// Set the maxMovement which will in turn calculate the _joystickBoundary
		this.maxJoystickMovement = _maxJoystickMovement;
		
		resetJoystick();
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
	
	
	public void addBackgroundSprite( Rect frame, int depth, UIUVRect uvFrame )
	{
		UISprite sprite = new UISprite( frame, depth, uvFrame, true );
		sprite.clientTransform.parent = this.clientTransform;
		UI.instance.addSprite( sprite );
		sprite.clientTransform.localPosition = new Vector3( _joystickOffset.x, _joystickOffset.y, depth );
		sprite.updateTransform();
	}
	
	
	// Resets the sprite to default position and zeros out the position vector
	private void resetJoystick()
	{
		_joystickSprite.clientTransform.localPosition = _joystickOffset;
		_joystickSprite.updateTransform();
		position.x = position.y = 0.0f;
		
		// If we have a highlightedUVframe, swap the original back in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = _normalUVframe;
	}
	
	
	private void layoutJoystick( Vector2 localTouchPosition )
	{
		// Clamp the new position based on the boundaries we have set.  Dont forget to reverse the Y axis!
		Vector3 newPosition = Vector3.zero;
		newPosition.x = Mathf.Clamp( localTouchPosition.x, _joystickBoundary.minX, _joystickBoundary.maxX );
		newPosition.y = Mathf.Clamp( -localTouchPosition.y, _joystickBoundary.minY, _joystickBoundary.maxY );
		
		// Set the new position and update the transform		
		_joystickSprite.clientTransform.localPosition = newPosition;
		_joystickSprite.updateTransform();
		
		// Get a value between -1 and 1 for position
		position.x = ( newPosition.x - _joystickOffset.x ) / _maxJoystickMovement;
		position.y = ( newPosition.y - _joystickOffset.y ) / _maxJoystickMovement;
		
		// Adjust for dead zone	
		float absoluteX = Mathf.Abs( position.x );
		float absoluteY = Mathf.Abs( position.y );
	
		if( absoluteX < deadZone.x )
		{
			// Report the joystick as being at the center if it is within the dead zone
			position.x = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			position.x = Mathf.Sign( position.x ) * ( absoluteX - deadZone.x ) / ( 1 - deadZone.x );
		}
		
		if( absoluteY < deadZone.y )
		{
			// Report the joystick as being at the center if it is within the dead zone
			position.y = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			position.y = Mathf.Sign( position.y ) * ( absoluteY - deadZone.y ) / ( 1 - deadZone.y );
		}
	}
	
	
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		highlighted = true;
		
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
		
		// If we have a highlightedUVframe, swap it in
		if( highlightedUVframe != UIUVRect.zero )
			_joystickSprite.uvFrame = highlightedUVframe;
	}
	
	
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
	{
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
	}
	
	
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		// Set highlighted to avoid calling super
		this.highlighted = false;
		
		// Reset back to default state
		this.resetJoystick();
	}
	
}


