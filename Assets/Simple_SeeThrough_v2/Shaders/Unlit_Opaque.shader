Shader "Simple SeeThrough v2/Unlit Objects"
{
	Properties
	{
        [Enum(Off,0,Front,1,Back,2)] _CullMode ("Culling Mode", int) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 8
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex ("_MainTex (RGBA)", 2D) = "white" {}

		[Header(Cutout)]
		[Toggle(ISCUTOUT)] _isCutout ("Is Cutout?", Float) = 0
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

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
			"Queue" = "Geometry" 
			"RenderType" = "Opaque" 
		}

        Lighting Off 
		ZWrite On 
		Cull [_CullMode]
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
            ZTest [_ZTest]
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_unlit
            #pragma shader_feature ISCUTOUT
            #include "SCTCGUnlit.cginc"
			ENDCG
		}

		Pass
		{
			Tags
			{
				"Queue" = "Geometry+1"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
			ZTest LEqual

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

				if (c.r > 0.49 && c.r < 0.51 && c.g == 0.0 && c.b > 0.49 && c.b < 0.51)
				{
					c.r = _TeamColor.r * 0.6 * c.a;
					c.g = _TeamColor.g * 0.6 * c.a;
					c.b = _TeamColor.b * 0.6 * c.a;
				}

				c.rgb *= c.a;

				UNITY_APPLY_FOG(IN.fogCoord, c);
				return c;
			}

			ENDCG
		}
	}
}
