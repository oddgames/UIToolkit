using UnityEngine;
using System.Collections;



public class ScrollableContainerManager : MonoBehaviour
{
	void Start()
	{
		var scrollable = new UIScrollableVerticalLayout( 10 );
		scrollable.position = new Vector3( 0, -30, 0 );
		scrollable.setSize( 300, Screen.height / 1.4f );
		
		for( var i = 0; i < 20; i++ )
		{
			UIButton button;
			if( i % 2 == 0 )
			{
				button = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
			}
			else
			{
				button = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
			}
			
			scrollable.addChild( button );
		}

		//scrollable.pixelsFromBottomRight( 10, 10 );
	}
}
