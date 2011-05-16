using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIVerticalLayout : UIHorizontalLayout
{	
	public UIVerticalLayout( int spacing, int padding ) : base( spacing, padding )
	{
	}


	/// <summary>
	/// Responsible for laying out the child UISprites
	/// </summary>
	protected override void layoutChildren()
	{
		// start with the padding, then add each object + spacing then end with padding
		_width = 0;
		_height = padding;
		
		var i = 0;
		var lastIndex = _children.Count;
		foreach( var item in _children )
		{
			// we add spacing for all but the first and last
			if( i != 0 && i != lastIndex )
				_height += spacing;
			
			var xPos = item.gameObjectOriginInCenter ? item.width / 2 : 0;
			var yPosModifier = item.gameObjectOriginInCenter ? item.height / 2 : 0;
			item.position = new Vector3( xPos, -( _height + yPosModifier ), item.position.z );
			
			// all items get their height added
			_height += item.height;
			
			// width will just be the width of the tallest item
			if( _width < item.width )
				_width = item.width;
			
			i++;
		}
		
		_height += padding;
	}

}
