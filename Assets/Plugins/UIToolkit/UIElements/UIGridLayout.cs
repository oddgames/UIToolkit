using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * gareth williams
 * 
 * compose layouts by attaching uisprites to a grid of m_clipss
 * 
 * overloaded functionality for
 * row and column spans
 * snapping objects to grid cell dimensions
 * 
 * overriden functionality for
 * adding an array of children to be automagically attached to the grid
 */

public class UIGridLayout : UIAbstractContainer
{
	public int columns;
	public int rows;
	
	UIAnchorInfo m_gridAnchor;
	
	public UIGridLayout( int columns, int rows, int cellPadding ) 
		: base( UIAbstractContainer.UILayoutType.AbsoluteLayout ) //don't manage layout for us
	{
		//padding
		_spacing = cellPadding;
		
		this.columns = columns;
		this.rows = rows;
		
		//top left
		m_gridAnchor = UIAnchorInfo.DefaultAnchorInfo();
		m_gridAnchor.ParentUIObject = this;
		m_gridAnchor.UIPrecision = UIPrecision.Pixel;
	}
	
	public void AddChildAt(UISprite child, int column, int row)
	{
		var anchorInfo = GetCellAnchorFor(column, row);
		
		// dont overwrite the sprites origin anchor!
		anchorInfo.OriginUIxAnchor = child.anchorInfo.OriginUIxAnchor;
		anchorInfo.OriginUIyAnchor = child.anchorInfo.OriginUIyAnchor;
		
		child.beginUpdates();
			child.anchorInfo = anchorInfo;
			child.refreshPosition();
		child.endUpdates();
		
		base.addChild(child);
	}
	
	#region overloads
	public void AddChildAt(UISprite child, int column, int row, int colSpan, int rowSpan)
	{
		float cellWidth  = Screen.width / columns;
		float cellHeight = Screen.height / rows;

		Vector3 scale = new Vector3(1, 1, 1);
		if(colSpan > 0)
		{
			scale.x = 1.0f / child.width * (colSpan * cellWidth);	
		}
		if(rowSpan > 0)
		{
			scale.y = 1.0f / child.height * (rowSpan * cellHeight);
		}
		
		child.scale = scale;
		AddChildAt(child, column, row);
	}
	
	public void AddChildAt(UISprite child, int column, int row, bool snapToGrid)
	{	
		Vector3 scale = new Vector3(1, 1, 1);
		if(snapToGrid)
		{
			scale.x = 1.0f / child.width * GetSnappedWidthToGrid(child);
			scale.y = 1.0f / child.height * GetSnappedHeightToGrid(child);		
		}
		
		child.scale = scale;
		AddChildAt(child, column, row);
	}
	#endregion
	
	public override void addChild (params UISprite[] children)
	{
		for (int i=0; i<children.Length; i++)
		{
			var toCol = Mathf.FloorToInt(i / columns);
			var toRow = i - toCol * columns;
			
			var anchorInfo = GetCellAnchorFor(toCol, toRow);
			
			// dont overwrite the sprites origin anchor!
			anchorInfo.OriginUIxAnchor = children[i].anchorInfo.OriginUIxAnchor;
			anchorInfo.OriginUIyAnchor = children[i].anchorInfo.OriginUIyAnchor;
			
			children[i].beginUpdates();
				children[i].anchorInfo = anchorInfo;
				children[i].refreshPosition();
			children[i].endUpdates();
		}
		
		base.addChild (children);
	}
	
	//helper functions
	UIAnchorInfo GetCellAnchorFor(int column, int row)
	{
		float cellWidth  = Screen.width / columns;
		float cellHeight = Screen.height / rows;
		
		var cellAnchor = UIAnchorInfo.DefaultAnchorInfo();
		cellAnchor.ParentUIObject = this;
		cellAnchor.ParentUIxAnchor = m_gridAnchor.OriginUIxAnchor;
		cellAnchor.ParentUIyAnchor = m_gridAnchor.OriginUIyAnchor;
		cellAnchor.OffsetX = UI.scaleFactor * cellWidth * column +_edgeInsets.left;
		cellAnchor.OffsetY = UI.scaleFactor * cellHeight * row + _edgeInsets.right;
		
		return cellAnchor;
	}

	float GetSnappedWidthToGrid (UISprite child)
	{
		float cellWidth = Screen.width / columns;
		int requiredColumns = Mathf.RoundToInt(child.width / cellWidth);
		float snappedWidth = cellWidth * requiredColumns;
		
		if(snappedWidth <= 0)
		{
			snappedWidth = cellWidth;	
		}
		
		return snappedWidth;
	}

	float GetSnappedHeightToGrid (UISprite child)
	{
		float cellHeight = Screen.height / rows;
		int requiredRows = Mathf.RoundToInt(child.height / cellHeight);
		float snappedHeight = cellHeight * requiredRows;
		
		if(snappedHeight <= 0)
		{
			snappedHeight = cellHeight;	
		}
		
		return snappedHeight;
	}
}
