using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableVerticalLayout : UIAbstractTouchableContainer
{
	private const int TOTAL_VELOCITY_SAMPLE_COUNT = 2;
	private const float SCROLL_DECELERATION_MODIFIER = 0.93f; // how fast should we slow down
	private float TOUCH_MAX_DELTA_FOR_ACTIVATION = UI.instance.isHD ? 10 : 5;
	private const float CONTENT_TOUCH_DELAY = 0.15f;
	
	private float _deltaTouch;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	private UIFakeTouch _lastTouch;
#else
	private Touch _lastTouch;
#endif
	private Vector2 _lastTouchPosition;
	private ITouchable _activeTouchable;
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
			var newTop = _scrollPosition - deltaMovement;
			
			// make sure we have some velocity and we are within our bounds
			if( Mathf.Abs( avgVelocity ) > 25 && newTop < _maxEdgeInset.y && newTop > _minEdgeInset.y )
			{
				_scrollPosition = newTop;
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
	
	
	private IEnumerator scrollToInset( int target )
	{
		var start = _scrollPosition;
		var startTime = Time.time;
		var duration = 0.4f;
		var running = true;
		
		while( !_isDragging && running )
		{
			// Get our easing position
			var easPos = Mathf.Clamp01( ( Time.time - startTime ) / duration );
			easPos = Easing.Quartic.easeOut( easPos );
			
			_scrollPosition = (int)Mathf.Lerp( start, target, easPos );
			layoutChildren();
			
			if( ( startTime + duration ) <= Time.time )
				running = false;
		
			yield return null;
		}
		layoutChildren();
	}
	
	
	public IEnumerator checkDelayedContentTouch()
	{
		yield return new WaitForSeconds( CONTENT_TOUCH_DELAY );
		
		if( _isDragging && Mathf.Abs( _deltaTouch ) < TOUCH_MAX_DELTA_FOR_ACTIVATION )
		{
			var fixedTouchPosition = new Vector2( _lastTouch.position.x, Screen.height - _lastTouch.position.y );
			_activeTouchable = getButtonForScreenPosition( fixedTouchPosition );
			if( _activeTouchable != null )
				_activeTouchable.onTouchBegan( _lastTouch, fixedTouchPosition );
		}
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
				// wrap the changes in a call to beginUpdates to avoid changing verts more than once
				child.beginUpdates();
				
				child.hidden = false;
				
				// are we clipping the top or bottom?
				if( topContained ) // clipping the bottom
				{
					var clippedHeight = child.position.y + touchFrame.yMax;
					
					child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width, clippedHeight, false, child.manager.textureSize );
					child.setClippedSize( child.width, clippedHeight, false );
				}
				else // clipping the top, so we need to adjust the position.y as well
				{
					var clippedHeight = child.height - child.position.y - touchFrame.yMin;
					
					child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width, clippedHeight, true, child.manager.textureSize );
					child.setClippedSize( child.width, clippedHeight, true );
				}
				
				// commit the changes
				child.endUpdates();
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
		// sanity check in case we lost a touch (happens with Unity on occassion)
		if( _activeTouchable != null )
		{
			// we dont pass onTouchEnded here because technically we are still over the ITouchable
			_activeTouchable.highlighted = false;
			_activeTouchable = null;
		}
		
		// if we have a couroutine running stop it
		//_manager.StopCoroutine( "checkDelayedContentTouch" );
		
		_deltaTouch = 0;
		_isDragging = true;
		_velocities.Clear();
		
		// kick off a new check
		_manager.StartCoroutine( checkDelayedContentTouch() );
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{
		// increment deltaTouch so we can pass on the touch if necessary
		_deltaTouch += touch.deltaPosition.y;
		_lastTouch = touch;
		
		// once we move too far unhighlight and stop tracking the touchable
		if( Mathf.Abs( _deltaTouch ) > TOUCH_MAX_DELTA_FOR_ACTIVATION && _activeTouchable != null )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable = null;
		}
			
		
		var newTop = _scrollPosition - touch.deltaPosition.y;
		if( newTop < _maxEdgeInset.y && newTop > _minEdgeInset.y ) // movement within the bounds (ie no bounce yet)
		{
			_scrollPosition = newTop;
			layoutChildren();
		}

		// pop any extra velocities and push the current velocity onto the stack
		if( _velocities.Count == TOTAL_VELOCITY_SAMPLE_COUNT )
			_velocities.Pop();
		_velocities.Push( touch.deltaPosition.y / Time.deltaTime );
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		_isDragging = false;
		
		// pass on the touch if we still have an active touchable
		if( _activeTouchable != null )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable.highlighted = false;
			_activeTouchable = null;
		}
		else
		{
			_manager.StartCoroutine( decelerate() );
		}
	}
	
	#endregion
	
	
	protected override void layoutChildren()
	{
		base.layoutChildren();
		clipToBounds();
	}
	
	
	/// <summary>
	/// Scrolls to the newTop (using the edgeInsets so 0 is the top and scrolling goes towards negatives)
	/// </summary>
	public void scrollTo( int newTop, bool animated )
	{
		if( animated )
		{
			_manager.StartCoroutine( scrollToInset( newTop ) );
		}
		else
		{
			_scrollPosition = newTop;
			layoutChildren();
		}
	}
	
}
