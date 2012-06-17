using System;
using UnityEngine;
using System.Collections.Generic;


public class UIToolkit : UISpriteManager
{
	// All access should go through instance
	static public UIToolkit instance = null;
	
	public bool displayTouchDebugAreas = false; // if true, gizmos will be used to show the hit areas in the editor
	private ITouchable[] _spriteSelected;
	
	// Holds all our touchable sprites
	private List<ITouchable> _touchableSprites = new List<ITouchable>();
	
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	private Vector2? lastMousePosition;
#endif

	
	#region MonoBehaviour Functions
	
	public bool useOutsideTouchControl = false; // if true, toolkit does not receive touches

	// used by outside touch controllers
	public List<ITouchable> touchableSprites
	{
		get { return _touchableSprites; } 
	}
	
	
	protected override void Awake()
	{
		// Set instance to this so we can be accessed from anywhere
		instance = this;
		
		base.Awake();

		_spriteSelected = new ITouchable[12];
		for( int i = 0; i < 12; ++i )
			_spriteSelected[i] = null;
	}


	protected void Update()
	{
		if( useOutsideTouchControl )
			return;
		
		// only do our touch processing if we have some touches
		if( Input.touchCount > 0 )
		{
			// Examine all current touches
			for( int i = 0; i < Input.touchCount; i++ )
			{
				lookAtTouch( Input.GetTouch( i ) );
			}
		} // end if Input.touchCount
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		else
		{
			// no touches. so check the mouse input if we are in the editor
			
			// check for each mouse state in turn, no elses here. They can all occur on the same frame!
			if( Input.GetMouseButtonDown( 0 ) )
			{
				lookAtTouch( UITouchMaker.createTouchFromInput( UIMouseState.DownThisFrame, ref lastMousePosition ) );
			}

			if( Input.GetMouseButton( 0 ) )
			{
				lookAtTouch( UITouchMaker.createTouchFromInput( UIMouseState.HeldDown, ref lastMousePosition ) );
			}
			
			if( Input.GetMouseButtonUp( 0 ) )
			{
				lookAtTouch( UITouchMaker.createTouchFromInput( UIMouseState.UpThisFrame, ref lastMousePosition ) );
			}			
			
			// handle hover states
			// if we have a previously hovered sprite unhover it
			for( int i = 0, totalTouchables = _touchableSprites.Count; i < totalTouchables; i++ )
			{
				if( _touchableSprites[i].hoveredOver )
					_touchableSprites[i].hoveredOver = false;
			}
			
			var fixedMousePosition = new Vector2( Input.mousePosition.x, Screen.height - Input.mousePosition.y );
			var hoveredSprite = getButtonForScreenPosition( fixedMousePosition );
			if( hoveredSprite != null )
			{
				if( !hoveredSprite.highlighted )
					hoveredSprite.hoveredOver = true;
			}
		}
#endif
	}


	protected void LateUpdate()
	{
		// take care of updating our UVs, colors or bounds if necessary
		if( meshIsDirty )
		{
			meshIsDirty = false;
			updateMeshProperties();
		}
	}

	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	protected void OnApplicationQuit()
	{
		material.mainTexture = null;
		instance = null;
		Resources.UnloadUnusedAssets();
	}
	
	
	protected void OnDestroy()
	{
		material.mainTexture = null;
		instance = null;
		Resources.UnloadUnusedAssets();
	}
	

#if UNITY_EDITOR
	// Debug display of our trigger state
	void OnDrawGizmos()
	{
		if( !displayTouchDebugAreas )
			return;

		// set to whatever color you want to represent
		Gizmos.color = Color.yellow;
		
		// we’re going to draw the gizmo in local space
		Gizmos.matrix = transform.localToWorldMatrix;
	   
		foreach( var item in _touchableSprites )
		{
			var pos = new Vector3( item.touchFrame.x + item.touchFrame.width/2, -( item.touchFrame.y + item.touchFrame.height/2 ), 0 );
			Gizmos.DrawWireCube( pos, new Vector3( item.touchFrame.width, item.touchFrame.height, 5 ) );
		}
		
		/* TODO: fix the debug touches.  they arent lined up correctly in the camera preview
		// display debug touches
		if( Input.touchCount > 0 )
		{
			Gizmos.color = Color.green;
			for( int i = 0; i < Input.touchCount; i++ )
			{
				var touch = Input.GetTouch( i );
				var pos = _uiCamera.ScreenToWorldPoint( touch.position );
				Gizmos.DrawCube( pos, new Vector3( 20, 20, 5 ) );
			}
		}
		else
		{
			// display debug fake touches from the mouse
			if( Input.GetMouseButton( 0 ) )
			{
				Vector2? fakeVec = Vector2.zero;
				var touch = UIFakeTouch.fromInput( ref fakeVec );
				var pos = _uiCamera.ScreenToWorldPoint( touch.position );
				Gizmos.DrawCube( pos, new Vector3( 20, 20, 5 ) );
			}
		}
		*/
	}
#endif

	#endregion


	#region Add/Remove Element and Button functions

	public void addTouchableSprite( ITouchable touchableSprite )
	{
		if( touchableSprite is UISprite )
			addSprite( touchableSprite as UISprite );
			
		// Add the sprite to our touchables and sort them		
		addToTouchables( touchableSprite );
	}
	
	
	/// <summary>
	/// Removes a sprite
	/// </summary>
	public void removeElement( UISprite sprite )
	{
		// If we are removing a ITouchable remove it from our internal array as well
		if( sprite is ITouchable )
			_touchableSprites.Remove( sprite as ITouchable );

		removeSprite( sprite );
	}
	
	
	public void removeFromTouchables( ITouchable touchable )
	{
		_touchableSprites.Remove( touchable );
	}

	
	public void addToTouchables( ITouchable touchable )
	{
		_touchableSprites.Add( touchable );
		// Sort the sprites container
		_touchableSprites.Sort();

	}

	#endregion

	
	#region Touch management and analysis helpers
	
	// examines a touch and sends off began, moved and ended events
	private void lookAtTouch( Touch touch )
	{
		// tranform the touch position so the origin is in the top left
		var fixedTouchPosition = new Vector2( touch.position.x, Screen.height - touch.position.y );
		var button = getButtonForScreenPosition( fixedTouchPosition );

		bool touchEnded = ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled );
		
		if( touch.phase == TouchPhase.Began )
		{
			if( button != null )
			{
				_spriteSelected[touch.fingerId] = button;
				button.onTouchBegan( touch, fixedTouchPosition );
			}
			else
			{
				// deselect any selected sprites for this touch
				_spriteSelected[touch.fingerId] = null;
			}
		}
		else if( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
		{
			if( button != null && _spriteSelected[touch.fingerId] == button )
			{
				// stationary should get touchMoved as well...I think...still testing all scenarious
				// if we have a moving touch on a sprite keep sending touchMoved
				_spriteSelected[touch.fingerId].onTouchMoved( touch, fixedTouchPosition );
			}
			else if( _spriteSelected[touch.fingerId] != null )
			{
				// If we have a button that isn't the selected button end the touch on it because we moved off of it
				_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				_spriteSelected[touch.fingerId] = null;
			}
			else if( button != null && _spriteSelected[touch.fingerId] == null && button.allowTouchBeganWhenMovedOver )
			{
				// this happens when we started a touch not over a button then the finger slid over the button. if the
				// allowTouchBeganWhenMovedOver property is true, we count this as a touchBegan
				_spriteSelected[touch.fingerId] = button;
				button.onTouchBegan( touch, fixedTouchPosition );
			}
		}
		else if( touchEnded )
		{
			if( button != null )
			{
				// If we are getting an exit over a previously selected button send it an onTouchEnded
				if( _spriteSelected[touch.fingerId] != button && _spriteSelected[touch.fingerId] != null )
				{
					_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				}
				else if( _spriteSelected[touch.fingerId] == button )
				{
					_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, true );
				}
				
				// Deselect the touched sprite
				_spriteSelected[touch.fingerId] = null;
			}
			else if(_spriteSelected[touch.fingerId] != null)
			{
				// If we have a button that isn't the selected button end the touch on it because we moved off of it
				// quickly enough that we never got a TouchPhase.Moved or TouchPhase.Stationary
				_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				_spriteSelected[touch.fingerId] = null;
			}			
		}
	}

	
	// Gets the closets touchableSprite to the camera that contains the touchPosition
	private ITouchable getButtonForScreenPosition( Vector2 touchPosition )
	{
		// Run through our touchables in order.  They are sorted by z-index already.
		for( int i = 0, totalTouchables = _touchableSprites.Count; i < totalTouchables; i++ )
		{
			if( !_touchableSprites[i].hidden && _touchableSprites[i].hitTest( touchPosition ) )
				return _touchableSprites[i];
		}
		
		return null;
	}

	#endregion


}
