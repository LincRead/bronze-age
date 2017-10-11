
Shader "Custom/OutlineWorking"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (.1,.1,.1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		// Add values to determine if outlining is enabled and outline color.
		[PerRendererData] _Outline("Outline", Float) = 0
		[PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1
		[PerRendererData] _OccludeColor("Occlusion Color", Color) = (0,0,1,1)
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"CanUseSpriteAtlas" = "True"
			"Queue" = "Overlay+1"
		}

		Pass
		{
			ZWrite on
			Blend One OneMinusSrcAlpha
			ZTest Less
			AlphaToMask On

			CGPROGRAM

			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSize;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

				// If outline is enabled and there is a pixel, try to draw an outline.
				if (_Outline > 0 && c.a != 0)
				{
					float totalAlpha = 1.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
					}

					if (totalAlpha == 0)
					{
						c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
						c.rgb *= c.a;
					}

					else
					{
						c.a = 0;
						c.rgb *= c.a;
					}
				}

				return c;
			}

			ENDCG
		}

		Pass
		{
			Lighting Off
			ZWrite on
			Blend One OneMinusDstAlpha
			ZTest LEqual

			Tags
			{
				"RenderType" = "Transparent"
				"CanUseSpriteAtlas" = "True"
				"Queue" = "Geometry+1"
			}

			CGPROGRAM

			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _OccludeColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}

			ENDCG
		}
	}
}