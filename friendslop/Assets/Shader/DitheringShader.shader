Shader "Unlit/DitheringShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherStrength ("Dither Strength", Range(0,1)) = 1
        _ColorSteps ("Color Steps", Range(2, 64)) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _DitherStrength;
            float _ColorSteps;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float Bayer4x4(int x, int y)
            {
                int bayer[4][4] = {
                    {  0,  8,  2, 10 },
                    { 12,  4, 14,  6 },
                    {  3, 11,  1,  9 },
                    { 15,  7, 13,  5 }
                };
                return bayer[y & 3][x & 3] / 16.0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUV = i.uv * _ScreenParams.xy;
                float threshold = Bayer4x4((int)screenUV.x, (int)screenUV.y);
                fixed4 col = tex2D(_MainTex, i.uv);

                float steps = _ColorSteps;

                float3 quantized;
                quantized.r = floor(col.r * steps + threshold) / steps;
                quantized.g = floor(col.g * steps + threshold) / steps;
                quantized.b = floor(col.b * steps + threshold) / steps;

                float3 result = lerp(col.rgb, quantized, _DitherStrength);

                return float4(result, 1);
            }
            ENDCG
        }
    }
    FallBack Off
}
