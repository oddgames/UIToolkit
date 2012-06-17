using UnityEngine;
using System.Collections;


public class MultipleAtlasManager : MonoBehaviour
{
	// we setup one UIToolkit instance for our buttons and one for our text to demonstrate multiple atlases
	public UIToolkit buttonToolkit;
	public UIToolkit textToolkit;
	
	
	void Start()
	{
		// Scores button
		var scores = UIContinuousButton.create( buttonToolkit, "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.positionFromBottomRight( .02f, .02f );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 10 );
	
	
		// Options button
		var optionsButton = UIButton.create( buttonToolkit, "optionsUp.png", "optionsDown.png", 0, 0 );
		optionsButton.allowTouchBeganWhenMovedOver = true;
		optionsButton.positionFromBottomRight( .2f, .02f );
		optionsButton.highlightedTouchOffsets = new UIEdgeOffsets( 10 );
		
		// Text
		// setup our text manager which will parse our .fnt file and allow us to add text instances
		var text = new UIText( textToolkit, "prototype", "prototype.png" );

		var helloText = text.addTextInstance( "hello man.  I have a line\nbreak", 0, 0 );
        helloText.positionFromTopLeft( 0.1f, 0.05f );
	}
	

}
