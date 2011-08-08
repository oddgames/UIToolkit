using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIBackgroundLayout : UIAbstractContainer
{	
	public UIBackgroundLayout( string filename ) : base( UILayoutType.BackgroundLayout )
	{
		// grab the texture details for the background image
		var manager = UI.firstToolkit;
		int depth = 2;
		var background = manager.addSprite( filename, 0, 0, depth);
		addChild( background );
		
		//set dimensions of container based on background texture dimensions
		_width = background.width;
		_height = background.height;
	}
}
