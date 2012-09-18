Shader "Unite/Slide" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white"
		_Transparent ("Transparent", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		

		CGPROGRAM
		#pragma surface surf Unlit alpha

		sampler2D _MainTex;
		float _Transparent;

		half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) {
          half NdotL = dot (s.Normal, lightDir);
          half4 c;
          c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
          c.a = s.Alpha;
          return c;
      }


		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			if (_Transparent > 0 && c.r == c.g) {
				o.Emission = half3(0,0,0);
				o.Alpha = min(c.a, (1-c.r));
			} else {
				o.Emission = c.rgb;
				o.Alpha = c.a;
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

