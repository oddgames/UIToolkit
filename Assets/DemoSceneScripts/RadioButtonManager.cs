using UnityEngine;
using System.Collections;

public class RadioButtonManager : MonoBehaviour 
{

	UIScrollableHorizontalLayout m_scrollable;
	
	// Use this for initialization
	void Start () 
	{
		CreateScrollableMenuWithPips();
	}
	
	#region create ui helpers
	void CreateScrollableMenuWithPips()
	{
		m_scrollable = new UIScrollableHorizontalLayout( 20 );
		
		// we wrap the addition of all the sprites with a begin updates so it only lays out once when complete
		m_scrollable.beginUpdates();
		
			// if you plan on making the scrollable wider than the item width you need to set your edgeInsets so that the
			// left + right inset is equal to the extra width you set
			float itemWidth = 250f;
			float leftInset;
			float rightInset;
			leftInset = rightInset = (Screen.width - itemWidth) / 2;
			
			m_scrollable.edgeInsets = new UIEdgeInsets( 0, Mathf.RoundToInt(leftInset), 0, Mathf.RoundToInt(rightInset) );
			
			var scrollerHeight 	= UI.scaleFactor * itemWidth;
			var scrollerWidth 	= UI.scaleFactor * ( itemWidth + leftInset + rightInset ); // item width + 150 extra width
			m_scrollable.setSize( scrollerWidth, scrollerHeight );
			
			// paging will snap to the nearest page when scrolling
			m_scrollable.pagingEnabled = true;
			m_scrollable.pageWidth = itemWidth * UI.scaleFactor;
			
			// center the scrollable horizontally
			m_scrollable.position = new Vector3( ( Screen.width - scrollerWidth ) / 2, -Screen.height + scrollerHeight, 0 );
			
			for( var i = 0; i < 20; i++ )
			{
				var button = UIButton.create( "marioPanel.png", "marioPanel.png", 0, 0 );
				m_scrollable.addChild( button );
			}
			
		m_scrollable.endUpdates();
		m_scrollable.endUpdates(); // this is a bug. it shouldnt need to be called twice
		
		//pips
		var pips = new UIRadioButtonGroup(0, UIAbstractContainer.UILayoutType.Horizontal);
		
		int pageCount = Mathf.CeilToInt(20 * itemWidth / scrollerWidth);
		for (int i = 0; i < pageCount; i++)
		{		
			var toggle = UIToggleButton.create("emptyUp.png", "emptyDown.png", "emptyUp.png", 0, 0);
			
			pips.addChild(toggle);
		}
		
		pips.beginUpdates();
			float screenUnits = 1.0f / Screen.width;
			float halfWidth = 140 * pageCount / 2;
			float offset = halfWidth * screenUnits;
			pips.positionFromCenter(-50 * screenUnits, -offset);
		pips.endUpdates();
		
		pips.OnSelect += HandlePipsOnSelect;
		pips.IndexOfCurrentlySelected = 1;
	}
	#endregion
	
	#region handlers
	void HandlePipsOnSelect (UIRadioButtonGroup sender, UIToggleButton selected)
	{
		print ("pips selection changed");	
		
		int pageIndex = Mathf.RoundToInt( 20f / sender.NumberOfChildren * sender.IndexOfCurrentlySelected );
		m_scrollable.scrollToPage(pageIndex);
	}
	#endregion handlers
}
