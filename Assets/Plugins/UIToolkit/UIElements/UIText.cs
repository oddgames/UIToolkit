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
	

	/// <summary>
	/// Sets and draws the text string displayed on screen
	/// </summary>
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
	

	/// <summary>
	/// Clears the text from the screen
	/// </summary>
	public void clear()
	{
		if( textIndex < 0 )
			return;
		
		_parentText.deleteText( textIndex );
		_text = null;
		textIndex = -1;
	}
	

	/// <summary>
	/// Sets the color for the text.  All colors will be changed.
	/// </summary>
	public void setColorForAllLetters( Color color )
	{
		this.color = color;
		_parentText.updateColorForTextInstance( ref this );
	}
	

	/// <summary>
	/// Moves the text from it's current position to a new position that is currentPosition + position
	/// </summary>
	public IEnumerator positionBy( float duration, Vector3 position, UIEaseType ease )
	{
		var textSprites = _parentText.textSprites[textIndex];
		
		// variables to handle the state of the animations
		var running = true;
		var startTime = Time.time;
		var startPositions = new Vector3[textSprites.Length];
		var targetPositions = new Vector3[textSprites.Length];
		
		// save off the start positions and calculate the targets for each sprite
		for( var i = 0; i < textSprites.Length; i++ )
		{
			startPositions[i] = textSprites[i].localPosition;
			targetPositions[i] = textSprites[i].localPosition + position;
		}
		
		while( running )
		{				
			// Get our easing position
			float easPos = Mathf.Clamp01( ( Time.time - startTime ) / duration );
			easPos = ease( easPos );
			
			// do the actual movement
			for( var i = 0; i < textSprites.Length; i++ )
				textSprites[i].localPosition = Vector3.Lerp( startPositions[i], targetPositions[i], easPos );
			
			// See if we are done with our animation yet
			if( ( startTime + duration ) <= Time.time )
				running = false;
			else
				yield return null;
		} // end while
	}

}


public class UIText : System.Object 
{
	
	public static int ASCII_NEWLINE = 10;
	public static int ASCII_SPACE = 32;
	public static int ASCII_HYPHEN_MINUS = 45;
	
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
	private Vector2 _textureOffset;
	private UIToolkit _manager;
	
	public UITextLineWrapMode wrapMode = UITextLineWrapMode.None;
	public float lineWrapWidth = 500.0f;
	
	private List<UISprite[]> _textSprites = new List<UISprite[]>(); // all the sprites that make up each string we are showing
	public List<UISprite[]> textSprites
	{
		get { return _textSprites; }
	}
	

	/// <summary>
	/// Creates a UIText instance which can then be used to create actual text sprites
	/// </summary>
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
		
		// Since the font config data adjusts for padding, but TexturePacker trimming removes it,
		// We need to sub out the trimmed amount coming back from the manager.
		var info = _manager.textureInfoForFilename( textureFilename );
		
		this._textureOffset = new Vector2( rect.x - info.sourceSize.x, rect.y - info.sourceSize.y );
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
	
	
	/// <summary>
	/// Draw text on screen, create each quad and send it to the manager
	/// </summary>
	private int drawText( string text, float xPos, float yPos, float scale, int depth, Color color )
	{		
		float dx = xPos;
		float dy = 0;
		
		float offsetY;
		
		int fontLineSkip = 0;
			
		int charId = 0;
		
		UISprite[] sprites = null;
		
		// Perform word wrapping ahead of sprite allocation!
		text = wrapText(text, scale);
		
		
		int length = text.Length;
		sprites = new UISprite[length];
		
		
		for( var i = 0; i < text.Length; i++ )
	    {
	    	charId = System.Convert.ToInt32( text[i] );
			
			
			if( charId == ASCII_NEWLINE )
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
			// Use curpos instead of i to compensate for line wrapping hyphenation
			sprites[i] = spriteForCharId( charId, dx, dy, scale, depth );
			_manager.addSprite( sprites[i] );
			sprites[i].color = color;
			
			// See below @NOTE re: offsetx vs. xadvance bugfix.
			// advance the position to draw the next letter
			dx += _fontDetails[charId].xadvance * scale;
			
		}
		
		// add all sprites at once to the array, we use this later to delete the strings
		_textSprites.Add( sprites );
		
		return _textSprites.Count - 1;
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
					else if( dx > lineWrapWidth ) 
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
				float spaceLeft = lineWrapWidth;
			
				for( var i = 0; i < length; i++ )
				{
					var size = wordWidth( words[i], scale );
					if( size + spaceWidth > spaceLeft ) 
					{
						// Insert line break before word.
						newText +=  "\n" + words[i] + " ";
					
						// Reset space left on line
						spaceLeft = lineWrapWidth - size;
					} 
					else 
					{
						// Insert word
						newText += words[i] + " ";
						spaceLeft = spaceLeft - (size + spaceWidth);
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
	/// Convenience method to instantiate a new UISprite for a font character.
	/// </summary>
	private UISprite spriteForCharId( int charId, float xPos, float yPos, float scale, int depth )
	{
		var uvRect = new UIUVRect( (int)_textureOffset.x + _fontDetails[charId].posX, (int)_textureOffset.y + _fontDetails[charId].posY, _fontDetails[charId].w, _fontDetails[charId].h, _manager.textureSize );
		
		// NOTE: This contains a bugfix from the previous version where offsetx was being used
		// in the wrong spot according to the angelcode spec. xadvance is the complete character width
		// and offsetx is supposed to be used only during character rendering, not during cursor advance.
		// Please note that yPos already has offsety built in.
			return new UISprite( new Rect( xPos + _fontDetails[charId].offsetx * scale,
				                              yPos, 
				                              _fontDetails[charId].w * scale, 
				                              _fontDetails[charId].h * scale ),
				                    depth, uvRect, false );
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
		text = wrapText(text, scale);
		
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


	/// <summary>
	/// Empty all the arrays of text sprites
	/// </summary>
	public void removeAllText()
	{
		for( var i = _textSprites.Count - 1; i >= 0; i-- )
			deleteText( i );
		
		_textSprites.Clear();
	}

}
