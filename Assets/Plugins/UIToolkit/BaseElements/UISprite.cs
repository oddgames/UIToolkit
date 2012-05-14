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
	private float _clippedWidth;
	private float _clippedHeight;
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

	protected UIUVRect _uvFrame; // UV coordinates and size for the sprite
	protected UIUVRect _uvFrameClipped; // alternate UV coordinates for when a sprite it clipped
	protected bool _clipped; // set to true when the sprite is clipped so the clipped uvFrame is used
	private float _clippedTopYOffset;
	private float _clippedLeftXOffset;
	
    protected Vector3[] meshVerts; // Pointer to the array of vertices in the mesh
    protected Vector2[] uvs; // Pointer to the array of UVs in the mesh
	protected Dictionary<string, UISpriteAnimation> spriteAnimations;
	
	
	public UISprite(){}
	
	
	public UISprite( Rect frame, int depth, UIUVRect uvFrame ) : this( frame, depth, uvFrame, false )
	{}
	

    public UISprite( Rect frame, int depth, UIUVRect uvFrame, bool gameObjectOriginInCenter ) : base()
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
		
		_uvFrame = uvFrame;
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
		get { return _clipped ? _uvFrameClipped : _uvFrame; }
		set
		{
			// Dont bother changing if the new value isn't different
			if( _uvFrame != value )
			{
				_uvFrame = value;
				
				// if we are clipped, we need to provide a new, clipped frame here as well as set the _uvFrame
				if( _clipped )
				{
					var clippedWidth = _uvFrameClipped.getWidth( manager.textureSize );
					var clippedHeight = _uvFrameClipped.getHeight( manager.textureSize );
					
					_uvFrameClipped = value.rectClippedToBounds( clippedWidth, clippedHeight, _uvFrameClipped.clippingPlane, manager.textureSize );
				}
				
				manager.updateUV( this );
			}
		}
	}
	
	
	/// <summary>
	/// Alternate uvFrame for when the sprite is clipped. Set to UIUVRect.zero to remove clipping or set
	/// clipped to false.
	/// Note: setting this automatically sets the sprite as clipped
	/// </summary>
	public virtual UIUVRect uvFrameClipped
	{
		get { return _uvFrameClipped; }
		set
		{
			_uvFrameClipped = value;
				
			// if we were not set to zero then we use the clipped frame
			_clipped = value != UIUVRect.zero;
			manager.updateUV( this );
		}
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
	
	
	public bool clipped
	{
		get { return _clipped; }
		set
		{
			if( value == _clipped )
				return;
			
			_clipped = value;
			_clippedTopYOffset = _clippedLeftXOffset = 0;
			
			updateVertPositions();
			manager.updateUV( this );
		}
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
		updateVertPositions();
		updateTransform();
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
		
		updateVertPositions();
        updateTransform();
    }
	
	
	/// <summary>
	/// Updates the vert positions using either the normal width/height or the clipped if appropriate
	/// </summary>
	public void updateVertPositions()
	{
		var width = _clipped ? _clippedWidth : _width;
		var height = _clipped ? _clippedHeight : _height;
		
		if( gameObjectOriginInCenter )
		{
			// Some objects need to rotate so we set the origin at the center of the GO
			v1 = new Vector3( -width / 2 + _clippedLeftXOffset, height / 2 - _clippedTopYOffset, 0 );   // Upper-left
			v2 = new Vector3( -width / 2 + _clippedLeftXOffset, -height / 2 - _clippedTopYOffset, 0 );  // Lower-left
			v3 = new Vector3( width / 2 + _clippedLeftXOffset, -height / 2 - _clippedTopYOffset, 0 );   // Lower-right
			v4 = new Vector3( width / 2 + _clippedLeftXOffset, height / 2 - _clippedTopYOffset, 0 );    // Upper-right
		}
		else
		{
			// Make the origin the top-left corner of the GO
			v1 = new Vector3( _clippedLeftXOffset, -_clippedTopYOffset, 0 );   // Upper-left
			v2 = new Vector3( _clippedLeftXOffset, -height - _clippedTopYOffset, 0 );  // Lower-left
			v3 = new Vector3( width + _clippedLeftXOffset, -height - _clippedTopYOffset, 0 );   // Lower-right
			v4 = new Vector3( width + _clippedLeftXOffset, -_clippedTopYOffset, 0 );    // Upper-right
		}
	}
	
	
	/// <summary>
    /// Sets the physical dimensions of the sprite in the XY plane to be used when clipped. We need separate sizes because
    /// the sprite should still have the same height/width for measuring even though it is clipped by another view.
    /// Note: setting this DOES NOT automatically set the sprite as clipped. Size should be set after uvFrameClipped!
    /// </summary>
    public void setClippedSize( float width, float height, UIClippingPlane clippingPlane )
    {
		_clippedWidth = width;
		_clippedHeight = height;
		
		switch( clippingPlane )
		{
			case UIClippingPlane.Left:
			{
				_clippedLeftXOffset = _width - _clippedWidth;
				break;
			}
			case UIClippingPlane.Right:
			{
				_clippedLeftXOffset = 0;
				break;
			}
			case UIClippingPlane.Top:
			{
				_clippedTopYOffset = _height - _clippedHeight;
				break;
			}
			case UIClippingPlane.Bottom:
			{
				_clippedTopYOffset = 0;
				break;
			}
		}

		updateVertPositions();
		updateTransform();
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
		pos.x += _width / 2;
		pos.y -= _height / 2;
		clientTransform.position = pos;
		
		gameObjectOriginInCenter = true;
        _anchorInfo.OriginUIxAnchor = UIxAnchor.Center;
        _anchorInfo.OriginUIyAnchor = UIyAnchor.Center;

		setSize( _width, _height );
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

