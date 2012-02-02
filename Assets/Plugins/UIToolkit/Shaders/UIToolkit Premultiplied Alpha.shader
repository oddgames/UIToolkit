Shader "UIToolkit/Premultiplied Alpha"
{
	Properties
	{
		_TintColor( "Tint Color", Color ) = ( 0.5, 0.5, 0.5, 0.5 )
		_MainTex( "Particle Texture", 2D ) = "white" {}
	}
	
	Category
	{
		Tags { "Queue" = "Transparent" }
		Blend One OneMinusSrcAlpha
		ColorMask RGBA
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
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
