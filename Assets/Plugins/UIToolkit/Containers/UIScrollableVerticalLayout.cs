using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableVerticalLayout : UIAbstractTouchableContainer
{
	public UIScrollableVerticalLayout( int spacing ) : base( UILayoutType.Vertical, spacing )
	{}


	protected override void clipChild( UISprite child )
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

				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Bottom, child.manager.textureSize );
				child.setClippedSize( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Bottom );
			}
			else // clipping the top, so we need to adjust the position.y as well
 			{
				var clippedHeight = child.height - child.position.y - touchFrame.yMin;

				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Top, child.manager.textureSize );
				child.setClippedSize( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Top );
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
		recurseAndClipChildren( child );
	}
	

	#region ITouchable

	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		// increment deltaTouch so we can pass on the touch if necessary
		_deltaTouch += touch.deltaPosition.y;
		_lastTouch = touch;

		// once we move too far unhighlight and stop tracking the touchable
		if( _activeTouchable != null && Mathf.Abs( _deltaTouch ) > TOUCH_MAX_DELTA_FOR_ACTIVATION )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable = null;
		}


		var newTop = _scrollPosition - touch.deltaPosition.y;
		
		// are we dragging above/below the scrollables boundaries?
		_isDraggingPastExtents = ( newTop > 0 || newTop < _minEdgeInset.y );
		
		// if we are dragging past our extents dragging is no longer 1:1. we apply an exponential falloff
		if( _isDraggingPastExtents )
		{
			// how far from the top/bottom are we?
			var distanceFromSource = 0f;
			
			if( newTop > 0 ) // stretching down
				distanceFromSource = newTop;
			else
				distanceFromSource = Mathf.Abs( _contentHeight + newTop - height );
			
			// we need to know the percentage we are from the source
			var percentFromSource = distanceFromSource / height;
			
			// apply exponential falloff so that the further we are from source the less 1 pixel drag actually goes
			newTop = _scrollPosition - ( touch.deltaPosition.y * Mathf.Pow( 0.04f, percentFromSource ) );
		}
		
		
		_scrollPosition = newTop;
		layoutChildren();

		// pop any extra velocities and push the current velocity onto the stack
		if( _velocities.Count == TOTAL_VELOCITY_SAMPLE_COUNT )
			_velocities.Dequeue();
		_velocities.Enqueue( touch.deltaPosition.y / Time.deltaTime );
	}

	#endregion


}
