﻿#ifndef UNIVERSAL_TOON_META_PASS_INCLUDED
#define UNIVERSAL_TOON_META_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 uv0          : TEXCOORD0;
    float2 uv1          : TEXCOORD1;
    float2 uv2          : TEXCOORD2;
    #ifdef _TANGENT_TO_WORLD
    float4 tangentOS     : TANGENT;
    #endif
};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
};

Varyings ToonVertexMeta(Attributes input)
{
    Varyings output;
    output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
    output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
    return output;
}

half4 ToonFragmentMeta(Varyings input) : SV_Target
{
    SurfaceDataToon surfaceData;
    InitializeSurfaceDataToon(input.uv, surfaceData);

    BRDFDataToon brdfData;
    InitializeBRDFDataToon(surfaceData, brdfData);

    MetaInput metaInput;
    metaInput.Albedo = brdfData.diffuse;
    metaInput.SpecularColor = surfaceData.specular;
    metaInput.Emission = surfaceData.emission;

    return MetaFragment(metaInput);
}

#endif