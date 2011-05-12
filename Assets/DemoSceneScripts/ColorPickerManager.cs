using UnityEngine;
using System.Collections;

public class ColorPickerManager : MonoBehaviour
{
	UIColorPicker colorPicker;
	UIButton chosenColor;
	
	int numRecent = 10;
	UIButton[] recentColors;


	void Start()
	{
		colorPicker = UIColorPicker.create( "ColorPalette.png", 0, 0, 1 );
		colorPicker.positionFromTopLeft( 0.3f, 0.2f );
		colorPicker.highlightedTouchOffsets = new UIEdgeOffsets( 40 );
		colorPicker.onColorChange += onColorChange;
		colorPicker.onColorChangeBegan += onColorChangeBegan;
		
		chosenColor = UIButton.create("ColorButtonUp.png", "ColorButtonDown.png", 0, 0 );
		chosenColor.pixelsFromTopRight(50, 50);
		chosenColor.color = Color.white;
		
		recentColors = new UIButton[numRecent];
		for( var i = 0; i < numRecent; i++ )
		{
			recentColors[i] = UIButton.create( "ColorButtonUp.png", "ColorButtonDown.png", 0, 0 );
			recentColors[i].pixelsFromBottomLeft( 50, 50 + (int)( i * recentColors[i].width ) );
			recentColors[i].onTouchUpInside += recentColorButtonTouchUp;
		}
	}
	
	void onColorChangeBegan( UIColorPicker sender, Color newColor, Color oldColor )
	{
		for( var i = numRecent - 1; i > 0; i-- )
		{
			recentColors[i].color = recentColors[i - 1].color;
		}
		recentColors[0].color = oldColor;
		chosenColor.color = newColor;
	}


	void onColorChange( UIColorPicker sender, Color newColor, Color oldColor )
	{
		chosenColor.color = newColor;
	}

		
	void recentColorButtonTouchUp( UIButton sender )
	{
		int index = getRecentColorIndex( sender );		
		chooseRecentColor( index );
	}


	private int getRecentColorIndex( UIButton sender )
	{
		for( var i = 0; i < numRecent; i++ )
		{
			if( recentColors[i] == sender )
			{
				return i;
			}
		}
		return -1;
	}


	private void chooseRecentColor( int recentColorIndex )
	{
		Color recentColorChosen = recentColors[recentColorIndex].color;
		
		for( var i = recentColorIndex; i > 0; i-- )
		{
			recentColors[i].color = recentColors[i - 1].color;
		}
		recentColors[0].color = colorPicker.colorPicked;
		colorPicker.colorPicked = recentColorChosen;
		chosenColor.color = recentColorChosen;
	}
}
