using UnityEngine;
using System.Collections;


public enum UIxAnchor { Left, Right };
public enum UIyAnchor { Top, Bottom };


public static class UIRelative
{
	/// <summary>
	/// Determine if running in HD and multiply pixelOffsets accordingly.
	/// </summary>
	public static int pixelDensityMultiplier()
	{
		return UI.instance.isHD ? 2 : 1;
	}

	
	/// <summary>
	/// Percent to offset from the anchor.  If the anchor is right, the width will be used to make the offset
	/// from the right-most point of the sprite.
	/// </summary>
	public static float xPercentFrom( UIxAnchor anchor, float percentOffset )
	{
		return xPercentFrom( anchor, percentOffset, 0 );
	}


	public static float xPercentFrom( UIxAnchor anchor, float percentOffset, float width )
	{
		switch( anchor )
		{
			case UIxAnchor.Left:
				return percentOffset * Screen.width;
			case UIxAnchor.Right:
				return Screen.width - ( percentOffset * Screen.width ) - width;
		}
		return 0f;
	}


	/// <summary>
	/// Pixels to offset from the anchor.  If the anchor is right, the width will be used to make the offset
	/// from the right-most point of the sprite.
	/// </summary>
	public static float xPixelsFrom( UIxAnchor anchor, int pixelOffset )
	{
		return xPixelsFrom( anchor, pixelOffset, 0 );
	}


	public static float xPixelsFrom( UIxAnchor anchor, int pixelOffset, float width )
	{
		switch( anchor )
		{
			case UIxAnchor.Left:
				return pixelOffset * pixelDensityMultiplier();
			case UIxAnchor.Right:
				return Screen.width - pixelOffset*pixelDensityMultiplier() - width;
		}
		return 0f;
	}


	/// <summary>
	/// Percent to offset from the anchor.  If the anchor is bottom, the height will be used to make the offset
	/// from the height-most point of the sprite.
	/// </summary>
	public static float yPercentFrom( UIyAnchor anchor, float percentOffset )
	{
		return yPercentFrom( anchor, percentOffset, 0 );
	}


	public static float yPercentFrom( UIyAnchor anchor, float percentOffset, float height )
	{
		switch( anchor )
		{
			case UIyAnchor.Top:
				return percentOffset * Screen.height;
			case UIyAnchor.Bottom:
				return Screen.height - ( percentOffset * Screen.height ) - height;
		}
		return 0f;
	}


	/// <summary>
	/// Pixels to offset from the anchor.  If the anchor is bottom, the height will be used to make the offset
	/// from the bottom-most point of the sprite.
	/// </summary>
	public static float yPixelsFrom( UIyAnchor anchor, int pixelOffset )
	{
		return yPixelsFrom( anchor, pixelOffset, 0 );
	}


	public static float yPixelsFrom( UIyAnchor anchor, int pixelOffset, float height )
	{
		switch( anchor )
		{
			case UIyAnchor.Top:
				return pixelOffset * pixelDensityMultiplier();
			case UIyAnchor.Bottom:
				return Screen.height - pixelOffset*pixelDensityMultiplier() - height;
		}
		return 0f;
	}


	public static Vector2 center( float width, float height )
	{
		var pos = Vector2.zero;
		
		pos.x = Screen.width / 2 - width / 2;
		pos.y = Screen.height / 2 - height / 2;
		
		return pos;
	}

}
