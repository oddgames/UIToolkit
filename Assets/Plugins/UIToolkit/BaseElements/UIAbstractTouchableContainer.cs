using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Container class that is 
/// </summary>
public abstract class UIAbstractTouchableContainer : UIAbstractContainer, ITouchable, IComparable
{
	#region properties and fields
	
	protected UIToolkit _manager; // Reference to the sprite manager in which this sprite resides
	protected Vector2 _minEdgeInset; // lets us know how far we can scroll
	
	protected const int TOTAL_VELOCITY_SAMPLE_COUNT = 3;
	protected const float SCROLL_DECELERATION_MODIFIER = 0.93f; // how fast should we slow down
	protected float TOUCH_MAX_DELTA_FOR_ACTIVATION = 5; // this is the SD setting. the constructor will modify this for HD/XD
	protected const float CONTENT_TOUCH_DELAY = 0.1f;
	
	protected bool _isDragging;
	protected bool _isDraggingPastExtents;
	protected Queue<float> _velocities = new Queue<float>( TOTAL_VELOCITY_SAMPLE_COUNT );
	
	// touch handling helpers
	protected float _deltaTouch;
	protected Touch _lastTouch;
	protected Vector2 _lastTouchPosition;
	protected ITouchable _activeTouchable;
	
	// paging
	public bool pagingEnabled; // enables paging support which will snap scrolling. page size is the container width
	public float pageWidth; // width for page snapping. should be set to the size of the items in the container
	
	
	public override float width
	{
		get { return _touchFrame.width; }
	}


	public override float height
	{
		get { return _touchFrame.height; }
	}
	
	#endregion
	
	
	public UIAbstractTouchableContainer( UILayoutType layoutType, int spacing ) : this( UI.firstToolkit, layoutType, spacing )
	{}
	
	
	// Default constructor
	public UIAbstractTouchableContainer( UIToolkit manager, UILayoutType layoutType, int spacing ) : base( layoutType )
	{
		TOUCH_MAX_DELTA_FOR_ACTIVATION *= UI.scaleFactor;
		_spacing = spacing * UI.scaleFactor;
		_manager = manager;
		
		_manager.addToTouchables( this );
	}
	
	
	/// <summary>
	/// Springs the scrollable back to its bounds.
	/// </summary>
	/// <returns>
	/// The back to bounds.
	/// </returns>
	/// <param name='elasticityModifier'>
	/// lower numbers will snap back slower than higher
	/// </param>
	protected IEnumerator springBackToBounds( float elasticityModifier )
	{
		var targetScrollPosition = 0f;
		if( _scrollPosition < 0 ) // stretching up or right
		{
			if( layoutType == UIAbstractContainer.UILayoutType.Horizontal )
				targetScrollPosition = _minEdgeInset.x;
			else
				targetScrollPosition = _minEdgeInset.y;
		}
		
		while( !_isDragging )
		{
			// how far from the top/bottom are we?
			var distanceFromTarget = _scrollPosition - targetScrollPosition;

			// we need to know the percentage we are from the source
			var divisor = layoutType == UIAbstractContainer.UILayoutType.Horizontal ? width : height;
			var percentFromSource = distanceFromTarget / divisor;
			
			// how many pixels should we snap back?
			var factor = Mathf.Abs( Mathf.Pow( elasticityModifier, percentFromSource * percentFromSource ) - 0.9f );
			var snapBack = distanceFromTarget * factor;

			_scrollPosition -= snapBack;
			layoutChildren();

			// once we are moving less then a 0.2 pixel stop the animation and hit out target
			if( Mathf.Abs( snapBack ) < 0.2f )
			{
				_scrollPosition = targetScrollPosition;
				break;
			}

			yield return null;
		}
		
		layoutChildren();
		
		_isDraggingPastExtents = false;
	}
	
	
	protected IEnumerator decelerate()
	{
		if( _isDraggingPastExtents )
		{
			yield return _manager.StartCoroutine( springBackToBounds( 2f ) );
		}
		else if( _velocities.Count > 0 ) // bail out if we have no velocities!
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
				var newOffset = _scrollPosition;

				if( layoutType == UIAbstractContainer.UILayoutType.Horizontal )
					newOffset += deltaMovement;
				else
					newOffset -= deltaMovement;
				
				var absVelocity = Mathf.Abs( avgVelocity );
				
				// if paging is enabled once we slow down we will snap to a page
				if( pagingEnabled && absVelocity < 2500 )
				{
					// if we are past the extents let the handler below do the scrolling
					if( !_isDraggingPastExtents )
						scrollToNearestPage();
					break;
				}
				
				// make sure we have some velocity and we are within our bounds
				if( absVelocity < 25 )
					break;
				
				// use x for horizontal and y for vertical
				float lowerBounds;
				if( layoutType == UIAbstractContainer.UILayoutType.Horizontal )
					lowerBounds = _minEdgeInset.x;
				else
					lowerBounds = _minEdgeInset.y;
				
				if( newOffset < 0 && newOffset > lowerBounds )
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
	
	
	private void scrollToNearestPage()
	{
		// which page is closest?
		var page = Mathf.RoundToInt( Math.Abs( _scrollPosition ) / ( pageWidth + spacing ) );
		scrollToPage( page );
	}


	protected IEnumerator scrollToInset( int target )
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

			if( easPos == 1 )
				running = false;

			yield return null;
		}
		layoutChildren();
	}
	

	protected IEnumerator checkDelayedContentTouch()
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
	
	
	protected abstract void clipChild( UISprite child );
	
	
	protected void clipToBounds()
	{
		// clip hidden children
		foreach( var child in _children )
			clipChild( child );
	}
	
	
	protected void recurseAndClipChildren( UIObject child )
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
						recurseAndClipChildren( ti );
					}
				}
			}
		}
	}


	protected override void layoutChildren()
	{
		base.layoutChildren();
		clipToBounds();
	}


	public override void transformChanged()
	{
		// we moved so adjust the touchFrame
		setSize( _touchFrame.width, _touchFrame.height );
		
		// call through to base which will relayout our children
		base.transformChanged();
	}
	
	
	public override void endUpdates()
	{
		base.endUpdates();
	
		// after updates finish and everything is layed out we can grab the content size
		calculateMinMaxInsets();
	}

	
	/// <summary>
	/// Calcualates the min/max edge inset in both the x and y direction. This is called in response to the touchFrame
	/// or contentSize of the container changing
	/// </summary>
	private void calculateMinMaxInsets()
	{
		_minEdgeInset.x = -_contentWidth + _touchFrame.width;
		_minEdgeInset.y = -_contentHeight + _touchFrame.height;
		
		// now that we have new insets clip
		clipToBounds();
	}
	
	
	ITouchable testTouchable( UIObject touchableObj, Vector2 touchPosition )
	{
		foreach( Transform t in touchableObj.client.transform )
		{
			UIElement uie = t.GetComponent<UIElement>();
			if( uie != null )
			{
				UIObject o = t.GetComponent<UIElement>().UIObject;
				if( o != null )
				{
					var touched = testTouchable( o, touchPosition );
					if( touched != null )
						return touched;
				}
			}
		}

		ITouchable touchable = touchableObj as ITouchable;
		if( touchable != null )
		{
			if( touchable.hitTest( touchPosition ) )
				return touchable as ITouchable;
		}

		return null;
	}
	
	
	protected ITouchable getButtonForScreenPosition( Vector2 touchPosition )
	{
		// we loop backwards so that any clipped elements at the top dont try to override the hitTest
		// due to their frame overlapping the touchable below
		for( var i = _children.Count - 1; i >= 0; i-- )
		{
			var touchable = _children[i];
			if( touchable != null )
			{
				ITouchable touched = testTouchable( touchable, touchPosition ); // Recursive
				if( touched != null )
					return touched;
			}
		}
		
		return null;
	}
	
	
	#region ITouchable
	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public bool hoveredOver { get; set; } // not really used for containers
#endif
	public bool highlighted { get; set; } // not really used for containers
	protected Rect _touchFrame;
	public Rect touchFrame { get { return _touchFrame; } } // we dont allow setting through the setter. the method is empty only to implement the interface
	public bool allowTouchBeganWhenMovedOver { get; set; }

	
	/// <summary>
	/// Tests if a point is inside the current touchFrame
	/// </summary>
	public bool hitTest( Vector2 point )
	{
		return touchFrame.Contains( point );
	}
	
	
	// Touch handlers.  Subclasses should override onTouchMoved
	public virtual void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		// sanity check in case we lost a touch (happens with Unity on occassion)
		if( _activeTouchable != null )
		{
			// we dont pass onTouchEnded here because technically we are still over the ITouchable
			_activeTouchable.highlighted = false;
			_activeTouchable = null;
		}

		_deltaTouch = 0;
		_isDragging = true;
		_velocities.Clear();

		// kick off a new check
		_manager.StartCoroutine( checkDelayedContentTouch() );
	}


	public virtual void onTouchMoved( Touch touch, Vector2 touchPos )
	{

	}


	public virtual void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
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
	
	
	public void scrollToPage( int page )
	{
		// take the spacing into account when scrolling
		var pageSpacing = page * spacing;
		_manager.StartCoroutine( scrollToInset( (int)( -page * pageWidth - pageSpacing ) ) );
	}
	
	
	/// <summary>
	/// Sets the size of the touchable area of the layout. This is also where children will be clipped to
	/// </summary>
	public void setSize( float width, float height )
	{
		_touchFrame = new Rect( position.x, -position.y, width, height );
		calculateMinMaxInsets();
	}
	
	
	/// <summary>
	/// Clear all childs
	/// </summary>
	public void Clear() {
		_children.Clear();
		layoutChildren();
	}	
	
	
	/// <summary>
	/// Override so that we can remove the touchable sprites. The container needs to manage all touches.
	/// </summary>
	public override void addChild( params UISprite[] children )
	{
		base.addChild( children );
		
		// after the children are added we can grab the width/height which are freshly calculated if _suspendUpdates is false
		if( !_suspendUpdates )
			calculateMinMaxInsets();
		
		// we will manually handle touches so remove any children that are touchable
		foreach( var child in children )
		{
			if( child is ITouchable )
				child.manager.removeFromTouchables( child as ITouchable );
		}
	}
	
	
    /// <summary>
    /// IComparable - sorts based on the z value of the client
    /// </summary>
	public int CompareTo( object obj )
    {
        if( obj is ITouchable )
        {
            var temp = obj as ITouchable;
            return position.z.CompareTo( temp.position.z );
        }
		
		return -1;
    }

}
