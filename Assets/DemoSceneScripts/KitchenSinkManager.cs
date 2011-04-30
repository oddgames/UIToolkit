using UnityEngine;
using System.Collections;

public class KitchenSinkManager : MonoBehaviour
{
	public GUIText swipeText;
	

	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		var playButton = UIButton.create( "playUp.png", "playDown.png", 10, 10, 6 );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		playButton.onTouchUpInside += ( sender )  => Debug.Log( "clicked the button: " + sender );
		
		
		// Scores button
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", 10, 75 );
		scores.centerize();
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += onTouchUpInsideScoresButton;
		scores.onTouchIsDown += ( sender ) => Debug.Log( "touch is down: " + Time.time );
		
		
		// Options button
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", 10, 130 );
		optionsButton.onTouchUpInside += onTouchUpInsideOptionsButton;
		
		
		// Knob
		var knob = new UIKnob( UI.instance.textureInfoForFilename( "knobUp.png" ), 270, 60 );
		knob.highlightedUVframe = UI.instance.uvRectForFilename( "knobDown.png" );
		knob.normalTouchOffsets = new UIEdgeOffsets( 10 ); // give the knob a bit extra touch area
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		knob.onKnobChanged += onKnobChanged;
		knob.value = 0.3f;
		
		
		// Horizontal Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		var hSlider = UISlider.create( "sliderKnob.png", "hSlider.png", 10, 220, UISliderLayout.Horizontal );
		hSlider.highlightedTouchOffsets = new UIEdgeOffsets( 30, 20, 30, 20 );
		hSlider.onChange += ( sender, val ) => Debug.Log( val );
		hSlider.value = 0.6f;
		
		
		// Vertical Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		var vSlider = UISlider.create( "vSliderKnob.png", "vSlider.png", 430, 10, UISliderLayout.Vertical );
		vSlider.highlightedTouchOffsets = new UIEdgeOffsets( 20, 30, 20, 30 );
		vSlider.continuous = true;
		vSlider.onChange += ( sender, val ) => Debug.Log( val );
		vSlider.value = 0.3f;
		
		
		// Toggle Button
		var toggleButton = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", 300, 260 );
		toggleButton.onToggle += onToggleButtonChanged;
		toggleButton.selected = true;
		
		
		// Progress/Health bar (be sure the bar is on a lower level than the GUIProgressBar
		var progressBar = UIProgressBar.create( "progressBar.png", "progressBarBorder.png", 5, 3, 200, 150 );
		progressBar.resizeTextureOnChange = true;
		progressBar.value = 0.4f;
		
		
		// Test movement
		StartCoroutine( marqueePlayButton( playButton ) );
		StartCoroutine( animateProgressBar( progressBar ) );
		StartCoroutine( pulseOptionButton( optionsButton ) );
		
		
		/*		
		// Swipe detector view - big, giant touchbleSprite behind all others
		UISwipeDetector detector = new UISwipeDetector( new Rect( 0, 60f, Screen.width, Screen.height - 60f ), 10, new UIUVRect( 450, 50, 408, 306 ) );
		detector.action = onSwipe;
		UI.instance.addTouchableSprite( detector );
		*/
	}
	
	
	#region CoRoutine animation tests that do not use the GUIAnimation system
	
	// Play coroutine that animates a button marquee style
	private IEnumerator marqueePlayButton( UIButton playButton )
	{
		while( true )
		{
			// Make sure we arent off the right side of the screen
			Vector3 pos = playButton.clientTransform.position;
			if( pos.x > Screen.width + playButton.width / 2 )
			{
				pos.x = -playButton.width;
				playButton.clientTransform.position = pos;
			}
			
			playButton.clientTransform.Translate( 2.0f, 0, 0 );
			playButton.updateTransform();
			
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
	
	
	#region GUIAnimations
	
	private IEnumerator animateLocalScale( UISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.localScale;
		
		// Go back and forth.  The chain() method will return when the animation is done
		var ani = sprite.to( duration, UIAnimationProperty.LocalScale, to, Easing.Sinusoidal.factory(), Easing.EasingType.Out );
		yield return ani.chain();
		
		sprite.to( duration, UIAnimationProperty.LocalScale, originalPosition, Easing.Circular.factory(), Easing.EasingType.In );
	}


	private IEnumerator animatePosition( UISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.position;
		
		// Go back and forth.  The chain() method will return when the animation is done
		var ani = sprite.to( duration, UIAnimationProperty.Position, to, Easing.Quintic.factory(), Easing.EasingType.InOut );
		yield return ani.chain();
		
		sprite.to( duration, UIAnimationProperty.Position, originalPosition, Easing.Quintic.factory(), Easing.EasingType.In );
	}


	private IEnumerator animateRotation( UISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.eulerAngles;
		
		// Go back and forth.  The chain() method will return when the animation is done
		var ani = sprite.to( duration, UIAnimationProperty.EulerAngles, to, Easing.Sinusoidal.factory(), Easing.EasingType.Out );
		yield return ani.chain();
		
		sprite.to( duration, UIAnimationProperty.EulerAngles, originalPosition, Easing.Circular.factory(), Easing.EasingType.In );
	}
	
	
	private IEnumerator pulseOptionButton( UIButton optionsButton )
	{
		UIAnimation ani;
		
		while( true )
		{
			ani = optionsButton.to( 0.7f, UIAnimationProperty.Alpha, 0.1f, Easing.Linear.factory(), Easing.EasingType.In );
			yield return ani.chain();
			
			ani = optionsButton.to( 0.7f, UIAnimationProperty.Alpha, 1.0f, Easing.Linear.factory(), Easing.EasingType.Out );
			yield return ani.chain();
		}
	}
	
	#endregion;

	
	#region Callbacks
	
	// Swipe callback
	public void onSwipe( UISwipeDetector sender, SwipeDirection direction )
	{
		swipeText.text = direction.ToString();
	}


	// Button callback
	public void onTouchUpInsideScoresButton( UIButton sender )
	{
		StartCoroutine( animateLocalScale( sender, new Vector3( 1.3f, 1.3f, 1 ), 0.3f ) );
	}
	
	
	public void onTouchUpInsideOptionsButton( UIButton sender )
	{
		// Rotation should be around the z axis
		StartCoroutine( animateRotation( sender, new Vector3( 0, 0, 359 ), 1.0f ) );
		               
		// Dont forget to make the y negative because our origin is the top left
		StartCoroutine( animatePosition( sender, new Vector3( 270, -200, 1 ), 1.0f ) );
	}
	
	
	// Knob callback
	public void onKnobChanged( UIKnob sender, float value )
	{
		//Debug.Log( "onKnobChanged: " + value );
	}
		

	// Slider callback
	public void onHSliderChanged( UISlider sender, float value )
	{
		//Debug.Log( "onHSliderChanged to: " + value );
	}
	
	
	public void onVSliderChanged( UISlider sender, float value )
	{
		//Debug.Log( "onVSliderChanged to: " + value );
	}

	
	public void onToggleButtonChanged( UIToggleButton sender, bool selected )
	{
		//Debug.Log( "onToggleButtonChanged to: " + selected );
	}
	
	#endregion;


}
