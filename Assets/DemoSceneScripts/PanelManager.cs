using UnityEngine;
using System.Collections;


public class PanelManager : MonoBehaviour
{
	public UIToolkit panelToolkit;
	public UIToolkit fontToolkit;
	
	private UITextInstance vTextInstance;
	private UITextInstance bTextInstance;
	
	private UIBackgroundLayout bLayout;
	private UIHorizontalLayout hLayout;
	private UIVerticalPanel vPanel;
	
	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( panelToolkit, "playUp.png", "playDown.png", 0, 0 );

		// Scores button
		var scores = UIContinuousButton.create( panelToolkit, "scoresUp.png", "scoresDown.png", 0, 0 );
	
		// Options button
		var optionsButton = UIButton.create( panelToolkit, "optionsUp.png", "optionsDown.png", 0, 0 );
		
		// Vertical panel
		vPanel = UIVerticalPanel.create( panelToolkit, "vertPanelTop.png", "vertPanelMiddle.png", "vertPanelBottom.png" );
		
		// Text sample
		var text = new UIText( fontToolkit, "prototype", "prototype.png" );
		
		vTextInstance = text.addTextInstance( "Testing text", 0, 0, 0.3f );
		
		vPanel.beginUpdates();
		vPanel.spacing = 20;
		vPanel.edgeInsets = new UIEdgeInsets( 30, 10, 20, 10 );
		vPanel.addChild( playButton, scores, optionsButton );
		vPanel.addTextInstanceChild( vTextInstance );
		vPanel.endUpdates();
		
		vPanel.positionCenter();
		
		var hLayoutButton = UIButton.create( panelToolkit, "optionsUp.png", "optionsDown.png", 0, 0 );
		
		var hTextInstance = text.addTextInstance( "Test horizontal text", 0, 0, 0.3f );
		
		hLayout = new UIHorizontalLayout( 20 );
		
		hLayout.addChild( hLayoutButton );
		hLayout.addTextInstanceChild( hTextInstance );
		
		hLayout.position = new Vector3( 10, -10, 0 );
		
		// Background layout
		
		bTextInstance = text.addTextInstance( "Text in bg layout", 0, 0, 0.20f );
		
		bLayout = new UIBackgroundLayout( "playUp.png" );
		
		bLayout.position = new Vector3( Screen.width - bLayout.width, -( Screen.height - bLayout.height ), 0 );
		bLayout.addTextInstanceChild( bTextInstance );
		bTextInstance.localPosition = new Vector3( 10, -15, 0 );
		
		
		StartCoroutine( "AnimatePanels" );
	}
	
	IEnumerator AnimatePanels() 
	{
		yield return new WaitForSeconds( 2.0f );
		
		hLayout.hidden = true;
		vTextInstance.text = "Here we go!";
		
		vPanel.scaleTo( 3.0f, new Vector3( 1.2f, 1.2f, 1 ), Easing.Quartic.easeOut );
		
		bTextInstance.text = "Going up...";
		bTextInstance.text = "Going up 2";
		bLayout.positionTo( 3.0f, new Vector3( bLayout.position.x, bLayout.position.y + 100, bLayout.position.z ), Easing.Quartic.easeInOut );
		
		yield return new WaitForSeconds( 3.0f );
		
		AnimateSequenceTwo();
	}
	
	void AnimateSequenceTwo()
	{
		
		vPanel.positionTo( 0.5f, new Vector3( Screen.width * 2, vPanel.position.y, 0 ), Easing.Quartic.easeOut );
		bTextInstance.text = "Done moving.";
	}
}
