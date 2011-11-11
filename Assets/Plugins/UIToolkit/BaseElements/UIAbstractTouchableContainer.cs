using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Container class that is 
/// </summary>
public abstract class UIAbstractTouchableContainer : UIAbstractContainer, ITouchable, IComparable
{
	protected UIToolkit _manager; // Reference to the sprite manager in which this sprite resides
	protected Vector2 _contentSize;
	protected Vector2 _minEdgeInset;
	protected Vector2 _maxEdgeInset;
	
	
	public UIAbstractTouchableContainer( UILayoutType layoutType, int spacing ) : this( UI.firstToolkit, layoutType, spacing )
	{}
	
	
	// Default constructor
	public UIAbstractTouchableContainer( UIToolkit manager, UILayoutType layoutType, int spacing ) : base( layoutType )
	{
		_spacing = spacing;
		_manager = manager;
		
		_manager.addToTouchables( this );
		
		// listen to changes to our own transform so we can move the touchFrame
		this.onTransformChanged += transformChanged;
	}
	
	
	protected abstract void clipToBounds();
	
	
	public override void transformChanged()
	{
		// we moved so adjust the touchFrame
		setSize( _touchFrame.width, _touchFrame.height );
		
		// call through to base which will relayout our children
		base.transformChanged();
	}

	
	/// <summary>
	/// Calcualates the min/max edge inset in both the x and y direction. This is called in response to the touchFrame
	/// or contentSize of the container changing
	/// </summary>
	private void calculateMinMaxInsets()
	{
		_minEdgeInset.x = _contentSize.x - _touchFrame.width;
		_minEdgeInset.y = -_contentSize.y + _touchFrame.height;
		
		_maxEdgeInset.y = 0;
		_maxEdgeInset.x = 0;
		
		// now that we have new insets clip
		clipToBounds();
	}
	

	protected ITouchable getButtonForScreenPosition( Vector2 touchPosition )
	{
		// we loop backwards so that any clipped elements at the top dont try to override the hitTest
		// due to their frame overlapping the touchable below
		for( int i = _children.Count - 1; i >= 0; i-- )
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

	
	/// <summary>
	/// Override so that we can remove the touchable sprites. The container needs to manage all touches.
	/// </summary>
	public override void addChild( params UISprite[] children )
	{
		base.addChild( children );
		
		// after the children are added we can grab the width/height which are freshly calculated
		_contentSize.x = width;
		_contentSize.y = height;
		calculateMinMaxInsets();
		
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
