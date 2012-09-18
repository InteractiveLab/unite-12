Shader "Custom/GameOfLife" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NewTex ("New (RGB)", 2D) = "black" {}
		_DecayFactor("Decay Factor", Range(0, 1)) = 0.97
	}
	SubShader {
		Pass { 
			Fog { Mode Off }
			Cull Off
			ZWrite Off
			AlphaTest Off
			Blend Off
			CGPROGRAM
			#pragma target 3.0	
			#pragma exclude_renderers gles flash
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex, _NewTex;
			float _GridStepX;
			float _GridStepY;
			float _DecayFactor;

			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};	

			v2f vert (appdata v) {
				v2f o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				o.uv = float4( v.texcoord.xy, 0, 0 );
				return o;
			}
			half4 frag( v2f i ) : COLOR {
				float4 alive = tex2D(_MainTex, i.uv.xy);
				float4 alive2 = tex2D(_MainTex, i.uv.xy - float2(0, _GridStepY));
				float4 totalAlive = (ceil(tex2D(_MainTex, i.uv.xy + float2(_GridStepX, _GridStepY))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(_GridStepX, 0))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(_GridStepX, -_GridStepY))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(0, -_GridStepY))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(-_GridStepX, -_GridStepY))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(-_GridStepX, 0))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(-_GridStepX, _GridStepY))) +
								  ceil(tex2D(_MainTex, i.uv.xy + float2(0, _GridStepY))));
				float newColor = 0;
				int total = round(totalAlive.r);
				if (alive.r == 0) {
					if (total == 3) {
						newColor = 1;
					}
				} else {
					if (total == 2 || total == 3) {
						newColor = 1;
					} 
				}

				float cx = ceil(tex2D(_NewTex, i.uv.xy).x-.8) + newColor;
				float4 c = float4(cx, cx, cx, 1);
				return c;
			}
			ENDCG
		}
	}

	FallBack "Diffuse"
}
