#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct BRDFDataToon
{
    half3 diffuse;
    half3 specular;
    half perceptualRoughness;
    half roughness;
    half roughness2;
    half grazingTerm;
    half normalizationTerm;     // roughness * 4.0 + 2.0
    half roughness2MinusOne;    // roughness^2 - 1.0
#ifdef _HAIRSPECULAR
    half specularExponent;
#endif
};


inline void InitializeBRDFDataToon(SurfaceDataToon surfaceData, InputDataToon inputData, out BRDFDataToon outBRDFData)
{
#ifdef _SPECULAR_SETUP
    half reflectivity = ReflectivitySpecular(surfaceData.specular);
    half oneMinusReflectivity = 1.0h - reflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * (half3(1.0h, 1.0h, 1.0h) - surfaceData.specular); //Default PBR
    outBRDFData.diffuse = surfaceData.albedo; //Better result for toon shader
    outBRDFData.specular = surfaceData.specular;
#else
    half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
    half reflectivity = 1.0 - oneMinusReflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * oneMinusReflectivity;
    outBRDFData.diffuse = surfaceData.albedo; 
    outBRDFData.specular = lerp(kDieletricSpec.rgb, surfaceData.albedo,surfaceData.metallic);
#endif
    outBRDFData.grazingTerm = saturate(surfaceData.smoothness + reflectivity);
    outBRDFData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness);
    outBRDFData.roughness = max(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), HALF_MIN_SQRT);
    outBRDFData.roughness2 = max(outBRDFData.roughness * outBRDFData.roughness,HALF_MIN);
#ifdef _HAIRSPECULAR
    outBRDFData.specularExponent = RoughnessToBlinnPhongSpecularExponent(outBRDFData.roughness);
#endif
    outBRDFData.normalizationTerm = outBRDFData.roughness * 4.0h + 2.0h;
    outBRDFData.roughness2MinusOne = outBRDFData.roughness2 - 1.0h;
}

#ifdef _RIMLIGHT
half3 RimLight(BRDFDataToon brdfData,half3 normalWS,half3 viewDirectionWS,half3 lightDirectionWS)
{
    half fresnel = pow((1.0 - saturate(dot(normalWS, viewDirectionWS))),_RimPow);
    half LdotV = saturate(dot(lightDirectionWS,-viewDirectionWS));
    half NdotL = saturate(dot(normalWS,lightDirectionWS)); 
    fresnel *= saturate(LdotV+NdotL);
    half3 rimColor;
#ifdef _BLENDRIM
    rimColor = lerp(brdfData.diffuse,_RimColor,fresnel);
#else
    rimColor = _RimColor;
#endif
    fresnel = StepFeatherToon(fresnel,1,_RimStep,_RimFeather);
    return fresnel*rimColor;
}
#endif

#ifdef _HAIRSPECULAR
half2 DirectSpecularHairToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData,half3 normalWS,half3 lightDirectionWS,half3 viewDirectionWS,half3 bitangentWS)
{
    half3 t1 = ShiftTangent(bitangentWS,normalWS,surfaceData.specularShift1);
    half3 t2 = ShiftTangent(bitangentWS,normalWS,surfaceData.specularShift2);
    half LdotV = dot(lightDirectionWS,viewDirectionWS);
    float invLenLV = rsqrt(max(2.0 * LdotV + 2.0,FLT_EPS));
    half3 H = (lightDirectionWS+viewDirectionWS) * invLenLV;
    half spec1 = D_KajiyaKay(t1,H,brdfData.specularExponent);
    half spec2 = D_KajiyaKay(t2,H,brdfData.specularExponent);
    half maxSpec = (brdfData.specularExponent + 2) * rcp(2 * PI);
    half s1 = StepFeatherToon(spec1,maxSpec,_SpecularStep,_SpecularFeather);
    half s2 = StepFeatherToon(spec2,maxSpec,_SpecularStep,_SpecularFeather)*_Specular2Mul;
    return s1+s2;
}
#else
half DirectSpecularToon(BRDFDataToon brdfData,half3 normalWS,half3 lightDirectionWS,half3 viewDirectionWS,half step,half feather)
{
    float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));
    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(lightDirectionWS, halfDir));
    half LoH2 = LoH * LoH;
    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;
    half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);
    half maxSpecularTerm = 1.0h / ((brdfData.roughness2MinusOne + 1.00001f) * max(0.1h, LoH2) * brdfData.normalizationTerm);
    specularTerm = StepFeatherToon(specularTerm,maxSpecularTerm,step,feather);
    return specularTerm;
}
#endif  

half3 GlossyEnvironmentToon(half3 reflectVector, half perceptualRoughness, half occlusion)
{
#if !defined(_ENVIRONMENTREFLECTIONS_OFF)
    half mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
    half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip);
#if !defined(UNITY_USE_NATIVE_HDR)
    half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
#else
    half3 irradiance = encodedIrradiance.rbg;
#endif
    return irradiance * occlusion;
#else
    return _GlossyEnvironmentColor.rgb * occlusion;
#endif
}

half3 SpecularBDRFToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half3 bitangentWS, half radiance)
{
#ifndef _SPECULARHIGHLIGHTS_OFF
    #ifdef _HAIRSPECULAR
        half specularTerm = DirectSpecularHairToon(brdfData,surfaceData,normalWS,lightDirectionWS,viewDirectionWS,bitangentWS);
    #else
        half specularTerm = DirectSpecularToon(brdfData,normalWS,lightDirectionWS,viewDirectionWS,_SpecularStep,_SpecularFeather);
    #endif
//#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
//#endif
    half3 specularColor = specularTerm * brdfData.specular * radiance;
    return specularColor;
#else
    return 0;
#endif
}

half3 GlobalIlluminationToon(BRDFDataToon brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half fresnelTerm = Pow4(1.0 - saturate(dot(normalWS, viewDirectionWS)));
    half3 indirectDiffuse = bakedGI * occlusion * brdfData.diffuse;
    half3 reflection = GlossyEnvironmentToon(reflectVector, brdfData.perceptualRoughness, occlusion);
    float surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
    half3 indirectSpecular = surfaceReduction * reflection * lerp(brdfData.specular, brdfData.grazingTerm, fresnelTerm);
    return indirectDiffuse + indirectSpecular;
}

half3 DoubleShadowToon(SurfaceDataToon surfaceData,half3 BaseColor,half2 radiance)
{
    half3 finalColor = lerp(lerp(surfaceData.shadow2,surfaceData.shadow1,radiance.y),BaseColor,radiance.x);
    return finalColor;
}

#ifndef _DIFFUSERAMPMAP
half2 RadianceToon(SurfaceDataToon surfaceData, half3 normalWS, half3 lightDirectionWS,half lightAttenuation)
{
    half2 radiance;
    lightAttenuation = lerp(StepAntiAliasing(lightAttenuation,0.5),lightAttenuation,_Shadow1Feather);
    #ifdef _INSHADOWMAP
    lightAttenuation = saturate(lightAttenuation*surfaceData.inShadow);
    #endif
    half H_Lambert = 0.5*dot(normalWS, lightDirectionWS)+0.5;
    radiance.x = DiffuseRadianceToon(H_Lambert,_Shadow1Step,_Shadow1Feather)*lightAttenuation;
    radiance.y = DiffuseRadianceToon(H_Lambert,_Shadow2Step,_Shadow2Feather);
    return radiance;
}
#else
half3 RampRadianceToon(SurfaceDataToon surfaceData, half3 normalWS, half3 lightDirectionWS,half lightAttenuation)
{
    half3 radiance;
    lightAttenuation = StepAntiAliasing(lightAttenuation,0.5);
    #ifdef _INSHADOWMAP
    lightAttenuation = saturate(lightAttenuation*surfaceData.inShadow);
    #endif
    half H_Lambert = 0.5*dot(normalWS, lightDirectionWS)+0.5;
    radiance.xyz =  SAMPLE_TEXTURE2D_LOD(_DiffuseRampMap,sampler_LinearClamp,half2(H_Lambert,_DiffuseRampV),0).xyz*lightAttenuation;
    return radiance;
}
#endif

half3 LightingToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData,Light light, half3 normalWS, half3 viewDirectionWS,half3 bitangentWS)
{
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half3 color;
#ifdef _DIFFUSERAMPMAP
    half3 radiance = RampRadianceToon(surfaceData,normalWS, light.direction,lightAttenuation);
    half3 specularColor = SpecularBDRFToon(brdfData, surfaceData, normalWS, light.direction, viewDirectionWS, bitangentWS, radiance.x);
    half3 diffuseColor = radiance.xyz*brdfData.diffuse;
    color = specularColor+diffuseColor;
#else
    half2 radiance = RadianceToon(surfaceData,normalWS,light.direction,lightAttenuation);
    half3 specularColor = SpecularBDRFToon(brdfData, surfaceData, normalWS, light.direction, viewDirectionWS, bitangentWS,radiance.x);
    half3 diffuseColor = DoubleShadowToon(surfaceData,brdfData.diffuse,radiance);
    color = specularColor+diffuseColor;
#endif 
#ifdef _RIMLIGHT
    color += RimLight(brdfData,normalWS,viewDirectionWS,light.direction);
#endif
    return color*light.color;
}

half4 FragmentLitToon(InputDataToon inputData, SurfaceDataToon surfaceData)
{
    BRDFDataToon brdfData;
    InitializeBRDFDataToon(surfaceData, inputData, brdfData);
    half3 bitangentWS;
#ifdef _HAIRSPECULAR
    bitangentWS = inputData.bitangentWS;
#endif
    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
    #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
    #elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
    #else
    half4 shadowMask = half4(1, 1, 1, 1);
    #endif
    Light mainLight = GetMainLight(inputData.shadowCoord,inputData.positionWS,shadowMask);
#if defined(_SCREEN_SPACE_OCCLUSION)
    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
    mainLight.color *= aoFactor.directAmbientOcclusion;
    inputData.bakedGI *= aoFactor.indirectAmbientOcclusion;
#endif
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIlluminationToon(brdfData, inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS);
    color += LightingToon(brdfData, surfaceData,mainLight, inputData.normalWS, inputData.viewDirectionWS,bitangentWS);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
    #if defined(_SCREEN_SPACE_OCCLUSION)
        light.color *= aoFactor.directAmbientOcclusion;
    #endif
        color += LightingToon(brdfData, surfaceData,light, inputData.normalWS, inputData.viewDirectionWS, bitangentWS);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += surfaceData.emission;
    return half4(color, surfaceData.alpha);
}

#endif