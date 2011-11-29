using UnityEngine;

public static class IPositionablePositioningExtensions
{
    #region Relative offset methods


    /// <summary>
    /// Positions a sprite in the center of its parent.
    /// </summary>
    /// <param name="sprite"></param>
    public static void positionCenter( this IPositionable sprite )
    {
        sprite.positionFromCenter( 0f, 0f );
    }


    /// <summary>
    /// Positions a sprite relatively to the center of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromCenter( this IPositionable sprite, float percentFromTop, float percentFromLeft )
    {
        sprite.positionFromCenter( percentFromTop, percentFromLeft, UIyAnchor.Center, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the center of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromCenter( this IPositionable sprite, float percentFromTop, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-left corner of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromTopLeft( this IPositionable sprite, float percentFromTop, float percentFromLeft )
    {
        sprite.positionFromTopLeft( percentFromTop, percentFromLeft, UIyAnchor.Top, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-left corner of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromTopLeft( this IPositionable sprite, float percentFromTop, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-right corner of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    public static void positionFromTopRight( this IPositionable sprite, float percentFromTop, float percentFromRight )
    {
        sprite.positionFromTopRight( percentFromTop, percentFromRight, UIyAnchor.Top, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-right corner of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromTopRight( this IPositionable sprite, float percentFromTop, float percentFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromRight;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-left corner of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromBottomLeft( this IPositionable sprite, float percentFromBottom, float percentFromLeft )
    {
        sprite.positionFromBottomLeft( percentFromBottom, percentFromLeft, UIyAnchor.Bottom, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-left corner of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromBottomLeft( this IPositionable sprite, float percentFromBottom, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-right corner of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    public static void positionFromBottomRight( this IPositionable sprite, float percentFromBottom, float percentFromRight )
    {
        sprite.positionFromBottomRight( percentFromBottom, percentFromRight, UIyAnchor.Bottom, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-right corner of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromBottomRight( this IPositionable sprite, float percentFromBottom, float percentFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromRight;
        anchorInfo.OffsetY = percentFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parent.
    /// Value is percentage of screen height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    public static void positionFromTop( this IPositionable sprite, float percentFromTop )
    {
        sprite.positionFromTop( percentFromTop, 0f, UIyAnchor.Top, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromTop( this IPositionable sprite, float percentFromTop, float percentFromLeft )
    {
        sprite.positionFromTop( percentFromTop, percentFromLeft, UIyAnchor.Top, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromTop( this IPositionable sprite, float percentFromTop, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parent.
    /// Value is percentage of screen height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    public static void positionFromBottom( this IPositionable sprite, float percentFromBottom )
    {
        sprite.positionFromBottom( percentFromBottom, 0f, UIyAnchor.Bottom, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromBottom( this IPositionable sprite, float percentFromBottom, float percentFromLeft )
    {
        sprite.positionFromBottom( percentFromBottom, percentFromLeft, UIyAnchor.Bottom, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromBottom">Percentage from bottom - positive values places the sprite closer to the top</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromBottom( this IPositionable sprite, float percentFromBottom, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent.
    /// Value is percentage of screen width to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromLeft( this IPositionable sprite, float percentFromLeft )
    {
        sprite.positionFromLeft(0f, percentFromLeft, UIyAnchor.Center, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    public static void positionFromLeft( this IPositionable sprite, float percentFromTop, float percentFromLeft )
    {
        sprite.positionFromLeft( percentFromTop, percentFromLeft, UIyAnchor.Center, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromLeft">Percentage from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromLeft( this IPositionable sprite, float percentFromTop, float percentFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromLeft;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent.
    /// Value is percentage of screen width to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    public static void positionFromRight( this IPositionable sprite, float percentFromRight )
    {
        sprite.positionFromRight(0f, percentFromRight, UIyAnchor.Center, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    public static void positionFromRight( this IPositionable sprite, float percentFromTop, float percentFromRight )
    {
        sprite.positionFromRight( percentFromTop, percentFromRight, UIyAnchor.Center, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent, with specific local anchors.
    /// Values are percentage of screen width/height to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="percentFromTop">Percentage from top - positive values places the sprite closer to the bottom</param>
    /// <param name="percentFromRight">Percentage from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void positionFromRight( this IPositionable sprite, float percentFromTop, float percentFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = percentFromRight;
        anchorInfo.OffsetY = percentFromTop;
        anchorInfo.UIPrecision = UIPrecision.Percentage;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }

    #endregion


    #region Pixel offset methods

    /// <summary>
    /// Positions a sprite relatively to the center of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromCenter( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft )
    {
        sprite.pixelsFromCenter( pixelsFromTop, pixelsFromLeft, UIyAnchor.Center, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the center of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromCenter( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-left corner of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromTopLeft( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft )
    {
        sprite.pixelsFromTopLeft( pixelsFromTop, pixelsFromLeft, UIyAnchor.Top, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-left corner of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromTopLeft( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-right corner of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    public static void pixelsFromTopRight( this IPositionable sprite, int pixelsFromTop, int pixelsFromRight )
    {
        sprite.pixelsFromTopRight( pixelsFromTop, pixelsFromRight, UIyAnchor.Top, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-right corner of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromTopRight( this IPositionable sprite, int pixelsFromTop, int pixelsFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromRight;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-left corner of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromBottomLeft( this IPositionable sprite, int pixelsFromBottom, int pixelsFromLeft )
    {
        sprite.pixelsFromBottomLeft( pixelsFromBottom, pixelsFromLeft, UIyAnchor.Bottom, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-left corner of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromBottomLeft( this IPositionable sprite, int pixelsFromBottom, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-right corner of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    public static void pixelsFromBottomRight( this IPositionable sprite, int pixelsFromBottom, int pixelsFromRight )
    {
        sprite.pixelsFromBottomRight( pixelsFromBottom, pixelsFromRight, UIyAnchor.Bottom, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-right corner of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromBottomRight( this IPositionable sprite, int pixelsFromBottom, int pixelsFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromRight;
        anchorInfo.OffsetY = pixelsFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parents.
    /// Value is pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    public static void pixelsFromTop( this IPositionable sprite, int pixelsFromTop )
    {
        sprite.pixelsFromTop( pixelsFromTop, 0, UIyAnchor.Top, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromTop( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft )
    {
        sprite.pixelsFromTop( pixelsFromTop, pixelsFromLeft, UIyAnchor.Top, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the top-center of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromTop( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parents.
    /// Value is pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    public static void pixelsFromBottom( this IPositionable sprite, int pixelsFromBottom )
    {
        sprite.pixelsFromBottom( pixelsFromBottom, 0, UIyAnchor.Bottom, UIxAnchor.Center );
    }

    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromBottom( this IPositionable sprite, int pixelsFromBottom, int pixelsFromLeft )
    {
        sprite.pixelsFromBottom( pixelsFromBottom, pixelsFromLeft, UIyAnchor.Bottom, UIxAnchor.Center );
    }


    /// <summary>
    /// Positions a sprite relatively to the bottom-center of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromBottom">Pixels from bottom - positive values places the sprite closer to the top</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromBottom( this IPositionable sprite, int pixelsFromBottom, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromBottom;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent.
    /// Value is pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromLeft( this IPositionable sprite, int pixelsFromLeft )
    {
        sprite.pixelsFromLeft(0, pixelsFromLeft, UIyAnchor.Center, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    public static void pixelsFromLeft( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft )
    {
        sprite.pixelsFromLeft( pixelsFromTop, pixelsFromLeft, UIyAnchor.Center, UIxAnchor.Left );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-left of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromLeft">Pixels from left - positive values places the sprite closer to the right</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromLeft( this IPositionable sprite, int pixelsFromTop, int pixelsFromLeft, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromLeft;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent.
    /// Value is pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    public static void pixelsFromRight( this IPositionable sprite, int pixelsFromRight )
    {
        sprite.pixelsFromRight(0, pixelsFromRight, UIyAnchor.Center, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    public static void pixelsFromRight( this IPositionable sprite, int pixelsFromTop, int pixelsFromRight )
    {
        sprite.pixelsFromRight( pixelsFromTop, pixelsFromRight, UIyAnchor.Center, UIxAnchor.Right );
    }


    /// <summary>
    /// Positions a sprite relatively to the center-right of its parent, with specific local anchors.
    /// Values are pixels to move away from the parent.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pixelsFromTop">Pixels from top - positive values places the sprite closer to the bottom</param>
    /// <param name="pixelsFromRight">Pixels from right - positive values places the sprite closer to the left</param>
    /// <param name="yAnchor">Sprite vertical anchor</param>
    /// <param name="xAnchor">Sprite horizontal anchor</param>
    public static void pixelsFromRight( this IPositionable sprite, int pixelsFromTop, int pixelsFromRight, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        // Update anchor information
        UIAnchorInfo anchorInfo = sprite.anchorInfo;
        anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
        anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
        anchorInfo.UIxAnchor = xAnchor;
        anchorInfo.UIyAnchor = yAnchor;
        anchorInfo.OffsetX = pixelsFromRight;
        anchorInfo.OffsetY = pixelsFromTop;
        anchorInfo.UIPrecision = UIPrecision.Pixel;

        // Set new anchor information
        sprite.anchorInfo = anchorInfo;

        // Refresh position
        sprite.refreshPosition();
    }

    #endregion


    /// <summary>
    /// Adjusts anchor offsets for a new parent anchor and updates the sprite's anchor infomation.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="newParent"></param>
    public static void adjustAnchorForNewParent( this IPositionable sprite, IPositionable newParent )
    {
        // Get anchor info
        UIAnchorInfo anchorInfo = sprite.anchorInfo;

        // Get parent anchors
        Vector3 oldParentAnchorPosition = parentAnchorPosition( anchorInfo.ParentUIObject, anchorInfo.ParentUIyAnchor, anchorInfo.ParentUIxAnchor );
        Vector3 newParentAnchorPosition = parentAnchorPosition( newParent, anchorInfo.ParentUIyAnchor, anchorInfo.ParentUIxAnchor );
        Vector3 diffAnchor = newParentAnchorPosition - oldParentAnchorPosition;

        // Adjust for sprite anchor offset
        diffAnchor.x -= UIRelative.xAnchorAdjustment( anchorInfo.UIxAnchor, sprite.width, anchorInfo.OriginInCenter );
        diffAnchor.y += UIRelative.yAnchorAdjustment( anchorInfo.UIyAnchor, sprite.height, anchorInfo.OriginInCenter );

        // Adjust parent anchor offsets
        if ( anchorInfo.UIPrecision == UIPrecision.Percentage )
        {
            anchorInfo.OffsetX += UIRelative.xPercentTo( anchorInfo.UIxAnchor, diffAnchor.x );
            anchorInfo.OffsetY -= UIRelative.yPercentTo( anchorInfo.UIyAnchor, diffAnchor.y );
        }
        else
        {
            anchorInfo.OffsetX += UIRelative.xPixelsTo( anchorInfo.UIxAnchor, diffAnchor.x );
            anchorInfo.OffsetY -= UIRelative.yPixelsTo( anchorInfo.UIyAnchor, diffAnchor.y );
        }

        // Update parent
        anchorInfo.ParentUIObject = newParent;

        // Set update anchor info
        sprite.anchorInfo = anchorInfo;
    }


    /// <summary>
    /// Refreshes the sprite's position according to its anchor information.
    /// </summary>
    /// <param name="sprite"></param>
    public static void refreshPosition( this IPositionable sprite )
    {
        // Get anchor info
        UIAnchorInfo anchorInfo = sprite.anchorInfo;

        // Get parent anchor position
				Vector3 position = parentAnchorPosition( anchorInfo.ParentUIObject, anchorInfo.ParentUIyAnchor, anchorInfo.ParentUIxAnchor );


        // Add position offset
        if ( anchorInfo.UIPrecision == UIPrecision.Percentage )
        {
						position.x += xPercentFromParent( anchorInfo.ParentUIObject,anchorInfo.UIxAnchor, anchorInfo.OffsetX );
						position.y -= yPercentFromParent( anchorInfo.ParentUIObject, anchorInfo.UIyAnchor, anchorInfo.OffsetY );
        }
        else
        {
            position.x += UIRelative.xPixelsFrom( anchorInfo.UIxAnchor, anchorInfo.OffsetX );
            position.y -= UIRelative.yPixelsFrom( anchorInfo.UIyAnchor, anchorInfo.OffsetY );
        }

        // Adjust for anchor offset
        position.x -= UIRelative.xAnchorAdjustment( anchorInfo.UIxAnchor, sprite.width, anchorInfo.OriginInCenter );
        position.y += UIRelative.yAnchorAdjustment( anchorInfo.UIyAnchor, sprite.height, anchorInfo.OriginInCenter );

        // Set new position
        sprite.position = position;
    }


    /// <summary>
    /// Returns anchor position for a given parent and fallback to Screen if parent is null.
    /// </summary>
    /// <param name="sprite">Provided parent</param>
    /// <param name="yAnchor">Vertical anchor</param>
    /// <param name="xAnchor">Horizontal anchor</param>
    /// <returns>Adjusted anchor position</returns>
    private static Vector3 parentAnchorPosition( IPositionable sprite, UIyAnchor yAnchor, UIxAnchor xAnchor )
    {
        Vector3 position;
        float width, height;
        bool originInCenter = false;
        // Determine correct parent values
        if ( sprite == null )
        {
            position = Vector3.zero;
            width = Screen.width;
            height = Screen.height;
        }
        else
        {
            position = sprite.position;
            width = sprite.width;
            height = sprite.height;
            originInCenter = sprite.anchorInfo.OriginInCenter;
        }

        // Adjust anchor offset
        position.x += UIRelative.xAnchorAdjustment( xAnchor, width, originInCenter );
        position.y -= UIRelative.yAnchorAdjustment( yAnchor, height, originInCenter );

        return position;
    }


    /// <summary>
    /// Returns width of parent or screen if parent is null
    /// </summary>
    /// <param name="sprite">Provided parent</param>
    /// <returns>Width of parent ( or screen )</returns>
	private static float parentWidth( IPositionable sprite )
	{
		return ( sprite == null ) ? Screen.width : sprite.width;
	}


	/// <summary>
	/// Returns height of parent or screen if parent is null
	/// </summary>
	/// <param name="sprite">Provided parent</param>
	/// <returns>Height of parent ( or screen )</returns>
	private static float parentHeight( IPositionable sprite )
	{
		return ( sprite == null ) ? Screen.height : sprite.height;
	}


	/// <summary>
	/// Calculates relative offset based on parent sprite width percentage.
	/// </summary>
	/// <param name="sprite">Provided parent ( or null for screen )</param>
	/// <param name="anchor">Sprite horizontal anchor</param>
	/// <param name="percentOffset">Percentage offset - 1 is 100%</param>
	/// <returns></returns>
	private static float xPercentFromParent( IPositionable sprite, UIxAnchor anchor, float percentOffset )
	{
		if ( percentOffset == 0)
			return 0;
		float offset = parentWidth( sprite ) * percentOffset;
		return ( anchor == UIxAnchor.Right ) ? -offset : offset;

	}


	/// <summary>
	/// Calculates relative offset based on parent sprite height percentage.
	/// </summary>
	/// <param name="sprite">Provided parent ( or null for screen )</param>
	/// <param name="anchor">Sprite vertical anchor</param>
	/// <param name="percentOffset">Percentage offset - 1 is 100%</param>
	/// <returns></returns>
	private static float yPercentFromParent( IPositionable sprite, UIyAnchor anchor, float percentOffset )
	{
		if ( percentOffset == 0)
			return 0;
		float offset = parentHeight( sprite ) * percentOffset;
		return ( anchor == UIyAnchor.Bottom ) ? -offset : offset;

	}

}
