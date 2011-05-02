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
		leftJoystick = UIJoystick.create( "joystickUp.png", new Rect( 0, 160, 240, 200 ), 240 / 2, -150 / 2 );
		leftJoystick.deadZone = new Vector2( 0.6f, 0.6f );
		leftJoystick.setJoystickHighlightedFilename( "joystickDown.png" );


		// bottom, right quadrant of screen
		rightJoystick = UIJoystick.create( "joystickUp.png", new Rect( 240, 160, 240, 200 ), 240 / 2, -150 / 2 );
		rightJoystick.deadZone = new Vector2( 0.5f, 0.5f );
		rightJoystick.setJoystickHighlightedFilename( "joystickDown.png" );	
		rightJoystick.addBackgroundSprite( "joystickTrack.png" );
	}
	

	void Update()
	{
		rightText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", rightJoystick.position.x, rightJoystick.position.y );
		leftText.text = string.Format( "x: {0:0.00}, y: {1:0.00}", leftJoystick.position.x, leftJoystick.position.y );
	}

}
