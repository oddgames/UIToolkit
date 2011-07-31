using UnityEngine;
using System;


public abstract class UITouchableSprite : UISprite, IComparable
{
	public int touchCount;
	public UIUVRect disabledUVframe; // when disabled, this UV frame will be used if it is set
	public UIUVRect hoveredUVframe; // when hovered over, this UV frame will be used if it is set
	
	protected UIEdgeOffsets _normalTouchOffsets;
	protected UIEdgeOffsets _highlightedTouchOffsets;
	protected Rect _highlightedTouchFrame;
	protected Rect _normalTouchFrame;
	protected UIUVRect _tempUVframe; // Holds a copy of the uvFrame while in either the highlighted or disabled state
	
	protected bool touchFrameIsDirty = true; // Indicates if the touchFrames need to be recalculated
	
	protected bool _highlighted;
	protected bool _disabled;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	protected bool _hoveredOver;
#endif
	
	public UITouchableSprite( Rect frame, int depth, UIUVRect uvFrame ):base( frame, depth, uvFrame )
	{
		_tempUVframe = uvFrame;
	}
	
	
	// constructor for when the need to have a centered UISprite arises (I'm looking at you UIKnob)
	public UITouchableSprite( Rect frame, int depth, UIUVRect uvFrame, bool gameObjectOriginInCenter ):base( frame, depth, uvFrame, gameObjectOriginInCenter )
	{
	}
	

	#region Transform passthrough properties - we need to set the touchFrame dirty when these change
	
	public new Vector3 position
	{
		get { return clientTransform.position; }
		set
		{
			clientTransform.position = value;
			updateTransform();
		}
	}


	public new Vector3 localPosition
	{
		get { return clientTransform.localPosition; }
		set
		{
			clientTransform.localPosition = value;
			updateTransform();
		}
	}
	
	#endregion

	
	#region Properties and Getters/Setters

	// Adds or subtracts from the frame of the button to define a hit area
	public UIEdgeOffsets highlightedTouchOffsets
	{
		get { return _highlightedTouchOffsets; }
		set
		{
			_highlightedTouchOffsets = value;
			touchFrameIsDirty = true;
		}
	}


	// Adds or subtracts from the frame of the button to define a hit area
	public UIEdgeOffsets normalTouchOffsets
	{
		get { return _normalTouchOffsets; }
		set
		{
			_normalTouchOffsets = value;
			touchFrameIsDirty = true;
		}
	}


	// Returns a frame to use to see if this element was touched
	public Rect touchFrame
	{
		get
		{
			// if we are disabled, we have no touchFrame to touch
			if( _disabled )
				return UISprite._rectZero;

			// If the frame is dirty, recalculate it
			if( touchFrameIsDirty )
			{
				touchFrameIsDirty = false;
				
				// grab the normal frame of the sprite then add the offsets to get our touch frames
				// remembering to offset if we have our origin in the center
				Rect normalFrame = new Rect( clientTransform.position.x, -clientTransform.position.y, width, height );
				
				if( gameObjectOriginInCenter )
				{
					normalFrame.x -= width / 2;
					normalFrame.y -= height / 2;
				}

				_normalTouchFrame = _normalTouchOffsets.addToRect( normalFrame );
				_highlightedTouchFrame = _highlightedTouchOffsets.addToRect( normalFrame );
			}
			
			// Either return our highlighted or normal touch frame
			return ( _highlighted ) ? _highlightedTouchFrame : _normalTouchFrame;
		}
	}


	// Override transform() so we can mark the touchFrame as dirty
	public override void updateTransform()
	{
		base.updateTransform();
		
		touchFrameIsDirty = true;
	}

	#endregion;


	// Tests if a point is inside the current touchFrame
	public bool hitTest( Vector2 point )
	{
		return touchFrame.Contains( point );
	}

	
	// Indicates if there is a finger over this element
	public virtual bool highlighted
	{
		get { return _highlighted; }
		set { _highlighted = value;	}
	}
	
	
	// indicates if the mouse pointer is hovering over this element
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual bool hoveredOver
	{
		get { return _hoveredOver; }
		set
		{
			// no hovered UV? no continue
			if( hoveredUVframe.Equals( UIUVRect.zero ) )
				return;
			
			if( _hoveredOver != value )
			{
				_hoveredOver = value;

				// if we have a hoveredUVframe use it
				if( value )
					uvFrame = hoveredUVframe;
				else
					uvFrame = _tempUVframe;
			}
		}
	}
#endif

	
	// a disabled UITouchableSprite will have a touchFrame of all zeros
	public virtual bool disabled
	{
		get { return _disabled; }
		set
		{
			if( _disabled != value )
			{
				_disabled = value;
				
				// if we have a disabledUVframe use it
				if( value && !disabledUVframe.Equals( UIUVRect.zero ) )
					uvFrame = disabledUVframe;
				else
					uvFrame = _tempUVframe;
			}
		}
	}
	

	// Transforms a point to local coordinates (origin is top left)
	protected Vector2 inverseTranformPoint( Vector2 point )
	{
		return new Vector2( point.x - _normalTouchFrame.xMin, point.y - _normalTouchFrame.yMin );
	}
	
	
	public override void centerize()
	{
		touchFrameIsDirty = true;
		base.centerize();
	}


	#region Touch handlers
	
	// Touch handlers.  Subclasses should override these to get their specific behaviour
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchBegan( UIFakeTouch touch, Vector2 touchPos )
#else
	public virtual void onTouchBegan( Touch touch, Vector2 touchPos )
#endif
	{
		highlighted = true;
	}


#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchMoved( UIFakeTouch touch, Vector2 touchPos )
#else
	public virtual void onTouchMoved( Touch touch, Vector2 touchPos )
#endif
	{

	}
	

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public virtual void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public virtual void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		highlighted = false;
	}

	#endregion;
	

    // IComparable - sorts based on the z value of the client
	public int CompareTo( object obj )
    {
        if( obj is UITouchableSprite )
        {
            var temp = obj as UITouchableSprite;
            return position.z.CompareTo( temp.position.z );
        }
		
		return -1;
    }

}

