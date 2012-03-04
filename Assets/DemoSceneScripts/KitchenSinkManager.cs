using UnityEngine;
using System.Collections;


public class KitchenSinkManager : MonoBehaviour
{
	public AudioClip scoresSound;
	public AudioClip optionsSound;
	

	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, 0 );
        playButton.positionFromTopLeft( 0.05f, 0f );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		playButton.onTouchUpInside += ( sender ) => Debug.Log( "clicked the button: " + sender );

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		// hover state example for illustration purposes. the playButton will get the scoresDown image when hovered over
		playButton.hoveredUVframe = UI.firstToolkit.uvRectForFilename( "scoresDown.png" );
#endif
		
		// Scores button
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", 0, 0 );
		scores.positionFromTopLeft( .24f, .02f );
		scores.centerize(); // centerize the button so we can scale it from the center
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += onTouchUpInsideScoresButton;
		scores.onTouchIsDown += ( sender ) => Debug.Log( "touch is down: " + Time.time );
		scores.touchDownSound = scoresSound;
        scores.autoRefreshPositionOnScaling = false;
	
	
		// Options button
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", 0, 0 );
		optionsButton.positionFromTopLeft( .43f, .02f );
		optionsButton.onTouchUpInside += onTouchUpInsideOptionsButton;
		optionsButton.touchDownSound = optionsSound;
		
		
		// Knob
		var knob = UIKnob.create( "knobUp.png", "knobDown.png", 0, 0 );
		knob.positionFromTopLeft( .39f, .5f );
		knob.normalTouchOffsets = new UIEdgeOffsets( 10 ); // give the knob a bit extra touch area
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		knob.onKnobChanged += onKnobChanged;
		knob.value = 0.3f;
		
		
		// Horizontal Slider
		var hSlider = UISlider.create( "sliderKnob.png", "hSlider.png", 0, 0, UISliderLayout.Horizontal );
		hSlider.positionFromTopLeft( .7f, .02f );
		hSlider.highlightedTouchOffsets = new UIEdgeOffsets( 30, 20, 30, 20 );
		hSlider.onChange += ( sender, val ) => Debug.Log( val );
		hSlider.value = 0.6f;


		// Vertical Slider
		var vSlider = UISlider.create( "vSliderKnob.png", "vSlider.png", 0, 0, UISliderLayout.Vertical );
		vSlider.positionFromTopRight( .17f, .05f );
		vSlider.highlightedTouchOffsets = new UIEdgeOffsets( 20, 30, 20, 30 );
		vSlider.continuous = true;
		vSlider.onChange += ( sender, val ) => Debug.Log( val );
		vSlider.value = 0.3f;

		
		// Toggle Button
		var toggleButton = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 0, 0 );
		toggleButton.positionFromTopRight( .3f, .2f );
		toggleButton.onToggle += ( sender, newValue ) => hSlider.hidden = !newValue;
		toggleButton.selected = true;
		

		// Progress/Health bar
		var progressBar = UIProgressBar.create( "progressBar.png", 0, 0 );
		progressBar.positionFromBottomLeft( .05f, .02f );
		progressBar.resizeTextureOnChange = true;
		progressBar.value = 0.4f;
		
		
		// animated sprite
		var animatedSprite = UI.firstToolkit.addSprite( "Gai_1.png", 0, 0, 1 );
		var anim = animatedSprite.addSpriteAnimation( "anim", 0.15f, "Gai_1.png", "Gai_2.png", "Gai_3.png", "Gai_4.png", "Gai_5.png", "Gai_6.png", "Gai_7.png", "Gai_8.png", "Gai_9.png", "Gai_10.png", "Gai_11.png", "Gai_12.png" );
		animatedSprite.positionFromBottomRight( .0f, .25f );
		anim.loopReverse = true; // optinally loop in reverse
		animatedSprite.playSpriteAnimation( "anim", 5 );
		
		
		// Test movement
		StartCoroutine( marqueePlayButton( playButton ) );
		StartCoroutine( animateProgressBar( progressBar ) );
		StartCoroutine( pulseOptionButton( optionsButton ) );
		
		
		// UIObjects can be used like panels to group other UIObjects
		var panel = new UIObject();
		scores.parentUIObject = panel;
		optionsButton.parentUIObject = panel;
		
		StartCoroutine( animatePanel( panel ) );
	}
	
	
	private IEnumerator animatePanel( UIObject sprite )
	{
		while( true )
		{
			yield return new WaitForSeconds( 2 );
			
			var ani = sprite.positionTo( 0.7f, new Vector3( 200f, 0, 1 ), Easing.Quartic.easeIn );
			ani.autoreverse = true;
		}
	}
	
	
	#region Coroutine animation tests that do not use the UIAnimation system
	
	// Play coroutine that animates a button marquee style
	private IEnumerator marqueePlayButton( UIButton playButton )
	{
		while( true )
		{
			// Make sure we arent off the right side of the screen
			Vector3 pos = playButton.position;
			if( pos.x > Screen.width + playButton.width / 2 )
			{
				pos.x = -playButton.width;
				playButton.position = pos;
			}
			
			pos.x += 2f;
			
			playButton.position = pos;
			
			yield return 0;
		}
	}
	
	
	private IEnumerator animateProgressBar( UIProgressBar progressBar )
	{
		float value = 0.0f;
		
		while( true )
		{
			// Make sure the progress doesnt exceed 1
			if( value > 1.0f )
			{
				// Swap the progressBars resizeTextureOnChange property
				progressBar.resizeTextureOnChange = !progressBar.resizeTextureOnChange;
				value = 0.0f;
			}
			else
			{
				value += 0.01f;
			}
			
			progressBar.value = value;
			
			yield return 0;
		}
	}
	
	#endregion;
	
	
	#region UIAnimations

	private IEnumerator animatePosition( UISprite sprite, Vector3 to, float duration )
	{
		//Vector3 originalPosition = sprite.localPosition;
		
		// Go back and forth.  The chain() method will return when the animation is done
		var ani = sprite.positionTo( duration, to, Easing.Quartic.easeInOut );
		ani.autoreverse = true;
		
		yield return null;
		//sprite.positionTo( duration, originalPosition, Easing.Quintic.easeIn );		
	}


	private IEnumerator animateRotation( UISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.eulerAngles;
		
		// rotate.  The chain() method will return when the animation is done
		var ani = sprite.eulerAnglesTo( duration, to, Easing.Sinusoidal.easeOut );
		yield return ani.chain();

		sprite.eulerAnglesTo( duration, originalPosition, Easing.Circular.easeIn );
	}
	
	
	private IEnumerator pulseOptionButton( UIButton optionsButton )
	{
		UIAnimation ani;
		
		while( true )
		{
			ani = optionsButton.alphaTo( 0.7f, 0.1f, Easing.Linear.easeIn );
			yield return ani.chain();
			
			ani = optionsButton.alphaTo( 0.7f, 1.0f, Easing.Linear.easeOut );
			yield return ani.chain();
		}
	}
	
	#endregion;

	
	#region Callbacks

	// Button callback
	public void onTouchUpInsideScoresButton( UIButton sender )
	{
		var ani = sender.scaleFromTo( 0.3f, new Vector3( 1, 1, 1 ), new Vector3( 1.3f, 1.3f, 1 ), Easing.Sinusoidal.easeOut );
		ani.autoreverse = true;
		ani.onComplete = () => Debug.Log( "done scaling button" ); // example of completion handler
	}
	
	
	public void onTouchUpInsideOptionsButton( UIButton sender )
	{
		// Rotation should be around the z axis
		StartCoroutine( animateRotation( sender, new Vector3( 0, 0, 359 ), 1.0f ) );
		               
		// Dont forget to make the y negative because our origin is the top left
		StartCoroutine( animatePosition( sender, new Vector3( Screen.width * 0.5f, -Screen.height * 0.7f, 1 ), 1.0f ) );
	}
	
	
	// Knob callback
	public void onKnobChanged( UIKnob sender, float value )
	{
		Debug.Log( "onKnobChanged: " + value );
	}

	
	#endregion;


}
