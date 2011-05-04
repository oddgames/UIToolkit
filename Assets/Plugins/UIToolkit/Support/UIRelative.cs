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
	public static int xPercentFrom( UIxAnchor anchor, float percentOffset, int width = 0 )
	{
		switch( anchor )
		{
			case UIxAnchor.Left:
				return (int)( percentOffset * Screen.width );
			case UIxAnchor.Right:
				return (int)( Screen.width - ( percentOffset * Screen.width ) - width );
		}
		return 0;
	}


	/// <summary>
	/// Pixels to offset from the anchor.  If the anchor is right, the width will be used to make the offset
	/// from the right-most point of the sprite.
	/// </summary>
	public static int xPixelsFrom( UIxAnchor anchor, int pixelOffset, int width = 0 )
	{
		switch( anchor )
		{
			case UIxAnchor.Left:
				return pixelOffset;
			case UIxAnchor.Right:
				return Screen.width - pixelOffset - width;
		}
		return 0;
	}


	/// <summary>
	/// Percent to offset from the anchor.  If the anchor is bottom, the height will be used to make the offset
	/// from the height-most point of the sprite.
	/// </summary>
	public static int yPercentFrom( UIyAnchor anchor, float percentOffset, int height = 0 )
	{
		switch( anchor )
		{
			case UIyAnchor.Top:
				return (int)( percentOffset * Screen.height );
			case UIyAnchor.Bottom:
				return (int)( Screen.height - ( percentOffset * Screen.height ) - height );
		}
		return 0;
	}


	/// <summary>
	/// Pixels to offset from the anchor.  If the anchor is bottom, the height will be used to make the offset
	/// from the bottom-most point of the sprite.
	/// </summary>
	public static int yPixelsFrom( UIyAnchor anchor, int pixelOffset, int height = 0 )
	{
		switch( anchor )
		{
			case UIyAnchor.Top:
				return pixelOffset;
			case UIyAnchor.Bottom:
				return (int)( Screen.height - pixelOffset - height );
		}
		return 0;
	}


	public static UIVector2 center( float width, float height )
	{
		var pos = UIVector2.zero;
		
		pos.x = (int)( Screen.width / 2 - width / 2 );
		pos.y = (int)( Screen.height / 2 - height / 2 );
		
		return pos;
	}

}
