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
		this.top = top;
		this.left = left;
		this.bottom = bottom;
		this.right = right;
	}


	// Used to expand or contract a rect by this
	public Rect addToRect( Rect frame )
	{
		// Clamp x and y to be greater than zero
		return new Rect
		(
			 Mathf.Clamp( frame.x - left, 0, Screen.width ),
			 Mathf.Clamp( frame.y - top, 0, Screen.height ),
			 frame.width + right + left,
			 frame.height + bottom + top
		);
	}
	
}
