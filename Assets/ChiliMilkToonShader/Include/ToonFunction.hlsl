#ifndef TOON_FUNCTION_INCLUDED
#define TOON_FUNCTION_INCLUDED
 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//https://github.com/Jason-Ma-233/JasonMaToonRenderPipeline
//Get Smooth Outline NormalWS
float3 GetSmoothedWorldNormal(float2 uv7, float3x3 t_tbn)
{
    float3 normal = float3(uv7, 0);
    normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
    return mul(normal, t_tbn);
}

//Use for HairSpecular,Roughness To BlinnPhong
float RoughnessToBlinnPhongSpecularExponent(float roughness)
{
    return clamp(2 * rcp(roughness * roughness) - 2, FLT_EPS, rcp(FLT_EPS));
}

//AntiAliasing,we use to calculate shadow
half StepAntiAliasing(half x, half y)
{
    half v = x - y;
    return saturate(v / (fwidth(v)+HALF_MIN));//fwidth(x) = abs(ddx(x) + ddy(x))
}

inline half StepFeatherToon(half value,half step,half feather)
{
    return saturate((value-step+feather)/feather);
}

#endif