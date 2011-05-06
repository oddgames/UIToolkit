using UnityEngine;
using System.Collections;


public static class UISpritePositioningExtensions
{
	/// <summary>
	/// Positions a sprite in the center of the screen
	/// </summary
	public static void positionCenter( this UISprite sprite )
	{
		var pos = sprite.position;
		var centerPos = UIRelative.center( sprite.width, sprite.height );
		pos.x = centerPos.x;
		pos.y = -centerPos.y;
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the top-left corner of the screen.  Values are percentage of screen width/height to move away from the corner.
	/// </summary
	public static void positionFromTopLeft( this UISprite sprite, float percentFromTop, float percentFromLeft )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPercentFrom( UIxAnchor.Left, percentFromLeft );
		pos.y = -UIRelative.yPercentFrom( UIyAnchor.Top, percentFromTop );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the top-right corner of the screen.  Values are percentage of screen width/height to move away from the corner.
	/// The right boundary will be measured from the sprites right-most point
	/// </summary
	public static void positionFromTopRight( this UISprite sprite, float percentFromTop, float percentFromRight )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPercentFrom( UIxAnchor.Right, percentFromRight, sprite.width );
		pos.y = -UIRelative.yPercentFrom( UIyAnchor.Top, percentFromTop );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the bottom-left corner of the screen.  Values are percentage of screen width/height to move away from the corner.
	/// The bottom boundary will be measured from the sprites bottom-most point
	/// </summary
	public static void positionFromBottomLeft( this UISprite sprite, float percentFromBottom, float percentFromLeft )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPercentFrom( UIxAnchor.Left, percentFromLeft );
		pos.y = -UIRelative.yPercentFrom( UIyAnchor.Bottom, percentFromBottom, sprite.height );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the bottom-right corner of the screen.  Values are percentage of screen width/height to move away from the corner.
	/// The right boundary will be measured from the sprites right-most point
	/// The bottom boundary will be measured from the sprites bottom-most point
	/// </summary
	public static void positionFromBottomRight( this UISprite sprite, float percentFromBottom, float percentFromRight )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPercentFrom( UIxAnchor.Right, percentFromRight, sprite.width );
		pos.y = -UIRelative.yPercentFrom( UIyAnchor.Bottom, percentFromBottom, sprite.height );
		
		sprite.position = pos;
	}





	/// <summary>
	/// Positions a sprite relatively from the top-left corner of the screen.  Values are pixels from the corner.
	/// </summary
	public static void pixelsFromTopLeft( this UISprite sprite, int pixelsFromTop, int pixelsFromLeft )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPixelsFrom( UIxAnchor.Left, pixelsFromLeft );
		pos.y = -UIRelative.yPixelsFrom( UIyAnchor.Top, pixelsFromTop );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the top-right corner of the screen.  Values are pixels from the corner.
	/// The right boundary will be measured from the sprites right-most point
	/// </summary
	public static void pixelsFromTopRight( this UISprite sprite, int pixelsFromTop, int pixelsFromRight )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPixelsFrom( UIxAnchor.Right, pixelsFromRight, sprite.width );
		pos.y = -UIRelative.yPixelsFrom( UIyAnchor.Top, pixelsFromTop );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the bottom-left corner of the screen.  Values are pixels from the corner.
	/// The bottom boundary will be measured from the sprites bottom-most point
	/// </summary
	public static void pixelsFromBottomLeft( this UISprite sprite, int pixelsFromBottom, int pixelsFromLeft )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPixelsFrom( UIxAnchor.Left, pixelsFromLeft );
		pos.y = -UIRelative.yPixelsFrom( UIyAnchor.Bottom, pixelsFromBottom, sprite.height );
		
		sprite.position = pos;
	}


	/// <summary>
	/// Positions a sprite relatively from the bottom-right corner of the screen.  Values are pixels from the corner.
	/// The right boundary will be measured from the sprites right-most point
	/// The bottom boundary will be measured from the sprites bottom-most point
	/// </summary
	public static void pixelsFromBottomRight( this UISprite sprite, int pixelsFromBottom, int pixelsFromRight )
	{
		var pos = sprite.position;
		pos.x = UIRelative.xPixelsFrom( UIxAnchor.Right, pixelsFromRight, sprite.width );
		pos.y = -UIRelative.yPixelsFrom( UIyAnchor.Bottom, pixelsFromBottom, sprite.height );
		
		sprite.position = pos;
	}

}
