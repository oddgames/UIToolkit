using UnityEngine;
using System.Collections;


public class PositioningManager : MonoBehaviour
{
	// we setup one UIToolkit instance for our buttons and one for our text to demonstrate multiple atlases
	public UIToolkit buttonToolkit;
	public UIToolkit textToolkit;
	
	
	private UIButton _scoresButton;
	
	
	void Start()
	{
		// create a vertical layout to house our buttons
		var vBox = new UIVerticalLayout( 10 );
		vBox.edgeInsets = new UIEdgeInsets( 10, 5, 10, 0 );
		
		// create some buttons to control the positioning. we will add text to them se create the UIText we will use as well
		var text = new UIText( textToolkit, "prototype", "prototype.png" );
		
		var positions = new string[] { "top", "top-left", "top-right", "bottom", "bottom-left", "bottom-right" };
		foreach( var pos in positions )
		{			
			// create the button
			var touchable = UIButton.create( "emptyUp.png", "emptyDown.png", 0, 0 );
			touchable.userData = pos;
			touchable.onTouchUpInside += onButtonTouched;

			// add the text
			var helloText = text.addTextInstance( pos, 0, 0, 0.5f, -1, Color.white, UITextAlignMode.Center, UITextVerticalAlignMode.Middle );
			helloText.parentUIObject = touchable;
			helloText.positionCenter();
			
			vBox.addChild( touchable );
		}
		
		
		// Scores button. we will use this to demo positioning
		_scoresButton = UIContinuousButton.create( buttonToolkit, "scoresUp.png", "scoresDown.png", 0, 0 );
		_scoresButton.positionCenter();
	}
	
	
	private void onButtonTouched( UIButton button )
	{
		var positionText = button.userData as string;
		
		switch( positionText )
		{
			case "top":
				_scoresButton.pixelsFromTop( 10 );
				break;
			case "top-left":
				_scoresButton.pixelsFromTopLeft( 10, (int)_scoresButton.width / UI.scaleFactor );
				break;
			case "top-right":
				_scoresButton.pixelsFromTopRight( 0, 0 );
				break;
			case "bottom":
				_scoresButton.pixelsFromBottom( 5 );
				break;
			case "bottom-left":
				_scoresButton.pixelsFromBottomLeft( 5, (int)_scoresButton.width / UI.scaleFactor );
				break;
			case "bottom-right":
				_scoresButton.pixelsFromBottomRight( 0, 0 );
				break;
		}
	}
	

}
