using System;
using UnityEngine;


[Flags]
public enum SwipeDirection
{
    Left 		= ( 1 << 0 ),
    Right 		= ( 1 << 1 ),
    Up 			= ( 1 << 2 ),
    Down 		= ( 1 << 4 ),
    Horizontal 	= ( Left | Right ),
    Vertical 	= ( Up | Down ),
    All 		= ( Horizontal | Vertical )
}

public enum SwipeDetectionStatus
{
	Waiting,
	Failed,
	Done
}


public class TouchInfo
{
	public Vector2 startPoint;
	public float startTime;
	public SwipeDirection swipeDetectionState; // The current swipes that are still possibly valid
	public SwipeDirection completedSwipeDirection; // If a successful swipe occurs, this will be the type
	public SwipeDirection swipesToDetect; // Bitmask of SwipeDirections with the swipes that should be looked for
	public SwipeDetectionStatus swipeDetectionStatus; // Current status of the detector
	
	
	public TouchInfo( SwipeDirection swipesToDetect )
	{
		this.swipesToDetect = swipesToDetect;
		startPoint = Vector2.zero;
		startTime = 0.0f;
		swipeDetectionState = SwipeDirection.Horizontal;
		completedSwipeDirection = 0;
		swipeDetectionStatus = SwipeDetectionStatus.Waiting;
	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public void resetWithTouch( UIFakeTouch touch )
#else
	public void resetWithTouch( Touch touch )
#endif
	{
		// Initialize the detectionState only with the swipe types we want to listen for
		swipeDetectionState = swipesToDetect;
		startPoint = touch.position;
		startTime = Time.time;
		swipeDetectionStatus = SwipeDetectionStatus.Waiting;
	}
	
}
