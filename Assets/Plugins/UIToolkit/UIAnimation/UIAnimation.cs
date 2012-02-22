using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum UIAnimationProperty { Position, Scale, EulerAngles, Alpha, Color };

public class UIAnimation
{
	// we store the currently running animations so that we don't run the same one twice
	private static Dictionary<UIObject, List<UIAnimation>> _spriteAnimations = new Dictionary<UIObject, List<UIAnimation>>();
	
	private bool _running;
	public bool running { get { return _running; } }
	
	private UIAnimationProperty _aniProperty;
	public UIAnimationProperty aniProperty { get { return _aniProperty; } }
	
	public Action onComplete;
	public bool autoreverse;
	
	private float startTime;
	private UIObject sprite;
	private float duration;
    private bool affectedByTimeScale;
	private System.Func<float, float> ease;
	
	// target properties of different types for the different animations
	private Vector3 start;
	private Vector3 target;
	private float startFloat;
	private float targetFloat;
	private Color startColor;
	private Color targetColor;


	public UIAnimation(UIObject sprite, float duration, UIAnimationProperty aniProperty, Vector3 start, Vector3 target, System.Func<float, float> ease, bool affectedByTimeScale = true)
	{
		this.sprite = sprite;
		this.duration = duration;
		_aniProperty = aniProperty;
		this.ease = ease;
		this.affectedByTimeScale = affectedByTimeScale;
		this.target = target;
		this.start = start;
		
		_running = true;
		startTime = affectedByTimeScale ? Time.time : Time.realtimeSinceStartup;
	}


	public UIAnimation(UIObject sprite, float duration, UIAnimationProperty aniProperty, float startFloat, float targetFloat, System.Func<float, float> ease, bool affectedByTimeScale = true)
	{
		this.sprite = sprite;
		this.duration = duration;
		_aniProperty = aniProperty;
		this.ease = ease;
		this.affectedByTimeScale = affectedByTimeScale;
		this.targetFloat = targetFloat;
		this.startFloat = startFloat;
		
		_running = true;
		startTime = affectedByTimeScale ? Time.time : Time.realtimeSinceStartup;
	}


	public UIAnimation(UIObject sprite, float duration, UIAnimationProperty aniProperty, Color startColor, Color targetColor, System.Func<float, float> ease, bool affectedByTimeScale = true)
	{
		this.sprite = sprite;
		this.duration = duration;
		_aniProperty = aniProperty;
		this.ease = ease;
		this.affectedByTimeScale = affectedByTimeScale;
		this.startColor = startColor;
		this.targetColor = targetColor;
		
		_running = true;
		startTime = affectedByTimeScale ? Time.time : Time.realtimeSinceStartup;
	}

	
	// coroutine that marshals the animation
	public IEnumerator animate()
	{
		_running = true;
		
		// check for any previously running animations of this type.  we dont want them fighting
		if( _spriteAnimations.ContainsKey( sprite ) )
		{
			var sameAnimationType = _spriteAnimations[sprite].Where( i => i.aniProperty == aniProperty ).FirstOrDefault();
			if( sameAnimationType != null )
			{
				sameAnimationType.stop();
				_spriteAnimations[sprite].Remove( sameAnimationType );
			}
		}
		else
		{
			// prep the storage bin.  we know that we don't have the sprite in the dict so add it with a clean list
			_spriteAnimations.Add( sprite, new List<UIAnimation>() );
		}
		
		// add the current animation to our storage
		_spriteAnimations[sprite].Add( this );
		
		// Store our start time
		startTime = affectedByTimeScale ? Time.time : Time.realtimeSinceStartup;

		while( _running )
		{
			float currentTime = affectedByTimeScale ? Time.time : Time.realtimeSinceStartup;
			// Get our easing position
			var easPos = Mathf.Clamp01((currentTime - startTime) / duration);
			easPos = ease( easPos );
			
			// Set the proper property
			switch( _aniProperty )
			{
				case UIAnimationProperty.Position:
					sprite.localPosition = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.Scale:
					sprite.scale = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.EulerAngles:
					sprite.eulerAngles = Vector3.Lerp( start, target, easPos );
					break;
				case UIAnimationProperty.Alpha:
					Color currentColor = sprite.color;
					currentColor.a = Mathf.Lerp( startFloat, targetFloat, easPos );
					sprite.color = currentColor;
					break;
				case UIAnimationProperty.Color:
					sprite.color = Color.Lerp( startColor, targetColor, easPos );
					break;
			}

			// See if we are done with our animation yet
			if ((startTime + duration) <= currentTime)
			{
				// if we are set to autoreverse, flip around a few variables and continue
				if( autoreverse )
				{
					autoreverse = false; // make sure this only happens once!
					
					// flip our start and target
					var temp = start;
					start = target;
					target = temp;
					// flip alpha variables
					var tempFloat = startFloat;
					startFloat = targetFloat;
					targetFloat = tempFloat;
					// flip color variables
					var tempColor = startColor;
					startColor = targetColor;
					targetColor = tempColor;
					// reset the start time
					startTime = currentTime;
				}
				else
				{
					_running = false;
				}
			}
			else
			{
				yield return null;
			}
		} // end while
		

		// clean out the animation from the storage bin
		_spriteAnimations[sprite].Remove( this );
		
		// clear out the sprite from our dict if there are no active animations
		if( _spriteAnimations[sprite].Count == 0 )
			_spriteAnimations.Remove( sprite );
		
		// fire the onComplete handler AFTER cleaning up our animation storage bin
		if( onComplete != null )
			onComplete();
	} // end animate


	// Used to chain animations.  This will return when the animation completes
	public WaitForSeconds chain()
	{
		var multiplier = autoreverse ? 2 : 1;
		return new WaitForSeconds(startTime + (duration * multiplier) - Time.time);
	}


	public IEnumerator realChain()
	{
		var multiplier = autoreverse ? 2 : 1;
		var endTime = startTime + (duration * multiplier);
		while (endTime - Time.realtimeSinceStartup > 0f)
		{
			yield return null;
		}
	}
	
	
	public void stop()
	{
		_running = false;
	}
	
	
	/// <summary>
	/// resets the start value to the current. useful for when the sprite has moved/scaled/changed and you still want to use the same UIAnimation
	/// </summary>
	public void restartStartToCurrent()
	{
		switch( _aniProperty )
		{
			case UIAnimationProperty.Position:
				start = sprite.localPosition;
				break;
			case UIAnimationProperty.Scale:
				start = sprite.scale;
				break;
			case UIAnimationProperty.EulerAngles:
				start = sprite.eulerAngles;
				break;
			case UIAnimationProperty.Alpha:
				Color currentColor = sprite.color;
				startFloat = currentColor.a;
				break;
			case UIAnimationProperty.Color:
				startColor = sprite.color;
				break;
		}
	}
	
	
	/// <summary>
	/// sets the duration. this will have no effect if an animation is currently running
	/// </summary>
	public void setDuration( float newDuration )
	{
		if( !_running )
			duration = newDuration;
	}
	
	
	/// <summary>
	/// sets the target value. this will have no effect if an animation is currently running
	/// </summary>
	public void setTarget( Vector3 newTarget )
	{
		if( _running )
			return;
		
		switch( _aniProperty )
		{
			case UIAnimationProperty.Position:
			case UIAnimationProperty.Scale:
			case UIAnimationProperty.EulerAngles:
				target = newTarget;
				break;
		}
	}
	
}