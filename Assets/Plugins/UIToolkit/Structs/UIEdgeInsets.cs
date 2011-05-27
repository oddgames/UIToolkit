using UnityEngine;


public struct UIEdgeInsets
{
	public int top;
	public int left;
	public int bottom;
	public int right;
	
	
	// conveninece constructor to create insets evenly for all 4 sides
	public UIEdgeInsets( int insetForAllSides ) : this( insetForAllSides, insetForAllSides, insetForAllSides, insetForAllSides )
	{
	
	}

	
	public UIEdgeInsets( int top, int left, int bottom, int right )
	{
		var multiplier = UI.instance.isHD ? 2 : 1;
		
		this.top = top * multiplier;
		this.left = left * multiplier;
		this.bottom = bottom * multiplier;
		this.right = right * multiplier;
	}

}
