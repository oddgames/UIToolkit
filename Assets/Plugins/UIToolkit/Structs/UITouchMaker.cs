using UnityEngine;
using System;
using System.Collections;
using System.Reflection;



#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
public enum UIMouseState
{
	UpThisFrame,
	DownThisFrame,
	HeldDown
};


/// <summary>
/// this class now exists only to allow standalones/web players to create UITouchWrapper objects
/// </summary>
public struct UITouchMaker
{
	public static UITouchWrapper createTouch( int finderId, int tapCount, Vector2 position, Vector2 deltaPos, float timeDelta, TouchPhase phase )
	{
		var self = new UITouchWrapper();
		//ValueType valueSelf = self;
		//var type = typeof( UITouchWrapper );
		/*
		type.GetField( "m_FingerId", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, finderId );
		type.GetField( "m_TapCount", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, tapCount );
		type.GetField( "m_Position", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, position );
		type.GetField( "m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, deltaPos );
		type.GetField( "m_TimeDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, timeDelta );
		type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, phase );
		
		return (UITouchWrapper)valueSelf;
		*/
		
		self.fingerId = finderId;
		self.tapCount = tapCount;
		self.position = position;
		self.deltaPosition = deltaPos;
		self.deltaTime = timeDelta;
		self.phase = phase;
		return self;
	}
	
	
	public static UITouchWrapper createTouchFromInput( UIMouseState mouseState, ref Vector2? lastMousePosition )
	{
		//var self = new UITouchWrapper();
		//ValueType valueSelf = self;
		//var type = typeof( UITouchWrapper );
		
		var self = new UITouchWrapper();
		
		var currentMousePosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		
		if(lastMousePosition.HasValue)
		// if we have a lastMousePosition use it to get a delta
		if( lastMousePosition.HasValue ) self.deltaPosition = currentMousePosition - (Vector2)lastMousePosition;
			//type.GetField( "m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, currentMousePosition - lastMousePosition );
		
		if( mouseState == UIMouseState.DownThisFrame ) // equivalent to touchBegan
		{
			//type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Began );
			self.phase = TouchPhase.Began;
			lastMousePosition = Input.mousePosition;
		}
		else if( mouseState == UIMouseState.UpThisFrame ) // equivalent to touchEnded
		{
			//type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Ended );
			self.phase = TouchPhase.Ended;
			lastMousePosition = null;
		}
		else // UIMouseState.HeldDown - equivalent to touchMoved/Stationary
		{
			//type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Moved );
			self.phase = TouchPhase.Moved;
			lastMousePosition = Input.mousePosition;
		}
		
		//type.GetField( "m_Position", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, currentMousePosition );
		self.position = currentMousePosition;
		
		return self;
		//return (UITouchWrapper)valueSelf;
	}
}
#endif