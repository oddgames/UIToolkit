using UnityEngine;
using System.Collections.Generic;


public class UIStateButton : UIButton
{
	public delegate void UIStateButtonStateChange( UIStateButton sender, int state );
	public event UIStateButtonStateChange onStateChange;
	
	private bool _rollOverState = true;
	private int _state = 0;
	private int maxState = 0;
		
	private UIUVRect[] _uvFrames;
	private UIUVRect[] _uvHighlightFrames;
	

	#region Constructors/Destructor
	
	new public static UIStateButton create( string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIStateButton.create( UI.firstToolkit, filename, highlightedFilename, xPos, yPos );
	}
	
	
	new public static UIStateButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos )
	{
		return UIStateButton.create( manager, filename, highlightedFilename, xPos, yPos, 1 );
	}
	
	
	new public static UIStateButton create( UIToolkit manager, string filename, string highlightedFilename, int xPos, int yPos, int depth )
	{
		var filenames = new string[1] {filename};
		var highlightedFilenames = new string[1] {highlightedFilename};
		return UIStateButton.create( manager, filenames, highlightedFilenames, xPos, yPos, depth );
	}	
	
	
	public static UIStateButton create( string[] filenames, string[] highlightedFilenames, int xPos, int yPos )
	{
		return UIStateButton.create( UI.firstToolkit, filenames, highlightedFilenames, xPos, yPos );
	}
	
	
	public static UIStateButton create( UIToolkit manager, string[] filenames, string[] highlightedFilenames, int xPos, int yPos )
	{
		return UIStateButton.create( manager, filenames, highlightedFilenames, xPos, yPos, 1 );
	}
		
	
	public static UIStateButton create( UIToolkit manager, string[] filenames, string[] highlightedFilenames, int xPos, int yPos, int depth )
	{
		
		// grab the texture details for the initial state
		var firstNormalTI = manager.textureInfoForFilename( filenames[0] );
		var frame = new Rect( xPos, yPos, firstNormalTI.frame.width, firstNormalTI.frame.height );

		// get the initial highlighted state
		var firstHighlightedTI = firstNormalTI;
		if (highlightedFilenames.Length > 0) {
			manager.textureInfoForFilename( highlightedFilenames[0] );
		}
		
		var button = new UIStateButton( manager, frame, depth, firstNormalTI.uvRect, firstHighlightedTI.uvRect );

		button.addFrames(filenames, highlightedFilenames);
		
		return button;
	}
	
	
	public UIStateButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect highlightedUVframe ):base( manager, frame, depth, uvFrame, highlightedUVframe )
	{
	}


	#endregion;
	
	
	/// <summary>
	/// Flag whether state wraps back to 0 after last state hit
	/// </summary>
	public bool rollOverState
	{
		get { return _rollOverState; }
		set { _rollOverState = value; }
	}
	
	
	public int state
	{
		get { return _state; }
		set
		{
			if( _state == value )
				return ;

			_state = value;
			adjustForStateRollover( _state );	
			setFramesForState( _state );
		}
	}
	
	
	public UIUVRect[] uvFrames
	{
		get { return _uvFrames; }
	}
	
	
	public UIUVRect[] uvHighlightFrames
	{
		get { return _uvHighlightFrames; }
	}
	
	
	public void addFrames( string[] normal, string[] highlighted )
	{
		var normals = loadFrames( normal );
		var highlights = loadFrames( highlighted );
		addFrames( normals, highlights );
	}
	
	
	public void addFrames( UIUVRect[] normal, UIUVRect[] highlighted )
	{
		_uvFrames = normal;
		maxState = _uvFrames.Length;
		_state = 0;
		
		if( highlighted == null || highlighted.Length == 0 )
		{
			_uvHighlightFrames = normal;
		}
		else if( normal.Length == highlighted.Length )
		{
			_uvHighlightFrames = highlighted;
		}
		else
		{
			// don't have same number of highlighted as normal
			Debug.LogError( "Highlight frames count does not match normal frames count" );
			_uvHighlightFrames = normal;
		}
	}
	

	public override void onTouchEnded( UITouchWrapper touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		if( touchWasInsideTouchFrame )
		{
			_state++;
			adjustForStateRollover( _state );
		}
		
		base.onTouchEnded( touch, touchPos, touchWasInsideTouchFrame );

		if( touchWasInsideTouchFrame )
		{
			setFramesForState( _state );
			
			// If the touch was inside our touchFrame and we have an action, call it
			if( onStateChange != null )
				onStateChange( this , _state);
		}
	}
	
	
	private void adjustForStateRollover( int newState )
	{
		if( _state >= maxState )
		{
			if( _rollOverState )
				_state = 0;
			else
				_state = maxState - 1;
		}
	}
	
	
	private void setFramesForState( int newState )
	{
		uvFrame = _uvFrames[newState];
		highlightedUVframe = _uvHighlightFrames[newState];
	}
	
	
	private UIUVRect[] loadFrames( string[] filenames )
	{
		var frames = new UIUVRect[filenames.Length];
		for( var i = 0; i < filenames.Length; i++ )
		{
			var uv = this.manager.textureInfoForFilename( filenames[i] ).uvRect;
			frames[i] = uv;
		}
		return frames;
	}
	

}