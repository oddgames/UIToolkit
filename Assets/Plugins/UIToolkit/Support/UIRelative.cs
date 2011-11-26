using UnityEngine;

public enum UIxAnchor { Left, Right, Center };
public enum UIyAnchor { Top, Bottom, Center };
public enum UIPrecision { Percentage, Pixel };

public static class UIRelative
{
    /// <summary>
    /// Determine if running in HD and multiply pixelOffsets accordingly.
    /// </summary>
    public static int pixelDensityMultiplier()
    {
        return UI.instance.isHD ? 2 : 1;
    }
	
	
    #region Relative offset methods

    /// <summary>
    /// Calculates offset based on screen width percentage.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="percentOffset">Percentage offset - 1 is 100%</param>
    /// <returns></returns>
    public static float xPercentFrom( UIxAnchor anchor, float percentOffset )
    {
        // Get inital offset
        float offset = Screen.width * percentOffset;
		
        // If anchor is right the offset is flipped
        if( anchor == UIxAnchor.Right )
        {
            offset = Screen.width - offset;
        }
        return offset;
    }
	
	
    /// <summary>
    /// Calculates offset based on screen height percentage.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="percentOffset">Percentage offset - 1 is 100%</param>
    /// <returns></returns>
    public static float yPercentFrom( UIyAnchor anchor, float percentOffset )
    {
        // Get initial offset
        float offset = Screen.height * percentOffset;
		
        // If anchor is bottom the offset is flipped
        if( anchor == UIyAnchor.Bottom )
        {
            offset = Screen.height - offset;
        }
        return offset;
    }
	

    /// <summary>
    /// Calculates screen width percentage based on offset.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="offset">Position offset</param>
    /// <returns></returns>
    public static float xPercentTo( UIxAnchor anchor, float offset )
    {
        // Get initial percentage
        float percentOffset = offset / Screen.width;
		
        // If anchor isn't right the percentage is flipped
        if( anchor != UIxAnchor.Right )
        {
            percentOffset = -percentOffset;
        }
        return percentOffset;
    }
	
	
    /// <summary>
    /// Calculates screen height percentage based on offset.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="offset">Position offset</param>
    /// <returns></returns>
    public static float yPercentTo( UIyAnchor anchor, float offset )
    {
        // Get initial percentage
        float percentOffset = offset / Screen.height;
		
        // If anchor isn't bottom the percentage is flipped
        if( anchor != UIyAnchor.Bottom )
        {
            percentOffset = -percentOffset;
        }
        return percentOffset;
    }

    #endregion
	
	
    #region Pixel offset methods

    /// <summary>
    /// Calculates horizontal pixel offset based on SD or HD.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="pixelOffset">Fixed offset</param>
    /// <returns></returns>
    public static float xPixelsFrom( UIxAnchor anchor, float pixelOffset )
    {
        // Get initial offset
        float offset = pixelOffset * pixelDensityMultiplier();
		
        // If anchor is right the offset is flipped
        if( anchor == UIxAnchor.Right )
        {
            offset = -offset;
        }
        return offset;
    }


    /// <summary>
    /// Calculates vertical pixel offset based on SD or HD.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="pixelOffset">Fixed offset</param>
    /// <returns></returns>
    public static float yPixelsFrom( UIyAnchor anchor, float pixelOffset )
    {
        // Get initial offset
        float offset = pixelOffset * pixelDensityMultiplier();
		
        // If anchor is bottom the offset is flipped
        if( anchor == UIyAnchor.Bottom )
        {
            offset = -offset;
        }
        return offset;
    }


    /// <summary>
    /// Calculates fixed horizontal pixel offset based on SD or HD offset.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="offset">Relative offset</param>
    /// <returns></returns>
    public static float xPixelsTo( UIxAnchor anchor, float offset )
    {
        // Get initial fixed offset
        float pixelOffset = offset / pixelDensityMultiplier();
		
        // If anchor isn't right the fixed offset is flipped
        if( anchor != UIxAnchor.Right )
        {
            pixelOffset = -pixelOffset;
        }
        return pixelOffset;
    }


    /// <summary>
    /// Calculates fixed vertical pixel offset based on SD or HD offset.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="offset">Relative offset</param>
    /// <returns></returns>
    public static float yPixelsTo( UIyAnchor anchor, float offset )
    {
        // Get initial fixed offset
        float pixelOffset = offset / pixelDensityMultiplier();
		
        // If anchor isn't bottom the fixed offset is flipped
        if( anchor != UIyAnchor.Bottom )
        {
            pixelOffset = -pixelOffset;
        }
        return pixelOffset;
    }

    #endregion
	
	
    #region Anchor adjustment methods

    /// <summary>
    /// Finds horizontal adjustment for anchor, based on width and origin of sprite.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="width">Sprite width</param>
    /// <param name="originInCenter">True if origin is in center</param>
    /// <returns></returns>
    public static float xAnchorAdjustment( UIxAnchor anchor, float width, bool originInCenter )
    {
        float adjustment = 0f;
        switch( anchor )
        {
            case UIxAnchor.Left:
                if( originInCenter )
                {
                    adjustment -= width / 2f;
                }
                break;
            case UIxAnchor.Right:
                if( originInCenter )
                {
                    adjustment += width / 2f;
                }
                else
                {
                    adjustment += width;
                }
                break;
            case UIxAnchor.Center:
                if( !originInCenter )
                {
                    adjustment += width / 2f;
                }
                break;
        }
        return adjustment;
    }


    /// <summary>
    /// Finds vertical adjustment for anchor, based on height and origin of sprite.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="height">Sprite height</param>
    /// <param name="originInCenter">True if origin is in center</param>
    /// <returns></returns>
    public static float yAnchorAdjustment( UIyAnchor anchor, float height, bool originInCenter )
    {
        float adjustment = 0f;
        switch( anchor )
        {
            case UIyAnchor.Top:
                if( originInCenter )
                {
                    adjustment -= height / 2f;
                }
                break;
            case UIyAnchor.Bottom:
                if( originInCenter )
                {
                    adjustment += height / 2f;
                }
                else
                {
                    adjustment += height;
                }
                break;
            case UIyAnchor.Center:
                if( !originInCenter )
                {
                    adjustment += height / 2f;
                }
                break;
        }
        return adjustment;
    }

    #endregion
}
