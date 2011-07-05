using UnityEngine;


public class UITexture : UISprite
{
	private UIUVRect _normalUVframe;

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
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
	
		return new UITexture( manager, frame, depth, normalTI.uvRect);
	}

	public UITexture( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame):base( frame, depth, uvFrame )
	{
		_normalUVframe = uvFrame;
		
		manager.addSprite( this );
	}

	#endregion;

}