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
    public UIxAnchor OriginUIxAnchor;
    public UIyAnchor OriginUIyAnchor;
    public UIPrecision UIPrecision;
    public float OffsetX;
    public float OffsetY;
	
	
    /// <summary>
    /// Creates a default anchor info object anchored to top left corner.
    /// </summary>
    /// <returns>Default anchor info object</returns>
    public static UIAnchorInfo DefaultAnchorInfo()
    {
        return new UIAnchorInfo()
        {
            ParentUIObject = null,
            ParentUIxAnchor = UIxAnchor.Left,
            ParentUIyAnchor = UIyAnchor.Top,
            UIxAnchor = UIxAnchor.Left,
            UIyAnchor = UIyAnchor.Top,
            OriginUIxAnchor = UIxAnchor.Left,
            OriginUIyAnchor = UIyAnchor.Top,
            UIPrecision = UIPrecision.Pixel,
            OffsetX = 0f,
            OffsetY = 0f
        };
    }
	
	
#if UNITY_EDITOR
    public override string ToString()
    {
        return string.Format( "Parent UIObject: {0:g}\nParent UIxAnchor: {1}\nParent UIyAnchor: {2}\nUIxAnchor: {3}\nUIyAnchor: {4}\nOriginUIxAnchor: {5}\nOriginUIyAnchor: {6}\nUIPrecision: {7}\nOffsetX: {8}\nOffsetY: {9}",
            ParentUIObject, ParentUIxAnchor, ParentUIyAnchor, UIxAnchor, UIyAnchor, OriginUIxAnchor, OriginUIyAnchor, UIPrecision, OffsetX, OffsetY );
    }
#endif

}
