using UnityEngine;
using UIEaseType = System.Func<float, float>;
using System.Collections;
using System.Collections.Generic;


// addTextInstance (from the UIText class) returns one of these so we just need to do a .text on the instance to update it's text
public class UITextInstance : UIObject, IPositionable
{
	private UIText _parentText;
	private string _text;
    private float _scale;
	private UITextAlignMode _alignMode;
	private UITextVerticalAlignMode _verticalAlignMode;

	public Color[] colors;
	public List<UISprite> textSprites = new List<UISprite>(); // all the sprites that make up the string
	public UIToolkit manager
	{
		get { return _parentText.manager; }
	}

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
			if ( _text.Length < textSprites.Count )
			{
                for ( var i = textSprites.Count - 1; i >= _text.Length; i-- )
                {
                    var sprite = textSprites[i];
                    textSprites.RemoveAt( i );
                    _parentText.manager.removeElement( sprite );
                }
			}

			_parentText.updateText( this );
            updateSize();
		}
	}
	
	
    public float textScale
    {
        get { return _scale; }
        set
        {
            // Don't do anything if scale is the same
            if( _scale == value )
				return;

            _scale = value;
            _parentText.updateText( this );
            updateSize();
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


    public float xPos
    {
        get { return position.x; }
        set
        {
            Vector3 pos = position;
            pos.x = value;
            position = pos;
        }
    }


    public float yPos
    {
        get { return position.y; }
        set
        {
            Vector3 pos = position;
            pos.y = -value;
            position = pos;
        }
    }


    public int depth
    {
        get { return (int)position.z; }
        set
        {
            Vector3 pos = position;
            pos.z = value;
            position = pos;
        }
    }


    public UITextAlignMode alignMode
    {
        get { return _alignMode; }
        set
        {
            if (_alignMode == value)
                return;

            _alignMode = value;
            switch (_alignMode)
            {
                case UITextAlignMode.Left:
                    _anchorInfo.OriginUIxAnchor = UIxAnchor.Left;
                    break;
                case UITextAlignMode.Center:
                    _anchorInfo.OriginUIxAnchor = UIxAnchor.Center;
                    break;
                case UITextAlignMode.Right:
                    _anchorInfo.OriginUIxAnchor = UIxAnchor.Right;
                    break;
            }

            _parentText.updateText(this);
            updateSize();
        }
    }


    public UITextVerticalAlignMode verticalAlignMode
    {
        get { return _verticalAlignMode; }
        set
        {
            if (_verticalAlignMode == value)
                return;

            _verticalAlignMode = value;
            switch (_verticalAlignMode)
            {
                case UITextVerticalAlignMode.Top:
                    _anchorInfo.OriginUIyAnchor = UIyAnchor.Top;
                    break;
                case UITextVerticalAlignMode.Middle:
                    _anchorInfo.OriginUIyAnchor = UIyAnchor.Center;
                    break;
                case UITextVerticalAlignMode.Bottom:
                    _anchorInfo.OriginUIyAnchor = UIyAnchor.Bottom;
                    break;
            }

            _parentText.updateText(this);
            updateSize();
        }
    }


    public override Color color
    {
        get
        {
            return colors[0];
        }
        set
        {
            setColorForAllLetters(value);
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
		_parentText = parentText;
		_text = text;
        _scale = scale;
		this.colors = colors;
        position = new Vector3( xPos, -yPos, depth );
		_hidden = false;

        // Set anchor alignment
        _alignMode = alignMode;
        switch( alignMode )
        {
            case UITextAlignMode.Left:
                _anchorInfo.OriginUIxAnchor = UIxAnchor.Left;
                break;
            case UITextAlignMode.Center:
                _anchorInfo.OriginUIxAnchor = UIxAnchor.Center;
                break;
            case UITextAlignMode.Right:
                _anchorInfo.OriginUIxAnchor = UIxAnchor.Right;
                break;
        }
		
        // Set anchor vertical alignment
        _verticalAlignMode = verticalAlignMode;
        switch( verticalAlignMode )
        {
            case UITextVerticalAlignMode.Top:
                _anchorInfo.OriginUIyAnchor = UIyAnchor.Top;
                break;
            case UITextVerticalAlignMode.Middle:
                _anchorInfo.OriginUIyAnchor = UIyAnchor.Center;
                break;
            case UITextVerticalAlignMode.Bottom:
                _anchorInfo.OriginUIyAnchor = UIyAnchor.Bottom;
                break;
        }

        _anchorInfo.OffsetX = xPos;
        _anchorInfo.OffsetY = yPos;

        updateSize();
	}


    private void updateSize()
    {
        Vector2 size = _parentText.sizeForText( _text, _scale );

        _width = size.x;
        _height = size.y;
        
        this.refreshPosition();
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
	/// Destroys the UITextInstance object and its sprites
	/// </summary>
	public void destroy()
	{
		clear();
		Object.Destroy(client);
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
        base.transformChanged();
        this.refreshPosition();
	}
    
}