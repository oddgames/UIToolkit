using UnityEngine;
using System.Collections.Generic;


public class UISprite : UIObject, IPositionable
{
    public UIToolkit manager = null; // Reference to the sprite manager in which this sprite resides
    public bool ___hidden = false; // Indicates whether this sprite is currently hidden (DO NOT ACCESS DIRECTLY)
	public static readonly Rect _rectZero = new Rect( 0, 0, 0, 0 ); // used for disabled touch frames
	private bool _suspendUpdates; // when true, updateTransform and updateVertPositions will do nothing until endUpdates is called
	
    public new float width { get { return _width * scale.x; } }  // Width and Height of the sprite in worldspace units.
    public new float height { get { return _height * scale.y; } }
	public bool gameObjectOriginInCenter = false;  // Set to true to get your origin in the center.  Useful for scaling/rotating
    
    public Color _color; // The color to be used by all four vertices

    public int index; // Index of this sprite in its SpriteManager's list
	
	// Indices of the associated vertices in the actual mesh (shortcut to get straight to the right vertices in the vertex array)
	// Also houses indices of UVs in the mesh and color values
	public UIVertexIndices vertexIndices;
    public Vector3 v1 = new Vector3(); // The sprite's vertices in local space
    public Vector3 v2 = new Vector3();
    public Vector3 v3 = new Vector3();
    public Vector3 v4 = new Vector3();

	private UIUVRect _originalUV; // UV coordinates and size for the sprite
	private UIUVRect _displayUV; // final UV coords used for displaying, after possible clipping
	protected bool _clipped; // set to true when the sprite is clipped so the clipped uvFrame is used
	private Rect _localClipRect; // clipping rect in local space (with origin on upper-left corner of sprite)
	private Rect _localVisibleRect;
	
    protected Vector3[] meshVerts; // Pointer to the array of vertices in the mesh
    protected Vector2[] uvs; // Pointer to the array of UVs in the mesh
	protected Dictionary<string, UISpriteAnimation> spriteAnimations;
	
	
	public UISprite( Rect frame, int depth, UIUVRect uvFrame ) : this( frame, depth, uvFrame, false )
	{}
	

    public UISprite( Rect frame, int depth, UIUVRect uvFrame_, bool gameObjectOriginInCenter ) : base()
    {
		this.gameObjectOriginInCenter = gameObjectOriginInCenter;
        if( gameObjectOriginInCenter )
        {
            _anchorInfo.OriginUIxAnchor = UIxAnchor.Center;
            _anchorInfo.OriginUIyAnchor = UIyAnchor.Center;
        }
		
		// Setup our GO
		client.transform.position = new Vector3( frame.x, -frame.y, depth ); // Depth will affect z-index
		
		// Save these for later.  The manager will call initializeSize() when the UV's get setup
		_width = frame.width;
		_height = frame.height;
		
		uvFrame = uvFrame_;
    }
	
	
	public virtual void setSpriteImage( string filename )
	{
		uvFrame = manager.uvRectForFilename( filename );
	}

	
	/// <summary>
	/// subclasses that override need to properly return the clipped frame!
	/// </summary>
	public virtual UIUVRect uvFrame
	{
		get { return _originalUV; }
		set
		{
			// Dont bother changing if the new value isn't different
			if (_originalUV == value)
				return;

			_originalUV = value;
			updateDisplayUV();
		}
	}

	public virtual UIUVRect displayUV { get { return _displayUV; } }
	
	private void updateDisplayUV()
	{
		if (clipped) {
			Rect r = _localVisibleRect;
			if (gameObjectOriginInCenter) {
				r.x += _width / 2;
				r.y += _height / 2;
			}
			_displayUV = _originalUV.Intersect(r, manager.textureSize);
		} else {
			_displayUV = _originalUV;
		}

		if (!_suspendUpdates && manager)
			manager.updateUV(this);
	}

    public virtual bool hidden
    {
        get { return ___hidden; }
        set
        {
            // No need to do anything if we're already in this state:
            if( value == ___hidden )
                return;

            if( value )
                manager.hideSprite( this );
            else
                manager.showSprite( this );
        }
    }
	
	
	public bool clipped { get { return _clipped; } }

	private void updateVisibleRect()
	{
		if (clipped) {
			float newX = Mathf.Max(_localClipRect.xMin, 0f) / scale.x;
			float newY = Mathf.Max(_localClipRect.yMin, 0f) / scale.y;
			float newWidth  = Mathf.Min(_localClipRect.xMax, width) / scale.x - newX;
			float newHeight = Mathf.Min(_localClipRect.yMax, height) / scale.y - newY;
			if (gameObjectOriginInCenter) {
				newX -= _width / 2;
				newY -= _height / 2;
			}
			_localVisibleRect = new Rect(newX, newY, newWidth, newHeight);
		} else {
			if (gameObjectOriginInCenter) {
				_localVisibleRect = new Rect(-_width / 2, -height / 2, _width, _height);
			} else {
				_localVisibleRect = new Rect(0f, 0f, _width, _height);
			}
		}
	}

	private void disableClipping()
	{
		if (!clipped)
			return;

		_clipped = false;
		updateVisibleRect();

		updateVertPositions();
		updateDisplayUV();
	}

	private void enableClipping(Rect localClipRect)
	{
		_clipped = true;

		_localClipRect = localClipRect;
		updateVisibleRect();

		updateVertPositions();
		updateDisplayUV();
	}

	#region Transform passthrough properties so we can update necessary verts when changes occur
	
	public override Vector3 position
	{
		get { return clientTransform.position; }
		set
		{
			base.position = value;
			updateTransform();
		}
	}


	public override Vector3 localPosition
	{
		get { return clientTransform.localPosition; }
		set
		{
			base.localPosition = value;
			updateTransform();
		}
	}
	
	
	public override Vector3 eulerAngles
	{
		get { return clientTransform.eulerAngles; }
		set
		{
			base.eulerAngles = value;
			updateTransform();
		}
	}


    public override Vector3 scale
    {
        get { return base.scale; }
        set
        {
            base.scale = value;
            updateTransform();
        }
    }


	public override Vector3 localScale
	{
		get { return clientTransform.localScale; }
		set
		{
			base.localScale = value;
			updateTransform();
		}
	}
	
	#endregion
	
	
	/// <summary>
	/// Call this when changing multiple properties at once that result in vert, position or uv changes.  Must be
	/// paired with a call to endUpdates!
	/// </summary>
	public void beginUpdates()
	{
		_suspendUpdates = true;
	}
	
	
	/// <summary>
	/// Commits any update made after beginUpdates was called
	/// </summary>
	public void endUpdates()
	{
		_suspendUpdates = false;
		updateDisplayUV();
		updateVertPositions();
		updateTransform();
	}

	public override void clipToRect(Rect r, bool recursive)
	{
		// Transform rect into local sprite space
		Rect lr = r;
		if (gameObjectOriginInCenter) {
			// Apply fixup so that clipping is uniform
			lr.x -= position.x - width / 2;
			lr.y -= -position.y - height / 2;
		} else {
			lr.x -= position.x;
			lr.y -= -position.y;
		}

		// Check if sprite is fully contained inside clip rect
		bool fullyContained =
			lr.x <= 0f &&
			lr.y <= 0f &&
			lr.x + lr.width > width &&
			lr.y + lr.height > height;

		// first, handle if we are fully visible
		if (fullyContained) {
			// unclip if we are clipped
			if (clipped)
				disableClipping();
			hidden = false;
		} else {
			// Check if sprite is fully outside clip rect
			bool fullyOutside =
				lr.x > width ||
				lr.y > height ||
				lr.x + lr.width < 0f ||
				lr.y + lr.height < 0f;

			if (fullyOutside) {
				// fully outside our bounds
				hidden = true;
			} else {
				// wrap the changes in a call to beginUpdates to avoid changing verts more than once
				beginUpdates();

				hidden = false;
				enableClipping(lr);

				// commit the changes
				endUpdates();
			}
		}

		base.clipToRect(r, recursive);
	}
	
	public override void transformChanged()
	{
        base.transformChanged();
		updateTransform();
	}


	// This gets called by the manager just after the UV's get setup
	public void initializeSize()
	{
		setSize( width, height );
		manager.updateUV( this );
	}

	
    /// <summary>
    /// Sets the physical dimensions of the sprite in the XY plane
    /// </summary>
    public void setSize( float width, float height )
    {
        _width = width;
        _height = height;

		updateVisibleRect();

		updateVertPositions();
        updateTransform();
    }
	
	
	/// <summary>
	/// Updates the vert positions using either the normal width/height or the clipped if appropriate
	/// </summary>
	public void updateVertPositions()
	{
		v1 = new Vector3(_localVisibleRect.xMin, -_localVisibleRect.yMin, 0f); // Upper-left
		v2 = new Vector3(_localVisibleRect.xMin, -_localVisibleRect.yMax, 0f); // Lower-left
		v3 = new Vector3(_localVisibleRect.xMax, -_localVisibleRect.yMax, 0f); // Lower-right
		v4 = new Vector3(_localVisibleRect.xMax, -_localVisibleRect.yMin, 0f); // Upper-right
	}
	
    /// <summary>
    /// Sets the vertex and UV buffers this sprite has been assigned
    /// </summary>
    public void setBuffers( Vector3[] v, Vector2[] uv )
    {
        meshVerts = v;
        uvs = uv;
    }
	

    // Applies the transform of the client GameObject and stores the results in the associated vertices of the overall mesh
    public virtual void updateTransform()
    {
		// if we are hidden or suspended, no need to update our positions as that would cause us to be visible again
		if( hidden || _suspendUpdates )
			return;
		
		meshVerts[vertexIndices.mv.one] = clientTransform.TransformPoint( v1 );
		meshVerts[vertexIndices.mv.two] = clientTransform.TransformPoint( v2 );
		meshVerts[vertexIndices.mv.three] = clientTransform.TransformPoint( v3 );
		meshVerts[vertexIndices.mv.four] = clientTransform.TransformPoint( v4 );

        manager.updatePositions();
    }
	
	
	// sets the sprites to have its origin at it's center and repositions it so it doesn't move from
	// a non centered origin
	public virtual void centerize()
	{
		if( gameObjectOriginInCenter )
			return;
		
		// offset our sprite in the x and y direction to "fix" the change that occurs when we reset to center
		var pos = clientTransform.position;
		pos.x += width / 2;
		pos.y -= height / 2;
		clientTransform.position = pos;
		
		gameObjectOriginInCenter = true;
        _anchorInfo.OriginUIxAnchor = UIxAnchor.Center;
        _anchorInfo.OriginUIyAnchor = UIyAnchor.Center;

		updateVisibleRect();
		updateVertPositions();
		updateDisplayUV();
	}
	

    // Sets the specified color and automatically notifies the GUISpriteManager to update the colors
	public override Color color
	{
		get { return _color; }
		set
		{
			_color = value;
			manager.updateColors( this );
		}
	}


	// Removes the sprite from the mesh and destroys it's client GO
	public virtual void destroy()
	{
		manager.removeElement( this );
	}


	#region Sprite Animation methods
	
	// Adds a sprite animation that can later be referenced by name.  Filenames should be the actual names of the files added to Texture Packer.
	public UISpriteAnimation addSpriteAnimation( string name, float frameTime, params string[] filenames )
	{
		// create the spriteAnimations dictionary on demand
		if( spriteAnimations == null )
			spriteAnimations = new Dictionary<string, UISpriteAnimation>();
		
		// get the UIUVRects for the sprite frames
		var uvRects = new List<UIUVRect>( filenames.Length );
		
		foreach( var filename in filenames )
			uvRects.Add( this.manager.uvRectForFilename( filename ) );
		
		var anim = new UISpriteAnimation( frameTime, uvRects );
		spriteAnimations[name] = anim;
		
		return anim;
	}
	

	// Plays the sprite animation referenced by name
	public void playSpriteAnimation( string name, int loopCount )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if( !spriteAnimations.ContainsKey( name ) )
			throw new System.Exception( "can't find sprite animation with name:" + name );
#endif
	
		playSpriteAnimation( spriteAnimations[name], loopCount );
	}
	

	// Plays a sprite animation directly
	public void playSpriteAnimation( UISpriteAnimation anim, int loopCount )
	{
		UI.instance.StartCoroutine( anim.play( this, loopCount ) );
	}
	
	#endregion
	
}

