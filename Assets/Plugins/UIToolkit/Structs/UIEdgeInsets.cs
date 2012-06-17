using UnityEngine;


public struct UIEdgeInsets
{
	public int top;
	public int left;
	public int bottom;
	public int right;
	
	
	// conveninece constructor to create insets evenly for all 4 sides
	public UIEdgeInsets( int insetForAllSides ) : this( insetForAllSides, insetForAllSides, insetForAllSides, insetForAllSides )
	{}

	
	public UIEdgeInsets( int top, int left, int bottom, int right )
	{
		this.top = top * UI.scaleFactor;
		this.left = left * UI.scaleFactor;
		this.bottom = bottom * UI.scaleFactor;
		this.right = right * UI.scaleFactor;
	}

}
