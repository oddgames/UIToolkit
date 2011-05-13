using UnityEngine;
using System.Collections;


public struct UIFakeTouch
{
	public int fingerId;
	public Vector2 position;
	public Vector2 deltaPosition;
	public float deltaTime;
	public int tapCount;
	public TouchPhase phase;
	
	
	public static UIFakeTouch fromTouch( Touch touch )
	{
		var fakeTouch = new UIFakeTouch();
		fakeTouch.fingerId = touch.fingerId;
		fakeTouch.position = touch.position;
		fakeTouch.deltaPosition = touch.deltaPosition;
		fakeTouch.deltaTime = touch.deltaTime;
		fakeTouch.phase = touch.phase;
		return fakeTouch;
	}


	public static UIFakeTouch fromInput( ref Vector2? lastMousePosition )
	{
		var fakeTouch = new UIFakeTouch();
		fakeTouch.fingerId = 2;
		
		// if we have a lastMousePosition use it to get a delta
		if( lastMousePosition.HasValue )
			fakeTouch.deltaPosition = Input.mousePosition - (Vector3)lastMousePosition;
		
		if( Input.GetMouseButtonDown( 0 ) ) // equivalent to touchBegan
		{
			fakeTouch.phase = TouchPhase.Began;
			lastMousePosition = Input.mousePosition;
		}
		else if( Input.GetMouseButtonUp( 0 ) ) // equivalent to touchEnded
		{
			fakeTouch.phase = TouchPhase.Ended;
			lastMousePosition = null;
		}
		else // equivalent to touchMoved/Stationary
		{
			fakeTouch.phase = TouchPhase.Moved;
			lastMousePosition = Input.mousePosition;
		}
		
		fakeTouch.position = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		
		return fakeTouch;
	}

}
