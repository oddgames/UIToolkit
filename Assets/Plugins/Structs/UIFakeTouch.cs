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


	public static UIFakeTouch fromInput()
	{
		var fakeTouch = new UIFakeTouch();
		fakeTouch.fingerId = 2;
		
		if( Input.GetMouseButtonDown( 0 ) )
			fakeTouch.phase = TouchPhase.Began;
		else if( Input.GetMouseButtonUp( 0 ) )
			fakeTouch.phase = TouchPhase.Ended;
		else
			fakeTouch.phase = TouchPhase.Moved;
		
		fakeTouch.position = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		
		return fakeTouch;
	}

}
