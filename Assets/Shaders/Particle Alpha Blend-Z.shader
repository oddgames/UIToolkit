Shader "Particles/Alpha Blended Z-enabled"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	Category
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off
		Lighting Off
		ZWrite On
		
		Fog { Color (0,0,0,0) }
		
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		// ---- Dual texture cards
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					constantColor [_TintColor]
					combine constant * primary
				}
				SetTexture [_MainTex]
				{
					combine texture * previous DOUBLE
				}
			}
		}
		
		// ---- Single texture cards (does not do color tint)
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					combine texture * primary
				}
			}
		}
	}
}
