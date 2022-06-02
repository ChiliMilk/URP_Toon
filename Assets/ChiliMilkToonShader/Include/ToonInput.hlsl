#ifndef TOON_INPUT_INCLUDED
#define TOON_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
TEXTURE2D(_BumpMap);     
TEXTURE2D(_EmissionMap);

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half3 _Shadow1Color;
half _Shadow1Step;
half _Shadow1Feather;
half _Shadow2Step;
half _Shadow2Feather;
half3 _Shadow2Color;
half _DiffuseRampV;
half _InShadowMapStrength;
half _SSAOStrength;
half _ReceiveHairShadowOffset;

half4 _EmissionColor;
half _OcclusionStrength;
half _Cutoff;
half _BumpScale;

half _Smoothness;
half _Metallic;
half4 _SpecColor;
half _SpecularStep;
half _SpecularFeather;
half _SpecularShift;
half _SpecularShiftIntensity;
float4 _SpecularShiftMap_ST;

half _RimBlendShadow;
half _RimBlendLdotV;
half3 _RimColor;
half _RimFlip;
half _RimStep;
half _RimFeather;

half _OutlineWidth;
half4 _OutlineColor;

half2 _LdotFL;
float4 _SDFShadowMap_TexelSize;

half3 _MatCapColor;
half _MatCapUVScale;
CBUFFER_END

TEXTURE2D(_ClipMask);
TEXTURE2D(_InShadowMap);
TEXTURE2D(_OcclusionMap);   
TEXTURE2D(_MetallicGlossMap);  
TEXTURE2D(_SpecGlossMap); 

#ifdef _SDFSHADOWMAP
TEXTURE2D(_SDFShadowMap);   SAMPLER(sampler_SDFShadowMap);
#endif

#ifdef _HAIRSPECULAR
TEXTURE2D(_SpecularShiftMap);   SAMPLER(sampler_SpecularShiftMap);
#endif

#ifdef _DIFFUSERAMPMAP
TEXTURE2D( _DiffuseRampMap);  SAMPLER(sampler_LinearClamp);
#endif

#if defined(_RECEIVE_HAIRSHADOWMASK) && defined(_HAIRSHADOWMASK)
    TEXTURE2D(_HairShadowMask);   SAMPLER(sampler_HairShadowMask);
#endif

#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_BaseMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_BaseMap, uv)
#endif

#ifdef _MATCAP
TEXTURE2D(_MatCapMap);   SAMPLER(sampler_MatCapMap_Linear_Clamp);
#endif

struct SurfaceDataToon
{
    half3 albedo;
    half3 specular;
    half metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;
};

struct InputDataToon
{
    float3  positionWS;
    half3   normalWS;
    half3   viewDirectionWS;
    float4  shadowCoord;
    half    fogCoord;
    half3   vertexLighting;
    half3   bakedGI;
    float2  normalizedScreenSpaceUV;
    half4   shadowMask;
    half3   tangentWS;
    half3   bitangentWS;
    float depth;
};

struct ToonData
{
    half specularShift;
    //Shadow
#ifndef _DIFFUSERAMPMAP
    half3 shadow1;
    half shadow1Step;
    half shadow1Feather;
    half3 shadow2;
    half shadow2Step;
    half shadow2Feather;
#endif
    half inShadow;
#ifdef _SDFSHADOWMAP
    half sdfShadowMask;
    half LdotF;
#endif
    half hairShadowMask;
    half ssao;
    
    half specularStep;
    half specularFeather;
    
    half3 rimColor;
    half rimFlip;
    half rimStep;
    half rimFeather;
    half rimBlendShadow;
    half rimBlendLdotV;
};

#ifdef _MATCAP
half3 SampleMatCap(half2 uv)
{
    return SAMPLE_TEXTURE2D(_MatCapMap, sampler_MatCapMap_Linear_Clamp, (uv - _MatCapUVScale) / (1 - 2 * _MatCapUVScale)).rgb * _MatCapColor;
}
#endif

half SampleAmbientOcclusionToon(float2 normalizedScreenSpaceUV, half occlusion)
{
    half ssao = 1.0;
#if defined(_SCREEN_SPACE_OCCLUSION)
    ssao = SampleAmbientOcclusion(normalizedScreenSpaceUV);
    ssao = StepFeatherToon(ssao, _Shadow1Step, _Shadow1Feather);
    ssao = lerp(1, ssao, _SSAOStrength);
#endif
    ssao = min(ssao, occlusion);
    return ssao;
}


half SampleHairShadowMask(float2 normalizedScreenSpaceUV, float depth)
{
#if defined(_RECEIVE_HAIRSHADOWMASK) && defined(_HAIRSHADOWMASK)
    half3 lightDirectionWS = GetMainLight().direction;
    half3 lightDirectionVS = TransformWorldToViewDir(lightDirectionWS,true);
    float2 samplerUV = normalizedScreenSpaceUV + lightDirectionVS.xy * min(depth,0.01) * _ReceiveHairShadowOffset;
    float hairDepth = SAMPLE_TEXTURE2D(_HairShadowMask,sampler_HairShadowMask,samplerUV);
    half hairMask = step(hairDepth,depth);
    return hairMask;
#else
    return 1.0;
#endif
    
}

half SampleSDFShadowMask(float2 uv)
{
#ifdef _SDFSHADOWMAP
    uv.x *= step(0,_LdotFL.y) * 2 - 1;
    float mask = SAMPLE_TEXTURE2D(_SDFShadowMap, sampler_SDFShadowMap,uv);
    return mask;
#else
    return 1.0;
#endif
}

#ifdef _DIFFUSERAMPMAP
half3 SampleRampMap(half H_Lambert)
{
    return  SAMPLE_TEXTURE2D(_DiffuseRampMap,sampler_LinearClamp,half2(H_Lambert,_DiffuseRampV)).xyz;
}
#endif

half SampleClipMask(float2 uv)
{
#ifdef _ALPHATEST_ON
#ifdef _INVERSECLIPMASK
    return 1.0h - SAMPLE_TEXTURE2D(_ClipMask,sampler_BaseMap,uv).r;
#else
     return SAMPLE_TEXTURE2D(_ClipMask,sampler_BaseMap,uv).r;
#endif
#else
    return 1.0;
#endif
}

half Alpha(half albedoAlpha, half4 color, half cutoff)
{
    half alpha = color.a * albedoAlpha;
#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    half4 color = SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
    color.a *= SampleClipMask(uv);
    return color;
}

half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_BaseMap), half scale = 1.0h)
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_BaseMap, uv);
    #if BUMP_SCALE_NOT_SUPPORTED
        return UnpackNormal(n);
    #else
        return UnpackNormalScale(n, scale);
    #endif
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_PARAM(emissionMap, sampler_BaseMap))
{
#ifndef _EMISSION
    return 0;
#else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_BaseMap, uv).rgb * emissionColor;
#endif
}

half SampleInShadow(float2 uv)
{
    half inShadow = (1 - SAMPLE_TEXTURE2D(_InShadowMap,sampler_BaseMap,uv).r) * _InShadowMapStrength;
    return 1 - inShadow;
}

half SampleSpecularShift(float2 uv)
{
#ifdef _HAIRSPECULAR
    half specularShift = SAMPLE_TEXTURE2D(_SpecularShiftMap,sampler_SpecularShiftMap,uv*_SpecularShiftMap_ST.xy + _SpecularShiftMap_ST.zw).r*_SpecularShiftIntensity+_SpecularShift;
    return specularShift;
#else
    return 0.0;
#endif
}

half4 SampleMetallicSpecGloss(float2 uv, half smoothness)
{
    half4 specGloss = SAMPLE_METALLICSPECULAR(uv);
#if _SPECULAR_SETUP
    specGloss.rgb *= _SpecColor.rgb;
#else
    specGloss.rgb *= _Metallic.rrr;
#endif
    specGloss.a *= smoothness;
    return specGloss;
}

half SampleOcclusion(float2 uv)
{
#ifdef _OCCLUSIONMAP
// TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
#if defined(SHADER_API_GLES)
    return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_BaseMap, uv).g;
#else
    half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_BaseMap, uv).g;
    return LerpWhiteTo(occ, _OcclusionStrength);
#endif
#else
    return 1.0;
#endif
}

inline void InitializeSurfaceDataToon(float2 uv,out SurfaceDataToon outSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    half4 specGloss = SampleMetallicSpecGloss(uv, _Smoothness);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif
    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BaseMap), _BumpScale);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_BaseMap));
}

inline void InitializeToonData(float2 uv, float2 normalizedScreenSpaceUV,float3 albedo, half occlusion, float depth, out ToonData outToonData)
{
    outToonData.inShadow = SampleInShadow(uv);
#ifndef _DIFFUSERAMPMAP
    outToonData.shadow1 = albedo * _Shadow1Color;
    outToonData.shadow1Step = _Shadow1Step;
    outToonData.shadow1Feather = _Shadow1Feather;
    outToonData.shadow2 = albedo * _Shadow2Color;
    outToonData.shadow2Step = _Shadow2Step;
    outToonData.shadow2Feather = _Shadow2Feather;
#endif
    outToonData.ssao = SampleAmbientOcclusionToon(normalizedScreenSpaceUV, occlusion);
#ifdef _SDFSHADOWMAP
    outToonData.sdfShadowMask = SampleSDFShadowMask(uv);
    outToonData.LdotF = saturate(_LdotFL.x);
#endif
    outToonData.hairShadowMask = SampleHairShadowMask(normalizedScreenSpaceUV, depth);
    outToonData.specularStep = _SpecularStep;
    outToonData.specularFeather = _SpecularFeather;
    outToonData.specularShift = SampleSpecularShift(uv);
    
    outToonData.rimColor = _RimColor;
    outToonData.rimStep = _RimStep;
    outToonData.rimFeather = _RimFeather;
    outToonData.rimBlendShadow = _RimBlendShadow;
    outToonData.rimFlip = _RimFlip;
    outToonData.rimBlendLdotV = _RimBlendLdotV;
}
#endif