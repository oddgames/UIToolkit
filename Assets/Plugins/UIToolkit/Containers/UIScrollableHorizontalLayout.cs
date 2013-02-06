using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UIScrollableHorizontalLayout : UIAbstractTouchableContainer
{
	public UIScrollableHorizontalLayout( int spacing ) : base( UILayoutType.Horizontal, spacing )
	{}

	
	protected override void clipChild( UISprite child )
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
		recurseAndClipChildren( child );
	}
	
	
	#region ITouchable

	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
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

	#endregion

}
