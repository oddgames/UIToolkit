using System;
using UnityEngine;

public class UITouchWrapper
{
	
	private int m_FingerId = 0;
	private Vector2 m_Position = new Vector2();
	private Vector2 m_PositionDelta = new Vector2();
	private float m_TimeDelta = 0f;
	private int m_TapCount = 0;
	private TouchPhase m_Phase = TouchPhase.Stationary;
	
	private bool m_Locked = false;
	
	public UITouchWrapper()
	{
	}
	
	
	public bool locked
	{
		get
		{
			return m_Locked;
		}
		
		set
		{
			if(value == true) m_Locked = true;
		}
	}
	
	public int fingerId
	{
		get
		{
			return this.m_FingerId;
		}
		set
		{
			if(!locked) this.m_FingerId = value;
		}
	}
	
	public Vector2 position
	{
		get
		{
			return this.m_Position;
		}
		set
		{
			if(!locked) this.m_Position = value;
		}
	}
	public Vector2 deltaPosition
	{
		get
		{
			return this.m_PositionDelta;
		}
		set
		{
			if(!locked) this.m_PositionDelta = value;
		}
	}
	
	public float deltaTime
	{
		get
		{
			return this.m_TimeDelta;
		}
		set
		{
			if(!locked) this.m_TimeDelta = value;
		}
	}
	public int tapCount
	{
		get
		{
			return this.m_TapCount;
		}
		set
		{
			if(!locked) this.m_TapCount = value;
		}
	}
	public TouchPhase phase
	{
		get
		{
			return this.m_Phase;
		}
		set
		{
			if(!locked) this.m_Phase = value;
		}
	}
	
}

