using UnityEngine;


public class UITexture : UISprite
{
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with

	#region Constructors/Destructor
	
	public static UITexture create( string filename,  int xPos, int yPos )
	{
		return UITexture.create( UI.firstToolkit, filename, xPos, yPos );
	}
		
	public static UITexture create( UIToolkit manager, string filename, int xPos, int yPos )
	{
		return UITexture.create( manager, filename, xPos, yPos, 1 );
	}
	
	public static UITexture create( UIToolkit manager, string filename, int xPos, int yPos, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// create the button
		return new UITexture( manager, frame, depth, normalTI.uvRect);
	}

	public UITexture( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame):base( frame, depth, uvFrame )
	{
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		manager.addSprite( this );
	}

	#endregion;

}