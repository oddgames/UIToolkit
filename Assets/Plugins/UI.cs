using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Reflection;
using System.Reflection.Emit;
#endif


public enum UIxRelativeTo { Left, Right };
public enum UIyRelativeTo { Top, Bottom };


public class UI : UISpriteManager
{
	// All access should go through instance
	static public UI instance = null;
	
	private const int MAX_CHANGED_TOUCHES = 4;
	
	public bool displayTouchDebugAreas = false; // if true, gizmos will be used to show the hit areas in the editor
	public int drawDepth = 100;	
	public LayerMask UILayer = 0;
	[HideInInspector]
	public int layer;
	
	private Camera _uiCamera;
	private GameObject _uiCameraHolder;
	private UITouchableSprite[] _spriteSelected;
	private Vector2 _screenResolution;
	
	// Holds all our touchable sprites
	private List<UITouchableSprite> _touchableSprites = new List<UITouchableSprite>();
	
#if UNITY_EDITOR
	private Vector2? lastMousePosition;
#endif

	
	#region Unity MonoBehaviour Functions
	
	protected override void Awake()
	{
		// Set instance to this so we can be accessed from anywhere
		instance = this;
		
		base.Awake();

		// Create the camera
		_uiCameraHolder = new GameObject();
		_uiCameraHolder.transform.parent = gameObject.transform;
		_uiCameraHolder.AddComponent( "Camera" );
		
		_uiCamera = _uiCameraHolder.camera;
		_uiCamera.name = "UICamera";
		_uiCamera.clearFlags = CameraClearFlags.Depth;
		_uiCamera.nearClipPlane = 0.3f;
		_uiCamera.farClipPlane = 50.0f;
		_uiCamera.depth = drawDepth;
		_uiCamera.rect = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
		_uiCamera.orthographic = true;
		
		_screenResolution = new Vector2( Screen.width, Screen.height );
		_uiCamera.orthographicSize = Screen.height / 2;

		// Set the camera position based on the screenResolution/orientation
		_uiCamera.transform.position = new Vector3( _screenResolution.x / 2, -_screenResolution.y / 2, -10.0f );
		_uiCamera.cullingMask = UILayer;
		
		// Cache the layer for later use when adding sprites
		// UILayer.value is a mask, find which bit is set 
		for( int i = 0; i < sizeof( int ) * 8; i++ )
		{
			if( ( UILayer.value & (1 << i) ) == (1 << i) )
			{
				layer = i;
				break;
			}
		}

		_spriteSelected = new UITouchableSprite[MAX_CHANGED_TOUCHES];
		for( int i = 0; i < MAX_CHANGED_TOUCHES; ++i )
			_spriteSelected[i] = null;
	}


	protected void Update()
	{
		// only do our touch processing if we have some touches
		if( Input.touchCount > 0 )
		{
			// Examine all current touches
			for( int i = 0; i < Input.touchCount; i++ )
			{
#if UNITY_EDITOR
				lookAtTouch( UIFakeTouch.fromTouch( Input.GetTouch( i ) ) );
#else
				lookAtTouch( Input.GetTouch( i ) );
#endif
			}
		} // end if Input.touchCount
#if UNITY_EDITOR
		else
		{
			// no touches. so check the mouse input if we are in the editor
			if( Input.GetMouseButton( 0 ) || Input.GetMouseButtonUp( 0 ) )
				lookAtTouch( UIFakeTouch.fromInput() );
		}
#endif

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
			// touch position varies based on if we have the GO in the center
			var pos = item.clientTransform.position;
			if( !item.gameObjectOriginInCenter )
			{
				pos.x += item.width / 2;
				pos.y -= item.height / 2;
			}
			
			// we cant use the touchFrame.x/y directly because it's coordinate space is from the top left
			Gizmos.DrawWireCube( pos, new Vector3( item.touchFrame.width, item.touchFrame.height, 5 ) );
		}
		
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
				var touch = UIFakeTouch.fromInput();
				var pos = _uiCamera.ScreenToWorldPoint( touch.position );
				Gizmos.DrawCube( pos, new Vector3( 20, 20, 5 ) );
			}
		}
	}
#endif

	#endregion;


	#region Add/Remove Element and Button functions

	public void addTouchableSprite( UITouchableSprite touchableSprite )
	{
		addSprite( touchableSprite );
		
		// Add the sprite to our touchables and sort them		
		_touchableSprites.Add( touchableSprite );
		_touchableSprites.Sort();
	}
	
	
	// Removes a sprite or touchableSprite
	public void removeElement( UISprite sprite )
	{
		// If we are removing a GUITouchableSprite remove it from our internal array as well
		if( sprite is UITouchableSprite )
			_touchableSprites.Remove( sprite as UITouchableSprite );

		removeSprite( sprite );
	}

	#endregion;

	
	#region Touch management and analysis helpers
	
	// examines a touch and sends off began, moved and ended events
#if UNITY_EDITOR
	private void lookAtTouch( UIFakeTouch touch )
#else
	private void lookAtTouch( Touch touch )
#endif
	{
		// tranform the touch position so the origin is in the top left
		Vector2 fixedTouchPosition = new Vector2( touch.position.x, _screenResolution.y - touch.position.y );
		UITouchableSprite button = getButtonForScreenPosition( fixedTouchPosition );

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
				//if( touch.phase == TouchPhase.Moved )
				_spriteSelected[touch.fingerId].onTouchMoved( touch, fixedTouchPosition );
			}
			else if( _spriteSelected[touch.fingerId] != null )
			{
				// If we have a button that isn't the selected button end the touch on it because we moved off of it
				_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				_spriteSelected[touch.fingerId] = null;
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
		}
	}

	
	// Gets the closets touchableSprite to the camera that contains the touchPosition
	private UITouchableSprite getButtonForScreenPosition( Vector2 touchPosition )
	{
		// Run through our touchables in order.  They are sorted by z-index already.
		for( int i = 0, totalTouchables = _touchableSprites.Count; i < totalTouchables; i++ )
		{
			if( !_touchableSprites[i].hidden && _touchableSprites[i].hitTest( touchPosition ) )
				return _touchableSprites[i];
		}
		
		return null;
	}

	#endregion;
	
	
	#region Static layout helpers
	
	// helper function to create a vector from relative verbage. the x and y values should be between 0 and 1
	public static Vector2 relativeVec2( float xPercent, UIxRelativeTo xRelative, float yPercent, UIyRelativeTo yRelative )
	{
		var vec = Vector2.zero;
		
		if( xRelative == UIxRelativeTo.Left )
			vec.x = xPercent * Screen.width;
		else
			vec.x = Screen.width - ( xPercent * Screen.width );
		
		if( yRelative == UIyRelativeTo.Top )
			vec.y = yPercent * Screen.height;
		else
			vec.y = Screen.height - ( yPercent * Screen.height );
		
		return vec;
	}

	#endregion

}
