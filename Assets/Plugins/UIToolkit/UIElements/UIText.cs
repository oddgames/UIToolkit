using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UIEaseType = System.Func<float, float>;
using System.Collections;


public enum UITextLineWrapMode
{
	None = 0,
	AlwaysHyphenate,
	MinimumLength
};

public enum UITextAlignMode
{
	Left = 0,
	Center,
	Right
};

public enum UITextVerticalAlignMode
{
	Top = 0,
	Middle,
	Bottom
}




public class UIText : System.Object 
{
	public static int ASCII_NEWLINE = 10;
	public static int ASCII_SPACE = 32;
	public static int ASCII_HYPHEN_MINUS = 45;
	// Break out the magic line height number into a static var.
	public static int ASCII_LINEHEIGHT_REFERENCE = 77;
	
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
	
	/// <summary>
	///  Forces known High-ASCII values (common when pasting text from word) down to low ASCII. Incurs a performance penalty, but might be helpful.
	/// </summary>
	public bool forceLowAscii = false;
	private bool hasLowAsciiQuotes = false;
	
	public float lineSpacing = 1.2f;
	
 	private UIFontCharInfo[] _fontDetails;
	private Vector2 _textureOffset;
	private UIToolkit _manager;
	public UIToolkit manager { get { return _manager; } }
	
	public UITextAlignMode alignMode = UITextAlignMode.Left;
	public UITextVerticalAlignMode verticalAlignMode = UITextVerticalAlignMode.Top;
	public UITextLineWrapMode wrapMode = UITextLineWrapMode.None;
	public float lineWrapWidth = 500.0f;
	

	/// <summary>
	/// Creates a UIText instance which can then be used to create actual text sprites
	/// </summary>
	public UIText( string fontFilename, string textureFilename ) : this( UI.firstToolkit, fontFilename, textureFilename )
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
		
		// Since the font config data adjusts for padding, but TexturePacker trimming removes it,
		// We need to sub out the trimmed amount coming back from the manager.
		var info = _manager.textureInfoForFilename( textureFilename );
		
		this._textureOffset = new Vector2( rect.x - info.spriteSourceSize.x, rect.y - info.spriteSourceSize.y );
	}

	
	/// <summary>
	/// Parse the fnt file with the font definition.  Font files should be in the Resources folder and have a .txt extension.
	/// Do not inluclude the file extension in the filename!
	/// </summary>
	private void loadConfigfile( string filename )
	{
		// should we load a double resolution font?
		if( UI.isHD )
			filename = filename + UI.instance.hdExtension;
	
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
						forceLowAsciiChar( ref tmp );
						idNum = System.Int32.Parse( tmp );
						
						if (idNum == 145 || idNum == 146 || idNum == 147 || idNum == 148)
							hasLowAsciiQuotes = true;
						
						_fontDetails[idNum].charID = new int();
						_fontDetails[idNum].charID = idNum;
					}
					else if( string.Equals( word1, "x" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].posX = new int();
						_fontDetails[idNum].posX = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "y" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].posY = new int();
						_fontDetails[idNum].posY = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "width" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].w = new int();
						_fontDetails[idNum].w = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "height" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].h = new int();
						_fontDetails[idNum].h = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].offsetx = new int();
						_fontDetails[idNum].offsetx = System.Int32.Parse(tmp);
					}
					else if( string.Equals( word1, "yoffset" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].offsety = new int();
						_fontDetails[idNum].offsety = System.Int32.Parse( tmp );
					}
					else if( string.Equals( word1, "xadvance" ) )
					{
						string tmp = wordsSplit[1].Substring( 0, wordsSplit[1].Length );
						forceLowAsciiChar( ref tmp );
						_fontDetails[idNum].xadvance = new int();
						_fontDetails[idNum].xadvance = System.Int32.Parse( tmp );
					}
				} // end foreach
			} // end foreach
		} // end while
	}
	
	
	/// <summary>
	/// Draw text on screen, create each quad and send it to the manager
	/// </summary>
	private void drawText( UITextInstance textInstance, float xPos, float yPos, float scale, int depth, Color[] color, UITextAlignMode instanceAlignMode, UITextVerticalAlignMode instanceVerticalAlignMode )
	{
        // Start by resetting textInstance position
        textInstance.position = Vector3.zero;
        bool hidden = textInstance.hidden;
		float dx = 0;
		float dy = 0;
		float offsetY;
		int fontLineSkip = 0;
		int charId = 0;
		

		// Perform word wrapping ahead of sprite allocation!
		var text = textInstance.text;
		text = wrapText( text, scale );
		
		int lineStartChar = 0;
		int lineEndChar = 0;
		
		float totalHeight = ( _fontDetails[ASCII_LINEHEIGHT_REFERENCE].h * scale * lineSpacing );
		
		for( var i = 0; i < text.Length; i++ )
	    {
	    	charId = System.Convert.ToInt32( text[i] );
			
			if( charId == ASCII_NEWLINE )
			{
				// calculate the size to center text on Y axis, based on its scale
				// 77 is the "M" char usually big enough to get a proper spaced
				// lineskip, use any other char if you want
				fontLineSkip += (int)( _fontDetails[ASCII_LINEHEIGHT_REFERENCE].h * scale * lineSpacing );
				totalHeight += (int)( _fontDetails[ASCII_LINEHEIGHT_REFERENCE].h * scale * lineSpacing );
				
				alignLine( textInstance.textSprites, lineStartChar, lineEndChar, dx, instanceAlignMode );
				
				lineStartChar = i + 1;
				
				dx = 0;
			}
			else
			{
				// calculate the size to center text on Y axis, based on its scale
				offsetY = _fontDetails[charId].offsety * scale;
				dy = offsetY + fontLineSkip;
			}
			
			// Extend end of line
			lineEndChar = i;

			// add quads for each char
			// Use curpos instead of i to compensate for line wrapping hyphenation
			// reuse a UISprite if we have one. if we don't, we need to set it's parent and add it to the textInstance's list
			var currentTextSprite = textInstance.textSpriteAtIndex( i );
			var addingNewTextSprite = currentTextSprite == null;
			
			currentTextSprite = configureSpriteForCharId( currentTextSprite, charId, dx, dy, scale, 0 );
			
			if( addingNewTextSprite )
			{
				currentTextSprite.color = color.Length == 1 ? color[0] : color[i];
				currentTextSprite.parentUIObject = textInstance;
				textInstance.textSprites.Add( currentTextSprite );
			}

            // Ensure the sprite is hidden if the textInstance is
            currentTextSprite.hidden = hidden;
			
			// See below @NOTE re: offsetx vs. xadvance bugfix.
			// advance the position to draw the next letter
			dx += _fontDetails[charId].xadvance * scale;
		}
		
		alignLine( textInstance.textSprites, lineStartChar, lineEndChar, dx, instanceAlignMode );
		verticalAlignText( textInstance.textSprites, totalHeight, _fontDetails[ASCII_LINEHEIGHT_REFERENCE].offsety * scale * lineSpacing, instanceVerticalAlignMode );

        // Re-position textInstance
        textInstance.position = new Vector3(xPos, yPos, depth);
	}
	
	
	/// <summary>
	/// Performs horizontal alignment of each line independently.
	/// </summary>
	private void alignLine( List<UISprite> sprites, int lineStartChar, int lineEndChar, float lineWidth, UITextAlignMode instanceAlignMode ) 
	{
		if( instanceAlignMode == UITextAlignMode.Left )
			return;
		
		
		if( instanceAlignMode == UITextAlignMode.Center )
		{
			// Go from start character to end character, INCLUSIVE.
			for ( var i = lineStartChar; i <= lineEndChar; i++ )
			{
				if( sprites[i] != null )
					sprites[i].position = new Vector3( sprites[i].position.x - lineWidth / 2.0f, sprites[i].position.y, sprites[i].position.z );
			}
		} 
		else if( instanceAlignMode == UITextAlignMode.Right )
		{
			// Go from start character to end character, INCLUSIVE.
			for ( var i = lineStartChar; i <= lineEndChar; i++ )
			{
				if ( i < sprites.Count && sprites[i] != null )
					sprites[i].position = new Vector3( sprites[i].position.x - lineWidth, sprites[i].position.y, sprites[i].position.z );
			}
			
		}
	}
	
	
	/// <summary>
	/// Performs vertical alignment of entire paragraph to the positioning originally provided.
	/// </summary>
	private void verticalAlignText( List<UISprite> sprites, float totalHeight, float charOffset, UITextVerticalAlignMode instanceVerticalAlignMode ) 
	{
		if ( instanceVerticalAlignMode == UITextVerticalAlignMode.Top )
			return;
		
		
		var numSprites = sprites.Count;
		for( int i = 0; i < numSprites; i++ ) 
		{
			if( instanceVerticalAlignMode == UITextVerticalAlignMode.Middle )
				sprites[i].position = new Vector3( sprites[i].position.x, sprites[i].position.y + totalHeight/2 + charOffset, sprites[i].position.z );
			else if( instanceVerticalAlignMode == UITextVerticalAlignMode.Bottom )
				sprites[i].position = new Vector3( sprites[i].position.x, sprites[i].position.y + totalHeight + charOffset, sprites[i].position.z );
		}
	}
	
	
	/// <summary>
	/// Text-wrapping function performs function according to UIText wrapMode setting.
	/// Beware, there are (minor) allocation and performance penalties to word wrapping!
	/// </summary>
	private string wrapText( string text, float scale )
	{
		var newText = string.Empty;
		float dx = 0;
		//float dy = 0;
		int length = 0;
		
		// Use Double-size wrap length in HD mode.
		var scaledWrapWidth = lineWrapWidth * UI.scaleFactor;
		
		switch( wrapMode )
		{
			case UITextLineWrapMode.None:
			{
				// No-op
				newText = text;
				break;
			}
			case UITextLineWrapMode.AlwaysHyphenate:
			{
				length = text.Length;
				for( var i = 0; i < length; i++ ) 
				{
					var charId = System.Convert.ToInt32( text[i] );	
					var charWidth = _fontDetails[charId].xadvance;
				
					if( charId == ASCII_NEWLINE ) 
					{
						newText += "\n";
						dx = 0;	
					}
					else if( dx > scaledWrapWidth ) 
					{
						int prevCharId = ASCII_SPACE;
						if( i > 1 )
						{
							prevCharId = text[i-1];
						}
					
						// Wrap here, unless this character or previous character is a space.
						if( charId == ASCII_SPACE )
						{
							// If this is a space, do a simple line break and skip the space.
							newText += "\n";
						}
						else if( prevCharId == ASCII_SPACE )
						{
							// Add the character, but do not hyphenate line.
							newText += "\n" + text[i];
						}
						else 
						{
							// use ASCII hyphen-minus to wrap.
							newText += "-\n" + text[i];
						}
					
						// New line, break.
						dx = 0;
					} 
					else 
					{
						newText += text[i];	
					}
					dx += charWidth;
				}
				break;
			}
			case UITextLineWrapMode.MinimumLength:
			{
				// Break text into words
				var words = text.Split( new char[]{ ' ' } );
				length = words.Length;
				float spaceWidth = wordWidth( " ", scale );
				float spaceLeft = scaledWrapWidth;
			
				for( var i = 0; i < length; i++ )
				{
					var size = wordWidth( words[i], scale );
					if( size + spaceWidth > spaceLeft ) 
					{
						// Insert line break before word.
						newText +=  "\n" + words[i] + " ";
					
						// Reset space left on line
						spaceLeft = scaledWrapWidth - size;
					} 
					else 
					{
						// Insert word
						newText += words[i] + " ";
						spaceLeft = spaceLeft - ( size + spaceWidth );
					}
					
				}
				break;
			}
		} // end case
		
		return newText;
	}


	/// <summary>
	/// Convenience method to calculate width of a word.
	/// </summary>
	private float wordWidth( string word, float scale )
	{
		// Convert the word into char array.
		var width = 0f;
		foreach( var c in word )
		{
			var charId = System.Convert.ToInt32( c );
			
			width += _fontDetails[charId].xadvance * scale;
		}
		return width;
	}

	
	/// <summary>
	/// Convenience method to configure and optionally instantiate a new UISprite for a font character.
	/// </summary>
	private UISprite configureSpriteForCharId( UISprite sprite, int charId, float xPos, float yPos, float scale, int depth )
	{
		var uvRect = new UIUVRect( (int)_textureOffset.x + _fontDetails[charId].posX, (int)_textureOffset.y + _fontDetails[charId].posY, _fontDetails[charId].w, _fontDetails[charId].h, _manager.textureSize );
		
		// NOTE: This contains a bugfix from the previous version where offsetx was being used
		// in the wrong spot according to the angelcode spec. xadvance is the complete character width
		// and offsetx is supposed to be used only during character rendering, not during cursor advance.
		// Please note that yPos already has offsety built in.
		var rect = new Rect( xPos + _fontDetails[charId].offsetx* scale,
				             yPos, 
				             _fontDetails[charId].w, 
				             _fontDetails[charId].h);
		
		if( sprite == null )
		{
			sprite = new UISprite( rect, depth, uvRect, false );
			_manager.addSprite( sprite );
		}
		else
		{
			sprite.uvFrame = uvRect;
			sprite.position = new Vector3( rect.x, -rect.y, depth );
			sprite.setSize( rect.width, rect.height );
		}

		// We scale the sprite this way so it will work with the container clipping
		sprite.autoRefreshPositionOnScaling = false;
		sprite.scale = new Vector3( scale, scale, 1 );
		
		return sprite;
	}
	

	/// <summary>
	/// Returns the actual size that will be required to display the text
	/// </summary>
	public Vector2 sizeForText( string text )
	{
		return sizeForText( text, 1f );
	}
	
	
	public Vector2 sizeForText( string text, float scale )
	{
		float dx = 0;
		float dxMax = 0;
		float dy = 0;
		
		float offsetY;
		int fontLineSkip = 0;
		int charId = 0;
		
		// Simulate text wrapping
		text = wrapText( text, scale );
		
		// Simulated origin of 0, 0
		
		//float xPos = 0;
		//float yPos = 0;
		
		for( var i = 0; i < text.Length; i++ )
	    {
	    	charId = System.Convert.ToInt32( text[i] );
			
			
			if( charId == ASCII_NEWLINE )
			{
				// calculate the size to center text on Y axis, based on its scale
				// 77 is the "M" char usually big enough to get a proper spaced
				// lineskip, use any other char if you want
				// this looked like a duplicate code bug, so I commented it out.
				//fontLineSkip += (int)( _fontDetails[77].h * scale );
				
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
			
		
			// advance the position to draw the next letter
			dx += _fontDetails[charId].xadvance * scale;
			
			// we want the longest line
			if( dxMax < dx )
				dxMax = dx;
		}
		
		return new Vector2( dxMax > 0 ? dxMax : dx, dy + ( _fontDetails[77].h * scale ) );
	}


	/// <summary>
	/// Creates a new UITextInstance and draws the text at the given position.  The UITextInstance is mutable and can
	/// be changed at any time
	/// </summary>
	public UITextInstance addTextInstance( string text, float xPos, float yPos )
	{
		return addTextInstance( text, xPos, yPos, 1f, 1 );
	}
	
	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale )
	{
		return addTextInstance( text, xPos, yPos, scale, 1 );
	}
	
	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale, int depth )
	{
		return addTextInstance( text, xPos, yPos, scale, depth, Color.white );
	}

	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale, int depth, Color color )
	{
		return addTextInstance( text, xPos, yPos, scale, depth, color, this.alignMode, this.verticalAlignMode );
	}
	
	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale, int depth, Color color, UITextAlignMode alignMode, UITextVerticalAlignMode verticalAlignMode )
	{
		return addTextInstance( text, xPos, yPos, scale, depth, new Color[] { color }, alignMode, verticalAlignMode );
	}
	
	
	public UITextInstance addTextInstance( string text, float xPos, float yPos, float scale, int depth, Color[] colors, UITextAlignMode alignMode, UITextVerticalAlignMode verticalAlignMode )
	{
		if( forceLowAscii )
			forceLowAsciiString( ref text );
		
		var textInstance = new UITextInstance( this, text, xPos, yPos, scale, depth, colors, alignMode, verticalAlignMode );
        textInstance.parent = _manager.transform;
        drawText(textInstance, textInstance.xPos, textInstance.yPos, textInstance.textScale, textInstance.depth, colors, textInstance.alignMode, textInstance.verticalAlignMode);
		
		return textInstance;
	}

	
	public void updateText( UITextInstance textInstance )
	{
		// kill the current text then draw some new text
		drawText( textInstance, textInstance.xPos, textInstance.yPos, textInstance.textScale, textInstance.depth, textInstance.colors, textInstance.alignMode, textInstance.verticalAlignMode );
	}
	
	
	void forceLowAsciiChar( ref string character ) 
	{
		// Perform character conversions.
		// NOTE: These will use the low-ASCII addresses for quotes and dashes to be safe.
		if ( character == "8211" ) character = "150";
		else if ( character == "8212" ) character = "151";
		else if ( character == "8216" ) character = "145";
		else if ( character == "8217" ) character = "146";
		else if ( character == "8220" ) character = "147";
		else if ( character == "8221" ) character = "148";
		
	}
	
	
	void forceLowAsciiString( ref string text ) 
	{
		text = text.Replace( char.ConvertFromUtf32( 8211 ), char.ConvertFromUtf32( 150 ) ); // Hyphen or En-Dash
		text = text.Replace( char.ConvertFromUtf32( 8212 ), char.ConvertFromUtf32( 151 ) ); // Em-Dash
		// NOTE: Convert to ASCII 0x27 for straight single quotes, but might have access to low values.
		if ( hasLowAsciiQuotes ) 
		{
			text = text.Replace( char.ConvertFromUtf32( 8216 ), char.ConvertFromUtf32( 145 ) ); // Left Single Quotation Mark
			text = text.Replace( char.ConvertFromUtf32( 8217 ), char.ConvertFromUtf32( 146 ) ); // Right Single Quotation Mark
		}
		else
		{
			text = text.Replace( char.ConvertFromUtf32( 8216 ), char.ConvertFromUtf32( 39 ) ); // Move to Straight Quotation Mark
			text = text.Replace( char.ConvertFromUtf32( 8217 ), char.ConvertFromUtf32( 39 ) ); // Move to Straight Quotation Mark
		}
		// NOTE: Convert to ASCII 0x22 for straight double quotes
		if ( hasLowAsciiQuotes ) 
		{
			text = text.Replace( char.ConvertFromUtf32( 8220 ), char.ConvertFromUtf32( 147 ) ); // Left Double Quotation Mark
			text = text.Replace( char.ConvertFromUtf32( 8221 ), char.ConvertFromUtf32( 148 ) ); // Right Double Quotation Mark
		}
		else
		{
			text = text.Replace( char.ConvertFromUtf32( 8220 ), char.ConvertFromUtf32( 34 ) ); // Left Double Quotation Mark
			text = text.Replace( char.ConvertFromUtf32( 8221 ), char.ConvertFromUtf32( 34 ) ); // Right Double Quotation Mark
		}
	}

}
