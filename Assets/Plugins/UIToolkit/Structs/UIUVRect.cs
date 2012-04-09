using UnityEngine;


public struct UIUVRect
{
	public Vector2 lowerLeftUV;
	public Vector2 uvDimensions;

	/// <summary>
	/// Convenience property to return a UVRect of all zeros
	/// </summary>
	public static UIUVRect zero;
	

	/// <summary>
	/// Automatically converts coordinates to UV space as specified by textureSize
	/// </summary>
	public UIUVRect( int x, int y, int width, int height, Vector2 textureSize )
	{
		lowerLeftUV = new Vector2( x / textureSize.x, 1.0f - ( ( y + height ) / textureSize.y ) );
		uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
	}

	private static float lerp(float x, float x1, float y0, float y1)
	{
		return y0 + x * ((y1 - y0)/x1);
	}

	public UIUVRect Intersect(Rect localVisibleRect, Vector2 texSize)
	{
		float spr_w = Mathf.Abs(uvDimensions.x * texSize.x);
		float spr_h = Mathf.Abs(uvDimensions.y * texSize.y);

		float base_uv_l = lowerLeftUV.x;
		float base_uv_r = lowerLeftUV.x + uvDimensions.x;
		float base_uv_u = lowerLeftUV.y + uvDimensions.y;
		float base_uv_b = lowerLeftUV.y;

		float uv_l = lerp(localVisibleRect.xMin, spr_w, base_uv_l, base_uv_r);
		float uv_r = lerp(localVisibleRect.xMax, spr_w, base_uv_l, base_uv_r);
		float uv_u = lerp(localVisibleRect.yMin, spr_h, base_uv_u, base_uv_b);
		float uv_b = lerp(localVisibleRect.yMax, spr_h, base_uv_u, base_uv_b);

		UIUVRect r;
		r.lowerLeftUV = new Vector2(uv_l, uv_b);
		r.uvDimensions = new Vector2(uv_r - uv_l, uv_u - uv_b);
		return r;
	}
	
	public float getWidth( Vector2 textureSize )
	{
		return uvDimensions.x * textureSize.x;
	}
	
	
	public float getHeight( Vector2 textureSize )
	{
		return uvDimensions.y * textureSize.y;
	}
	
	
	/// <summary>
	/// doubles everything for retina texture
	/// </summary>
	public void doubleForHD()
	{
		lowerLeftUV.x *= 2;
		lowerLeftUV.y *= 2;
		uvDimensions.x *= 2;
		uvDimensions.y *= 2;
	}


	#region Operator overloads
	
	public static bool operator ==( UIUVRect lhs, UIUVRect rhs )
	{
		return ( lhs.lowerLeftUV == rhs.lowerLeftUV && lhs.uvDimensions == rhs.uvDimensions );
	}


	public static bool operator !=( UIUVRect lhs, UIUVRect rhs )
	{
		return ( lhs.lowerLeftUV != rhs.lowerLeftUV || lhs.uvDimensions != rhs.uvDimensions );
	}


	public override bool Equals( object obj )
	{
		if( ( obj is UIUVRect ) && this == (UIUVRect)obj )
			return true;
		
		return false;
	}


	public override int GetHashCode()
	{
		return lowerLeftUV.GetHashCode() ^ uvDimensions.GetHashCode();
	}
	
	
	public override string ToString()
	{
		return string.Format( "x: {0}, y: {1}, w: {2}, h: {3}", lowerLeftUV.x, lowerLeftUV.y, uvDimensions.x, uvDimensions.y );
	}

	#endregion;

}
