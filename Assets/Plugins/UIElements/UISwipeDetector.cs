using System;
using UnityEngine;


public delegate void UISwipeDetectorDetectedSwipe( UISwipeDetector sender, SwipeDirection direction );

public class UISwipeDetector : UITouchableSprite
{
	private TouchInfo[] touchInfoArray;
	
	public UISwipeDetectorDetectedSwipe action = null; // Delegate for when we get a swipe
	public float timeToSwipe = 0.5f;	
	public float allowedVariance = 35.0f;
	public float minimumDistance = 40.0f;
	public SwipeDirection swipesToDetect = SwipeDirection.All;

	public UISwipeDetector( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		touchInfoArray = new TouchInfo[5];
	}
	
	
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		if( touchInfoArray[touch.fingerId] == null )
			touchInfoArray[touch.fingerId] = new TouchInfo( swipesToDetect );
		
		// Reset the TouchInfo with the current touch
		touchInfoArray[touch.fingerId].resetWithTouch( touch );
	}

	
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
	{
		if( processTouchInfoWithTouch( touchInfoArray[touch.fingerId], touch ) )
		{
			// Got a swipe
			action( this, touchInfoArray[touch.fingerId].completedSwipeDirection );
			touchInfoArray[touch.fingerId].swipeDetectionStatus = SwipeDetectionStatus.Done;
		}
	}
	
	
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		//Debug.Log( "TOUCH ENDED" );
		//Debug.Log( string.Format( "x: {0}, y: {1}", touch.position.x, touch.position.y ) );
	}
	
	
	private bool processTouchInfoWithTouch( TouchInfo touchInfo, Touch touch )
	{
		// If we already completed the swipe detection or if none are availalbe get out of here
		if( touchInfo.swipeDetectionStatus != SwipeDetectionStatus.Waiting )
			return false;

		// If we have a time stipulation and we exceeded it stop listening for swipes
		if( timeToSwipe > 0.0f && ( Time.time - touchInfo.startTime ) > timeToSwipe )
		{
			touchInfo.swipeDetectionStatus = SwipeDetectionStatus.Failed;
			return false;
		}
		
		// Check the delta move positions.  We can rule out at least 2 directions
		if( touch.deltaPosition.x > 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Left;
		else
			touchInfo.swipeDetectionState &= ~SwipeDirection.Right;
		
		if( touch.deltaPosition.y < 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Up;
		else
			touchInfo.swipeDetectionState &= ~SwipeDirection.Down;
		
		//Debug.Log( string.Format( "swipeStatus: {0}", touchInfo.swipeDetectionState ) );
		
		// Grab the total distance moved in both directions
		float xDeltaAbs = Math.Abs( touchInfo.startPoint.x - touch.position.x );
		float yDeltaAbs = Math.Abs( touchInfo.startPoint.y - touch.position.y );
		
		// Only check for swipes in directions that havent been ruled out yet
		if( ( touchInfo.swipeDetectionState & SwipeDirection.Left ) != 0 )
		{
			if( xDeltaAbs > minimumDistance )
			{
				if( yDeltaAbs < allowedVariance )
				{
					touchInfo.completedSwipeDirection = SwipeDirection.Left;
					return true;
				}
				
				// We exceeded our variance so this swipe is no longer allowed
				touchInfo.swipeDetectionState &= ~SwipeDirection.Left;
			}
		}

		
		// Right check
		if( ( touchInfo.swipeDetectionState & SwipeDirection.Right ) != 0 )
		{
			if( xDeltaAbs > minimumDistance )
			{
				if( yDeltaAbs < allowedVariance )
				{
					touchInfo.completedSwipeDirection = SwipeDirection.Right;
					return true;
				}
				
				// We exceeded our variance so this swipe is no longer allowed
				touchInfo.swipeDetectionState &= ~SwipeDirection.Right;
			}
		}
		
		// Up check
		if( ( touchInfo.swipeDetectionState & SwipeDirection.Up ) != 0 )
		{
			if( yDeltaAbs > minimumDistance )
			{
				if( xDeltaAbs < allowedVariance )
				{
					touchInfo.completedSwipeDirection = SwipeDirection.Up;
					return true;
				}
				
				// We exceeded our variance so this swipe is no longer allowed
				touchInfo.swipeDetectionState &= ~SwipeDirection.Up;
			}
		}
		
		// Down check
		if( ( touchInfo.swipeDetectionState & SwipeDirection.Down ) != 0 )
		{
			if( yDeltaAbs > minimumDistance )
			{
				if( xDeltaAbs < allowedVariance )
				{
					touchInfo.completedSwipeDirection = SwipeDirection.Down;
					return true;
				}
				
				// We exceeded our variance so this swipe is no longer allowed
				touchInfo.swipeDetectionState &= ~SwipeDirection.Down;
			}
		}
		
		return false;
	}
}

