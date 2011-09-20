using UnityEngine;

public struct UIEdgeOffsets
{
	public int top;
	public int left;
	public int bottom;
	public int right;
	
	
	// conveninece constructor to create offsets evenly for all 4 sides
	public UIEdgeOffsets( int offsetForAllSides ) : this( offsetForAllSides, offsetForAllSides, offsetForAllSides, offsetForAllSides )
	{
	
	}

	
	public UIEdgeOffsets( int top, int left, int bottom, int right )
	{
		var multiplier = UI.instance.isHD ? 2 : 1;
		
		this.top = top * multiplier;
		this.left = left * multiplier;
		this.bottom = bottom * multiplier;
		this.right = right * multiplier;
	}
}
