using UnityEngine;
using System.Collections;



public class ScrollableContainerManager : MonoBehaviour
{
	void Start()
	{
		var scrollable = new UIScrollableVerticalLayout( 10 );
		scrollable.position = new Vector3( 0, -50, 0 );
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
		
		
		// click to scroll to a specific offset
		var scores = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.positionFromTopRight( 0, 0 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += ( sender ) =>
		{
			scrollable.scrollTo( -10, true );
		};
		
		
		scores = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.positionFromBottomRight( 0, 0 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += ( sender ) =>
		{
			scrollable.scrollTo( -600, true );
		};
		
		
		scores = UIButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.centerize();
		scores.positionFromTopRight( 0.5f, 0 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += ( sender ) =>
		{
			var target = scrollable.position;
			target.x += 50;
			target.y += 50;
			scrollable.positionTo( 1, target, Easing.Quintic.easeIn );
		};
	}
}
