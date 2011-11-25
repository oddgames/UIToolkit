using UnityEngine;


public interface IPositionable
{
	float width { get; }
	float height { get; }
	Vector3 position { get; set; }
    UIAnchorInfo anchorInfo { get; set; }
}

