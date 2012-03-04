using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableHorizontalLayout : UIAbstractTouchableContainer
{
	private const int TOTAL_VELOCITY_SAMPLE_COUNT = 3;
	private const float SCROLL_DECELERATION_MODIFIER = 0.93f; // how fast should we slow down
	private float TOUCH_MAX_DELTA_FOR_ACTIVATION = UI.instance.isHD ? 10 : 5;
	private const float CONTENT_TOUCH_DELAY = 0.1f;
	
	private float _deltaTouch;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	private UIFakeTouch _lastTouch;
#else
	private Touch _lastTouch;
#endif
	private Vector2 _lastTouchPosition;
	private ITouchable _activeTouchable;
	private bool _isDragging;
	private bool _isDraggingPastExtents;
	private Queue<float> _velocities = new Queue<float>( TOTAL_VELOCITY_SAMPLE_COUNT );


	public UIScrollableHorizontalLayout( int spacing ) : base( UILayoutType.Horizontal, spacing )
	{}
	
	
	/// <summary>
	/// Springs the scrollable back to its bounds.
	/// </summary>
	/// <returns>
	/// The back to bounds.
	/// </returns>
	/// <param name='elasticityModifier'>
	/// lower numbers will snap back slower than higher
	/// </param>
	private IEnumerator springBackToBounds( float elasticityModifier )
	{
		var targetScrollPosition = 0f;
		if( _scrollPosition < 0 ) // stretching up
			targetScrollPosition = _minEdgeInset.x;
		
		while( !_isDragging )
		{
			// how far from the top/bottom are we?
			var distanceFromTarget = _scrollPosition - targetScrollPosition;

			// we need to know the percentage we are from the source
			var percentFromSource = distanceFromTarget / width;
			
			// how many pixels should we snap back?
			var factor = Mathf.Abs( Mathf.Pow( elasticityModifier, percentFromSource * percentFromSource ) - 0.9f );
			var snapBack = distanceFromTarget * factor;

			_scrollPosition -= snapBack;
			layoutChildren();

			// once we are moving less then a 1/2 pixel stop the animation
			if( Mathf.Abs( snapBack ) < 0.5f )
				break;

			yield return null;
		}
		
		_isDraggingPastExtents = false;
	}


	private IEnumerator decelerate()
	{
		if( _isDraggingPastExtents )
		{
			yield return _manager.StartCoroutine( springBackToBounds( 2f ) );
		}
		else
		{
			// get the average velocity by summing all the velocities and dividing by count
			float total = 0;
			foreach( var v in _velocities )
				total += v;
	
			var avgVelocity = total / _velocities.Count;
			var elasticDecelerationModifier = 0.7f;
			
			while( !_isDragging )
			{
				var deltaMovement = avgVelocity * Time.deltaTime;
				var newOffset = _scrollPosition + deltaMovement;
	
				// make sure we have some velocity and we are within our bounds
				if( Mathf.Abs( avgVelocity ) < 25 )
					break;

				if( newOffset < 0 && newOffset > _minEdgeInset.x )
				{
					_scrollPosition = newOffset;
					layoutChildren();
					avgVelocity *= SCROLL_DECELERATION_MODIFIER;

					yield return null;
				}
				else
				{
					_isDraggingPastExtents = true;
					
					_scrollPosition = newOffset;
					layoutChildren();
					avgVelocity *= elasticDecelerationModifier;
					
					// we up the elasticDecelerationModifier each iteration
					elasticDecelerationModifier -= 0.1f;
					
					if( elasticDecelerationModifier <= 0 )
						break;
					
					yield return null;
				}
			}
			
			if( _isDraggingPastExtents )
				yield return _manager.StartCoroutine( springBackToBounds( 0.9f ) );
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


	private void clipChild( UISprite child )
	{
		var leftContained = child.position.x >= touchFrame.xMin && child.position.x <= touchFrame.xMax;
		var rightContained = child.position.x + child.width >= touchFrame.xMin && child.position.x + child.width <= touchFrame.xMax;
		
		// first, handle if we are fully visible
		if( leftContained && rightContained )
		{
			// unclip if we are clipped
			if( child.clipped )
				child.clipped = false;
			child.hidden = false;
		}
		else if( leftContained || rightContained )
		{
			// wrap the changes in a call to beginUpdates to avoid changing verts more than once
			child.beginUpdates();
			child.hidden = false;
			
			// are we clipping the left or right?
			if( leftContained ) // clipping the right
 			{
				var clippedWidth = touchFrame.xMax - child.position.x;
				
				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( clippedWidth / child.scale.x, child.height / child.scale.y, UIClippingPlane.Right, child.manager.textureSize );
				child.setClippedSize( clippedWidth / child.scale.x, child.height / child.scale.y, UIClippingPlane.Right );
			}
			else // clipping the left, so we need to adjust the position.x as well
 			{
				var clippedWidth = child.width + child.position.x - touchFrame.xMin;
				
				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( clippedWidth / child.scale.x, child.height / child.scale.y, UIClippingPlane.Left, child.manager.textureSize );
				child.setClippedSize( clippedWidth / child.scale.x, child.height / child.scale.y, UIClippingPlane.Left );
			}

			// commit the changes
			child.endUpdates();
		}
		else
		{
			// fully outside our bounds
			child.hidden = true;
		}

		// Recurse
		recurseChildren( child );
	}
	
	
	private void recurseChildren( UIObject child )
	{
		foreach( Transform t in child.client.transform )
		{
			UIElement uie = t.GetComponent<UIElement>();
			if( uie != null )
			{
				UIObject o = t.GetComponent<UIElement>().UIObject;
				if( o != null )
				{
					UISprite s = o as UISprite;
					if( s != null )
					{
						clipChild( s );
					}
					else
					{
						UITextInstance ti = o as UITextInstance;
						if( ti != null )
						{
							// Special handeling for text
							foreach( UISprite glyph in ti.textSprites )
								clipChild( glyph );
						}
						recurseChildren( ti );
					}
				}
			}
		}
	}
	
	
	protected override void clipToBounds()
	{
		// clip hidden children
		foreach( var child in _children )
			clipChild( child );
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
		_deltaTouch += touch.deltaPosition.x;
		_lastTouch = touch;

		// once we move too far unhighlight and stop tracking the touchable
		if( _activeTouchable != null && Mathf.Abs( _deltaTouch ) > TOUCH_MAX_DELTA_FOR_ACTIVATION )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable = null;
		}


		var newOffset = _scrollPosition + touch.deltaPosition.x;
		
		// are we dragging above/below the scrollables boundaries?
		_isDraggingPastExtents = ( newOffset > 0 || newOffset < _minEdgeInset.x );
		
		// if we are dragging past our extents dragging is no longer 1:1. we apply an exponential falloff
		if( _isDraggingPastExtents )
		{
			// how far from the top/bottom are we?
			var distanceFromSource = 0f;
			
			if( newOffset > 0 ) // stretching down
				distanceFromSource = newOffset;
			else
				distanceFromSource = Mathf.Abs( _contentWidth + newOffset - width );
			
			// we need to know the percentage we are from the source
			var percentFromSource = distanceFromSource / width;
			
			// apply exponential falloff so that the further we are from source the less 1 pixel drag actually goes
			newOffset = _scrollPosition + ( touch.deltaPosition.x * Mathf.Pow( 0.04f, percentFromSource ) );
		}
		
		_scrollPosition = newOffset;
		layoutChildren();

		// pop any extra velocities and push the current velocity onto the stack
		if( _velocities.Count == TOTAL_VELOCITY_SAMPLE_COUNT )
			_velocities.Dequeue();
		_velocities.Enqueue( touch.deltaPosition.x / Time.deltaTime );
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
	/// Scrolls to the new offset (using the edgeInsets so 0 is the top and scrolling goes towards negatives)
	/// </summary>
	public void scrollTo( int newOffset, bool animated )
	{
		if( animated )
		{
			_manager.StartCoroutine( scrollToInset( newOffset ) );
		}
		else
		{
			_scrollPosition = newOffset;
			layoutChildren();
		}
	}

}
