

struct MRData
{
    // GBuffer0
    float3 albedo;     // RGB
    float occlusion;   // A (occlusion packed in alpha channel)
    
    // GBuffer1
    float3 wnormal;    // RGB
    float smoothness;  // A (smoothness packed in alpha channel)
    
    // GBuffer2
    float metallic;    // R
    float roughness;   // G
    int materialID;  // B
    
    //GBuffer3

};

struct ModularRenderPacked
{
    float4 albedoOcclusion : SV_Target0;
    float4 wnormalSmoothness : SV_Target1;
    float4 metallicRoughnessMaterialId : SV_Target2;
};

float3 EncodeNormal(float3 n)
{
    return n * 0.5 + 0.5;
}

float3 DecodeNormal(float3 encoded)
{
    return encoded * 2.0 - 1.0;
}

ModularRenderPacked Pack(MRData data) {
    ModularRenderPacked output;
    output.albedoOcclusion = float4(data.albedo, data.occlusion);
    output.wnormalSmoothness = float4(EncodeNormal(data.wnormal), data.smoothness);
    output.metallicRoughnessMaterialId = float4(data.metallic, data.roughness, data.materialID * 0.003921568627451, 0);
    return output;
}

MRData Unpack(float4 tex0, float4 tex1, float4 tex2)
{
    MRData data;
    data.albedo = tex0.rgb;
    data.occlusion = tex0.a;
    data.wnormal = DecodeNormal(tex1.rgb);
    data.smoothness = tex1.a;
    data.metallic = tex2.r;
    data.roughness = tex2.g;
    data.materialID = tex2.b * 255;
    return data;
}

MRData Unpack(sampler2D tex0, sampler2D tex1, sampler2D tex2, float2 uv)
{
    float4 pack0 = tex2D(tex0, uv);
    float4 pack1 = tex2D(tex1, uv);
    float4 pack2 = tex2D(tex2, uv);
    return Unpack(pack0, pack1, pack2);
}

MRData DefaultMRData()
{
    MRData d;
    d.albedo = float3(1, 1, 1);
    d.occlusion = 1;
    d.wnormal = float3(0, 1, 0);
    d.smoothness = 0;
    d.metallic = 0;
    d.roughness = 0;
    d.materialID = 0;
    return d;
}