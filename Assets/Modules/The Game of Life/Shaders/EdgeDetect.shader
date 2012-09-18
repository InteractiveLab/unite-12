Shader "Custom/Edge Detect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Treshold ("Treshold", Float) = 0.2
	_YScew ("Y Scew", Float) = 1
	_YDisplacement ("Y Displacement", Float) = 0
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

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform float _Treshold, _YScew, _YDisplacement;

struct v2f {
	float4 pos : POSITION;
	float2 uv[3] : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
	uv = float2(uv.x, uv.y * _YScew);
	o.uv[0] = uv + float2(0, _YDisplacement);
	o.uv[1] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) + float2(0, _YDisplacement);
	o.uv[2] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) + float2(0, _YDisplacement);
	return o;
}


half4 frag (v2f i) : COLOR
{
	half4 original = tex2D(_MainTex, i.uv[0]);

	half3 p1 = original.rgb;
	half3 p2 = tex2D( _MainTex, i.uv[1] ).rgb;
	half3 p3 = tex2D( _MainTex, i.uv[2] ).rgb;
	
	half3 diff = p1 * 2 - p2 - p3;
	half len = dot(diff,diff);
	if( len <= _Treshold )
		original.rgb = 0;
		
	return original;
}
ENDCG
	}
}

Fallback off

}