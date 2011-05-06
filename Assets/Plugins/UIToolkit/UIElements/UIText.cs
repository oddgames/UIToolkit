using UnityEngine;
using System.IO;
using System.Collections.Generic;



// addTextInstance returns one of these so we just need to do a .text on the instance to update it
public struct UITextInstance
{
	private UIText parentText;
	private string _text;
	
	public int xPos;
	public int yPos;
	public float scale;
	public int depth;
	public int textIndex;
	public Color color;

	
	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			parentText.updateText( ref this );
		}
	}
	
	
	public UITextInstance( UIText parentText, string text, int xPos, int yPos, float scale, int depth, Color color )
	{
		this.parentText = parentText;
		_text = text;
		this.xPos = xPos;
		this.yPos = yPos;
		this.scale = scale;
		this.depth = depth;
		this.textIndex = -1;
		this.color = color;
	}
	
	
	public void clear()
	{
		if( textIndex < 0 )
			return;
		
		parentText.deleteText( textIndex );
		_text = null;
		textIndex = -1;
	}
	
	
	public void setColorForAllLetters( Color color )
	{
		this.color = color;
		parentText.updateColorForTextInstance( ref this );
	}

}


public class UIText : System.Object 
{
	private struct UIFontCharInfo
	{	
		public int charID;
		public int posX;
		public int posY;
		public int w;
		public int h;
		public int offsetx;
		public int offsety;
		public int xadvance;
	}

	
	public float lineSpacing = 1.2f;
	
 	private UIFontCharInfo[] arrayFonts;
	private List<UISprite[]> textSprites = new List<UISprite[]>(); // all the sprites that make up each string we are showing
	private Vector2 textureOffset;


	public UIText( string fontFilename, string textureFilename )
	{
		arrayFonts = new UIFontCharInfo[256];
		for( int i = 0; i < arrayFonts.Length; i++ )
			arrayFonts[i] = new UIFontCharInfo();
		
		loadConfigfile( fontFilename );
		
		// grab the texture offset from the UI
		var rect = UI.instance.frameForFilename( textureFilename );
		this.textureOffset = new Vector2( rect.x, rect.y );
	}

	
	/// <summary>
	/// Parse the fnt file with the font definition.  Font files should be in the Resources folder and have a .txt extension.
	/// Do not inluclude the file extension in the filename!
	/// </summary>
	private void loadConfigfile( string filename )
	{
		// should we load a double resolution font?
		if( UI.instance.isHD )
			filename = filename + "2x";
	
		var asset = Resources.Load( filename, typeof( TextAsset ) ) as TextAsset;
		if( asset == null )
			Debug.LogError( "Could not find font config file in Resources folder: " + filename );
	
		int idNum = 0;
		
		foreach( var input in asset.text.Split( new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries ) )
		{
			//first split line into "space" chars
       		string[] words = input.Split(' ');
			foreach( string word in words )
        	{
				//then split line into "=" sign to get the values for each component
				string[] wordsSplit = word.Split( '=' );
				foreach( string word1 in wordsSplit )
       	 		{
					if( string.Equals( word1, "id" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						idNum = System.Int32.Parse( tmp );
						arrayFonts[idNum].charID = new int();
						arrayFonts[idNum].charID = idNum;
					}
					else if( string.Equals( word1, "x" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].posX = new int();
						arrayFonts[idNum].posX = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "y" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].posY = new int();
						arrayFonts[idNum].posY = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "width" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].w = new int();
						arrayFonts[idNum].w = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "height" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].h = new int();
						arrayFonts[idNum].h = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].offsetx = new int();
						arrayFonts[idNum].offsetx = System.Int32.Parse(tmp);
					}
					else if( string.Equals( word1, "yoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].offsety = new int();
						arrayFonts[idNum].offsety = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xadvance" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						arrayFonts[idNum].xadvance = new int();
						arrayFonts[idNum].xadvance = System.Int32.Parse( tmp );
					}
				} // end foreach
			} // end foreach
		} // end while
	}
	
	
	// draw text on screen, create each quad and send it to the manager
	private int drawText( string text, float xPos, float yPos, float scale, int depth, Color color )
	{		
		float dx = xPos;
		float dy = 0;
		float textWidth;
		float offsetY;
		
		int fontLineSkip = 0;
			
		int charId = 0;
		
		UISprite[] sprites = null;
		
		int length = text.Length;
		sprites = new UISprite[length];
		
		
		for( var i = 0; i < text.Length; i++ )
	    {
	    	charId = System.Convert.ToInt32( text[i] );
			
			// "10" is the new line char
			if( charId == 10 )
			{
				// calculate the size to center text on Y axis, based on its scale
				// 77 is the "M" char usually big enough to get a proper spaced
				// lineskip, use any other char if you want
				fontLineSkip += (int)( arrayFonts[77].h * scale * lineSpacing );
				dx = xPos;
			}
			else
			{
				// calculate the size to center text on Y axis, based on its scale
				offsetY = arrayFonts[charId].offsety * scale;
				dy =  yPos + offsetY + fontLineSkip;
			}

			// add quads for each char
			var uvRect = new UIUVRect( (int)textureOffset.x + arrayFonts[charId].posX, (int)textureOffset.y + arrayFonts[charId].posY, arrayFonts[charId].w, arrayFonts[charId].h );
			sprites[i] = new UISprite( new Rect( dx, dy, arrayFonts[charId].w * scale, arrayFonts[charId].h * scale ), depth, uvRect, false );
			UI.instance.addSprite( sprites[i] );
			sprites[i].color = color;

			// calculate the size to advance, based on its scale
			textWidth = arrayFonts[charId].xadvance * scale;
		
			// advance the position to draw the next letter
			dx += textWidth + arrayFonts[charId].offsetx;
		}
		
		// add all sprites at once to the array, we use this later to delete the strings
		textSprites.Add( sprites );
		
		return textSprites.Count - 1;
	}
	
	
	public UIVector2 sizeForText( string text, float scale = 1.0f )
	{
		float dx = 0;
		float dxMax = 0;
		float dy = 0;
		float textWidth;
		float offsetY;
		int fontLineSkip = 0;
		int charId = 0;
		
		
		for( var i = 0; i < text.Length; i++ )
	    {
	    	charId = System.Convert.ToInt32( text[i] );
			
			// "10" is the new line char
			if( charId == 10 )
			{
				// calculate the size to center text on Y axis, based on its scale
				// 77 is the "M" char usually big enough to get a proper spaced
				// lineskip, use any other char if you want
				fontLineSkip += (int)( arrayFonts[77].h * scale );
				
				// add a small bit of spacing
				fontLineSkip += (int)( arrayFonts[77].h * scale * lineSpacing );
				dx = 0;
			}
			else
			{
				// calculate the size to center text on Y axis, based on its scale
				offsetY = arrayFonts[charId].offsety * scale;
				dy =  0 + offsetY + fontLineSkip;
			}

			// calculate the size to advance, based on its scale
			textWidth = arrayFonts[charId].xadvance * scale;
		
			// advance the position to draw the next letter
			dx += textWidth + arrayFonts[charId].offsetx;
			
			// we want the longest line
			if( dxMax < dx )
				dxMax = dx;
		}
		
		return new UIVector2( (int)dxMax > 0 ? (int)dxMax : (int)dx, (int)( dy + ( arrayFonts[77].h * scale ) ) );
	}

	
	// this will create a new UITextInstance and draw the text
	public UITextInstance addTextInstance( string text, int xPos, int yPos, float scale = 1f, int depth = 1 )
	{
		return this.addTextInstance( text, xPos, yPos, scale, depth, Color.white );
	}

	
	public UITextInstance addTextInstance( string text, int xPos, int yPos, float scale, int depth, Color color )
	{
		var textInstance = new UITextInstance( this, text, xPos, yPos, scale, depth, color );
		textInstance.textIndex = drawText( text, xPos, yPos, scale, depth, color );
		
		return textInstance;
	}

	
	public void updateText( ref UITextInstance textInstance )
	{
		// kill the current text then draw some new text
		deleteText( textInstance.textIndex );
		textInstance.textIndex = drawText( textInstance.text, textInstance.xPos, textInstance.yPos, textInstance.scale, textInstance.depth, textInstance.color );
	}
	
	
	public void updateColorForTextInstance( ref UITextInstance textInstance )
	{
		// how many sprites are we updated?
		int length = textSprites[textInstance.textIndex].Length;

		for( int i = 0; i < length; i++ )
			textSprites[textInstance.textIndex][i].color = textInstance.color;
	}
	
	
	public void deleteText( int textIndex )
	{
		// bounds checker
		if( textIndex < 0 || textIndex > textSprites.Count - 1 )
			return;
		
		// how many sprites are we cleaning up?
		int length = textSprites[textIndex].Length;

		for( int i = 0; i < length; i++ )
			UI.instance.removeElement( textSprites[textIndex][i] );
		
		textSprites[textIndex] = null;
	}


	// empty all the arrays
	public void removeAllText()
	{
		for( var i = textSprites.Count - 1; i >= 0; i-- )
			deleteText( i );
		
		textSprites.Clear();
	}

}
