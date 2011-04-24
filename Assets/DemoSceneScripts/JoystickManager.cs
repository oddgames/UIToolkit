using UnityEngine;
using System.Collections;

public class JoystickManager : MonoBehaviour
{
	UIJoystick leftJoystick;
	UIJoystick rightJoystick;
	
	public GUIText leftText;
	public GUIText rightText;

	void Start()
	{
		// Save the texture size locally for easy access
		Vector2 textureSize = UI.instance.textureSize;
		
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		// Left Joystick - be sure the last parameter is true so that the translation point is the center
		UISprite leftJoystickSprite = new UISprite( new Rect( 10, 10, 100, 100 ), 1, new UIUVRect( 0, 0, 234, 234, textureSize ), true );
		
		leftJoystick = new UIJoystick( new Rect( 0, 120, 240, 200 ), 1, UIUVRect.zero, leftJoystickSprite, new Vector2( 240 / 2, -200 / 2 ) );
		leftJoystick.deadZone = new Vector2( 0.1f, 0.1f );
		leftJoystick.normalize = true;
		UI.instance.addTouchableSprite( leftJoystick );
		
		
		// Right Joystick		
		UISprite rightJoystickSprite = new UISprite( new Rect( 10, 10, 100, 100 ), 1, new UIUVRect( 250, 0, 234, 234, textureSize ), true );
		rightJoystick = new UIJoystick( new Rect( 240, 120, 240, 200 ), 1, UIUVRect.zero, rightJoystickSprite, new Vector2( 240 / 2, -200 / 2 ) );
		rightJoystick.highlightedUVframe = new UIUVRect( 250, 240, 234, 234, textureSize );
		rightJoystick.addBackgroundSprite( new Rect( 0, 0, 120, 120 ), 50, new UIUVRect( 250, 500, 305, 305, textureSize ) );
		UI.instance.addTouchableSprite( rightJoystick );
	}
	

	void Update()
	{
		rightText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", rightJoystick.position.x, rightJoystick.position.y );
		leftText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", leftJoystick.position.x, leftJoystick.position.y );
	}

}
