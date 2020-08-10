#ifndef TOON_PROPERTY_INCLUDED
#define TOON_PROPERTY_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _SpecColor;
half4 _EmissionColor;
half _Cutoff;
half _Smoothness;
half _Metallic;
half _BumpScale;
float4 _SpecularShiftMap_ST;
half _SpecularShiftIntensity;
half _SpecularShift1Add;
half _Smoothness2Mul;
half _SpecularShift2Add;
half _Specular2Mul;
half _DiffuseRampV;
half _OcclusionStrength;
half _ShadowMinus;
half _ShadowStep;
half _ShadowFeather;
half _SpecularStep;
half _SpecularFeather;
half _InShadowMapStrength;
half3 _RimColor;
half _RimPow;
half _RimStep;
half _RimFeather;
half _OutlineWidth;
half4 _OutlineColor;
CBUFFER_END

TEXTURE2D(_ClipMask);   SAMPLER(sampler_ClipMask);
#ifdef _INSHADOWMAP
TEXTURE2D(_InShadowMap);  SAMPLER(sampler_InShadowMap);
#endif
#ifdef _SHADEMAP
TEXTURE2D(_ShadeMap);   SAMPLER(sampler_ShadeMap);
#endif
TEXTURE2D(_OcclusionMap);   SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_SpecGlossMap);   SAMPLER(sampler_SpecGlossMap);
TEXTURE2D(_SmoothnessMap);  SAMPLER(sampler_SmoothnessMap);
#ifdef _SPECULARSHIFTMAP
TEXTURE2D(_SpecularShiftMap);   SAMPLER(sampler_SpecularShiftMap);
#endif
#ifdef _DIFFUSERAMPMAP
TEXTURE2D( _DiffuseRampMap);  SAMPLER(sampler_DiffuseRampMap);
#endif

#ifdef _SPECULAR_SETUP
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
    #define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

struct SurfaceDataToon
{
    half3 albedo;
    half3 specular;
    half  metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;
    //Specular
    half specularStep;
    half specularFeather;
    #ifdef _HAIRSPECULAR
        half specularShift1;
        half specularShift2;
        half specular2Mul;
        half smoothness2;
    #endif
    //Shadow
    #ifdef _INSHADOWMAP
        half inShadow;
    #endif
    #ifdef _SHADEMAP
        half3 shade;
    #endif
    //Ramp
    #ifdef _DIFFUSERAMPMAP
    half diffuseRampV;
    half shadowMinus;
    #else
    half shadowMinus;
    half shadowStep;
    half shadowFeather;
    #endif
    //rim
    #ifdef _RIMLIGHT
    half3 rimColor;
    half rimPow;
    half rimStep;
    half rimFeather;
    #endif
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
#ifdef _HAIRSPECULAR
    half3   tangentWS;
    half3   bitangentWS;
#endif
};

half SampleClipMask(float2 uv)
{
#ifdef _INVERSECLIPMASK
    return 1.0h-SAMPLE_TEXTURE2D(_ClipMask,sampler_ClipMask,uv).r;
#else
     return SAMPLE_TEXTURE2D(_ClipMask,sampler_ClipMask,uv).r;
#endif
}

half SampleSmoothness(float2 uv,half smoothness)
{
    return SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_SmoothnessMap, uv).r*smoothness;
}

half SampleShadow(float2 uv)
{
#ifdef _INSHADOWMAP
    half inShadow = SAMPLE_TEXTURE2D(_InShadowMap,sampler_InShadowMap,uv).r*_InShadowMapStrength;
    return inShadow;
#else
    return 0.0;
#endif
}

half3 SampleShade(float2 uv)
{
#ifdef _SHADEMAP
    half3 Shade = SAMPLE_TEXTURE2D(_ShadeMap,sampler_ShadeMap,uv);
    return Shade;
#else
    return 0.0;
#endif
}
half SampleSpecularShift(float2 uv,half shiftAdd)
{
#ifdef _SPECULARSHIFTMAP
    half specularShift = SAMPLE_TEXTURE2D(_SpecularShiftMap,sampler_SpecularShiftMap,uv).r*_SpecularShiftIntensity+shiftAdd;
    return specularShift;
#else
    return _SpecularShiftIntensity+shiftAdd;
#endif
}

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha,half smoothness)
{
    half4 specGloss;
#ifdef _METALLICSPECGLOSSMAP
    specGloss = SAMPLE_METALLICSPECULAR(uv);
    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        specGloss.a = albedoAlpha * smoothness;
    #else
        specGloss.a *= smoothness;
    #endif
#else
    #if _SPECULAR_SETUP
        specGloss.rgb = _SpecColor.rgb;
    #else
        specGloss.rgb = _Metallic.rrr;
    #endif

    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        specGloss.a = albedoAlpha * smoothness;
    #else
        specGloss.a = smoothness;
    #endif
#endif
    return specGloss;
}

half SampleOcclusion(float2 uv)
{
#ifdef _OCCLUSIONMAP
// TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
#if defined(SHADER_API_GLES)
    return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
#else
    half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
    return LerpWhiteTo(occ, _OcclusionStrength);
#endif
#else
    return 1.0;
#endif
}

//Lit Meta Use
inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a*SampleClipMask(uv), _BaseColor, _Cutoff);

    half smoothness = SampleSmoothness(uv,_Smoothness);
    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a,smoothness);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

inline void InitializeSurfaceDataToon(float2 uv,float2 uv2,out SurfaceDataToon outSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a*SampleClipMask(uv), _BaseColor, _Cutoff);
    half smoothness = SampleSmoothness(uv,_Smoothness);
    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a,smoothness);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
#ifdef _HAIRSPECULAR
    outSurfaceData.specularShift1 = SampleSpecularShift(uv2,_SpecularShift1Add);
    outSurfaceData.specularShift2 = SampleSpecularShift(uv2,_SpecularShift2Add);
    outSurfaceData.specular2Mul = _Specular2Mul;
    outSurfaceData.smoothness2 = outSurfaceData.smoothness * _Smoothness2Mul;
#endif
#ifdef _INSHADOWMAP
    outSurfaceData.inShadow = SampleShadow(uv);
#endif
#ifdef _SHADEMAP
    outSurfaceData.shade = SampleShade(uv)*_BaseColor.rgb;
#endif
    outSurfaceData.specularStep = _SpecularStep;
    outSurfaceData.specularFeather = _SpecularFeather;
#ifdef _RIMLIGHT
    outSurfaceData.rimColor = _RimColor;
    outSurfaceData.rimPow = _RimPow;
    outSurfaceData.rimStep = _RimStep;
    outSurfaceData.rimFeather = _RimFeather;
#endif
#ifdef _DIFFUSERAMPMAP
    outSurfaceData.diffuseRampV = _DiffuseRampV;
    outSurfaceData.shadowMinus = _ShadowMinus;
#else
    outSurfaceData.shadowMinus = _ShadowMinus;
    outSurfaceData.shadowStep = _ShadowStep;
    outSurfaceData.shadowFeather = _ShadowFeather;
#endif
}
#endif