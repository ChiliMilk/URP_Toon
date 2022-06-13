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
#if defined _HAIRSPECULAR || defined _HAIRSPECULARVIEWNORMAL
    half specularExponent;
#endif
};

inline void InitializeBRDFDataToon(SurfaceDataToon surfaceData, out BRDFDataToon outBRDFData)
{
#ifdef _SPECULAR_SETUP
    half reflectivity = ReflectivitySpecular(surfaceData.specular);
    half oneMinusReflectivity = 1.0h - reflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * (half3(1.0h, 1.0h, 1.0h) - surfaceData.specular); //Default PBR
    outBRDFData.diffuse = surfaceData.albedo;
    outBRDFData.specular = surfaceData.specular;
#else
    half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
    half reflectivity = 1.0 - oneMinusReflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * oneMinusReflectivity;
    outBRDFData.diffuse = surfaceData.albedo;
    outBRDFData.specular = lerp(kDieletricSpec.rgb, surfaceData.albedo, surfaceData.metallic);
#endif
    outBRDFData.grazingTerm = saturate(surfaceData.smoothness + reflectivity);
    outBRDFData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness);
    outBRDFData.roughness = max(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), HALF_MIN_SQRT);
    outBRDFData.roughness2 = max(outBRDFData.roughness * outBRDFData.roughness, HALF_MIN);
#if defined _HAIRSPECULAR || defined _HAIRSPECULARVIEWNORMAL
    outBRDFData.specularExponent = RoughnessToBlinnPhongSpecularExponent(outBRDFData.roughness);
#endif
    outBRDFData.normalizationTerm = outBRDFData.roughness * 4.0h + 2.0h;
    outBRDFData.roughness2MinusOne = outBRDFData.roughness2 - 1.0h;
}

#ifdef _HAIRSPECULAR
half DirectSpecularHairToon(half specularExponent, half specularShift, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half3 bitangentWS,half specularStep, half specularFeather)
{
    half3 t = ShiftTangent(bitangentWS,normalWS,specularShift);
    half LdotV = dot(lightDirectionWS,viewDirectionWS);
    float invLenLV = rsqrt(max(2.0 * LdotV + 2.0,FLT_EPS));
    half3 H = (lightDirectionWS+viewDirectionWS) * invLenLV;
    half spec = D_KajiyaKay(t,H,specularExponent);

    half normalizeSpec = spec * rcp(specularExponent + 2) * 2 * PI;
    spec *= StepFeatherToon(normalizeSpec,specularStep,specularFeather);

    return spec;
}
#elif defined _HAIRSPECULARVIEWNORMAL
half DirectSpecularHairViewNormalToon(half specularExponent, half3 normalWS, half3 viewDirectionWS, half specularStep, half specularFeather)
{
    half NdotV = saturate(dot(normalize(normalWS.xz), normalize(viewDirectionWS.xz)));
    half spec = pow(NdotV, specularExponent);

    return StepFeatherToon(spec, specularStep, specularFeather);
}
#else
half DirectSpecularToon(BRDFDataToon brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half step, half feather)
{
    float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));
    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(lightDirectionWS, halfDir));
    half LoH2 = LoH * LoH;
    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;
    half d2 = half(d * d);
    
    half specularTerm = brdfData.roughness2 / (d2 * max(0.1h, LoH2) * brdfData.normalizationTerm);
    half normalizeSpec = brdfData.roughness2 * brdfData.roughness2 * rcp(d2);
    
    specularTerm *= StepFeatherToon(normalizeSpec, step, feather);
    return specularTerm;
}
#endif  

half3 SpecularBDRFToon(BRDFDataToon brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half3 bitangentWS, half specularShift, half specularStep, half specularFeather)
{
#ifndef _SPECULARHIGHLIGHTS_OFF
    #ifdef _HAIRSPECULAR
        half specularTerm = DirectSpecularHairToon(brdfData.specularExponent,specularShift,normalWS,lightDirectionWS,viewDirectionWS,bitangentWS,specularStep,specularFeather);
    #elif defined _HAIRSPECULARVIEWNORMAL
    half specularTerm = DirectSpecularHairViewNormalToon(brdfData.specularExponent, normalWS, viewDirectionWS, specularStep, specularFeather);
    #else
    half specularTerm = DirectSpecularToon(brdfData, normalWS, lightDirectionWS, viewDirectionWS, specularStep, specularFeather);
    #endif
//#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
//#endif
    half3 specularColor = specularTerm * brdfData.specular;
    return specularColor;
#else
    return 0;
#endif
}

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

half HairShadowMaskAtten(half hairShadowMask, half H_Lambert)
{
    half hairAtten = lerp(1, hairShadowMask, H_Lambert);
    return hairAtten;
}

#ifdef _SDFSHADOWMAP
half SDFShadowRadiance(half sdfShadowMask, half2 LdotF, half3 lightDirectionWS, half lightAttenuation, half inShadow, half hairShadowMask, half shadowStep, half shadowFeather)
{    
    half radiance = 1 - saturate( (1 - sdfShadowMask - LdotF - (shadowStep * 2 - 1)) / shadowFeather + 1);
    lightAttenuation = lerp(StepAntiAliasing(lightAttenuation,0.5),lightAttenuation,shadowFeather);
    lightAttenuation = saturate(lightAttenuation * inShadow);
    radiance *= lightAttenuation * HairShadowMaskAtten(hairShadowMask,radiance);
    
    return radiance;
}
#else

half3 DoubleShadowToon(half3 shadow1, half3 shadow2, half3 baseColor, half2 radiance)
{
    half3 finalColor = lerp(lerp(shadow2,shadow1,radiance.y),baseColor,radiance.x);
    return finalColor;
}

#ifndef _DIFFUSERAMPMAP
half2 RadianceToon(half3 normalWS, half3 lightDirectionWS, half lightAttenuation, half inShadow, half hairShadowMask, half shadow1Step, half shadow1Feather, half shadow2Step, half shadow2Feather)
{
    half2 radiance;
    lightAttenuation = lerp(StepAntiAliasing(lightAttenuation,0.5),lightAttenuation,shadow1Feather);
    lightAttenuation = saturate(lightAttenuation * inShadow);
    half H_Lambert = 0.5 * dot(normalWS, lightDirectionWS) + 0.5;
    radiance.x = StepFeatherToon(H_Lambert, shadow1Step, shadow1Feather) * lightAttenuation * HairShadowMaskAtten(hairShadowMask, H_Lambert);
    radiance.y = StepFeatherToon(H_Lambert,shadow2Step,shadow2Feather);
    return radiance;
}
#else
half3 RampRadianceToon(half3 normalWS, half3 lightDirectionWS, half lightAttenuation, half inShadow, half hairShadowMask)
{
    half3 radiance;
    lightAttenuation = StepAntiAliasing(lightAttenuation,0.5);
    lightAttenuation = saturate(lightAttenuation * inShadow);
    half H_Lambert = 0.5 * dot(normalWS, lightDirectionWS) + 0.5;
    radiance.xyz =  SampleRampMap(H_Lambert) * lightAttenuation * HairShadowMaskAtten(hairShadowMask,H_Lambert);
    return radiance;
}
#endif
#endif

float3 RimLight(half3 rimColor, half3 normalWS, half3 viewDirectionWS, half3 lightDirectionWS, half rimStep, half rimFeather, half rimBlendShadow, half rimBlendLdotV, half rimFlip, half radiance)
{
    half LdotV = dot(-lightDirectionWS, viewDirectionWS) * 0.5 + 0.5;

    half fresnel = (1.0 - saturate(dot(normalWS, viewDirectionWS)));
    fresnel = StepFeatherToon(fresnel, rimStep, rimFeather);
    fresnel = lerp(fresnel, fresnel * LdotV, rimBlendLdotV);
    half3 color = rimColor * fresnel;
    radiance = lerp(radiance, 1 - radiance, rimFlip);
    color = lerp(color, color * radiance, rimBlendShadow);

    return  color;
}

half3 MatCapLight(half3 normalWS, half3 viewDirection, half radiance)
{
#ifdef _MATCAP
    half3 ViewNormal = TransformWorldToViewDir(normalWS);
    half2 ViewNormalAsMatCapUV = ViewNormal * 0.5 + 0.5;
    half3 color = SampleMatCap(ViewNormalAsMatCapUV);
    
    return color * radiance;
#else
    return 0;
#endif
}


half3 LightingToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData, InputDataToon inputData, ToonData toonData, Light light, half isMainLight)
{
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half3 color = half3(0.0h,0.0h,0.0h);

#ifdef _SDFSHADOWMAP
    half radiance = SDFShadowRadiance(toonData.sdfShadowMask,toonData.LdotF,light.direction,lightAttenuation,toonData.inShadow,toonData.hairShadowMask,toonData.shadow1Step,toonData.shadow1Feather);
    half3 specularColor = SpecularBDRFToon(brdfData, inputData.normalWS, light.direction, inputData.viewDirectionWS, inputData.bitangentWS, toonData.specularShift, toonData.specularStep,toonData.specularFeather) * radiance;
    half3 diffuseColor = lerp(toonData.shadow1,brdfData.diffuse,radiance);
    color = diffuseColor + specularColor;
#else
#ifdef _DIFFUSERAMPMAP
    half3 radiance = RampRadianceToon(inputData.normalWS, light.direction,lightAttenuation,toonData.inShadow,toonData.hairShadowMask);
    half3 specularColor = SpecularBDRFToon(brdfData, inputData.normalWS, light.direction, inputData.viewDirectionWS, inputData.bitangentWS, toonData.specularShift, toonData.specularStep,toonData.specularFeather) * radiance;
    half3 diffuseColor = radiance.xyz * brdfData.diffuse;
    color = specularColor + diffuseColor;
#else
    half2 radiance = RadianceToon(inputData.normalWS,light.direction,lightAttenuation,toonData.inShadow,toonData.hairShadowMask,toonData.shadow1Step,toonData.shadow1Feather,toonData.shadow2Step,toonData.shadow2Feather);
    half3 specularColor = SpecularBDRFToon(brdfData, inputData.normalWS, light.direction, inputData.viewDirectionWS, inputData.bitangentWS, toonData.specularShift, toonData.specularStep, toonData.specularFeather) * radiance.x;
    half3 diffuseColor = DoubleShadowToon(toonData.shadow1,toonData.shadow2,brdfData.diffuse,radiance);
    color +=  specularColor + diffuseColor;
#endif
#endif
    color += isMainLight * RimLight(toonData.rimColor, inputData.normalWS, inputData.viewDirectionWS, light.direction, toonData.rimStep, toonData.rimFeather, toonData.rimBlendShadow, toonData.rimBlendLdotV,toonData.rimFlip ,radiance.x);
    color += isMainLight * MatCapLight(inputData.normalWS, inputData.viewDirectionWS, radiance.x);
    return color * light.color;
}

half4 FragmentLitToon(InputDataToon inputData, SurfaceDataToon surfaceData, ToonData toonData)
{
    BRDFDataToon brdfData;
    InitializeBRDFDataToon(surfaceData, brdfData);

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
    #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
    #elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
    #else
    half4 shadowMask = half4(1, 1, 1, 1);
    #endif
   
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    Light mainLight = GetMainLight(inputData.shadowCoord,inputData.positionWS,shadowMask);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIlluminationToon(brdfData, inputData.bakedGI, toonData.ssao, inputData.normalWS, inputData.viewDirectionWS);
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        color += LightingToon(brdfData, surfaceData, inputData, toonData, mainLight, 1);
    }

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
        if(IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            color += LightingToon(brdfData, surfaceData, inputData, toonData, light, 0);
        }
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += surfaceData.emission;
    return half4(color, surfaceData.alpha);
}

#endif