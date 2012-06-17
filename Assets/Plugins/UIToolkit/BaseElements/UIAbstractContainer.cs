using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class UIAbstractContainer : UIObject, IPositionable
{
	public enum UIContainerAlignMode { Left, Center, Right };
	private UIContainerAlignMode _alignMode = UIContainerAlignMode.Left;
	public UIContainerAlignMode alignMode { get { return _alignMode; } set { _alignMode = value; layoutChildren(); } } // relayout when alignMode changes

	public enum UIContainerVerticalAlignMode { Top, Middle, Bottom };
	private UIContainerVerticalAlignMode _verticalAlignMode = UIContainerVerticalAlignMode.Top;
	public UIContainerVerticalAlignMode verticalAlignMode { get { return _verticalAlignMode; } set { _verticalAlignMode = value; layoutChildren(); } } // relayout when verticalAlignMode changes

	public enum UILayoutType { Horizontal, Vertical, BackgroundLayout, AbsoluteLayout };
	private UILayoutType _layoutType;
	public UILayoutType layoutType { get { return _layoutType; } set { _layoutType = value; layoutChildren(); } } // relayout when layoutType changes

	protected int _spacing; // spacing is the space between each object
	public int spacing { get { return _spacing; } set { _spacing = value; layoutChildren(); } } // relayout when spacing changes

	public UIEdgeInsets _edgeInsets; // pixel padding insets for top, left, bottom and right
	public UIEdgeInsets edgeInsets { get { return _edgeInsets; } set { _edgeInsets = value; layoutChildren(); } } // relayout when edgeInsets changes

	protected float _scrollPosition; // scroll position calculated from the top (vertical) or left (horizontal)

	protected List<UISprite> _children = new List<UISprite>();
	protected bool _suspendUpdates; // when true, layoutChildren will do nothing


	protected float _contentWidth;
	protected float _contentHeight;

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

		// listen to changes to our own transform so we can move the touchFrame
		this.onTransformChanged += transformChanged;
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
	public virtual void endUpdates()
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

		// Get HD factor
		var hdFactor = 1f / UI.scaleFactor;

		// rules for vertical and horizontal layouts
		if( _layoutType == UIAbstractContainer.UILayoutType.Horizontal || _layoutType == UIAbstractContainer.UILayoutType.Vertical )
		{
			// start with the insets, then add each object + spacing then end with insets
			_contentWidth = _edgeInsets.left;
			_contentHeight = _edgeInsets.top;

			// create UIAnchorInfo to control positioning
			var anchorInfo = UIAnchorInfo.DefaultAnchorInfo();
			anchorInfo.ParentUIObject = this;

			if( _layoutType == UIAbstractContainer.UILayoutType.Horizontal )
			{
				// Set anchor information
				switch( _verticalAlignMode )
				{
					case UIContainerVerticalAlignMode.Top:
						anchorInfo.UIyAnchor = UIyAnchor.Top;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
						anchorInfo.OffsetY = _edgeInsets.top * hdFactor;
						break;
					case UIContainerVerticalAlignMode.Middle:
						anchorInfo.UIyAnchor = UIyAnchor.Center;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
						break;
					case UIContainerVerticalAlignMode.Bottom:
						anchorInfo.UIyAnchor = UIyAnchor.Bottom;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
						anchorInfo.OffsetY = _edgeInsets.bottom * hdFactor;
						break;
				}

				var i = 0;
				var lastIndex = _children.Count;
				foreach( var item in _children )
				{
					// we add spacing for all but the first and last
					if( i != 0 && i != lastIndex )
						_contentWidth += _spacing;

					// Set anchor offset
					anchorInfo.OffsetX = ( _contentWidth + _scrollPosition ) * hdFactor;

					// dont overwrite the sprites origin anchor!
					anchorInfo.OriginUIxAnchor = item.anchorInfo.OriginUIxAnchor;
					anchorInfo.OriginUIyAnchor = item.anchorInfo.OriginUIyAnchor;

					item.anchorInfo = anchorInfo;

					// all items get their width added
					_contentWidth += item.width;

					// height will just be the height of the tallest item
					if( _contentHeight < item.height )
						_contentHeight = item.height;

					i++;
				}
			}
			else // vertical alignment
			{
				// Set anchor information
				switch( _alignMode )
				{
					case UIContainerAlignMode.Left:
						anchorInfo.UIxAnchor = UIxAnchor.Left;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
						anchorInfo.OffsetX = _edgeInsets.left * hdFactor;
						break;
					case UIContainerAlignMode.Center:
						anchorInfo.UIxAnchor = UIxAnchor.Center;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
						break;
					case UIContainerAlignMode.Right:
						anchorInfo.UIxAnchor = UIxAnchor.Right;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
						anchorInfo.OffsetX = _edgeInsets.right * hdFactor;
						break;
				}

				var i = 0;
				var lastIndex = _children.Count;
				foreach( var item in _children )
				{
					// we add spacing for all but the first and last
					if( i != 0 && i != lastIndex )
						_contentHeight += _spacing;

					// Set anchor offset
					anchorInfo.OffsetY = ( _contentHeight + _scrollPosition ) * hdFactor;
					//anchorInfo.OffsetX = item.anchorInfo.OffsetX;
					
					// dont overwrite the sprites origin anchor!
					anchorInfo.OriginUIxAnchor = item.anchorInfo.OriginUIxAnchor;
					anchorInfo.OriginUIyAnchor = item.anchorInfo.OriginUIyAnchor;
					item.anchorInfo = anchorInfo;

					// all items get their height added
					_contentHeight += item.height;

					// width will just be the width of the widest item
					if( _contentWidth < item.width )
						_contentWidth = item.width;

					i++;
				}
			}

			// add the right and bottom edge inset to finish things off
			_contentWidth += _edgeInsets.right;
			_contentHeight += _edgeInsets.bottom;
		}
		else if( _layoutType == UIAbstractContainer.UILayoutType.AbsoluteLayout )
		{
			foreach( var item in _children )
			{
				if( !item.hidden )
				{
					item.localPosition = new Vector3( item.position.x, item.position.y, item.position.z );

					// find the width that contains the item with the largest offset/width
					if( _contentWidth < item.localPosition.x + item.width )
						_contentWidth = item.localPosition.x + item.width;

					// find the height that contains the item with the largest offset/height
					if (_contentHeight < -item.localPosition.y + item.height)
						_contentHeight = -item.localPosition.y + item.height;
				}
			}
		}

		// Refresh child position to proper positions
		foreach( var item in _children )
			item.refreshPosition();
	}


	public override void transformChanged()
	{
		base.transformChanged();
		layoutChildren();
	}
	
	
	public void matchSizeToContentSize()
	{
		_width = _contentWidth;
		_height = _contentHeight;
	}

}
