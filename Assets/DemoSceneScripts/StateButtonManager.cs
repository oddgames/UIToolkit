using UnityEngine;
using System.Collections;


public class StateButtonManager : MonoBehaviour
{
	
	private UITextInstance _state;
	private UITextInstance _touched;
	private int _count = 0;


	void Start()
	{
		var filenames = new string[] { "playUp.png", "scoresUp.png", "optionsUp.png" };
		var highlightedFilenames = new string[] { "playDown.png", "scoresDown.png", "optionsDown.png" };

		var stateButton = UIStateButton.create( filenames, highlightedFilenames, 0, 0 );
		stateButton.positionCenter();
		stateButton.onStateChange += onStateChange;
		
		// can hook up to UIButton events too
		stateButton.onTouchUpInside += onButtonTouched;
	}
	
	
	private void onStateChange( UIStateButton sender, int state )
	{
		Debug.Log( "Button is at state: " + state );
	}
	
	
	// since its a UIButton event, the type is UIButton
	private void onButtonTouched( UIButton sender )
	{
		_count++;
		Debug.Log( string.Format( "Button was pressed {0} times", _count ) );
	}

}
