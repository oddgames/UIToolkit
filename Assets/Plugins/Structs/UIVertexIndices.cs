
using System;

public struct UIVertexIndex
{
	public int one;
	public int two;
	public int three;
	public int four;
}


public struct UIVertexIndices
{
	public UIVertexIndex mv;
	public UIVertexIndex uv;
	public UIVertexIndex cv;
	
	
	public void initializeVertsWithIndex( int i )
	{
		// Setup indices of the vertices in the vertex buffer:
		mv.one = i * 4 + 0;
		mv.two = i * 4 + 1;
		mv.three = i * 4 + 2;
		mv.four = i * 4 + 3;
		
		// Setup the indices of the UV entries in the UV buffer:
		uv.one = i * 4 + 0;
		uv.two = i * 4 + 1;
		uv.three = i * 4 + 2;
		uv.four = i * 4 + 3;
		
		// Setup the indices to the color values:
		cv.one = i * 4 + 0;
		cv.two = i * 4 + 1;
		cv.three = i * 4 + 2;
		cv.four = i * 4 + 3;
	}

}

