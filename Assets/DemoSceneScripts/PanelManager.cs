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
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		
		
		// Vertical panel
		var vPanel = UIVerticalPanel.create( "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		vPanel.beginUpdates();
		vPanel.spacing = 20;
		vPanel.edgeInsets = new UIEdgeInsets( 60, 10, 30, 10 );
		vPanel.addChild( playButton, scores, optionsButton );
		vPanel.endUpdates();
		
		vPanel.positionCenter();
	}
	

}
