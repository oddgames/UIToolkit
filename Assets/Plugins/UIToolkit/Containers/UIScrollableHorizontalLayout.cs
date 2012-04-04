using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableHorizontalLayout : UIAbstractTouchableContainer
{
	public UIScrollableHorizontalLayout( int spacing ) : base( UILayoutType.Horizontal, spacing )
	{}

	#region ITouchable

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
			_activeTouchable.onTouchEnded(touch, touchPos, false);
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

	#endregion

}
