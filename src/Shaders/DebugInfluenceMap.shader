Shader "Unlit/DebugInfluenceMap"
{
	Properties
	{
		_InfluenceTex0 ("Texture", 2D) = "white" {}
		_InfluenceTex1 ("Texture", 2D) = "white" {}
		_InfluenceTex2 ("Texture", 2D) = "white" {}
		_InfluenceTex3 ("Texture", 2D) = "white" {}
		_InfluenceTex4 ("Texture", 2D) = "white" {}
		_InfluenceTex5 ("Texture", 2D) = "white" {}
		_InfluenceTex6 ("Texture", 2D) = "white" {}
		_InfluenceTex7 ("Texture", 2D) = "white" {}
		_InfluenceTex8 ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha // Alpha blend
		//Cull Off ZWrite Off ZTest Always
		AlphaTest Greater .3

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vertInput 
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertOutput 
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			uniform sampler2D _InfluenceTex0;
			uniform float4 _InfluenceTex0_ST;
			uniform float3 _InfluenceColor0;

			uniform sampler2D _InfluenceTex1;
			uniform float4 _InfluenceTex1_ST;
			uniform float3 _InfluenceColor1;

			uniform sampler2D _InfluenceTex2;
			uniform float4 _InfluenceTex2_ST;
			uniform float3 _InfluenceColor2;

			uniform sampler2D _InfluenceTex3;
			uniform float4 _InfluenceTex3_ST;
			uniform float3 _InfluenceColor3;

			uniform sampler2D _InfluenceTex4;
			uniform float4 _InfluenceTex4_ST;
			uniform float3 _InfluenceColor4;

			uniform sampler2D _InfluenceTex5;
			uniform float4 _InfluenceTex5_ST;
			uniform float3 _InfluenceColor5;

			uniform sampler2D _InfluenceTex6;
			uniform float4 _InfluenceTex6_ST;
			uniform float3 _InfluenceColor6;

			uniform sampler2D _InfluenceTex7;
			uniform float4 _InfluenceTex7_ST;
			uniform float3 _InfluenceColor7;

			vertOutput vert(vertInput input) 
			{
				vertOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, input.pos);
				o.uv = TRANSFORM_TEX(input.uv, _InfluenceTex0);
				return o;
			}

			fixed4 frag (vertOutput i) : COLOR
			{
				// sample the texture
				fixed influence0 = tex2D(_InfluenceTex0, i.uv).a;
				fixed influence1 = tex2D(_InfluenceTex1, i.uv).a;
				fixed influence2 = tex2D(_InfluenceTex2, i.uv).a;
				fixed influence3 = tex2D(_InfluenceTex3, i.uv).a;
				fixed influence4 = tex2D(_InfluenceTex4, i.uv).a;
				fixed influence5 = tex2D(_InfluenceTex5, i.uv).a;
				fixed influence6 = tex2D(_InfluenceTex6, i.uv).a;
				fixed influence7 = tex2D(_InfluenceTex7, i.uv).a;

				fixed3 col = (influence0 * _InfluenceColor0);
				col += (influence1 * _InfluenceColor1);
				col += (influence2 * _InfluenceColor2);
				col += (influence3 * _InfluenceColor3);
				col += (influence4 * _InfluenceColor4);
				col += (influence5 * _InfluenceColor5);
				col += (influence6 * _InfluenceColor6);
				col += (influence7 * _InfluenceColor7);

				fixed4 outCol = fixed4(col, 0.4);
				return outCol;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
