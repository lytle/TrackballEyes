Shader "Custom/FinalBlend" {
	Properties{
				_LeftEye("Left Eye (RGB)", 2D) = "white" {}
				_RightEye("Right Eye (RGB)", 2D) = "white" {}
				_Guide("Guide (RGB)", 2D) = "white" {}
				_Threshold("Threshold",Range(0,1)) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf NoLighting noforwardadd

			sampler2D _LeftEye;
			sampler2D _RightEye;
			sampler2D _Guide;
			float _Threshold;

			struct Input {
				float2 uv_LeftEye;
				float2 uv_RightEye;
				float2 uv_Guide;
			};
			
			void surf(Input IN, inout SurfaceOutput o) {
				half4 left = tex2D(_LeftEye, IN.uv_LeftEye);
				half4 right = tex2D(_RightEye, IN.uv_RightEye);
				half4 g = tex2D(_Guide, IN.uv_Guide);
				/*if ((g.r + g.g + g.b)*0.33333f < _Threshold)
					o.Albedo = right.rgb;
				else
					o.Albedo = left.rgb;*/
				o.Albedo.r = 0.65 * ((right.r * g.r) + (left.r * (1 - g.r)));
				o.Albedo.g = 0.65 * ((right.g * g.g) + (left.g * (1 - g.g)));
				o.Albedo.b = 0.65 * ((right.b * g.b) + (left.b * (1 - g.b)));

				o.Alpha = left.a;
			}

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				fixed4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}
		FallBack "Diffuse"
}