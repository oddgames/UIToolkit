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
		// bottom, left quadrant of screen
		leftJoystick = UIJoystick.create( "joystickUp.png", new Rect( 0, Screen.height / 2, Screen.width / 2, 200 ), Screen.width / 4, -150 / 2 );
		leftJoystick.deadZone = new Vector2( 0.6f, 0.6f );
		leftJoystick.setJoystickHighlightedFilename( "joystickDown.png" );


		// bottom, right quadrant of screen
		rightJoystick = UIJoystick.create( "joystickUp.png", new Rect( Screen.width / 2, Screen.height / 2, Screen.width / 2, 200 ), Screen.width / 4, -150 / 2 );
		rightJoystick.deadZone = new Vector2( 0.5f, 0.5f );
		rightJoystick.setJoystickHighlightedFilename( "joystickDown.png" );	
		rightJoystick.addBackgroundSprite( "joystickTrack.png" );
	}
	

	void Update()
	{
		rightText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", rightJoystick.joystickPosition.x, rightJoystick.joystickPosition.y );
		leftText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", leftJoystick.joystickPosition.x, leftJoystick.joystickPosition.y );
	}

}
