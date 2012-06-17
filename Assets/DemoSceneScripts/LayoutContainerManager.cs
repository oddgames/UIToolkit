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
		var hBox = new UIHorizontalLayout( 20 );
		hBox.addChild( playButton, scores, optionsButton );
		
		
		// VerticalLayout
		var vBox = new UIVerticalLayout( 20 );
		vBox.addChild( knob, toggleButton );
		vBox.matchSizeToContentSize();
		vBox.positionFromTopRight( 0, 0 );

		
		// Layouts can be animated like any UIObject
		StartCoroutine( animatePanel( hBox ) );
	}
	
	
	private IEnumerator animatePanel( UIAbstractContainer obj )
	{
		var objectHeight = ((UIHorizontalLayout)obj).height;
		while( true )
		{
			// flip our orientation to show on the fly layout changes
			obj.beginUpdates(); // more efficient way to change multiple properties wrapping the begin/endUpdates
			obj.layoutType = UIAbstractContainer.UILayoutType.Horizontal;
			obj.edgeInsets = new UIEdgeInsets( 0 );
			obj.endUpdates();
			
			yield return new WaitForSeconds( 2 );
			
			var ani = obj.positionTo( 0.7f, new Vector3( obj.position.x, -Screen.height + objectHeight, obj.position.z ), Easing.Quartic.easeIn );
			ani.autoreverse = true;
			
			yield return ani.chain();
			
			// flip our orientation to show on the fly layout changes
			obj.beginUpdates();
			obj.layoutType = UIAbstractContainer.UILayoutType.Vertical;
			obj.edgeInsets = new UIEdgeInsets( 20 );
			obj.endUpdates();
			
			yield return new WaitForSeconds( 2 );
		}
	}
	

}
