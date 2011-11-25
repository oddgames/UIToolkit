using UnityEngine;

/// <summary>
/// Holds information about positioning anchors
/// </summary>
public struct UIAnchorInfo
{
    public IPositionable ParentUIObject;
    public UIxAnchor ParentUIxAnchor;
    public UIyAnchor ParentUIyAnchor;
    public UIxAnchor UIxAnchor;
    public UIyAnchor UIyAnchor;
    public UIPrecision UIPrecision;
    public float OffsetX;
    public float OffsetY;
    public bool OriginInCenter;

    /// <summary>
    /// Creates a default anchor info object anchored to top left corner.
    /// </summary>
    /// <returns>Default anchor info object</returns>
    public static UIAnchorInfo DefaultAnchorInfo()
    {
        UIAnchorInfo anchorInfo = new UIAnchorInfo()
        {
            ParentUIObject = null,
            ParentUIxAnchor = UIxAnchor.Left,
            ParentUIyAnchor = UIyAnchor.Top,
            UIxAnchor = UIxAnchor.Left,
            UIyAnchor = UIyAnchor.Top,
            UIPrecision = UIPrecision.Percentage,
            OffsetX = 0f,
            OffsetY = 0f,
            OriginInCenter = false
        };
        return anchorInfo;
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        return string.Format("Parent UIObject: {0:g}\nParent UIxAnchor: {1}\nParent UIyAnchor: {2}\nUIxAnchor: {3}\nUIyAnchor: {4}\nUIPrecision: {5}\nOffsetX: {6}\nOffsetY: {7}\nOriginInCenter: {8}",
            ParentUIObject, ParentUIxAnchor, ParentUIyAnchor, UIxAnchor, UIyAnchor, UIPrecision, OffsetX, OffsetY, OriginInCenter);
    }
#endif
}
