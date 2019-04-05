Shader "Custom/GoToTransparent" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_LeftEye("Left Eye", 2D) = "white" {}
		_RightEye("Left Eye", 2D) = "white" {}
		_Blend("Cross-eye Blend", Range(0,1)) = 0.0
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert
			struct Input {
				float2 uv_LeftEye;
				float2 uv_RightEye;
			};
			sampler2D _LeftEye;
			sampler2D _RightEye;
			half _Blend;
			half _Color;


			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = lerp(tex2D(_LeftEye, IN.uv_LeftEye), tex2D(_RightEye, IN.uv_RightEye), _Blend.x) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}
		Fallback "Diffuse"
}