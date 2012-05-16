using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIBackgroundLayout : UIAbstractContainer
{
	public UISprite background;
	
	
	public UIBackgroundLayout( string filename ) : this( UI.firstToolkit, filename ) {}
	
	
	public UIBackgroundLayout( UIToolkit manager, string filename ) : base( UILayoutType.BackgroundLayout ) 
	{
		background = manager.addSprite( filename, 0, 0, 2 );
		addChild( background );
		
		// set dimensions of container based on background texture dimensions
		_width = background.width;
		_height = background.height;
	}

}
