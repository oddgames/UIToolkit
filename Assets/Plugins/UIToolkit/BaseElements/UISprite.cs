using UnityEngine;
using System.Collections.Generic;


public class UISprite : UIObject
{
    public UISpriteManager manager = null; // Reference to the sprite manager in which this sprite resides
    public bool ___hidden = false; // Indicates whether this sprite is currently hidden (DO NOT ACCESS DIRECTLY)

    public float width;  // Width and Height of the sprite in worldspace units. DO NOT SET THESE
    public float height; // THESE ARE PUBLIC TO AVOID THE GETTER PROPERTY OVERHEAD
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
	
    protected Vector3[] meshVerts; // Pointer to the array of vertices in the mesh
    protected Vector2[] UVs; // Pointer to the array of UVs in the mesh
	protected Dictionary<string, UISpriteAnimation> spriteAnimations;
	
	
	public UISprite( Rect frame, int depth, UIUVRect uvFrame ):this( frame, depth, uvFrame, false )
	{
		
	}
	

    public UISprite( Rect frame, int depth, UIUVRect uvFrame, bool gameObjectOriginInCenter ):base()
    {
		this.gameObjectOriginInCenter = gameObjectOriginInCenter;
		
		// Setup our GO
		client.transform.position = new Vector3( frame.x, -frame.y, depth ); // Depth will affect z-index
		
		// Save these for later.  The manager will call initializeSize() when the UV's get setup
		width = frame.width;
		height = frame.height;
		
		_uvFrame = uvFrame;
    }


	public virtual UIUVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			// Dont bother changing if the new value isn't different
			if( _uvFrame != value )
			{
				_uvFrame = value;
				manager.updateUV( this );
			}
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
	
	
	public override void transformChanged()
	{
		updateTransform();
	}

	// This gets called by the manager just after the UV's get setup
	public void initializeSize()
	{
		setSize( width, height );
		manager.updateUV( this );
	}

	
    // Sets the physical dimensions of the sprite in the XY plane
    public void setSize( float width, float height )
    {
        this.width = width;
        this.height = height;
		
		if( gameObjectOriginInCenter )
		{
			// Some objects need to rotate so we set the origin at the center of the GO
			Vector3 offset = Vector3.zero;
			v1 = offset + new Vector3( -width / 2, height / 2, 0 );   // Upper-left
			v2 = offset + new Vector3( -width / 2, -height / 2, 0 );  // Lower-left
			v3 = offset + new Vector3( width / 2, -height / 2, 0 );   // Lower-right
			v4 = offset + new Vector3( width / 2, height / 2, 0 );    // Upper-right
		}
		else
		{
			// Make the origin the top-left corner of the GO
	        v1 = new Vector3( 0, 0, 0 );   // Upper-left
	        v2 = new Vector3( 0, -height, 0 );  // Lower-left
	        v3 = new Vector3( width, -height, 0 );   // Lower-right
	        v4 = new Vector3( width, 0, 0 );    // Upper-right
		}
		
        updateTransform();
    }
	

    // Sets the vertex and UV buffers
    public void setBuffers( Vector3[] v, Vector2[] uv )
    {
        meshVerts = v;
        UVs = uv;
    }
	

    // Applies the transform of the client GameObject and stores the results in the associated vertices of the overall mesh
    public virtual void updateTransform()
    {
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
		setSize( width, height );
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
	
	
	#region Sprite Animation methods
	
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
	
	
	public void playSpriteAnimation( string name, int loopCount )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if( !spriteAnimations.ContainsKey( name ) )
			throw new System.Exception( "can't find sprite animation with name:" + name );
#endif
	
		playSpriteAnimation( spriteAnimations[name], loopCount );
	}
	
	
	public void playSpriteAnimation( UISpriteAnimation anim, int loopCount )
	{
		UI.instance.StartCoroutine( anim.play( this, loopCount ) );
	}
	
	#endregion
	
}

