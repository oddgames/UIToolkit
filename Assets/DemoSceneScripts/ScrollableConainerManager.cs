using UnityEngine;
using System.Collections;



public class ScrollableConainerManager : MonoBehaviour
{
	void Start()
	{
		//var scrollable = new Scro
		
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );

		
		// Scores button
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
	
	
		// Options button
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		
		
		// Knob
		var knob = UIKnob.create( "knobUp.png", "knobDown.png", 0, 0 );
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		knob.value = 0.3f;


		// Toggle Button
		var toggleButton = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
		toggleButton.selected = true;
		
		
		// HorizontalLayout
		var hBox = new UIHorizontalLayout( 20 );
		hBox.addChild( playButton, scores, optionsButton );

		
		// VerticalLayout
		var vBox = new UIVerticalLayout( 20 );
		vBox.addChild( knob, toggleButton );
		vBox.pixelsFromBottomRight( 10, 10 );
	}
}
