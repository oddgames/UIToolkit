using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum UIAnimationProperty { Position, LocalScale, EulerAngles, Alpha };

public class UIAnimation
{
	// we store the currently running animations so that we don't run the same one twice
	private static Dictionary<UISprite, List<UIAnimation>> _spriteAnimations = new Dictionary<UISprite, List<UIAnimation>>();
	
	private bool _running;
	public bool running { get { return _running; } }
	
	private UIAnimationProperty _aniProperty;
	public UIAnimationProperty aniProperty { get { return _aniProperty; } }
	
	public Action onComplete;
	public bool autoreverse;
	
	private float startTime;
	private UISprite sprite;
	private float duration;
	private System.Func<float, float> ease;
	private Vector3 start;
	private Vector3 target;
	private float startFloat;
	private float targetFloat;
	
	
	public UIAnimation( UISprite sprite, float duration, UIAnimationProperty aniProperty, Vector3 start, Vector3 target, System.Func<float, float> ease )
	{
		this.sprite = sprite;
		this.duration = duration;
		_aniProperty = aniProperty;
		this.ease = ease;
		
		this.target = target;
		this.start = start;
		
		_running = true;
		startTime = Time.time;
	}


	public UIAnimation( UISprite sprite, float duration, UIAnimationProperty aniProperty, float startFloat, float targetFloat, System.Func<float, float> ease )
	{
		this.sprite = sprite;
		this.duration = duration;
		_aniProperty = aniProperty;
		this.ease = ease;
		
		this.targetFloat = targetFloat;
		this.startFloat = startFloat;
		
		_running = true;
		startTime = Time.time;
	}
	
	
	// CoRoutine that marshals the animation
	public IEnumerator animate()
	{
		// check for any previously running animations of this type.  we dont want them fighting
		if( _spriteAnimations.ContainsKey( sprite ) )
		{
			var sameAnimationType = _spriteAnimations[sprite].Where( i => i.aniProperty == aniProperty ).FirstOrDefault();
			if( sameAnimationType != null )
			{
				Debug.Log( "Killing animatino" );
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
		startTime = Time.time;

		while( _running )
		{				
			// Get our easing position
			float easPos = Mathf.Clamp01( ( Time.time - startTime ) / duration );
			easPos = ease( easPos );
			
			// Set the proper property
			switch( _aniProperty )
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
			if( ( startTime + duration ) <= Time.time )
			{
				// if we are set to autoreverse, flip around a few variables and continue
				if( autoreverse )
				{
					autoreverse = false; // make sure this only happens once!
					
					// flip our start and target and reset the start time
					var temp = start;
					start = target;
					target = temp;
					startTime = Time.time;
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
		return new WaitForSeconds( startTime + duration - Time.time );
	}
	
	
	public void stop()
	{
		_running = false;
	}

}