Shader "Custom/TargetLaser" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Width ("Width", Float) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		
		PASS {
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		float4 _Color;
		float _Width;

		struct VS_In {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		struct VS_Out {
			float4 position : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct PS_Out {
			float4 color : COLOR;
		};

		VS_Out vert(VS_In input) {
			VS_Out output;
			output.position = mul(UNITY_MATRIX_MVP, input.vertex);
			output.color = _Color;
			output.texcoord = input.texcoord;
			return output;
		}

		PS_Out frag(VS_Out input) {
			PS_Out output;
			output.color = input.color;
			output.color.a *= (1-input.texcoord.y) * (sin(input.texcoord.x*3.14) - (1-_Width));
			return output;
		}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}
