using UnityEngine;
using System.Collections;


public class UIVerticalPanel : UIAbstractContainer
{
	private UISprite _topStrip;
	private UISprite _middleStrip;
	private UISprite _bottomStrip;
	
	private int _topStripHeight;
	private int _bottomStripHeight;
	
	protected bool _sizeToFit = true; // should the panel make itself the proper size to fit?
	public bool sizeToFit { get { return _sizeToFit; } set { _sizeToFit = value; layoutChildren(); } } // relayout when sizeToFit changes

	public override Vector3 position
	{
		get { return clientTransform.position; }
		set
		{
			base.position = value;
			layoutChildren();
		}
	}
	
	/// <summary>
	/// Hides the container and all of it's children along with the top, middle and bottom strip
	/// </summary>
    public override bool hidden
    {
        set
        {
            // No need to do anything if we're already in this state
            if( value == hidden )
                return;
			
			// let super handle hiding the children
			base.hidden = value;

			_topStrip.hidden = _middleStrip.hidden = _bottomStrip.hidden = value;
        }
    }


	public static UIVerticalPanel create( string topFilename, string middleFilename, string bottomFilename )
	{
		return create( UI.firstToolkit, topFilename, middleFilename, bottomFilename );
	}
	
	
	public static UIVerticalPanel create( UIToolkit manager, string topFilename, string middleFilename, string bottomFilename )
	{
		return new UIVerticalPanel( manager, topFilename, middleFilename, bottomFilename );
	}


	public UIVerticalPanel( UIToolkit manager, string topFilename, string middleFilename, string bottomFilename ) : base( UILayoutType.Vertical )
	{
		var texInfo = manager.textureInfoForFilename( topFilename );
		_topStrip = manager.addSprite( topFilename, 0, 0, 5 );
		_topStrip.parentUIObject = this;
		_topStripHeight = (int)texInfo.frame.height;
		_width = texInfo.frame.width;
		
		// we overlap the topStrip 1 pixel to ensure we get a good blend here
		_middleStrip = manager.addSprite( middleFilename, 0, _topStripHeight , 5 );
		_middleStrip.parentUIObject = this;
		
		texInfo = manager.textureInfoForFilename( middleFilename );
		_bottomStrip = manager.addSprite( bottomFilename, 0, 0, 5 );
		_bottomStrip.parentUIObject = this;
		_bottomStripHeight = (int)texInfo.frame.height;
	}


	/// <summary>
	/// Override so that we can set the zIndex to be higher than our background sprites
	/// </summary>
	public new void addChild( params UISprite[] children )
	{
		foreach( var child in children )
			child.zIndex = this.zIndex + 1;
		
		base.addChild( children );
	}


	/// <summary>
	/// Responsible for laying out the child UISprites
	/// </summary>
	protected override void layoutChildren()
	{
		base.layoutChildren();
		matchSizeToContentSize();
		
		// make sure we have enough height to work with.  totalAvailableHeight will be calculated as the minimum required height for the panel
		// if _height is greater than that, we will use it
		var totalAvailableHeight = _topStripHeight + _bottomStripHeight + 1 + _edgeInsets.top + _edgeInsets.bottom;
		if( _contentHeight > totalAvailableHeight )
			totalAvailableHeight = (int)_contentHeight;

		// now we move our sprites into position.  we have the proper height now so we can use that with a couple extra pixels to fill the gap
		var leftoverHeight = totalAvailableHeight - _topStripHeight - _bottomStripHeight + 3;
		
		_middleStrip.setSize( _middleStrip.width, leftoverHeight );
		_bottomStrip.localPosition = new Vector3( _bottomStrip.localPosition.x, -_contentHeight - _bottomStripHeight, _bottomStrip.localPosition.z );
	}

}
