Shader "Custom/GameOfLifeRenderer" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SecondTex ("Second (RGB)", 2D) = "white" {}
		_Color ("Base (RGB)", Color) = (0, 0, 0, 0)
		_Color2 ("Second (RGB)", Color) = (0, 0, 0, 0)
		_InverseColor ("Inverse Color", Range(0, 1)) = 0
		_Saturation ("Saturation", Range(0, 1)) = 1
		_Lerp("Texture Lerp", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex, _SecondTex;
		float4 _Color, _Color2;
		float _Lerp;
		float _InverseColor, _Saturation;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 c2 = tex2D (_SecondTex, IN.uv_MainTex);
			half4 color = (lerp(_Color*(c.r*_Saturation + c.g), _Color2*c2.r, _Lerp));
			o.Emission = (1 - color) * _InverseColor + (1 - _InverseColor) * color;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
