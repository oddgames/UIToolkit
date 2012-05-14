using UnityEngine;

public enum UIxAnchor { Left, Right, Center };
public enum UIyAnchor { Top, Bottom, Center };
public enum UIPrecision { Percentage, Pixel };

public static class UIRelative
{
    #region Relative offset methods

    /// <summary>
    /// Calculates offset based on screen width percentage.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="width">Parent width</param>
    /// <param name="percentOffset">Percentage offset - 1 is 100%</param>
    /// <returns></returns>
    public static float xPercentFrom( UIxAnchor anchor, float width, float percentOffset )
    {
        // Get inital offset
        float offset = width * percentOffset;
		
        // If anchor is right the offset is flipped
        if( anchor == UIxAnchor.Right )
        {
            offset = -offset;
        }
        return offset;
    }
	
	
    /// <summary>
    /// Calculates offset based on screen height percentage.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="height">Parent height</param>
    /// <param name="percentOffset">Percentage offset - 1 is 100%</param>
    /// <returns></returns>
    public static float yPercentFrom( UIyAnchor anchor, float height, float percentOffset )
    {
        // Get initial offset
        float offset = height * percentOffset;
		
        // If anchor is bottom the offset is flipped
        if( anchor == UIyAnchor.Bottom )
        {
            offset = -offset;
        }
        return offset;
    }
	

    /// <summary>
    /// Calculates screen width percentage based on offset.
    /// </summary>
    /// <param name="anchor">Sprite horizontal anchor</param>
    /// <param name="width">Parent width</param>
    /// <param name="offset">Position offset</param>
    /// <returns></returns>
    public static float xPercentTo( UIxAnchor anchor, float width, float offset )
    {
        if (width == 0f) return 0f;

        // Get initial percentage
        float percentOffset = offset / width;
		
        // If anchor is right the percentage is flipped
        if( anchor == UIxAnchor.Right )
        {
            percentOffset = -percentOffset;
        }
        return percentOffset;
    }
	
	
    /// <summary>
    /// Calculates screen height percentage based on offset.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="height">Parent height</param>
    /// <param name="offset">Position offset</param>
    /// <returns></returns>
    public static float yPercentTo( UIyAnchor anchor, float height, float offset )
    {
        if (height == 0f) return 0f;

        // Get initial percentage
        float percentOffset = offset / height;
		
        // If anchor is bottom the percentage is flipped
        if( anchor == UIyAnchor.Bottom )
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
        float offset = pixelOffset * UI.scaleFactor;
		
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
        float offset = pixelOffset * UI.scaleFactor;
		
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
        float pixelOffset = offset / UI.scaleFactor;
		
        // If anchor is right the fixed offset is flipped
        if( anchor == UIxAnchor.Right )
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
        float pixelOffset = offset / UI.scaleFactor;
		
        // If anchor is bottom the fixed offset is flipped
        if( anchor == UIyAnchor.Bottom )
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
    /// <param name="originAnchor">Sprite origin anchor</param>
    /// <returns></returns>
    public static float xAnchorAdjustment( UIxAnchor anchor, float width, UIxAnchor originAnchor )
    {
        var adjustment = 0f;
        switch( anchor )
        {
            case UIxAnchor.Left:
			{
                if( originAnchor == UIxAnchor.Center )
                {
                    adjustment -= width / 2f;
                }
                else if( originAnchor == UIxAnchor.Right )
                {
                    adjustment -= width;
                }
                break;
			}
            case UIxAnchor.Right:
			{
                if( originAnchor == UIxAnchor.Left )
                {
                    adjustment += width;
                }
                else if( originAnchor == UIxAnchor.Center )
                {
                    adjustment += width / 2f;
                }
                break;
			}
            case UIxAnchor.Center:
			{
                if( originAnchor == UIxAnchor.Left )
                {
                    adjustment += width / 2f;
                }
                else if (originAnchor == UIxAnchor.Right)
                {
                    adjustment -= width / 2f;
                }
                break;
			}
        }

        return adjustment;
    }


    /// <summary>
    /// Finds vertical adjustment for anchor, based on height and origin of sprite.
    /// </summary>
    /// <param name="anchor">Sprite vertical anchor</param>
    /// <param name="height">Sprite height</param>
    /// <param name="originAnchor">Sprite origin anchor</param>
    /// <returns></returns>
    public static float yAnchorAdjustment( UIyAnchor anchor, float height, UIyAnchor originAnchor )
    {
        float adjustment = 0f;
        switch( anchor )
        {
            case UIyAnchor.Top:
                if (originAnchor == UIyAnchor.Center)
                {
                    adjustment -= height / 2f;
                }
                else if (originAnchor == UIyAnchor.Bottom)
                {
                    adjustment -= height;
                }
                break;
            case UIyAnchor.Bottom:
                if (originAnchor == UIyAnchor.Top)
                {
                    adjustment += height;
                }
                else if (originAnchor == UIyAnchor.Center)
                {
                    adjustment += height / 2f;
                }
                break;
            case UIyAnchor.Center:
                if (originAnchor == UIyAnchor.Top)
                {
                    adjustment += height / 2f;
                }
                else if (originAnchor == UIyAnchor.Bottom)
                {
                    adjustment -= height / 2f;
                }
                break;
        }
        return adjustment;
    }

    #endregion
	
}
