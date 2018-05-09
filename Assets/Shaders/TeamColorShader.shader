// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/TeamColor"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _TeamColor("TeamColor", Color) = (1,0,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _TeamColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

				// Apply team color on Magenta pixels
				if (c.r == 1.0 && c.g == 0.0 && c.b == 1.0)
				{
					c = _TeamColor * c.a;
				}

				// Dark
				else if (c.r > 0.48 && c.r < 0.52 && c.g == 0.0 && c.b > 0.48 && c.b < 0.52)
				{
					c.r = _TeamColor.r * 0.5 * c.a;
					c.g = _TeamColor.g * 0.5 * c.a;
					c.b = _TeamColor.b * 0.5 * c.a;
				}

				// Darkened dark
				else if (c.r > 0.24 && c.r < 0.26 && c.g == 0.0 && c.b > 0.24 && c.b < 0.26)
				{
					c.r = _TeamColor.r * 0.25 * c.a;
					c.g = _TeamColor.g * 0.25 * c.a;
					c.b = _TeamColor.b * 0.25 * c.a;
				}

				c.rgb *= c.a;
					
				UNITY_APPLY_FOG(IN.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
}
