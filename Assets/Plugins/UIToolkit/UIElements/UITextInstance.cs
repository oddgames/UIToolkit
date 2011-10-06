using UnityEngine;
using UIEaseType = System.Func<float, float>;
using System.Collections;


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
	public int textIndex;
	public Color[] colors;

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
			_parentText.setHiddenForTextInstance( this, value );
			_hidden = value;
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
		this.textIndex = -1;
		this.colors = colors;
		_hidden = false;
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
		this.colors = new Color[] { color };
		_parentText.updateColorForTextInstance( this );
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
		_parentText.updateColorForTextInstance( this );
	}

	
	/// <summary>
	/// Overide transformChanged so that 
	/// </summary>
	public override void transformChanged()
	{
		Debug.Log( "wtf, transformChanged()" );
	}

}