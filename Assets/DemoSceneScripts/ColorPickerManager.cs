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
		colorPicker = UIColorPicker.create("ColorPalette.png", 0, 0, 1);
		colorPicker.positionFromTopLeft(0.3f, 0.2f);
		colorPicker.highlightedTouchOffsets = new UIEdgeOffsets(40);
		colorPicker.onColorChange += OnColorChange;
		colorPicker.onColorChangeBegan += OnColorChangeBegan;
		
		chosenColor = UIButton.create("ColorButtonUp.png", "ColorButtonDown.png", 0, 0);
		chosenColor.pixelsFromTopRight(50, 50);
		chosenColor.color = Color.white;
		
		recentColors = new UIButton[numRecent];
		for (int i = 0; i < numRecent; i++)
		{
			recentColors[i] = UIButton.create("ColorButtonUp.png", "ColorButtonDown.png", 0, 0);
			recentColors[i].pixelsFromBottomLeft(50, 50 + (int)(i * recentColors[i].width));
			recentColors[i].onTouchUpInside += RecentColorButtonTouchUp;
		}
	}
	
	void OnColorChangeBegan (UIColorPicker sender, Color newColor, Color oldColor)
	{
		for (int i = numRecent-1; i > 0; i--)
		{
			recentColors[i].color = recentColors[i - 1].color;
		}
		recentColors[0].color = oldColor;
		chosenColor.color = newColor;
	}

	void OnColorChange (UIColorPicker sender, Color newColor, Color oldColor)
	{
		chosenColor.color = newColor;
	}
	
	void RecentColorButtonTouchUp( UIButton sender )
	{
		int index = GetRecentColorIndex(sender);		
		ChooseRecentColor(index);
	}
	
	private int GetRecentColorIndex( UIButton sender )
	{
		for (int i = 0; i < numRecent; i++)
		{
			if(recentColors[i] == sender)
			{
				return i;
			}
		}
		return -1;
	}
	
	private void ChooseRecentColor (int recentColorIndex)
	{
		Color recentColorChosen = recentColors[recentColorIndex].color;
		
		for (int i = recentColorIndex; i > 0; i--) 
		{
			recentColors[i].color = recentColors[i - 1].color;
		}
		recentColors[0].color = colorPicker.ColorPicked;
		colorPicker.ColorPicked = recentColorChosen;
		chosenColor.color = recentColorChosen;
	}
}
