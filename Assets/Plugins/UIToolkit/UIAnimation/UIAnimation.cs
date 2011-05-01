using UnityEngine;
using System;
using System.Collections;


public enum UIAnimationProperty { Position, LocalScale, EulerAngles, Alpha };

public class UIAnimation
{	
	public bool running;
	
	float startTime;
	UISprite sprite;
	float duration;
	UIAnimationProperty aniProperty;
	Easing.EasingType easeType;
	IEasing ease;
	Vector3 start;
	Vector3 target;
	float startFloat;
	float targetFloat;
	
	
	public UIAnimation( UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 start, Vector3 target, IEasing ease, Easing.EasingType easeType )
	{
		this.sprite = sprite;
		this.duration = duration;
		this.aniProperty = aniProperty;
		this.ease = ease;
		this.easeType = easeType;
		
		this.target = target;
		this.start = start;
		
		this.running = true;
		startTime = Time.time;
	}


	public UIAnimation( UISprite sprite, float duration, UIAnimationProperty aniProperty, float startFloat, float targetFloat, IEasing ease, Easing.EasingType easeType )
	{
		this.sprite = sprite;
		this.duration = duration;
		this.aniProperty = aniProperty;
		this.ease = ease;
		this.easeType = easeType;
		
		this.targetFloat = targetFloat;
		this.startFloat = startFloat;
		
		this.running = true;
		startTime = Time.time;
	}
	
	
	// CoRoutine that marshals the animation
	public IEnumerator animate()
	{
		// Store our start time
		startTime = Time.time;

		while( running )
		{				
			// Get our easing position
			float easPos = Mathf.Clamp01( ( Time.time - startTime ) / duration );
			
			switch( easeType )
			{
				case Easing.EasingType.In:
					easPos = ease.easeIn( easPos );
					break;
				case Easing.EasingType.Out:
					easPos = ease.easeOut( easPos );
					break;
				default:
					easPos = ease.easeInOut( easPos );
					break;
			}
			
			// Set the proper property
			switch( aniProperty )
			{
				case UIAnimationProperty.Position:
					sprite.clientTransform.position = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.LocalScale:
					sprite.clientTransform.localScale = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.EulerAngles:
					sprite.clientTransform.eulerAngles = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.Alpha:
					Color currentColor = sprite.color;
					currentColor.a = Mathf.Lerp( startFloat, targetFloat, easPos );
					sprite.color = currentColor;
					break;
			}
			
			// Commit the changes back to the main mesh
			sprite.updateTransform();

			// See if we are done with our animation yet
			if( ( startTime + duration ) < Time.time )
				running = false;
			else
				yield return new WaitForEndOfFrame();
		} // end while

	} // end animate


	// Used to chain animations.  This will return when the animation completes
	public WaitForSeconds chain()
	{
		return new WaitForSeconds( startTime + duration - Time.time );
	}

}