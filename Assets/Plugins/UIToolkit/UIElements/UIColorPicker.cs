using UnityEngine;


public class UIColorPicker : UITouchableSprite
{
	public delegate void UIColorPickerChanged( UIColorPicker sender, Color newColor, Color oldColor );
	public event UIColorPickerChanged onColorChangeBegan;
	public event UIColorPickerChanged onColorChange;
	
	private Color _colorPicked = Color.white;
	private Vector2 textureCoords;
			
	#region Constructors/Destructor
	
	public static UIColorPicker create( string filename, int xPos, int yPos, int depth )
	{
		return UIColorPicker.create( UI.firstToolkit, filename, xPos, yPos, depth );
	}	


	public static UIColorPicker create( UIToolkit manager, string filename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		Vector2 textureCoords = new Vector2(normalTI.frame.x, normalTI.frame.y);
		
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// create the button
		return new UIColorPicker( manager, frame, depth, normalTI.uvRect, textureCoords );
	}


	public UIColorPicker( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, Vector2 textureCoords ):base( frame, depth, uvFrame )
	{
		//We store the coordinates of the top left of the subtexture
		this.textureCoords = textureCoords;
		manager.addTouchableSprite( this );
	}

	#endregion;
	
	public Color colorPicked
	{
		get { return _colorPicked; }
		set
		{
			if( value != _colorPicked )
			{
				_colorPicked = value;
			}
		}
	}


	// UITouchWrapper handlers
	public override void onTouchBegan( UITouchWrapper touch, Vector2 touchPos )
	{
		highlighted = true;
		
		Color oldColor = colorPicked;
		Vector2 textureCoord = getTouchTextureCoords( touchPos );
		colorPicked = getColorForPixel( (int)textureCoord.x, (int)textureCoord.y );
		
		if( onColorChangeBegan != null )
			onColorChangeBegan( this, colorPicked, oldColor );
	}


	public override void onTouchMoved( UITouchWrapper touch, Vector2 touchPos )
	{
		Color oldColor = colorPicked;
		Vector2 textureCoord = getTouchTextureCoords(touchPos);	
		colorPicked = getColorForPixel( (int)textureCoord.x, (int)textureCoord.y );
		
		if( onColorChange != null )
			onColorChange( this, colorPicked, oldColor );
	}


	// Given a touch position, this method determines the relative position on  the actual texture in the texture atlas.
	// Values are clamped to ensure the chosenColor is valid for a UIColorChooser with UIEdgeOffsets.
	// The y-axis is flipped because the texture coordinate system is inverted.
	private Vector2 getTouchTextureCoords( Vector2 touchPos )
	{
		float xChange = touchPos.x - position.x;
		xChange = Mathf.Clamp( xChange + (0.5f * width), 0, width - 1 );
		float xPos = textureCoords.x + xChange;
				
		float yChange = touchPos.y - ( -1 * position.y );
		yChange = Mathf.Clamp( yChange + (0.5f * height), 1, height );
		float yPos = manager.textureSize.y - ( textureCoords.y + yChange );
		
		return new Vector2( xPos, yPos );
	}


	public Color getColorForPixel( int xPos, int yPos )
	{
		var mainTex = manager.material.mainTexture as Texture2D;
		return mainTex.GetPixel( xPos, yPos );
	}


}