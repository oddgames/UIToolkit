using UnityEngine;


public struct UIUVRect
{
	public Vector2 lowerLeftUV;
	public Vector2 uvDimensions;
	public UIClippingPlane clippingPlane; // used internally for clipping
	
	private Vector2 _originalCoordinates; // used internally for clipping
	private int _originalWidth; // used internally for clipping


	/// <summary>
	/// Convenience property to return a UVRect of all zeros
	/// </summary>
	public static UIUVRect zero;
	

	/// <summary>
	/// Automatically converts coordinates to UV space as specified by textureSize
	/// </summary>
	public UIUVRect( int x, int y, int width, int height, Vector2 textureSize )
	{
		_originalCoordinates.x = x;
		_originalCoordinates.y = y;
		_originalWidth = width;
		
		lowerLeftUV = new Vector2( x / textureSize.x, 1.0f - ( ( y + height ) / textureSize.y ) );
		uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
		clippingPlane = UIClippingPlane.None;
	}

	
	public UIUVRect rectClippedToBounds( float width, float height, UIClippingPlane clippingPlane, Vector2 textureSize )
	{
		var uv = this;
		uv.clippingPlane = clippingPlane;
		
		// if we are clipping the top or right, only the uvDimensions need adjusting
		switch( clippingPlane )
		{
			case UIClippingPlane.Left:
			{
				var widthDifference = _originalWidth - width;
			
				uv.lowerLeftUV = new Vector2( ( ( _originalCoordinates.x + widthDifference ) / textureSize.x ), 1.0f - ( ( _originalCoordinates.y + height ) / textureSize.y ) );
				uv.uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
				break;
			}
			case UIClippingPlane.Right:
			{
				uv.lowerLeftUV = new Vector2( _originalCoordinates.x / textureSize.x, 1.0f - ( ( _originalCoordinates.y + height ) / textureSize.y ) );
				uv.uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
				break;
			}
			case UIClippingPlane.Top:
			{
				uv.uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
				break;
			}
			case UIClippingPlane.Bottom:
			{
				uv.lowerLeftUV = new Vector2( _originalCoordinates.x / textureSize.x, 1.0f - ( ( _originalCoordinates.y + height ) / textureSize.y ) );
				uv.uvDimensions = new Vector2( width / textureSize.x, height / textureSize.y );
				break;
			}
		}

		return uv;
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
