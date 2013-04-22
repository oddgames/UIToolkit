using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * A container for buttons that only allows one button to be selected at a time
 */

public class UIRadioButtonGroup : UIAbstractContainer
{
	public delegate void UIRadioGroupChanged( UIRadioButtonGroup sender, UIToggleButton selected );
	public event UIRadioGroupChanged OnSelect; // event for when we get a touchUpInside
	
	UIToggleButton m_currentlySelected;
	public UIToggleButton CurrentlySelected
	{
		get
		{
			return m_currentlySelected;	
		}
		set
		{
			UIToggleButton previouslySelected = m_currentlySelected;
			
			if(previouslySelected == value)
			{
				return;
			}
			
			m_currentlySelected = value;
			UpdateToggleStates();
			DispatchChangeEvent();
		}
	}
	
	public int IndexOfCurrentlySelected
	{
		get
		{
			 return _children.IndexOf(m_currentlySelected);	
		}
		set
		{
			int boundIndex = value;
			if(	boundIndex >= _children.Count )
			{
				boundIndex = _children.Count - 1;
			}
			
			if( boundIndex < 0 )
			{
				boundIndex = 0;
			}
			
			CurrentlySelected = _children[boundIndex] as UIToggleButton;
		}
	}

	public int NumberOfChildren
	{
		get
		{
			 return _children.Count;
		}
	}
	
	public UIRadioButtonGroup(int spacing, UILayoutType withLayout = UILayoutType.Vertical) 
		: base (withLayout)
	{
		_spacing = spacing;
	}
	
	public override void addChild (params UISprite[] children)
	{
		foreach(UISprite child in children)
		{
			if(child is UIToggleButton)
			{
				UIToggleButton toggle = child as UIToggleButton;
				toggle.onToggle += HandleToggleonToggle;
			}
			else
			{
				UnityEngine.Debug.LogWarning(child.GetType() + 
					" is not a UIToggleButton and will not be added to the radio group");
			}
		}
			
		base.addChild (children);
	}

	void HandleToggleonToggle (UIToggleButton sender, bool selected)
	{
		if(m_currentlySelected == sender)
		{
			return;
		}
		
		m_currentlySelected = sender;
		UpdateToggleStates();	
		DispatchChangeEvent();
	}
	
	void UpdateToggleStates()
	{
		foreach(UISprite child in _children)
		{
			UIToggleButton toggle = child as UIToggleButton;
			if(toggle == m_currentlySelected)
			{
				toggle.selected = true;
				toggle.disabled = true;
			}
			else
			{
				toggle.selected = false;
				toggle.disabled = false;
			}
		}
	}
	
	void DispatchChangeEvent()
	{
		if(OnSelect != null)
		{
			OnSelect(this, m_currentlySelected);	
		}	
	}
}

