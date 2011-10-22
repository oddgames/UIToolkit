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
	
	
	public UIUVRect rectClippedToBounds( Vector2 textureSize )
	{
		var uv = this;
		
		
		return uv;
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
