using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


public class UISpriteAnimation
{
	public bool loopReverse; // should we play the animation in reverse when we loop?
	public float frameTime = 0.2f;
	
	private List<UIUVRect> uvRects = new List<UIUVRect>();
	private bool _isPlaying;
	public bool isPlaying { get { return _isPlaying; } }


	public UISpriteAnimation( float frameTime, List<UIUVRect> uvRects )
	{
		this.frameTime = frameTime;
		this.uvRects = uvRects;
	}


	public IEnumerator play( UISprite sprite, int loopCount )
	{
		// store the original uvFrame so we can restore it when done
		var originalUVFrame = sprite.uvFrame;
		
		var totalFrames = uvRects.Count;
		var currentFrame = 0;
		var waiter = new WaitForSeconds( frameTime );
		bool loopingForward = true;
		_isPlaying = true;
		
		// loop while we are playing and we havent finished looping
		while( _isPlaying && ( loopCount >= 0 || loopCount == -1 ) )
		{
			// what frame are we on?
			if( loopingForward )
				++currentFrame;
			else
				--currentFrame;
			
			// bounds check
			if( currentFrame < 0 || currentFrame == totalFrames )
			{
				// finished a loop, increment loop counter, reverse loop direction if necessary and reset currentFrame
				if( loopCount > 0 )
					--loopCount;
				
				if( loopReverse )
					loopingForward = !loopingForward;
				
				if( loopingForward )
					currentFrame = 0;
				else
					--currentFrame;
			}
			
			// set the new uvRect
			sprite.uvFrame = uvRects[currentFrame];
			
			yield return waiter;
		}
		
		// all done, restore the original frame
		sprite.uvFrame = originalUVFrame;
	}
	
	
	public void stop()
	{
		_isPlaying = false;
	}

}
