Shader "Toolkit/Outline (Single-Pass)"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Width ("Width", Range(0, 10)) = 1
    }
    SubShader
    {
        Pass{
        Cull Front

        CGPROGRAM

        #pragma vertex VertProg 
        #pragma fragment FragProg


            float _FloatTime;

        float3 getDeform(float3 p)
        {
            return (float3(p.xy, 0.3) * (cos(_FloatTime * 4 + p.z) + sin(p.x * 10 + _FloatTime * 9) + cos(p.y * 10 + _FloatTime * 3))) / 10 +
                float3(0, cos(_FloatTime * 4 + p.z + cos(p.z + _FloatTime)) / 6, 0);
        }

        fixed4 _Color;
        float _Width;

        half4 VertProg(half4 position : POSITION, half3 normal : NORMAL) : SV_POSITION
        {
            normal += getDeform(normal);
            half4 cP = UnityObjectToClipPos(position);
            half3 cN = mul((half3x3) UNITY_MATRIX_VP, mul((half3x3) UNITY_MATRIX_M, position));
            cP.xy += normalize(cN.xy) / _ScreenParams.xy * (_Width * cP.w * 10);
            return cP;
        }

        half4 FragProg() : SV_TARGET
        {
            return _Color;
        }
        ENDCG
        }
    }
    FallBack "Diffuse"
}
