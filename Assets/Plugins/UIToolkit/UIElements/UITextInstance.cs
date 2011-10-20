using UnityEngine;
using UIEaseType = System.Func<float, float>;
using System.Collections;
using System.Collections.Generic;


// addTextInstance (from the UIText class) returns one of these so we just need to do a .text on the instance to update it's text
public class UITextInstance : UIObject
{
	private UIText _parentText;
	private string _text;
	public UITextAlignMode alignMode;
	public UITextVerticalAlignMode verticalAlignMode;
	
	public float xPos;
	public float yPos;
	public float scale;
	public int depth;
	public Color[] colors;
	public List<UISprite> textSprites = new List<UISprite>(); // all the sprites that make up the string

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
			
			// cleanse our textSprites of any excess that we dont need
			if( _text.Length > textSprites.Count )
			{
				for( var i = textSprites.Count - 1; i > _text.Length; i-- )
				{
					var sprite = textSprites[i];
					textSprites.RemoveAt( i );
					_parentText.manager.removeElement( sprite );
				}
			}
			
			_parentText.updateText( this );
		}
	}
	
	private bool _hidden;
	public bool hidden 
	{
		get 
		{ 
			return _hidden; 
		}
		set 
		{
			_hidden = value;
			
			foreach( var sprite in textSprites )
				sprite.hidden = _hidden;
		}
	}
	

	
	/// <summary>
	/// Call the full constructor with default alignment modes brought from the parent UIText object.
	/// </summary>
	public UITextInstance( UIText parentText, string text, float xPos, float yPos, float scale, int depth, Color color ) : this( parentText, text, xPos, yPos, scale, depth, new Color[] { color }, parentText.alignMode, parentText.verticalAlignMode )
	{}
	
	
	/// <summary>
	/// Full constructor with per-instance alignment modes.
	/// </summary>
	public UITextInstance( UIText parentText, string text, float xPos, float yPos, float scale, int depth, Color[] colors, UITextAlignMode alignMode, UITextVerticalAlignMode verticalAlignMode ) : base()
	{
		client.transform.position = new Vector3( xPos, yPos, depth );
		
		this.alignMode = alignMode;
		this.verticalAlignMode = verticalAlignMode;
		_parentText = parentText;
		_text = text;
		this.xPos = xPos;
		this.yPos = yPos;
		this.scale = scale;
		this.depth = depth;
		this.colors = colors;
		_hidden = false;
	}
	
	
	private void applyColorToSprites()
	{
		// how many sprites are we updated?
		var length = textSprites.Count;
		
		// we either make all the letters the same color or each letter a different color
		if( colors.Length == 1 )
		{
			for( int i = 0; i < length; i++ )
				textSprites[i].color = colors[0];
		}
		else
		{
			for( int i = 0; i < length; i++ )
				textSprites[i].color = colors[i];
		}
	}
	
	
	/// <summary>
	/// Returns either the sprite at the given index (if it exists) or null
	/// </summary>
	public UISprite textSpriteAtIndex( int index )
	{
		if( textSprites.Count > index )
			return textSprites[index];
		return null;
	}
	
	
	/// <summary>
	/// Clears the text from the screen
	/// </summary>
	public void clear()
	{
		_text = null;
		
		foreach( var sprite in textSprites )
			_parentText.manager.removeElement( sprite );
		
		textSprites.Clear();
	}
	

	/// <summary>
	/// Sets the color for the text.  All colors will be changed.
	/// </summary>
	public void setColorForAllLetters( Color color )
	{
		this.colors = new Color[] { color };
		applyColorToSprites();
	}


	/// <summary>
	/// Sets the color for each character in the text.  colors should contain at least the number of colors as there
	/// are characters in the text.
	/// </summary>
	/// <param name="colors">
	/// A <see cref="Color[]"/>
	/// </param>
	public void setColorPerLetter( Color[] colors )
	{
		// sanity check
		if( colors.Length < _text.Length )
			return;
		
		this.colors = colors;
		applyColorToSprites();
	}

	
	/// <summary>
	/// Overide transformChanged so that 
	/// </summary>
	public override void transformChanged()
	{
		
	}

}