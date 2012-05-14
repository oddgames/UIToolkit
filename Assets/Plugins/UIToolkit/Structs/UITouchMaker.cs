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
/// this class now exists only to allow standalones/web players to create Touch objects
/// </summary>
public struct UITouchMaker
{
	public static Touch createTouch( int finderId, int tapCount, Vector2 position, Vector2 deltaPos, float timeDelta, TouchPhase phase )
	{
		var self = new Touch();
		ValueType valueSelf = self;
		var type = typeof( Touch );
		
		type.GetField( "m_FingerId", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, finderId );
		type.GetField( "m_TapCount", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, tapCount );
		type.GetField( "m_Position", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, position );
		type.GetField( "m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, deltaPos );
		type.GetField( "m_TimeDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, timeDelta );
		type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, phase );
		
		return (Touch)valueSelf;
	}
	
	
	public static Touch createTouchFromInput( UIMouseState mouseState, ref Vector2? lastMousePosition )
	{
		var self = new Touch();
		ValueType valueSelf = self;
		var type = typeof( Touch );
		
		var currentMousePosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		
		// if we have a lastMousePosition use it to get a delta
		if( lastMousePosition.HasValue )
			type.GetField( "m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, currentMousePosition - lastMousePosition );
		
		if( mouseState == UIMouseState.DownThisFrame ) // equivalent to touchBegan
		{
			type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Began );
			lastMousePosition = Input.mousePosition;
		}
		else if( mouseState == UIMouseState.UpThisFrame ) // equivalent to touchEnded
		{
			type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Ended );
			lastMousePosition = null;
		}
		else // UIMouseState.HeldDown - equivalent to touchMoved/Stationary
		{
			type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, TouchPhase.Moved );
			lastMousePosition = Input.mousePosition;
		}
		
		type.GetField( "m_Position", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, currentMousePosition );
		
		return (Touch)valueSelf;
	}
}
#endif