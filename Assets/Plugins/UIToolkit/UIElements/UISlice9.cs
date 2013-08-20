/// <summary>
/// Touchable Slice-9 Implementation for UIToolkit
/// Created by David O'Donoghue (ODD Games)
/// </summary>
using UnityEngine;


public class UISlice9 : UITouchableSprite
{
	public delegate void UISlice9TouchUpInside( UISlice9 sender );
	public delegate void UISlice9TouchDown( UISlice9 sender );
	public delegate void UISlice9TouchUp( UISlice9 sender );

	public event UISlice9TouchUpInside onTouchUpInside; // event for when we get a touchUpInside
	public event UISlice9TouchDown onTouchDown; // event for when we get a touchUpInside
	public event UISlice9TouchUp onTouchUp; // event for when we get a touchUpInside
	
	public UIUVRect highlightedUVframe;
	public AudioClip touchDownSound;
	public Vector2 initialTouchPosition;
	UISprite[] spriteSlices = new UISprite[9];


	// Sets the uvFrame of the original UISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _clipped ? _uvFrameClipped : _uvFrame; }
		set
		{
			base.uvFrame = value;
			
			manager.updateUV( this );
			
		}
	}
	
	public override bool highlighted
	{
		set
		{
			// Only set if it is different than our current value
			if( _highlighted != value )
			{			
				_highlighted = value;
				
				if( value )
					base.uvFrame = highlightedUVframe;
				else
					base.uvFrame = _tempUVframe;
			}
		}
	}

	// override hidden so we can set the touch frame to dirty when being shown
	public override bool hidden
    {
        get { return ___hidden; }
        set
        {
            base.hidden = value;

			foreach( UISprite sprite in spriteSlices )
				sprite.hidden = value;
        }
    }

	
	#region Constructors/Destructor

	/// <summary>
	/// Create a Slice-9 touchable sprite
	/// </summary>
	/// <param name='filename'>
	/// Filename
	/// </param>
	/// <param name='highlightedFilename'>
	/// Highlighted filename
	/// </param>
	/// <param name='xPos'>
	/// X position
	/// </param>
	/// <param name='yPos'>
	/// Y position
	/// </param>
	/// <param name='width'>
	/// Width
	/// </param>
	/// <param name='height'>
	/// Height
	/// </param>
	/// <param name='sTP'>
	/// Top Margin
	/// </param>
	/// <param name='sRT'>
	/// Right Margin
	/// </param>
	/// <param name='sBT'>
	/// Bottom Margin
	/// </param>
	/// <param name='sLT'>
	/// Left Margin
	/// </param>
	public static UISlice9 create( string filename, string highlightedFilename, int xPos, int yPos, int width, int height, int sTP, int sRT, int sBT, int sLT )
	{
		return UISlice9.create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos, width, height, sTP, sRT, sBT, sLT );
	}
	
	/// <summary>
	/// Create a Slice-9 touchable sprite
	/// </summary>
	/// <param name='manager'>
	/// Manager.
	/// </param>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	/// <param name='highlightedFilename'>
	/// Highlighted filename.
	/// </param>
	/// <param name='xPos'>
	/// X position.
	/// </param>
	/// <param name='yPos'>
	/// Y position.
	/// </param>
	/// <param name='width'>
	/// Width.
	/// </param>
	/// <param name='height'>
	/// Height.
	/// </param>
	/// <param name='sTP'>
	/// Top Margin
	/// </param>
	/// <param name='sRT'>
	/// Right Margin
	/// </param>
	/// <param name='sBT'>
	/// Bottom Margin
	/// </param>
	/// <param name='sLT'>
	/// Left Margin
	/// </param>
	public static UISlice9 create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int width, int height, int sTP, int sRT, int sBT, int sLT )
	{
		return UISlice9.create( manager, filename, highlightedFilename, xPos, yPos, width, height, sTP, sRT, sBT, sLT, 1 );
	}
	
	/// <summary>
	/// Create a Slice-9 touchable sprite
	/// </summary>
	/// <param name='manager'>
	/// Manager.
	/// </param>
	/// <param name='filename'>
	/// Filename.
	/// </param>
	/// <param name='highlightedFilename'>
	/// Highlighted filename.
	/// </param>
	/// <param name='xPos'>
	/// X position.
	/// </param>
	/// <param name='yPos'>
	/// Y position.
	/// </param>
	/// <param name='width'>
	/// Width.
	/// </param>
	/// <param name='height'>
	/// Height.
	/// </param>
	/// <param name='sTP'>
	/// Top Margin
	/// </param>
	/// <param name='sRT'>
	/// Right Margin
	/// </param>
	/// <param name='sBT'>
	/// Bottom Margin
	/// </param>
	/// <param name='sLT'>
	/// Left Margin
	/// </param>
	public static UISlice9 create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int width, int height, int sTP, int sRT, int sBT, int sLT, int depth )
	{
		// grab the texture details for the normal state
		var normalTI = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, normalTI.frame.width, normalTI.frame.height );
		
		// get the highlighted state
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		// create the button
		return new UISlice9( manager, frame, depth, normalTI.uvRect, highlightedTI.uvRect, width, height, sTP, sRT, sBT, sLT );
	}


	public override void updateTransform()
	{
		//foreach( UISprite sprite in spriteSlices )
			//sprite.updateTransform();
	
		manager.updatePositions();
	}


	/// <summary>
	/// Initializes a new instance of the <see cref="UISlice9"/> class.
	/// </summary>
	/// <param name='manager'>
	/// Manager.
	/// </param>
	/// <param name='frame'>
	/// Frame.
	/// </param>
	/// <param name='depth'>
	/// Depth.
	/// </param>
	/// <param name='uvFrame'>
	/// Uv frame.
	/// </param>
	/// <param name='highlightedUVframe'>
	/// Highlighted U vframe.
	/// </param>
	/// <param name='width'>
	/// Final width
	/// </param>
	/// <param name='height'>
	/// Final height
	/// </param>
	/// <param name='sTP'>
	/// Top Margin
	/// </param>
	/// <param name='sRT'>
	/// Right Margin
	/// </param>
	/// <param name='sBT'>
	/// Bottom Margin
	/// </param>
	/// <param name='sLT'>
	/// Left Margin
	/// </param>
	public UISlice9( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe, int width, int height, int sTP, int sRT, int sBT, int sLT ):base( frame, depth, uvFrame )
	{
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		this.highlightedUVframe = highlightedUVframe;
		
		int stretchedWidth = width - ( sLT + sRT );
		int stretchedHeight = height - ( sTP + sBT );
		this.manager = manager;
		
		// Top Left
		spriteSlices[0] = new UISprite( new Rect( frame.x, frame.y, sLT, sTP ), depth, new UIUVRect( uvFrame.frame.x, uvFrame.frame.y, sLT, sTP, manager.textureSize ) );
		spriteSlices[0].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[0] );
		
		// Top Middle
		spriteSlices[1] = new UISprite( new Rect( frame.x + sLT, frame.y, stretchedWidth, sTP ), depth, new UIUVRect( uvFrame.frame.x + sLT, uvFrame.frame.y, uvFrame.frame.width - ( sRT + sLT ), sTP, manager.textureSize ) );
		spriteSlices[1].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[1] );
		
		// Top Right
		spriteSlices[2] = new UISprite( new Rect( frame.x + sLT + stretchedWidth, frame.y, sRT, sTP ), depth, new UIUVRect( uvFrame.frame.x + sLT + ( uvFrame.frame.width - ( sRT + sLT ) ), uvFrame.frame.y, sRT, sTP, manager.textureSize ) );
		spriteSlices[2].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[2] );
		
		// Middle Left
		spriteSlices[3] = new UISprite( new Rect( frame.x, frame.y + sTP, sLT, stretchedHeight ), depth, new UIUVRect( uvFrame.frame.x, uvFrame.frame.y + sTP, sLT, uvFrame.frame.height - ( sTP + sBT ), manager.textureSize ) );
		spriteSlices[3].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[3] );		
		
		// Middle Middle
		spriteSlices[4] = new UISprite( new Rect( frame.x + sLT, frame.y + sTP, stretchedWidth, stretchedHeight ), depth, new UIUVRect( uvFrame.frame.x + sLT, uvFrame.frame.y + sTP, uvFrame.frame.width - ( sLT + sRT ), uvFrame.frame.height - ( sBT + sTP ), manager.textureSize ) );
		spriteSlices[4].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[4] );		
		
		// Middle Right
		spriteSlices[5] = new UISprite( new Rect( frame.x + sLT + stretchedWidth, frame.y + sTP, sRT, stretchedHeight ), depth, new UIUVRect( uvFrame.frame.x + ( uvFrame.frame.width - sRT ), uvFrame.frame.y + sTP, sRT, uvFrame.frame.height - ( sBT + sTP ), manager.textureSize ) );
		spriteSlices[5].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[5] );		
		
		// Bottom Left
		spriteSlices[6] = new UISprite( new Rect( frame.x, frame.y + sTP + stretchedHeight, sLT, sBT ), depth, new UIUVRect( uvFrame.frame.x, uvFrame.frame.y + ( uvFrame.frame.height - sBT ), sLT, sBT, manager.textureSize ) );
		spriteSlices[6].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[6] );		
		
		// Bottom Middle
		spriteSlices[7] = new UISprite( new Rect( frame.x + sLT, frame.y + sTP + stretchedHeight, stretchedWidth, sBT ), depth, new UIUVRect( uvFrame.frame.x + sLT, uvFrame.frame.y + ( uvFrame.frame.height - sBT ), uvFrame.frame.width - ( sLT + sRT ), sBT, manager.textureSize ) );
		spriteSlices[7].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[7] );
		
		// Bottom Right
		spriteSlices[8] = new UISprite( new Rect( frame.x + sLT + stretchedWidth, frame.y + sTP + stretchedHeight, sRT, sBT ), depth, new UIUVRect( uvFrame.frame.x + sLT + ( uvFrame.frame.width - ( sRT + sLT ) ), uvFrame.frame.y + ( uvFrame.frame.height - sBT ), sRT, sBT, manager.textureSize ) );
		spriteSlices[8].client.transform.parent = this.client.transform;
		manager.addSprite( spriteSlices[8] );
		
		this.setSize( width, height );
		
		manager.addTouchableSprite( this );
	}

	#endregion;


	// Touch handlers
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		highlighted = true;
		
		initialTouchPosition = touch.position;
		
		if( touchDownSound != null )
			UI.instance.playSound( touchDownSound );
		
		if( onTouchDown != null )
			onTouchDown( this );
	}


	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		// If someone has un-highlighted us through code we are deactivated 
		// and should not fire the event
		if( !highlighted )
			return;
		
		highlighted = false;

		if( onTouchUp != null )
			onTouchUp( this );
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame && onTouchUpInside != null )
			onTouchUpInside( this );
	}


	public override void destroy()
	{
		
		foreach( UISprite sprite in spriteSlices )
		{
			sprite.destroy();
		}
		
		base.destroy();
		
		highlighted = false;
	}


	public override void centerize()
	{
		
		base.centerize();
		
		foreach( UISprite sprite in spriteSlices )
		{
			sprite.centerize();
		}
		
	}


	public override void setSpriteImage( string filename )
	{
		foreach( UISprite sprite in spriteSlices )
		{
			sprite.setSpriteImage( filename );
		}
	}


	public override void transformChanged()
	{
		base.transformChanged();
		
		foreach( UISprite sprite in spriteSlices )
		{
			sprite.transformChanged();
		}
	}
	
}
