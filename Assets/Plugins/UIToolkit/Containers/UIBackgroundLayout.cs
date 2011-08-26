using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIBackgroundLayout : UIAbstractContainer
{	
	public UIBackgroundLayout( string filename ) : this(UI.firstToolkit, filename) {}

	public UIBackgroundLayout( UIToolkit manager, string filename ) : base( UILayoutType.BackgroundLayout ) 
	{
		int depth = 2;
		var background = manager.addSprite( filename, 0, 0, depth);
		addChild( background );
		
		//set dimensions of container based on background texture dimensions
		_width = background.width;
		_height = background.height;
	}
}
