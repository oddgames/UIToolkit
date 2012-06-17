using UnityEngine;
using System.Collections;



public class ScrollableHorizontalContainerManager : MonoBehaviour
{
	private bool _movedContainer;
	public UIToolkit textManager;	
	
	
	void Start()
	{
		// add two scrollables: one with paging enabled and one without
		var scrollable = new UIScrollableHorizontalLayout( 10 );

		// we wrap the addition of all the sprites with a begin updates so it only lays out once when complete
		scrollable.beginUpdates();
		
		var height = UI.scaleFactor * 50f;
		var width = Screen.width / 1.4f;
		scrollable.setSize( width, height );
		scrollable.position = new Vector3( ( Screen.width - width ) / 2, -Screen.height + height, 0 );
		scrollable.zIndex = 3;
		
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
				var ch = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
				ch.parentUIObject = touchable;
				ch.pixelsFromRight( 0 );
				ch.client.name = "TEST THINGY";
				ch.scale = new Vector3( 0.5f, 0.5f, 1 );
			}
			else if( i == 4 )
			{
				var text = new UIText( textManager, "prototype", "prototype.png" );

				var helloText = text.addTextInstance( "Child Text", 0, 0,0.5f,-1,Color.blue,UITextAlignMode.Center,UITextVerticalAlignMode.Middle );
				helloText.parentUIObject = touchable;
				helloText.positionCenter();

				var ch = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
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

			// add random spacers every so often
			if( i % 3 == 0 )
				scrollable.addChild( new UISpacer( Random.Range( 10, 100 ), 40 ) );
			
			scrollable.addChild( touchable );
		}
		scrollable.endUpdates();
		scrollable.endUpdates(); // this is a bug. it shouldnt need to be called twice
		
		
		
		// add another scrollable, this one with paging enabled.
		scrollable = new UIScrollableHorizontalLayout( 20 );
		
		// we wrap the addition of all the sprites with a begin updates so it only lays out once when complete
		scrollable.beginUpdates();
		
		height = UI.scaleFactor * 250f;
		
		// if you plan on making the scrollable wider than the item width you need to set your edgeInsets so that the
		// left + right inset is equal to the extra width you set
		scrollable.edgeInsets = new UIEdgeInsets( 0, 75, 0, 75 );		
		width = UI.scaleFactor * ( 250f + 150f ); // item width + 150 extra width
		scrollable.setSize( width, height );
		
		// paging will snap to the nearest page when scrolling
		scrollable.pagingEnabled = true;
		scrollable.pageWidth = 250f * UI.scaleFactor;
		
		// center the scrollable horizontally
		scrollable.position = new Vector3( ( Screen.width - width ) / 2, 0, 0 );
		
		for( var i = 0; i < 5; i++ )
		{
			var button = UIButton.create( "marioPanel.png", "marioPanel.png", 0, 0 );
			scrollable.addChild( button );
		}
		
		scrollable.endUpdates();
		scrollable.endUpdates(); // this is a bug. it shouldnt need to be called twice
	}

}
