using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class UIAbstractContainer : UIObject, IPositionable
{
	public enum UILayoutType { Horizontal, Vertical, BackgroundLayout, AbsoluteLayout };
	private UILayoutType _layoutType;
	public UILayoutType layoutType { get { return _layoutType; } set { _layoutType = value; layoutChildren(); } } // relayout when layoutType changes
	
	protected int _spacing; // spacing is the space between each object
	public int spacing { get { return _spacing; } set { _spacing = value; layoutChildren(); } } // relayout when spacing changes
	
	public UIEdgeInsets _edgeInsets; // pixel padding insets for top, left, bottom and right
	public UIEdgeInsets edgeInsets { get { return _edgeInsets; } set { _edgeInsets = value; layoutChildren(); } } // relayout when edgeInsets changes
	
	protected float _width;
	public new float width { get { return _width; } }

	protected float _height;
	public new float height { get { return _height; } }
	
	protected float _scrollPosition; // scroll position calculated from the top
	
	protected List<UISprite> _children = new List<UISprite>();
	private bool _suspendUpdates; // when true, layoutChildren will do nothing
	
	

	/// <summary>
	/// Hides the container and all of it's children
	/// </summary>
	private bool _hidden = false;
    public virtual bool hidden
    {
        get { return _hidden; }
        set
        {
            // No need to do anything if we're already in this state
            if( value == _hidden )
                return;
			_hidden = value;

			// apply state to the children
			foreach( var child in _children )
				child.hidden = value;
        }
    }


	
	/// <summary>
	/// We need the layout type set from the getgo so we can default to vertical
	/// </summary>
	public UIAbstractContainer() : this( UILayoutType.Vertical )
	{}
	
	
	/// <summary>
	/// We need the layout type set from the getgo so we can lay things out properly
	/// </summary>
	public UIAbstractContainer( UILayoutType layoutType )
	{
		_layoutType = layoutType;
	}


	/// <summary>
	/// Adds a UISprite to the container and sets it to lay itself out
	/// </summary>
	public virtual void addChild( params UISprite[] children )
	{
		foreach( var child in children )
		{
			child.parentUIObject = this;
			_children.Add( child );
		}
		
		layoutChildren();
	}
	

	/// <summary>
	/// Removes a child from the container and optionally from it's manager.  If it is removed from
	/// it's manager it is no longer in existance so be sure to null out any references to it.
	/// </summary>
	public void removeChild( UISprite child, bool removeFromManager )
	{
#if UNITY_EDITOR
		// sanity check while we are in the editor
		if( !_children.Contains( child ) )
			throw new System.Exception( "could not find child in UIAbstractContainer: " + child );
#endif
		_children.Remove( child );
		layoutChildren();
		
		if( removeFromManager )
			child.manager.removeElement( child );
	}


	/// <summary>
	/// Call this when changing multiple properties at once that result in autolayout.  Must be
	/// paired with a call to endUpdates!
	/// </summary>
	public void beginUpdates()
	{
		_suspendUpdates = true;
	}
	
	
	/// <summary>
	/// Commits any update made after beginUpdates was called
	/// </summary>
	public void endUpdates()
	{
		_suspendUpdates = false;
		layoutChildren();
	}
	

	/// <summary>
	/// Responsible for laying out the child UISprites
	/// </summary>
	protected virtual void layoutChildren()
	{
		if( _suspendUpdates )
			return;

		// rules for vertical and horizontal layouts
		if( _layoutType == UIAbstractContainer.UILayoutType.Horizontal || _layoutType == UIAbstractContainer.UILayoutType.Vertical )
		{
			// start with the insets, then add each object + spacing then end with insets
			_width = _edgeInsets.left;
			_height = _edgeInsets.top + _scrollPosition;
				
			if( _layoutType == UIAbstractContainer.UILayoutType.Horizontal )
			{
				var i = 0;
				var lastIndex = _children.Count;
				foreach( var item in _children )
				{
					// we add spacing for all but the first and last
					if( i != 0 && i != lastIndex )
						_width += _spacing;
					
					var yPos = item.gameObjectOriginInCenter ? -item.height / 2 : 0;
					var xPosModifier = item.gameObjectOriginInCenter ? item.width / 2 : 0;
					item.localPosition = new Vector3( _width + xPosModifier, _edgeInsets.top + yPos, item.position.z );
	
					// all items get their width added
					_width += item.width;
					
					// height will just be the height of the tallest item
					if( _height < item.height )
						_height = item.height;
					
					i++;
				}
			}
			else // vertical alignment
			{
				var i = 0;
				var lastIndex = _children.Count;
				foreach( var item in _children )
				{
					// we add spacing for all but the first and last
					if( i != 0 && i != lastIndex )
						_height += _spacing;
					
					var xPos = item.gameObjectOriginInCenter ? item.width / 2 : 0;
					var yPosModifier = item.gameObjectOriginInCenter ? item.height / 2 : 0;
					
					item.localPosition = new Vector3( _edgeInsets.left + xPos, -( _height + yPosModifier ), item.position.z );
	
					// all items get their height added
					_height += item.height;
					
					// width will just be the width of the widest item
					if( _width < item.width )
						_width = item.width;
					
					i++;
				}
			}
			
			// add the right and bottom edge inset to finish things off
			_width += _edgeInsets.right;
			_height += _edgeInsets.bottom;
		}
		else if( _layoutType == UIAbstractContainer.UILayoutType.AbsoluteLayout )
		{
			foreach( var item in _children )
			{
				item.localPosition = new Vector3( item.position.x, item.position.y, item.position.z );
				
				// find the width that contains the item with the largest offset/width
				if( _width < item.localPosition.x + item.width )
					_width = item.localPosition.x + item.width;
				
				// find the height that contains the item with the largest offset/height
				if( _height < -item.localPosition.y + item.height )
					_height = -item.localPosition.y + item.height;
			}
		}
	}


	public override void transformChanged()
	{
		layoutChildren();
	}

}
