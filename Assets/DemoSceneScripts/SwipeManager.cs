using UnityEngine;
using System.Collections;


public class SwipeManager : MonoBehaviour
{
	void Start()
	{
		var swipeDetector = UISwipeDetector.create( new Rect( 0, 0, Screen.width, Screen.height ), 1 );
		swipeDetector.onSwipe += detectedSwipe;	
	}


	void detectedSwipe( UISwipeDetector sender, SwipeDirection direction )
	{
		Debug.Log( "swipe direction: " + direction );
	}

}
