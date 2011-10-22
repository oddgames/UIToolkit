using UnityEngine;
using System.Collections;



public abstract class UIAbstractTouchableContainer : UIAbstractContainer, ITouchable
{
	protected UIToolkit _manager; // Reference to the sprite manager in which this sprite resides

	
	public UIAbstractTouchableContainer( UILayoutType layoutType, int spacing ) : this( UI.firstToolkit, layoutType, spacing )
	{
		
	}
	
	
	// Default constructor
	public UIAbstractTouchableContainer( UIToolkit manager, UILayoutType layoutType, int spacing ) : base( layoutType )
	{
		_spacing = spacing;
		_manager = manager;
		
		_manager.addToTouchables( this );
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
		Debug.Log( "Moved!!" );
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
	/// Override so that we can remove the touchable sprites. The container needs to manage all touches.
	/// </summary>
	public override void addChild( params UISprite[] children )
	{
		base.addChild( children );
		
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
