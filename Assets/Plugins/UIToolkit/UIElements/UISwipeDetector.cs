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
	

	/// <summary>
	/// Creates a swipe detector with no related UI
	/// </summary>
	public static UISwipeDetector create( Rect frame, int depth = 1 )
	{
		// create the swipe detector
		var detector = new UISwipeDetector( frame, depth, UIUVRect.zero );
		UI.instance.addTouchableSprite( detector );
		
		return detector;
	}


	/// <summary>
	/// Creates a swipe detector with a texture backing the hit area
	/// </summary>
	public static UISwipeDetector create( string filename, int xPos, int yPos, int depth = 1 )
	{
		// grab the texture details
		var normalTI = UI.instance.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.size.x, normalTI.size.y );

		// create the swipe detector
		var detector = new UISwipeDetector( frame, depth, normalTI.uvRect );
		UI.instance.addTouchableSprite( detector );
		
		return detector;
	}


	public UISwipeDetector( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		touchInfoArray = new TouchInfo[5];
	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		if( touchInfoArray[touch.fingerId] == null )
			touchInfoArray[touch.fingerId] = new TouchInfo( swipesToDetect );
		
		// Reset the TouchInfo with the current touch
		touchInfoArray[touch.fingerId].resetWithTouch( touch );
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
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
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		//Debug.Log( "TOUCH ENDED" );
		//Debug.Log( string.Format( "x: {0}, y: {1}", touch.position.x, touch.position.y ) );
	}
*/

	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	private bool processTouchInfoWithTouch( TouchInfo touchInfo, UIFakeTouch touch )
#else
	private bool processTouchInfoWithTouch( TouchInfo touchInfo, Touch touch )
#endif
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

