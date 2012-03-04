using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Container class that is 
/// </summary>
public abstract class UIAbstractTouchableContainer : UIAbstractContainer, ITouchable, IComparable
{
	protected UIToolkit _manager; // Reference to the sprite manager in which this sprite resides
	protected Vector2 _minEdgeInset; // lets us know how far we can scroll
	
	
	public UIAbstractTouchableContainer( UILayoutType layoutType, int spacing ) : this( UI.firstToolkit, layoutType, spacing )
	{}
	
	
	// Default constructor
	public UIAbstractTouchableContainer( UIToolkit manager, UILayoutType layoutType, int spacing ) : base( layoutType )
	{
		_spacing = spacing;
		_manager = manager;
		
		_manager.addToTouchables( this );
	}
	
	
	protected abstract void clipToBounds();
	
	
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
	
	
	ITouchable TestTouchable( UIObject touchableObj, Vector2 touchPosition )
	{
		foreach( Transform t in touchableObj.client.transform )
		{
			UIElement uie = t.GetComponent<UIElement>();
			if( uie != null )
			{
				UIObject o = t.GetComponent<UIElement>().UIObject;
				if( o != null )
				{
					var touched = TestTouchable( o, touchPosition );
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
				ITouchable touched = TestTouchable( touchable, touchPosition ); // Recursive
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

	
	/// <summary>
	/// Tests if a point is inside the current touchFrame
	/// </summary>
	public bool hitTest( Vector2 point )
	{
		return touchFrame.Contains( point );
	}
	
	
	// Touch handlers.  Subclasses should override these to get their specific behaviour
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public virtual void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public virtual void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{

	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public virtual void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		
	}
	
	#endregion
	
	
	/// <summary>
	/// Sets the size of the touchable area of the layout. This is also where children will be clipped to
	/// </summary>
	public void setSize( float width, float height )
	{
		_touchFrame = new Rect( position.x, -position.y, width, height );
		calculateMinMaxInsets();
	}
	
	
	public override float width
	{
		get { return _touchFrame.width; }
	}


	public override float height
	{
		get { return _touchFrame.height; }
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
				_manager.removeFromTouchables( child as ITouchable );
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
