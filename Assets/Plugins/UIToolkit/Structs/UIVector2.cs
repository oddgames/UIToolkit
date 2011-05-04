using UnityEngine;
using System.Collections;


// vector with 2 ints instead of floats.  useful for pixel perfect layout
public struct UIVector2
{
	public int x;
	public int y;
	
	
	public UIVector2( int x, int y )
	{
		this.x = x;
		this.y = y;
	}
	
	
	public static UIVector2 zero
	{
		get
		{
			return new UIVector2( 0, 0 );
		}
	}

}
