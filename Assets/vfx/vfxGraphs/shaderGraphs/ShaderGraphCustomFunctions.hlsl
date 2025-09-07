#include "Assets\shaders\srp\Common.cginc"
#include "Assets\shaders\srp\Legacy.cginc"

void sgSampleSceneColor_float(float2 uv, out float3 color)
{
    color =  tex2D(_forsakenGrabPass, DRSScaleUV(uv)).rgb;
}

void DepthFade_float(float4 screenPos, float dist, out float depth)
{
    depth = DepthFade(screenPos, max(0.001, dist));
}