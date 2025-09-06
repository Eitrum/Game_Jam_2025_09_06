// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/StandardVertexNoise"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Normal("Normal Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NoiseStrength("Noise", Range(0,1)) = 0.2
        _NoiseFrequency("Frequency", Range(0,100)) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Normal;
        float _NoiseStrength;
        float _NoiseFrequency;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz * _NoiseFrequency;
            float3 diff = float3(cos(worldPos.x + 37.15 + worldPos.y * 0.31), sin(worldPos.y + 12.83 + worldPos.z * 0.31), cos(worldPos.z + 19.57 + worldPos.x * 0.31));
            diff = diff * diff * diff;
            v.vertex.xyz += diff * _NoiseStrength;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
