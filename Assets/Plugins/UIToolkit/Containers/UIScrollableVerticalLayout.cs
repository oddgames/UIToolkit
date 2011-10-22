using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableVerticalLayout : UIAbstractTouchableContainer
{
	private const int TOTAL_VELOCITY_SAMPLE_COUNT = 7;
	private const float SCROLL_DECELERATION_MODIFIER = 0.95f; // how fast should we slow down
	private bool _isDragging;
	private Stack<float> _velocities = new Stack<float>( TOTAL_VELOCITY_SAMPLE_COUNT );
	
	
	public UIScrollableVerticalLayout( int spacing ) : base( UILayoutType.Vertical, spacing )
	{
		
	}
	
	
	private IEnumerator decelerate()
	{
		// get the average velocity by summing all the velocities and dividing by count
		float total = 0;
		foreach( var v in _velocities )
			total += v;
		
		var avgVelocity = total / _velocities.Count;
		
		while( !_isDragging )
		{
			var deltaMovement = avgVelocity * Time.deltaTime;
			int newTop = _edgeInsets.top - (int)deltaMovement;
			
			// make sure we have some velocity and we are within our bounds
			if( Mathf.Abs( avgVelocity ) > 25 && newTop < _maxEdgeInset.y && newTop > _minEdgeInset.y )
			{
				_edgeInsets.top = newTop;
				layoutChildren();
				avgVelocity *= SCROLL_DECELERATION_MODIFIER;
				
				yield return null;
			}
			else
			{
				break;
			}
		}
	}
	
	
	private ITouchable getButtonForScreenPosition( Vector2 touchPosition )
	{
		for( int i = 0, totalChildren = _children.Count; i < totalChildren; i++ )
		{
			var touchable = _children[i] as ITouchable;
			if( touchable != null )
			{
				if( touchable.hitTest( touchPosition ) )
					return touchable;
			}
		}
		
		return null;
	}
	
	
	protected override void clipToBounds()
	{
		// clip hidden children
		foreach( var child in _children )
		{
			var topContained = child.position.y < -touchFrame.yMin && child.position.y > -touchFrame.yMax;
			var bottomContained = child.position.y - child.height < -touchFrame.yMin && child.position.y - child.height > -touchFrame.yMax;
			
			// first, handle if we are fully visible
			if( topContained && bottomContained )
			{
				// unclip if we are clipped
				if( child.clipped )
					child.clipped = false;
				child.hidden = false;
			}
			else if( topContained || bottomContained )
			{
				child.hidden = false;
				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( _manager.textureSize );
			}
			else
			{
				// fully outside our bounds
				child.hidden = true;
			}
		}
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
		_velocities.Clear();
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		int newTop = _edgeInsets.top - (int)touch.deltaPosition.y;
		if( newTop < _maxEdgeInset.y && newTop > _minEdgeInset.y )
		{
			_edgeInsets.top = newTop;
			layoutChildren();
		}
		
		// pop any extra velocities and push the current velocity onto the stack
		if( _velocities.Count == TOTAL_VELOCITY_SAMPLE_COUNT )
			_velocities.Pop();
		_velocities.Push( touch.deltaPosition.y / Time.deltaTime );
		
		//Debug.Log( "ed.top: " + newTop + ", maxEdge: " + _maxEdgeInset.y + ", minEdge: " + _minEdgeInset.y );
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
	
	
	protected override void layoutChildren()
	{
		base.layoutChildren();
		clipToBounds();
	}

}
