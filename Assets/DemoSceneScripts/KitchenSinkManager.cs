using UnityEngine;
using System.Collections;

public class KitchenSinkManager : MonoBehaviour
{
	public GUIText swipeText;
	
	void Awake()
	{
	   useGUILayout = false;
	}
	
	
	void Start()
	{
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		UISpriteButton playButton = UI.instance.addSpriteButton( new Rect( 10, 10, 108, 37 ), 3, new UIUVRect( 0, 0, 108, 37 ) );
		playButton.highlightedUVframe = new UIUVRect( 0, 37, 108, 37 );
		
		
		// Scores button
		UISpriteButton scores = UI.instance.addSpriteButton( new Rect( 10, 57, 108, 37 ), 3, new UIUVRect( 0, 74, 108, 37 ) );
		scores.highlightedUVframe = new UIUVRect( 0, 111, 108, 37 );
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 ); // Expand our highlighted touch area 30 pixels all around
		scores.action = onTouchUpInsideScoresButton;
		scores.color = new Color( 1, 1, 1, 0.5f );
		
		
		// Options button
		UISpriteButton optionsButton = UI.instance.addSpriteButton( new Rect( 10, 130, 108, 37 ), 2, new UIUVRect( 0, 148, 108, 37 ) ) as UISpriteButton;
		optionsButton.highlightedUVframe = new UIUVRect( 0, 148 + 37, 108, 37 );
		optionsButton.action = onTouchUpInsideOptionsButton;
		
		
		// Knob
		UIKnob knob = new UIKnob( new Rect( 200, 160, 72, 72 ), 3, new UIUVRect( 109, 0, 72, 72 ), onKnobChanged );
		knob.highlightedUVframe = new UIUVRect( 190, 0, 72, 72 );
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		UI.instance.addTouchableSprite( knob );
		knob.value = 0.3f;
		
		
		// Horizontal Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		UISprite hSliderKnob = UI.instance.addSprite( new Rect( 20, 245, 30, 50 ), new UIUVRect( 120, 130, 30, 50 ), 1 );
		UISlider hSlider = new UISlider( new Rect( 20, 250, 200, 40 ), 5, new UIUVRect( 120, 80, 200, 40 ), hSliderKnob, onHSliderChanged );
		UI.instance.addTouchableSprite( hSlider );
		// Increase our hit area a bit while we are tracking along the slider
		hSlider.highlightedTouchOffsets = new UIEdgeOffsets( 20, 30, 20, 30 );
		hSlider.value = 0.2f;
		
		
		// Vertical Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		UISprite vSliderKnob = UI.instance.addSprite( new Rect( 412, 50, 35, 10 ), new UIUVRect( 345, 130, 35, 10 ), 1 );
		UISlider vSlider = new UISlider( new Rect( 420, 50, 20, 200 ), 3, new UIUVRect( 320, 130, 20, 200 ), vSliderKnob, UISliderLayout.Vertical, onVSliderChanged );
		UI.instance.addTouchableSprite( vSlider );
		// Increase our hit area a bit while we are tracking along the slider
		vSlider.highlightedTouchOffsets = new UIEdgeOffsets( 20, 30, 20, 30 );
		vSlider.continuous = true;
		vSlider.value = 0.5f;
		
		
		// Toggle Button
		UIUVRect normalUVframe = new UIUVRect( 0, 400, 50, 50 );
		UIUVRect highlightedUVframe = new UIUVRect( 0, 450, 50, 50 );
		UIUVRect selectedUVframe = new UIUVRect( 50, 400, 50, 50 );
		Rect toggleFrame = new Rect( 270, 80, 50, 50 );
		UIToggleButton toggleButton = new UIToggleButton( toggleFrame, 2, normalUVframe, selectedUVframe, highlightedUVframe );
		toggleButton.action = onToggleButtonChanged;
		UI.instance.addTouchableSprite( toggleButton );
		toggleButton.selected = true; // Dont change this until the button has been added
		
		
		// Progress/Health bar (be sure the bar is on a lower level than the GUIProgressBar
		UISprite bar = UI.instance.addSprite( new Rect( 251, 267, 128, 8 ), new UIUVRect( 191, 430, 128, 8 ), 1 );
		UIProgressBar progressBar = new UIProgressBar( new Rect( 240, 250, 150, 30 ), 3, new UIUVRect( 180, 400, 150, 30 ), bar );
		progressBar.resizeTextureOnChange = true;
		UI.instance.addSprite( progressBar );
		progressBar.value = 0.0f;
		

		// Test movement
		StartCoroutine( marqueePlayButton( playButton ) );
		StartCoroutine( animateProgressBar( progressBar ) );
		StartCoroutine( pulseOptionButton( optionsButton ) );
		
		
		
		// Swipe detector view - big, giant touchbleSprite behind all others
		UISwipeDetector detector = new UISwipeDetector( new Rect( 0, 60f, Screen.width, Screen.height - 60f ), 10, new UIUVRect( 450, 50, 408, 306 ) );
		detector.action = onSwipe;
		UI.instance.addTouchableSprite( detector );
	}
	
	
	#region CoRoutine animation tests that do not use the GUIAnimation system
	
	// Play coroutine that animates a button marquee style
	private IEnumerator marqueePlayButton( UISpriteButton playButton )
	{
		while( true )
		{
			// Make sure we arent off the right side of the screen
			Vector3 pos = playButton.clientTransform.position;
			if( pos.x > Screen.width + playButton.width / 2 )
			{
				pos.x = -playButton.width / 2;
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
	
	
	private IEnumerator pulseOptionButton( UISpriteButton optionsButton )
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
	public void onTouchUpInsideScoresButton( UISpriteButton sender )
	{
		StartCoroutine( animateLocalScale( sender, new Vector3( 1.3f, 1.3f, 1 ), 0.5f ) );
	}
	
	
	public void onTouchUpInsideOptionsButton( UISpriteButton sender )
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
