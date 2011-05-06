using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIEaseType = System.Func<float, float>; 


public static class UISpriteAnimationExtensions
{
	#region Specific Animation methods
	
	// alpha
	public static UIAnimation alphaTo( this UISprite sprite, float duration, float target, UIEaseType ease )
	{
		return to( sprite, duration, UIAnimationProperty.Alpha, target, ease );
	}


	public static UIAnimation alphaFrom( this UISprite sprite, float duration, float target, UIEaseType ease )
	{
		return from( sprite, duration, UIAnimationProperty.Alpha, target, ease );
	}


	public static UIAnimation alphaFromTo( this UISprite sprite, float duration, float start, float target, UIEaseType ease )
	{
		return fromTo( sprite, duration, UIAnimationProperty.Alpha, start, target, ease );
	}

	
	// euler angles
	public static UIAnimation eulerAnglesTo( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return to( sprite, duration, UIAnimationProperty.EulerAngles, target, ease );
	}
	

	public static UIAnimation eulerAnglesFrom( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return from( sprite, duration, UIAnimationProperty.EulerAngles, target, ease );
	}
	
	
	public static UIAnimation eulerAnglesFromTo( this UISprite sprite, float duration, Vector3 start, Vector3 target, UIEaseType ease )
	{
		return fromTo( sprite, duration, UIAnimationProperty.EulerAngles, start, target, ease );
	}


	// local scale
	public static UIAnimation scaleTo( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return to( sprite, duration, UIAnimationProperty.LocalScale, target, ease );
	}
	

	public static UIAnimation scaleFrom( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return from( sprite, duration, UIAnimationProperty.LocalScale, target, ease );
	}
	
	
	public static UIAnimation scaleFromTo( this UISprite sprite, float duration, Vector3 start, Vector3 target, UIEaseType ease )
	{
		return fromTo( sprite, duration, UIAnimationProperty.LocalScale, start, target, ease );
	}


	// position
	public static UIAnimation positionTo( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return to( sprite, duration, UIAnimationProperty.Position, target, ease );
	}
	

	public static UIAnimation positionFrom( this UISprite sprite, float duration, Vector3 target, UIEaseType ease )
	{
		return from( sprite, duration, UIAnimationProperty.Position, target, ease );
	}
	
	
	public static UIAnimation positionFromTo( this UISprite sprite, float duration, Vector3 start, Vector3 target, UIEaseType ease )
	{
		return fromTo( sprite, duration, UIAnimationProperty.Position, start, target, ease );
	}

	
	#endregion
	
	
	#region Generic Animation methods
	
	// float version (for alpha)
	public static UIAnimation to( this UISprite sprite, float duration, UIAnimationProperty aniProperty, float target, UIEaseType ease )
	{
		return animate( sprite, true, duration, aniProperty, target, ease );
	}
	
	
	// Vector3 version
	public static UIAnimation to( this UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 target, UIEaseType ease )
	{
		return animate( sprite, true, duration, aniProperty, target, ease );
	}

	
	// float version
	public static UIAnimation from( this UISprite sprite, float duration, UIAnimationProperty aniProperty, float target, UIEaseType ease )
	{
		return animate( sprite, false, duration, aniProperty, target, ease );
	}
	

	// Vector3 version
	public static UIAnimation from( this UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 target, UIEaseType ease )
	{
		return animate( sprite, false, duration, aniProperty, target, ease );
	}


	// float version (for alpha)
	public static UIAnimation fromTo( this UISprite sprite, float duration, UIAnimationProperty aniProperty, float start, float target, UIEaseType ease )
	{
		return animate( sprite, duration, aniProperty, start, target, ease );
	}
	
	
	// Vector3 version
	public static UIAnimation fromTo( this UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 start, Vector3 target, UIEaseType ease )
	{
		return animate( sprite, duration, aniProperty, start, target, ease );
	}

	
	// Figures out the start value and kicks off the animation
	private static UIAnimation animate( UISprite sprite, bool animateTo, float duration, UIAnimationProperty aniProperty, float target, UIEaseType ease )
	{
		float current = 0.0f;
		
		// Grab the current value
		switch( aniProperty )
		{
			case UIAnimationProperty.Alpha:
				current = sprite.color.a;
				break;
		}

		float start = ( animateTo ) ? current : target;

		// If we are doing a 'from', the target is our current position
		if( !animateTo )
			target = current;
		
		return animate( sprite, duration, aniProperty, start, target, ease );
	}
	

	// Sets up and starts a new animation in a Coroutine - float version
	private static UIAnimation animate( UISprite sprite, float duration, UIAnimationProperty aniProperty, float start, float target, UIEaseType ease )
	{
		UIAnimation ani = new UIAnimation( sprite, duration, aniProperty, start, target, ease );
		UI.instance.StartCoroutine( ani.animate() );
		
		return ani;
	}
	

	// Figures out the start value and kicks off the animation
	private static UIAnimation animate( UISprite sprite, bool animateTo, float duration, UIAnimationProperty aniProperty, Vector3 target, UIEaseType ease )
	{
		Vector3 current = Vector3.zero;
		
		// Grab the current value
		switch( aniProperty )
		{
			case UIAnimationProperty.Position:
				current = sprite.position;
				break;
			case UIAnimationProperty.LocalScale:
				current = sprite.localScale;
				break;
			case UIAnimationProperty.EulerAngles:
				current = sprite.eulerAngles;
				break;
		}
		
		Vector3 start = ( animateTo ) ? current : target;
		
		// If we are doing a 'from', the target is our current position
		if( !animateTo )
			target = current;
		
		return animate( sprite, duration, aniProperty, start, target, ease );
	}


	// Sets up and starts a new animation in a Coroutine - Vector3 version
	private static UIAnimation animate( UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 start, Vector3 target, UIEaseType ease )
	{
		UIAnimation ani = new UIAnimation( sprite, duration, aniProperty, start, target, ease );
		UI.instance.StartCoroutine( ani.animate() );
		
		return ani;
	}
	
	#endregion

}
