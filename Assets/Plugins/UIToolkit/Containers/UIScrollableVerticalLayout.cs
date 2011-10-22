using UnityEngine;
using System.Collections;


/*
#define SCROLL_DEACCEL_RATE  0.95f
#define SCROLL_DEACCEL_DIST  1.0f
#define BOUNCE_DURATION      0.35f
*/


public class UIScrollableVerticalLayout : UIAbstractTouchableContainer
{
	private bool _isDragging;
	
	
	public UIScrollableVerticalLayout( int spacing ) : base( UILayoutType.Vertical, spacing )
	{}
	
	
	private IEnumerator decelerate()
	{
		while( !_isDragging )
		{
			Debug.Log( Time.deltaTime );
			yield return null;
		}
	}
	
	
	public void setSize( float width, float height )
	{
		_touchFrame = new Rect( position.x, -position.y, width, height );
	}
	
	
	#region ITouchable

	// Touch handlers.  Subclasses should override these to get their specific behaviour
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		_isDragging = true;
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		var yPos = position.y;
		var maxScroll = height - touchFrame.height;
		
		int newTop = _edgeInsets.top - (int)touch.deltaPosition.y;
		if( newTop < 0 && maxScroll > 0 )
			_edgeInsets.top = newTop;
		
		
		Debug.Log( "ed.top: " + newTop + ", touchHeight: " + _touchFrame.height + ", maxScroll: " + maxScroll );
		
		layoutChildren();
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		_isDragging = false;
		_manager.StartCoroutine( decelerate() );
	}
	
	#endregion

}
