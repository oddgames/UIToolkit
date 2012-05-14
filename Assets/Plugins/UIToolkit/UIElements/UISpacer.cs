using UnityEngine;
using System.Collections;



public class UISpacer : UISprite
{
    public override bool hidden
    {
        get { return ___hidden; }
        set {}
    }
	
	
	public override void updateTransform()
	{}
	
	
	public UISpacer( int width, int height )
	{
		manager = UI.firstToolkit;
		
		_width = width;
		_height = height;
	}
}
