Shader "Custom/Apply Texture" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
	_NewTex ("Base (RGB)", 2D) = "black" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex, _NewTex;

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
	o.uv = float2(uv.x, uv.y);
	return o;
}


half4 frag (v2f i) : COLOR
{
	float x = floor(i.uv.x * 128f) / 128f;
	float y = floor(i.uv.y * 72f) / 72f;
	//float4 alive = tex2D(_MainTex, float2(x,y));
	return tex2D(_MainTex, float2(x,y)) + tex2D(_NewTex, float2(x,y))*1000;
}
ENDCG
	}
}

Fallback off

}