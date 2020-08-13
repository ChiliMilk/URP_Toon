#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct BRDFDataToon
{
    half3 diffuse;
    half3 specular;
#ifdef _SHADEMAP
    half3 shade;
#endif
    half perceptualRoughness;
    half roughness;
    half roughness2;
    half grazingTerm;
    half normalizationTerm;     // roughness * 4.0 + 2.0
    half roughness2MinusOne;    // roughness^2 - 1.0
#ifdef _HAIRSPECULAR
    half specularExponent;
    half specularExponentSec;
#endif
};


inline void InitializeBRDFDataToon(SurfaceDataToon surfaceData, InputDataToon inputData, out BRDFDataToon outBRDFData)
{
#ifdef _SPECULAR_SETUP
    half reflectivity = ReflectivitySpecular(surfaceData.specular);
    half oneMinusReflectivity = 1.0h - reflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * (half3(1.0h, 1.0h, 1.0h) - surfaceData.specular); //Default PBR
    outBRDFData.diffuse = surfaceData.albedo; //Better result for toon shader
    #ifdef _SHADEMAP
    //outBRDFData.shade = surfaceData.shade * (half3(1.0h, 1.0h, 1.0h) - surfaceData.specular);
    outBRDFData.shade = surfaceData.shade;
    #endif
    outBRDFData.specular = surfaceData.specular;
#else
    half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);
    half reflectivity = 1.0 - oneMinusReflectivity;
    //outBRDFData.diffuse = surfaceData.albedo * oneMinusReflectivity;
    outBRDFData.diffuse = surfaceData.albedo; 
    #ifdef _SHADEMAP
    //outBRDFData.shade = surfaceData.shade * oneMinusReflectivity;
    outBRDFData.shade = surfaceData.shade;
    #endif
    outBRDFData.specular = lerp(kDieletricSpec.rgb, surfaceData.albedo,surfaceData.metallic);
#endif
    outBRDFData.grazingTerm = saturate(surfaceData.smoothness + reflectivity);
    outBRDFData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness);
    outBRDFData.roughness = max(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), HALF_MIN);
    outBRDFData.roughness2 = outBRDFData.roughness * outBRDFData.roughness;
#ifdef _HAIRSPECULAR
    outBRDFData.specularExponent = RoughnessToBlinnPhongSpecularExponent(outBRDFData.roughness);
    half perRoughnessSec = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness2); 
    half roughnessSec = max(PerceptualRoughnessToRoughness(perRoughnessSec),HALF_MIN);
    outBRDFData.specularExponentSec =  RoughnessToBlinnPhongSpecularExponent(roughnessSec);
#endif
    outBRDFData.normalizationTerm = outBRDFData.roughness * 4.0h + 2.0h;
    outBRDFData.roughness2MinusOne = outBRDFData.roughness2 - 1.0h;
#ifdef _ALPHAPREMULTIPLY_ON
    outBRDFData.diffuse *= surfaceData.alpha;
    surfaceData.alpha = surfaceData.alpha * oneMinusReflectivity + reflectivity;
#endif
}


half3 RimLight(BRDFDataToon brdfData,SurfaceDataToon surfaceData,half3 normalWS,half3 viewDirectionWS,half3 lightDirectionWS)
{
#ifdef _RIMLIGHT
    half fresnel = pow((1.0 - saturate(dot(normalWS, viewDirectionWS))),surfaceData.rimPow);
    half LdotV = saturate(dot(lightDirectionWS,-viewDirectionWS));
    half NdotL = saturate(dot(normalWS,lightDirectionWS)); 
    fresnel *= saturate(LdotV+NdotL);
#ifdef _BLENDRIM
    half3 rimColor = lerp(brdfData.diffuse,surfaceData.rimColor,fresnel);
#else
    half3 rimColor = surfaceData.rimColor;
#endif
    fresnel = SmoothstepToon(fresnel,1,surfaceData.rimStep,surfaceData.rimFeather);
    return fresnel*rimColor;
#else
    return 0.0h;
#endif
}

half DirectSpecularToon(BRDFDataToon brdfData,half3 normalWS,half3 lightDirectionWS,half3 viewDirectionWS,half step,half feather)
{
    float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));
    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(lightDirectionWS, halfDir));
    half LoH2 = LoH * LoH;
    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;
    half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);
    float maxD = brdfData.roughness2MinusOne + 1.00001f;
    half maxSpecularTerm = brdfData.roughness2 / ((maxD * maxD) * brdfData.normalizationTerm);
    specularTerm = SmoothstepToon(specularTerm,maxSpecularTerm,step,feather);
    return specularTerm;
}

#ifdef _HAIRSPECULAR
half2 DirectSpecularHairToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData,half3 normalWS,half3 lightDirectionWS,half3 viewDirectionWS,half3 bitangentWS)
{
    half3 t1 = ShiftTangent(bitangentWS,normalWS,surfaceData.specularShift1);
    half3 t2 = ShiftTangent(bitangentWS,normalWS,surfaceData.specularShift2);
    half LdotV = dot(lightDirectionWS,viewDirectionWS);
    float invLenLV = rsqrt(max(2.0 * LdotV + 2.0,FLT_EPS));
    half3 H = (lightDirectionWS+viewDirectionWS) * invLenLV;
    half spec1 = D_KajiyaKay(t1,H,brdfData.specularExponent);
    half spec2 = D_KajiyaKay(t2,H,brdfData.specularExponentSec);
    half maxSpec1 = (brdfData.specularExponent + 2) * rcp(2 * PI);
    half maxSpec2 = (brdfData.specularExponentSec+2) * rcp(2*PI);
    half S1 = SmoothstepToon(spec1,maxSpec1,surfaceData.specularStep,surfaceData.specularFeather);
    half S2 = SmoothstepToon(spec2,maxSpec2,surfaceData.specularStep,surfaceData.specularFeather)*surfaceData.specular2Mul;
    return S1+S2;
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

half3 DirectBDRFToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half3 bitangentWS, half3 lightColor, half4 radiance)
{
#ifndef _SPECULARHIGHLIGHTS_OFF
    #ifdef _HAIRSPECULAR
        half specularTerm = DirectSpecularHairToon(brdfData,surfaceData,normalWS,lightDirectionWS,viewDirectionWS,bitangentWS);
    #else
        half specularTerm = DirectSpecularToon(brdfData,normalWS,lightDirectionWS,viewDirectionWS,surfaceData.specularStep,surfaceData.specularFeather);
    #endif
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
    half3 color = (specularTerm * brdfData.specular * radiance.w + brdfData.diffuse * radiance.xyz)*lightColor;
    return color;
#else
    half3 color = brdfData.diffuse * radiance.xyz * lightColor;
    return color;
#endif
}

half3 GlobalIlluminationToon(BRDFDataToon brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half fresnelTerm = Pow4(1.0 - saturate(dot(normalWS, viewDirectionWS)));
#ifdef _SHADEMAP
    half3 indirectDiffuse = bakedGI * occlusion * brdfData.shade;
#else
    half3 indirectDiffuse = bakedGI * occlusion * brdfData.diffuse;
#endif
    half3 reflection = GlossyEnvironmentToon(reflectVector, brdfData.perceptualRoughness, occlusion);
    float surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
    half3 indirectSpecular = surfaceReduction * reflection * lerp(brdfData.specular, brdfData.grazingTerm, fresnelTerm);
    return indirectDiffuse + indirectSpecular;
}

half4 RadianceToon(SurfaceDataToon surfaceData, half3 normalWS, half3 lightDirectionWS,half lightAttenuation)
{
    half4 radiance;
#ifdef _DIFFUSERAMPMAP
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    radiance.xyz = SAMPLE_TEXTURE2D_LOD(_DiffuseRampMap,sampler_LinearClamp,half2(NdotL,surfaceData.diffuseRampV),0).xyz;
#else
    half NdotL = dot(normalWS, lightDirectionWS);
    NdotL = SmoothstepDiffuseToon(NdotL,surfaceData.shadowStep,surfaceData.shadowFeather);
    radiance.xyz = NdotL;
#endif
#ifdef _INSHADOWMAP
    lightAttenuation = saturate(lightAttenuation*surfaceData.inShadow);
#endif
    radiance.w = radiance.x;
#ifdef _DIFFUSERAMPMAP
    radiance.xyz = saturate(lightAttenuation+surfaceData.shadowMinus)*radiance.xyz;
#else
    lightAttenuation = lerp(lightAttenuation,StepAntiAliasing(lightAttenuation,0.5),saturate(1-surfaceData.shadowFeather));
    radiance.xyz = saturate((lightAttenuation*radiance.xyz)+surfaceData.shadowMinus);
#endif
    radiance.w*=lightAttenuation;
    return radiance;
}

half3 LightingToon(BRDFDataToon brdfData, SurfaceDataToon surfaceData,Light light, half3 normalWS, half3 viewDirectionWS,half3 bitangentWS)
{
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half4 radiance = RadianceToon(surfaceData,normalWS, light.direction,lightAttenuation);
    half3 color = DirectBDRFToon(brdfData, surfaceData,normalWS, light.direction, viewDirectionWS, bitangentWS, light.color, radiance);
#ifdef _RIMLIGHT
    half3 rimColor = RimLight(brdfData,surfaceData,normalWS,viewDirectionWS,light.direction);
    color += rimColor;
#endif
    return color;
}

half4 FragmentLitToon(InputDataToon inputData, SurfaceDataToon surfaceData)
{
    BRDFDataToon brdfData;
    InitializeBRDFDataToon(surfaceData, inputData, brdfData);
    half3 bitangentWS;
#ifdef _HAIRSPECULAR
    bitangentWS = inputData.bitangentWS;
#endif
    Light mainLight = GetMainLight(inputData.shadowCoord);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

    half3 color = GlobalIlluminationToon(brdfData, inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS);
    color += LightingToon(brdfData, surfaceData,mainLight, inputData.normalWS, inputData.viewDirectionWS,bitangentWS);

#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
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