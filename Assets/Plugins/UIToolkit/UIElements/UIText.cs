using UnityEngine;
using System.IO;
using System.Collections.Generic;



// addTextInstance returns one of these so we just need to do a .text on the instance to update it
public struct UITextInstance
{
	private UIText _parentText;
	private string _text;
	
	public float xPos;
	public float yPos;
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
			_parentText.updateText( ref this );
		}
	}
	
	
	public UITextInstance( UIText parentText, string text, float xPos, float yPos, float scale, int depth, Color color )
	{
		_parentText = parentText;
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
		
		_parentText.deleteText( textIndex );
		_text = null;
		textIndex = -1;
	}
	
	
	public void setColorForAllLetters( Color color )
	{
		this.color = color;
		_parentText.updateColorForTextInstance( ref this );
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
	
 	private UIFontCharInfo[] _fontDetails;
	private List<UISprite[]> _textSprites = new List<UISprite[]>(); // all the sprites that make up each string we are showing
	private Vector2 _textureOffset;
	private UIToolkit _manager;

	
	public UIText( string fontFilename, string textureFilename ):this( UI.firstToolkit, fontFilename, textureFilename )
	{	
	}
	
	
	public UIText( UIToolkit manager, string fontFilename, string textureFilename )
	{
		_manager = manager;
		_fontDetails = new UIFontCharInfo[256];
		for( int i = 0; i < _fontDetails.Length; i++ )
			_fontDetails[i] = new UIFontCharInfo();
		
		loadConfigfile( fontFilename );
		
		// grab the texture offset from the UI
		var rect = _manager.frameForFilename( textureFilename );
		this._textureOffset = new Vector2( rect.x, rect.y );
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
						_fontDetails[idNum].charID = new int();
						_fontDetails[idNum].charID = idNum;
					}
					else if( string.Equals( word1, "x" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].posX = new int();
						_fontDetails[idNum].posX = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "y" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].posY = new int();
						_fontDetails[idNum].posY = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "width" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].w = new int();
						_fontDetails[idNum].w = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "height" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].h = new int();
						_fontDetails[idNum].h = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].offsetx = new int();
						_fontDetails[idNum].offsetx = System.Int32.Parse(tmp);
					}
					else if( string.Equals( word1, "yoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].offsety = new int();
						_fontDetails[idNum].offsety = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xadvance" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						_fontDetails[idNum].xadvance = new int();
						_fontDetails[idNum].xadvance = System.Int32.Parse( tmp );
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
				fontLineSkip += (int)( _fontDetails[77].h * scale * lineSpacing );
				dx = xPos;
			}
			else
			{
				// calculate the size to center text on Y axis, based on its scale
				offsetY = _fontDetails[charId].offsety * scale;
				dy =  yPos + offsetY + fontLineSkip;
			}

			// add quads for each char
			var uvRect = new UIUVRect( (int)_textureOffset.x + _fontDetails[charId].posX, (int)_textureOffset.y + _fontDetails[charId].posY, _fontDetails[charId].w, _fontDetails[charId].h, _manager.textureSize );
			sprites[i] = new UISprite( new Rect( dx, dy, _fontDetails[charId].w * scale, _fontDetails[charId].h * scale ), depth, uvRect, false );
			_manager.addSprite( sprites[i] );
			sprites[i].color = color;

			// calculate the size to advance, based on its scale
			textWidth = _fontDetails[charId].xadvance * scale;
		
			// advance the position to draw the next letter
			dx += textWidth + _fontDetails[charId].offsetx;
		}
		
		// add all sprites at once to the array, we use this later to delete the strings
		_textSprites.Add( sprites );
		
		return _textSprites.Count - 1;
	}
	
	
	public Vector2 sizeForText( string text )
	{
		return sizeForText( text, 1f );
	}
	
	
	public Vector2 sizeForText( string text, float scale )
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
				fontLineSkip += (int)( _fontDetails[77].h * scale );
				
				// add a small bit of spacing
				fontLineSkip += (int)( _fontDetails[77].h * scale * lineSpacing );
				dx = 0;
			}
			else
			{
				// calculate the size to center text on Y axis, based on its scale
				offsetY = _fontDetails[charId].offsety * scale;
				dy =  0 + offsetY + fontLineSkip;
			}

			// calculate the size to advance, based on its scale
			textWidth = _fontDetails[charId].xadvance * scale;
		
			// advance the position to draw the next letter
			dx += textWidth + _fontDetails[charId].offsetx;
			
			// we want the longest line
			if( dxMax < dx )
				dxMax = dx;
		}
		
		return new Vector2( dxMax > 0 ? dxMax : dx, dy + ( _fontDetails[77].h * scale ) );
	}

	
	// this will create a new UITextInstance and draw the text
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale = 1f, int depth = 1 )
	{
		return this.addTextInstance( text, xPos, yPos, scale, depth, Color.white );
	}

	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale, int depth, Color color )
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
		int length = _textSprites[textInstance.textIndex].Length;

		for( int i = 0; i < length; i++ )
			_textSprites[textInstance.textIndex][i].color = textInstance.color;
	}
	
	
	public void deleteText( int textIndex )
	{
		// bounds checker
		if( textIndex < 0 || textIndex > _textSprites.Count - 1 )
			return;
		
		// how many sprites are we cleaning up?
		int length = _textSprites[textIndex].Length;

		for( int i = 0; i < length; i++ )
		{
			_manager.removeElement( _textSprites[textIndex][i] );
			_textSprites[textIndex][i] = null;
		}
		
		_textSprites[textIndex] = null;
	}


	// empty all the arrays
	public void removeAllText()
	{
		for( var i = _textSprites.Count - 1; i >= 0; i-- )
			deleteText( i );
		
		_textSprites.Clear();
	}

}
