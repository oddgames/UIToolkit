using System;
using UnityEngine;


public class UISwipeDetector : UITouchableSprite
{
	private TouchInfo[] touchInfoArray;
	
	public delegate void UISwipeDetectorDetectedSwipe( UISwipeDetector sender, SwipeDirection direction );
	public event UISwipeDetectorDetectedSwipe onSwipe; // event for when we get a swipe
	
	public float timeToSwipe = 0.5f;	
	public float allowedVariance = 35.0f;
	public float minimumDistance = 40.0f;
	public SwipeDirection swipesToDetect = SwipeDirection.All;
	public float swipeVelocity = 0f;
	
	
	/// <summary>
	/// Creates a swipe detector with no related UI
	/// </summary>
	public static UISwipeDetector create( Rect frame, int depth )
	{
		return create( UI.firstToolkit, frame, depth );
	}
	

	/// <summary>
	/// Creates a swipe detector with no related UI
	/// </summary>
	public static UISwipeDetector create( UIToolkit manager, Rect frame, int depth )
	{
		// create the swipe detector
		var detector = new UISwipeDetector( frame, depth, UIUVRect.zero );
		manager.addTouchableSprite( detector );
		
		return detector;
	}


	/// <summary>
	/// Creates a swipe detector with a texture backing the hit area
	/// </summary>
	public static UISwipeDetector create( UIToolkit manager, string filename, int xPos, int yPos, int depth )
	{
		// grab the texture details
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );

		// create the swipe detector
		var detector = new UISwipeDetector( frame, depth, normalTI.uvRect );
		manager.addTouchableSprite( detector );
		
		return detector;
	}


	public UISwipeDetector( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		touchInfoArray = new TouchInfo[12];
	}
	

	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		if( touchInfoArray[touch.fingerId] == null )
			touchInfoArray[touch.fingerId] = new TouchInfo( swipesToDetect );
		
		// Reset the TouchInfo with the current touch
		touchInfoArray[touch.fingerId].resetWithTouch( touch );
	}


	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		if( processTouchInfoWithTouch( touchInfoArray[touch.fingerId], touch ) )
		{
			// Got a swipe
			if( onSwipe != null )
				onSwipe( this, touchInfoArray[touch.fingerId].completedSwipeDirection );
			touchInfoArray[touch.fingerId].swipeDetectionStatus = SwipeDetectionStatus.Done;
		}
	}


/* Only used for debugging
	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		//Debug.Log( "TOUCH ENDED" );
		//Debug.Log( string.Format( "x: {0}, y: {1}", touch.position.x, touch.position.y ) );
	}
*/

	
	private bool processTouchInfoWithTouch( TouchInfo touchInfo, UITouchWrapper touch )
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
		

		// When dealing with standalones (non touch-based devices) we need to be careful what we examaine
		// We filter out all touches (mouse movements really) that didnt move
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		if( touch.deltaPosition.x != 0.0f || touch.deltaPosition.y != 0.0f )
		{
#endif

		// Check the delta move positions.  We can rule out at least 2 directions
		if( touch.deltaPosition.x > 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Left;
		if( touch.deltaPosition.x < 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Right;
		
		if( touch.deltaPosition.y < 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Up;			
		if( touch.deltaPosition.y > 0.0f )
			touchInfo.swipeDetectionState &= ~SwipeDirection.Down;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		}
#endif
		
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
					swipeVelocity = xDeltaAbs / timeToSwipe;
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
					swipeVelocity = xDeltaAbs / timeToSwipe;
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
					swipeVelocity = yDeltaAbs / timeToSwipe;
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
					swipeVelocity = yDeltaAbs / timeToSwipe;
					return true;
				}
				
				// We exceeded our variance so this swipe is no longer allowed
				touchInfo.swipeDetectionState &= ~SwipeDirection.Down;
			}
		}
		
		return false;
	}

}

