using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface IPositionable
{
	float width { get; }
	float height { get; }
	Vector3 position { get; set; }
}


public class HorizontalLayout : UIObject, IPositionable
{
	public int spacing; // spacing is the space between each object
	public int padding; // padding is the space before the first element and after the last element
	
	private float _width;
	public float width { get { return _width; } }

	private float _height;
	public float height { get { return _height; } }
	
	
	private List<UISprite> _children = new List<UISprite>();

	
	public HorizontalLayout( int spacing, int padding )
	{
		this.spacing = spacing;
		this.padding = padding;
	}
	

	/// <summary>
	/// Adds a UISprite to the container and sets it to lay itself out
	/// </summary>
	public void addChild( params UISprite[] children )
	{
		foreach( var child in children )
		{
			child.parentUIObject = this;
			_children.Add( child );
		}
		
		layoutChildren();
	}
	

	/// <summary>
	/// Removes a 
	/// </summary>
	public void removeChild( UISprite child )
	{
#if UNITY_EDITOR
		// sanity check while we are in the editor
		if( !_children.Contains( child ) )
			throw new System.Exception( "could not find child in HorizontalLayout: " + child );
#endif
		_children.Remove( child );
		layoutChildren();
	}
	

	/// <summary>
	/// 
	/// </summary>
	private void layoutChildren()
	{
		// start with the padding, then add each object + spacing then end with padding
		_width = padding;
		_height = 0;
		
		var i = 0;
		var lastIndex = _children.Count;
		foreach( var item in _children )
		{
			// we add spacing for all but the first and last
			if( i != 0 && i != lastIndex )
				_width += spacing;
			
			var yPos = item.gameObjectOriginInCenter ? -item.height / 2 : 0;
			var xPosModifier = item.gameObjectOriginInCenter ? item.width / 2 : 0;
			item.position = new Vector3( _width + xPosModifier, yPos, item.position.z );
			
			// all items get their width added
			_width += item.width;
			
			// height will just be the height of the tallest item
			if( _height < item.height )
				_height = item.height;
			
			i++;
		}
		
		_width += padding;
	}

}
