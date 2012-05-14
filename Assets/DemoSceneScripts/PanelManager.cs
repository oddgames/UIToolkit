using UnityEngine;
using System.Collections;


public class PanelManager : MonoBehaviour
{
	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, 0 );

		
		// Scores button
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
	
	
		// Options button
		var optionsButton = UIZoomButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		optionsButton.animationDuration = 0.2f;
		optionsButton.animationTargetScale = new Vector3( 1.4f, 1.4f, 1.4f );


		// Scores button
		var scores2 = UIContinuousButton.create("scoresUp.png", "scoresDown.png", 0, 0 );
		

		
		
		// Vertical panel
		var vPanel = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vPanel.beginUpdates();
		vPanel.spacing = 20;
		vPanel.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vPanel.addChild( playButton, scores, optionsButton, scores2 );
		vPanel.endUpdates();
		
		
		vPanel.positionCenter();
	}
	

}
