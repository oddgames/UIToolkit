using UnityEngine;
using System.Collections;


public class LayoutContainerManager : MonoBehaviour
{
	public AudioClip scoresSound;
	public AudioClip optionsSound;
	

	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );

		
		// Scores button
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.touchDownSound = scoresSound;
	
	
		// Options button
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		optionsButton.touchDownSound = optionsSound;
		
		
		// Knob
		var knob = UIKnob.create( "knobUp.png", "knobDown.png", 0, 0 );
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		knob.value = 0.3f;


		// Toggle Button
		var toggleButton = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
		toggleButton.selected = true;
		
		
		// HorizontalLayout
		var hBox = new HorizontalLayout( 20, 5 );
		hBox.addChild( playButton, scores, optionsButton );

		
		var hBox2 = new HorizontalLayout( 50, 0 );
		hBox2.addChild( knob, toggleButton );
		hBox2.positionCenter();

		
		// Layouts can be animated like any sprite
		StartCoroutine( animatePanel( hBox ) );
	}
	
	
	private IEnumerator animatePanel( UIObject sprite )
	{
		var objectHeight = ((HorizontalLayout)sprite).height;
		while( true )
		{
			yield return new WaitForSeconds( 3 );
			
			var ani = sprite.positionTo( 0.7f, new Vector3( sprite.position.x, -Screen.height + objectHeight, sprite.position.z ), Easing.Quartic.easeIn );
			ani.autoreverse = true;
		}
	}
	

}
