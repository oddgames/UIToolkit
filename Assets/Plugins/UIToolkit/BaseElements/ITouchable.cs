using UnityEngine;
using System.Collections;


public interface ITouchable
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	bool hoveredOver { get; set; }
#endif
	bool highlighted { get; set; }
	bool hidden { get; set; }
	bool allowTouchBeganWhenMovedOver { get; set; } // if true, we allow a touch moved over the button to fire onTouchBegan
	Rect touchFrame { get; }
	Vector3 position { get; set; }
	
	
	bool hitTest( Vector2 point );
	
	
	void onTouchBegan( UITouchWrapper touch, Vector2 touchPos );

	
	void onTouchMoved( UITouchWrapper touch, Vector2 touchPos );
	

	void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame );

}
