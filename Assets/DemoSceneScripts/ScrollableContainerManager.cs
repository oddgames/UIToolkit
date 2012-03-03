using UnityEngine;
using System.Collections;



public class ScrollableContainerManager : MonoBehaviour
{
	private bool _movedContainer;
	public UIToolkit textManager;	
	
	
	void Start()
	{
		var scrollable = new UIScrollableVerticalLayout( 10 );
		scrollable.position = new Vector3( 0, -50, 0 );
		var width = UI.instance.isHD ? 300 : 150;
		scrollable.setSize( width, Screen.height / 1.4f );
		
		for( var i = 0; i < 20; i++ )
		{
			UITouchableSprite touchable;
			if( i == 4 ) // text sprite
			{
				touchable = UIButton.create( "emptyUp.png", "emptyDown.png", 0, 0 );
			}
			else if( i % 3 == 0 )
			{
				touchable = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
			}
			else if( i % 2 == 0 )
			{
				touchable = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
			}
			else
			{
				touchable = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
			}
			
			if( i == 1 )
			{
				var ch = UIToggleButton.create("cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0);
				ch.parentUIObject = touchable;
				ch.pixelsFromRight(0);
				ch.client.name = "TEST THINGY";
				ch.scale = new Vector3(0.5f, 0.5f, 1);
			}
			else if( i == 4 )
			{
				var text = new UIText( textManager, "prototype", "prototype.png" );

				var helloText = text.addTextInstance( "Child Text", 0, 0,0.5f,-1,Color.blue,UITextAlignMode.Center,UITextVerticalAlignMode.Middle );
				helloText.parentUIObject = touchable;
				helloText.positionCenter();

				var ch = UIToggleButton.create("cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0);
				ch.parentUIObject = helloText;
				ch.pixelsFromRight( -16 );
				ch.client.name = "subsub";
				ch.scale = new Vector3( 0.25f, 0.25f, -2 );
			}

			
			// only add a touchUpInside handler for buttons
			if( touchable is UIButton )
			{
				var button = touchable as UIButton;
				
				// store i locally so we can put it in the closure scope of the touch handler
				var j = i;
				button.onTouchUpInside += ( sender ) => Debug.Log( "touched button: " + j );
			}

			
			scrollable.addChild( touchable );
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
			var moveBy = _movedContainer ? -100 : 100;
			if( !UI.instance.isHD )
				moveBy /= 2;
			target.x += moveBy * 2;
			target.y += moveBy;
			scrollable.positionTo( 0.4f, target, Easing.Quintic.easeIn );
			_movedContainer = !_movedContainer;
		};
	}
}
