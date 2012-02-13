Shader "UIToolkit/Alpha Enabled (Default)"
{
	Properties
	{
		_TintColor( "Tint Color", Color ) = ( 0.5, 0.5, 0.5, 1. )
		_MainTex( "Particle Texture", 2D ) = "white" {}
	}
	
	Category
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGBA
		Cull Off
		Lighting Off
		ZWrite On
		Fog { Mode Off }
		
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					constantColor[_TintColor]
					combine constant * primary
				}
				SetTexture[_MainTex]
				{
					combine texture * previous DOUBLE
				}
			}
		} // end subshader
	}
}
