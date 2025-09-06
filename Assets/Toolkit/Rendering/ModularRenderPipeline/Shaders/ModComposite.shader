Shader "Hidden/CompositeMRTs"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "ModularRenderCore.cginc"

            sampler2D _RT0;
            sampler2D _RT1;
            sampler2D _RT2;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float3 GetLighting(float3 normal){
                return float3(normal.y,0,0);
            }

            fixed4 frag(v2f i) : SV_Target {
                MRData data = Unpack(_RT0, _RT1, _RT2, i.uv);
                float3 color = data.albedo;
                float lightMulti = saturate(dot(data.wnormal, float3(0.71, 0.71, 0)));
                lightMulti = max(0.03, lightMulti);
                color *= lightMulti;
                return float4(color, 1);
                //return (c0 + c1 + c2) / 3; // average for demo
            }

            ENDCG
        }
    }
}