Shader "Custom/NewBlendShader"
{
    Properties
    {
		_LeftEye("Left Eye (RGB)", 2D) = "white" {}
		_RightEye("Right Eye (RGB)", 2D) = "white" {}
		_Guide("Guide (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 300

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv_LeftEye : TEXCOORD0;
				float2 uv_RightEye : TEXCOORD1;
				float2 uv_Guide : TEXCOORD2;
            };

            struct v2f
            {
				float2 uv_LeftEye : TEXCOORD0;
				float2 uv_RightEye : TEXCOORD1;
				float2 uv_Guide : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _LeftEye;
            float4 _LeftEye_ST;
			sampler2D _RightEye;
			float4 _RightEye_ST;
			sampler2D _Guide;
			float4 _Guide_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_LeftEye = TRANSFORM_TEX(v.uv_LeftEye, _LeftEye);
				o.uv_RightEye = TRANSFORM_TEX(v.uv_RightEye, _RightEye);
				o.uv_Guide = TRANSFORM_TEX(v.uv_Guide, _Guide);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the rendertextures
                half4 left = tex2D(_LeftEye, i.uv_LeftEye);
				half4 right = tex2D(_RightEye, i.uv_RightEye);
				half4 g = tex2D(_Guide, i.uv_Guide);

				half4 col;
				col.r = (right.r * g.r) + (left.r * (1 - g.r));
				col.g = (right.g * g.r) + (left.g * (1 - g.r));
				col.b = (right.b * g.r) + (left.b * (1 - g.r));
				col.a = (right.a * g.r) + (left.a * (1 - g.r));


                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
