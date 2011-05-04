using UnityEngine;
using System.Collections;

public class KitchenSinkManager : MonoBehaviour
{
	public GUIText swipeText;
	public AudioClip scoresSound;
	public AudioClip optionsSound;
	

	void Start()
	{
		// we will use these to help out with relatively positioning some items
		int x; 
		int y;
		

		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .05f );
		var playButton = UIButton.create( "playUp.png", "playDown.png", 0, y, 6 );
		playButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		playButton.onTouchUpInside += ( sender )  => Debug.Log( "clicked the button: " + sender );
		
		
		// Scores button
		x = UIRelative.xPercentFrom( UIxAnchor.Left, .02f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .24f );
		var scores = UIContinuousButton.create( "scoresUp.png", "scoresDown.png", x, y );
		scores.centerize();
		scores.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		scores.onTouchUpInside += onTouchUpInsideScoresButton;
		scores.onTouchIsDown += ( sender ) => Debug.Log( "touch is down: " + Time.time );
		scores.touchDownSound = scoresSound;
	
	
		// Options button
		x = UIRelative.xPercentFrom( UIxAnchor.Left, .02f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .43f );
		var optionsButton = UIButton.create( "optionsUp.png", "optionsDown.png", x, y );
		optionsButton.onTouchUpInside += onTouchUpInsideOptionsButton;
		optionsButton.touchDownSound = optionsSound;
		
		
		// Knob
		x = UIRelative.xPercentFrom( UIxAnchor.Left, .5f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .39f );
		var knob = UIKnob.create( "knobUp.png", "knobDown.png", x, y );
		knob.normalTouchOffsets = new UIEdgeOffsets( 10 ); // give the knob a bit extra touch area
		knob.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
		knob.onKnobChanged += onKnobChanged;
		knob.value = 0.3f;
		
		
		// Horizontal Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		x = UIRelative.xPercentFrom( UIxAnchor.Left, .02f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .7f );
		var hSlider = UISlider.create( "sliderKnob.png", "hSlider.png", x, y, UISliderLayout.Horizontal );
		hSlider.highlightedTouchOffsets = new UIEdgeOffsets( 30, 20, 30, 20 );
		hSlider.onChange += ( sender, val ) => Debug.Log( val );
		hSlider.value = 0.6f;
		
		
		// Vertical Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		x = UIRelative.xPercentFrom( UIxAnchor.Right, .1f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .21f );
		var vSlider = UISlider.create( "vSliderKnob.png", "vSlider.png", x, y, UISliderLayout.Vertical );
		vSlider.highlightedTouchOffsets = new UIEdgeOffsets( 20, 30, 20, 30 );
		vSlider.continuous = true;
		vSlider.onChange += ( sender, val ) => Debug.Log( val );
		vSlider.value = 0.3f;

		
		// Toggle Button
		x = UIRelative.xPercentFrom( UIxAnchor.Right, .3f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .35f );
		var toggleButton = UIToggleButton.create( "cbUnchecked.png", "cbChecked.png", "cbDown.png", x, y );
		toggleButton.onToggle += onToggleButtonChanged;
		toggleButton.selected = true;
		

		// Progress/Health bar (be sure the bar is on a lower level than the GUIProgressBar
		x = UIRelative.xPercentFrom( UIxAnchor.Left, .02f );
		y = UIRelative.yPercentFrom( UIyAnchor.Top, .9f );
		var progressBar = UIProgressBar.create( "progressBar.png", "progressBarBorder.png", 5, 3, x, y );
		progressBar.resizeTextureOnChange = true;
		progressBar.value = 0.4f;
		
		
		// animated sprite
		x = UIRelative.xPercentFrom( UIxAnchor.Right, .3f );
		y = UIRelative.yPercentFrom( UIyAnchor.Bottom, .2f );
		var animatedSprite = UI.instance.addSprite( "Gai_1.png", x, y, 1, true );
		var anim = animatedSprite.addSpriteAnimation( "anim", 0.15f, "Gai_1.png", "Gai_2.png", "Gai_3.png", "Gai_4.png", "Gai_5.png", "Gai_6.png", "Gai_7.png", "Gai_8.png", "Gai_9.png", "Gai_10.png", "Gai_11.png", "Gai_12.png" );
		anim.loopReverse = true; // optinally loop in reverse
		animatedSprite.playSpriteAnimation( "anim", 5 );
		
		
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
