/*
	Many thanks to the excellent work of Brady Wright from Above and Beyond Software for providing the community with the
	amazing SpriteManager back in the day.  A few bits of his original code are buried somewhere in here.  Be sure to check
	out his amazing products in the Unity Asset Store.
	http://forum.unity3d.com/threads/16763-SpriteManager-draw-lots-of-sprites-in-a-single-draw-call!?highlight=spritemanager
*/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public struct UITextureInfo
{
	public UIUVRect uvRect;
	public Rect frame;
	public Rect spriteSourceSize;
	public Vector2 sourceSize;
	public bool trimmed;
	public bool rotated;
}


public class UISpriteManager : MonoBehaviour
{
	#region ivars

	public enum UISpriteWindowOrder
	{
		CCW,
		CW
	};

	public string texturePackerConfigName;
	public Material material;
	public int layer;

	protected bool meshIsDirty = false; // Flag that gets set if any of the following flags are set.  No updates will happen unless this is true
	protected bool vertsChanged = false;    // Have changes been made to the vertices of the mesh since the last frame?
	protected bool uvsChanged = false;    // Have changes been made to the UVs of the mesh since the last frame?
	protected bool colorsChanged = false;   // Have the colors changed?
	protected bool vertCountChanged = false;// Has the number of vertices changed?
	protected bool updateBounds = false;    // Update the mesh bounds?

	private UISprite[] _sprites = new UISprite[0];    // Array of all sprites (the offset of the vertices corresponding to each sprite should be found simply by taking the sprite's index * 4 (4 verts per sprite).
	private UISpriteWindowOrder winding = UISpriteWindowOrder.CCW; // Which way to wind polygons
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private Mesh _mesh;
	[HideInInspector]
	public Vector2 textureSize = Vector2.zero;

	protected Vector3[] vertices = new Vector3[0];  // The vertices of our mesh
	protected int[] triIndices = new int[0];    // Indices into the vertex array
	protected Vector2[] UVs = new Vector2[0];       // UV coordinates
	protected Color[] colors = new Color[0];      // Color values
	
	[HideInInspector]
	public Dictionary<string, UITextureInfo> textureDetails; // texture details loaded from the TexturePacker config file

	#endregion;


	#region MonoBehaviour Functions

	virtual protected void Awake()
	{
		_meshFilter = gameObject.AddComponent<MeshFilter>();
		_meshRenderer = gameObject.AddComponent<MeshRenderer>();
		_meshRenderer.renderer.material = material;
		_mesh = _meshFilter.mesh;

		// Move the object to the origin so the objects drawn will not be offset from the objects they are intended to represent.
		// Offset on z axis according to the specified layer
		transform.position = new Vector3(0, 0, layer);
		transform.rotation = Quaternion.identity;
	}

	#endregion;


	#region Texture management

	public void loadTextureAndPrepareForUse()
	{
		// Duplicate the standard material so that we're not changing the
		// supplied one - it may be used by more than one UIToolkit object
		Material duplicateMaterial = new Material (material.shader);
		duplicateMaterial.CopyPropertiesFromMaterial(material);
		material = duplicateMaterial;
		
		// load our texture, at 2x if necessary
		if( UI.isHD )
			texturePackerConfigName = texturePackerConfigName + UI.instance.hdExtension;

		var texture = (Texture)Resources.Load( texturePackerConfigName, typeof(Texture) );
		if( texture == null )
			Debug.Log( "UI texture is being autoloaded and it doesn't exist.  Cannot find texturePackerConfigName: " + texturePackerConfigName );
		material.mainTexture = texture;
		// assign the duplicate material to the mesh renderer
		_meshRenderer.renderer.material = material;

		// Cache our texture size
		var t = material.mainTexture;
		textureSize = new Vector2( t.width, t.height );

		// load up the config file
		textureDetails = loadTexturesFromTexturePackerJSON( texturePackerConfigName, textureSize );
	}


	public static Dictionary<string, UITextureInfo> loadTexturesFromTexturePackerJSON( string filename, Vector2 textureSize )
	{
		var textures = new Dictionary<string, UITextureInfo>();

		var asset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if( asset == null )
			Debug.LogError( "Could not find Texture Packer json config file in Resources folder: " + filename );

		var jsonString = asset.text;
		var decodedHash = jsonString.hashtableFromJson();
		var frames = (IDictionary)decodedHash["frames"];

		foreach( System.Collections.DictionaryEntry item in frames )
		{
			// extract the info we need from the TexturePacker json file, mainly uvRect and size
			var frame = (IDictionary)((IDictionary)item.Value)["frame"];
			var frameX = int.Parse(frame["x"].ToString());
			var frameY = int.Parse(frame["y"].ToString());
			var frameW = int.Parse(frame["w"].ToString());
			var frameH = int.Parse(frame["h"].ToString());

			// for trimming support
			var spriteSourceSize = (IDictionary)((IDictionary)item.Value)["spriteSourceSize"];
			var spriteSourceSizeX = int.Parse(spriteSourceSize["x"].ToString());
			var spriteSourceSizeY = int.Parse(spriteSourceSize["y"].ToString());
			var spriteSourceSizeW = int.Parse(spriteSourceSize["w"].ToString());
			var spriteSourceSizeH = int.Parse(spriteSourceSize["h"].ToString());

			var sourceSize = (IDictionary)((IDictionary)item.Value)["sourceSize"];
			var sourceSizeX = int.Parse(sourceSize["w"].ToString());
			var sourceSizeY = int.Parse(sourceSize["h"].ToString());

			var trimmed = (bool)((IDictionary)item.Value)["trimmed"];
			var rotated = (bool)((IDictionary)item.Value)["rotated"];

			var ti = new UITextureInfo();
			ti.frame = new Rect( frameX, frameY, frameW, frameH );
			ti.uvRect = new UIUVRect( frameX, frameY, frameW, frameH, textureSize );
			ti.spriteSourceSize = new Rect( spriteSourceSizeX, spriteSourceSizeY, spriteSourceSizeW, spriteSourceSizeH );
			ti.sourceSize = new Vector2( sourceSizeX, sourceSizeY );
			ti.trimmed = trimmed;
			ti.rotated = rotated;

			textures.Add( item.Key.ToString(), ti );
		}

		// unload the asset
		asset = null;
		Resources.UnloadUnusedAssets();

		return textures;
	}


	/// <summary>
	/// grabs the UITextureInfo for the given filename
	/// </summary>
	public UITextureInfo textureInfoForFilename( string filename )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if( !textureDetails.ContainsKey( filename ) )
			throw new Exception( "can't find texture details for texture packer sprite:" + filename );
#endif
		return textureDetails[filename];
	}


	/// <summary>
	/// grabs the uvRect for the given filename
	/// </summary>
	public UIUVRect uvRectForFilename( string filename )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if (!textureDetails.ContainsKey(filename))
			throw new Exception("can't find texture details for texture packer sprite:" + filename);
#endif
		return textureDetails[filename].uvRect;
	}


	/// <summary>
	/// grabs the frame for the given filename
	/// </summary>
	public Rect frameForFilename( string filename )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if (!textureDetails.ContainsKey(filename))
			throw new Exception("can't find texture details for texture packer sprite:" + filename);
#endif
		return textureDetails[filename].frame;
	}


	#endregion


	#region Vertex and UV array management

	/// <summary>
	/// Enlarges the sprite array by the specified count and also resizes the UV and vertex arrays by the necessary corresponding amount.
	/// Returns the index of the first newly allocated element
	/// </summary>
	protected int expandMaxSpriteLimit(int count)
	{
		int firstNewElement = _sprites.Length;

		// Resize sprite array:
		UISprite[] tempSprites = _sprites;
		_sprites = new UISprite[_sprites.Length + count];
		tempSprites.CopyTo(_sprites, 0);

		// Vertices:
		Vector3[] tempVerts = vertices;
		vertices = new Vector3[vertices.Length + count * 4];
		tempVerts.CopyTo(vertices, 0);

		// UVs:
		Vector2[] tempUVs = UVs;
		UVs = new Vector2[UVs.Length + count * 4];
		tempUVs.CopyTo(UVs, 0);

		// Colors:
		Color[] tempColors = colors;
		colors = new Color[colors.Length + count * 4];
		tempColors.CopyTo(colors, 0);

		// Triangle indices:
		int[] tempTris = triIndices;
		triIndices = new int[triIndices.Length + count * 6];
		tempTris.CopyTo(triIndices, 0);

		// Inform existing sprites of the new vertex and UV buffers:
		for (int i = 0; i < firstNewElement; ++i)
			_sprites[i].setBuffers(vertices, UVs);

		// Setup the newly-added sprites and Add them to the list of available 
		// sprite blocks. Also initialize the triangle indices while we're at it:
		for (int i = firstNewElement; i < _sprites.Length; ++i) {
			// Init triangle indices:
			if (winding == UISpriteWindowOrder.CCW) {   // Counter-clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;  //    0_ 2            0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 1;  //  | /      Verts:  |   /|
				triIndices[i * 6 + 2] = i * 4 + 3;  // 1|/                1|/__|2

				triIndices[i * 6 + 3] = i * 4 + 3;  //      3
				triIndices[i * 6 + 4] = i * 4 + 1;  //   /|
				triIndices[i * 6 + 5] = i * 4 + 2;  // 4/_|5
			}
			else {   // Clockwise winding
				triIndices[i * 6 + 0] = i * 4 + 0;  //    0_ 1            0 ___ 3
				triIndices[i * 6 + 1] = i * 4 + 3;  //  | /      Verts:  |   /|
				triIndices[i * 6 + 2] = i * 4 + 1;  // 2|/                1|/__|2

				triIndices[i * 6 + 3] = i * 4 + 3;  //      3
				triIndices[i * 6 + 4] = i * 4 + 2;  //   /|
				triIndices[i * 6 + 5] = i * 4 + 1;  // 5/_|4
			}
		}

		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		vertCountChanged = true;
		meshIsDirty = true;

		return firstNewElement;
	}


	/// <summary>
	/// Performs any changes if the verts, UVs, colors or bounds changed
	/// </summary>
	protected void updateMeshProperties()
	{
		// Were changes made to the mesh since last time?
		if (vertCountChanged) {
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;

			_mesh.Clear();
			_mesh.vertices = vertices;
			_mesh.uv = UVs;
			_mesh.colors = colors;
			_mesh.triangles = triIndices;
		}
		else {
			if (vertsChanged) {
				vertsChanged = false;
				updateBounds = true;

				_mesh.vertices = vertices;
			}

			if (updateBounds) {
				_mesh.RecalculateBounds();
				updateBounds = false;
			}

			if (colorsChanged) {
				colorsChanged = false;
				_mesh.colors = colors;
			}

			if (uvsChanged) {
				uvsChanged = false;
				_mesh.uv = UVs;
			}
		} // end else
	}

	#endregion


	#region Add/Remove sprite functions

	/// <summary>
	/// Adds a new sprite to the mix at the given position
	/// </summary>
	public UISprite addSprite( string name, int xPos, int yPos )
	{
		return addSprite( name, xPos, yPos, 1, false );
	}


	public UISprite addSprite( string name, int xPos, int yPos, int depth )
	{
		return addSprite( name, xPos, yPos, depth, false );
	}


	public UISprite addSprite( string name, int xPos, int yPos, int depth, bool gameObjectOriginInCenter )
	{
#if UNITY_EDITOR
		// sanity check while in editor
		if( !textureDetails.ContainsKey( name ) )
			throw new Exception( "can't find texture details for texture packer sprite:" + name );
#endif
		var textureInfo = textureDetails[name];
		var positionRect = new Rect( xPos, yPos, textureInfo.frame.width, textureInfo.frame.height );

		return this.addSprite( positionRect, textureInfo.uvRect, depth, gameObjectOriginInCenter );
	}


	/// <summary>
	/// Shortcut for adding a new sprite using the raw materials
	/// </summary>
	private UISprite addSprite( Rect frame, UIUVRect uvFrame, int depth, bool gameObjectOriginInCenter )
	{
		// Create and initialize the new sprite
		UISprite newSprite = new UISprite( frame, depth, uvFrame, gameObjectOriginInCenter );
		addSprite( newSprite );

		return newSprite;
	}


	/// <summary>
	/// Adds a sprite to the manager
	/// </summary>
	public void addSprite(UISprite sprite)
	{
		// Initialize the new sprite and update the UVs		
		int i = 0;

		// Find the first available sprite index
		for(; i < _sprites.Length; i++)
		{
			if (_sprites[i] == null)
				break;
		}

		// did we find a sprite?  if not, expand our arrays
		if (i == _sprites.Length)
			i = expandMaxSpriteLimit(5);

		// Assign and setup the sprite
		_sprites[i] = sprite;

		sprite.index = i;
		sprite.manager = this as UIToolkit;
		sprite.parent = transform;

		sprite.setBuffers(vertices, UVs);

		// Setup indices of the sprites vertices, UV entries and color values
		sprite.vertexIndices.initializeVertsWithIndex(i);
		sprite.initializeSize();
		sprite.color = Color.white;

		// Set our flags to trigger a mesh update
		vertsChanged = true;
		uvsChanged = true;
		meshIsDirty = true;
	}


	protected void removeSprite(UISprite sprite)
	{
		vertices[sprite.vertexIndices.mv.one] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.two] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.three] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.four] = Vector3.zero;

		_sprites[sprite.index] = null;

		// remove any delegate callbacks, etc.
		sprite.parentUIObject = null;

		// This should happen when the sprite dies!!
		Destroy(sprite.client);

		vertsChanged = true;
		meshIsDirty = true;
	}

	#endregion;


	#region Show/Hide sprite functions

	public void hideSprite(UISprite sprite)
	{
		sprite.___hidden = true;

		vertices[sprite.vertexIndices.mv.one] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.two] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.three] = Vector3.zero;
		vertices[sprite.vertexIndices.mv.four] = Vector3.zero;

		vertsChanged = true;
		meshIsDirty = true;
	}


	public void showSprite(UISprite sprite)
	{
		if( !sprite.___hidden )
			return;

		sprite.___hidden = false;

		// Update the vertices.  This will end up caling updatePositions() to set the vertsChanged flag
		sprite.updateTransform();
	}

	#endregion;


	#region Update UV, colors and positions

	// Updates the UVs of the specified sprite and copies the new values into the mesh object.
	public void updateUV(UISprite sprite)
	{
		UVs[sprite.vertexIndices.uv.one] = sprite.uvFrame.lowerLeftUV + Vector2.up * sprite.uvFrame.uvDimensions.y; // Upper-left
		UVs[sprite.vertexIndices.uv.two] = sprite.uvFrame.lowerLeftUV; // Lower-left
		UVs[sprite.vertexIndices.uv.three] = sprite.uvFrame.lowerLeftUV + Vector2.right * sprite.uvFrame.uvDimensions.x; // Lower-right
		UVs[sprite.vertexIndices.uv.four] = sprite.uvFrame.lowerLeftUV + sprite.uvFrame.uvDimensions; // Upper-right

		uvsChanged = true;
		meshIsDirty = true;
	}


	// Updates the color values of the specified sprite and copies the new values into the mesh object.
	public void updateColors(UISprite sprite)
	{
		colors[sprite.vertexIndices.cv.one] = sprite.color;
		colors[sprite.vertexIndices.cv.two] = sprite.color;
		colors[sprite.vertexIndices.cv.three] = sprite.color;
		colors[sprite.vertexIndices.cv.four] = sprite.color;

		colorsChanged = true;
		meshIsDirty = true;
	}


	// Informs the SpriteManager that some vertices have changed position and the mesh needs to be reconstructed accordingly
	public void updatePositions()
	{
		vertsChanged = true;
		meshIsDirty = true;
	}

	#endregion;

}
