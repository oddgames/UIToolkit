using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour 
{
	
	// Use this for initialization
	void Start () 
	{
		var grid = new UIGridLayout(5, 5, 2);
		grid.edgeInsets = new UIEdgeInsets(2);
		
		//add buttons automagically
		UISprite[] buttons = new UISprite[9];
		for (var i=0; i<9; i++)
		{
			var depth = - (i+1);
			var button = UIButton.create( UI.firstToolkit, "emptyUp.png", "emptyDown.png", 0, 0, depth);
			buttons[i] = button;
		
		}
		grid.addChild(buttons);
		
		//add a button in a specific place with fixed width and height
		var anotherButton = UIButton.create( UI.firstToolkit, "emptyUp.png", "emptyDown.png", 0, 0, 10);
		//columns and rows are zero based
		grid.AddChildAt(anotherButton, 2, 0, 2, 2);
		
		//add buttons with snapping
		for (var i=0; i<2; i++)
		{
			for (var j=0; j<2; j++)
			{
				var button = UIButton.create( UI.firstToolkit, "emptyUp.png", "emptyDown.png", 0, 0, 11);
				grid.AddChildAt(button, 2+i, 2+j, true);
			}
		}
	}
}
