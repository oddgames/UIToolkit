using UnityEngine;
using System.Collections;


public enum UIxAnchor { Left, Right };
public enum UIyAnchor { Top, Bottom };


public static class UIRelative
{
	/// <summary>
	/// Percent to offset from the anchor.  If the anchor is right, the width will be used to make the offset
	/// from the right-most point of the sprite.
	/// </summary>
	public static float xPercentFrom( UIxAnchor anchor, float percentOffset, float width = 0f )
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
	public static float xPixelsFrom( UIxAnchor anchor, float pixelOffset, float width = 0f )
	{
		switch( anchor )
		{
			case UIxAnchor.Left:
				return pixelOffset;
			case UIxAnchor.Right:
				return Screen.width - pixelOffset - width;
		}
		return 0f;
	}


	/// <summary>
	/// Percent to offset from the anchor.  If the anchor is bottom, the height will be used to make the offset
	/// from the height-most point of the sprite.
	/// </summary>
	public static float yPercentFrom( UIyAnchor anchor, float percentOffset, float height = 0f )
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
	public static float yPixelsFrom( UIyAnchor anchor, float pixelOffset, float height = 0f )
	{
		switch( anchor )
		{
			case UIyAnchor.Top:
				return pixelOffset;
			case UIyAnchor.Bottom:
				return Screen.height - pixelOffset - height;
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
