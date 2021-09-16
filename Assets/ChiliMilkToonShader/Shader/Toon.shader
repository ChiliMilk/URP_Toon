Shader "ChiliMilk/Toon"
{
    Properties
    {
        // Surface Options
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
		[HideInInspector][ToggleOff]_AlphaClip("__clip", Float) = 0.0
        [ToggleOff] _InverseClipMask("_InverseClipMask",Float) = 0.0
        _ClipMask("ClipMask",2D) = "white"{}
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _SurfaceType("__surface", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0

        //Stencil
        [HideInInspector]_StencilType("StencilType",Float) = 0
        [ToggleOff]_EnableStencil("EnableStencil",Float) = 0 
        _StencilChannel ("Stencil Channel", int) =1
        [HideInInspector]_StencilComp("Stencil Comp",int) = 0
        [HideInInspector]_StencilOp ("Stencil Op",int) = 0

        // Default 
        _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        //Shadow
        _BaseMap("Albedo", 2D) = "white" {}
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Shadow1Color("Shadow1Color", Color) = (0.5, 0.5, 0.5, 0.5)
        //_Shadow1Map("Shadow1 Map",2D) = "white" {} 
        _Shadow1Step("Shadow1 Step",Range(0.0,1.0)) = 0.5
        _Shadow1Feather("Shadow1 Feather",Range(0.0,1.0)) = 0.0
        _Shadow2Color("Shadow2Color", Color) = (0.0, 0.0, 0.0, 0.0)
        //_Shadow2Map("Shadow2 Map",2D) = "white" {} 
        _Shadow2Step("Shadow1 Step",Range(0.0,1.0)) = 0.3
        _Shadow2Feather("Shadow1 Feather",Range(0.0,1.0)) = 0.0
        _InShadowMap("Shadow Map",2D) = "white"{}
        _InShadowMapStrength("ShadowMap Strength",Range(0.0,1.0)) = 1.0
        [ToggleOff] _CastHairShadowMask("CastHairShadowMask(FrontHair)",Float) = 0.0
        [ToggleOff] _ReceiveHairShadowMask("ReceiveHairShadowMask",Float) = 0.0

        //Specular
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}
        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular", 2D) = "white" {}
        _SpecularStep("SpecularStep",Range(0.0,1)) = 0.5
        _SpecularFeather("SpecularFeather",Range(0.0,1))= 0
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

        //Ramp
        [ToggleOff]_EnableRampMap("EnableRampMap",Float) = 0.0
        _DiffuseRampMap("DiffuseRampMap",2D) = "white"{}
        _DiffuseRampV("DiffuseRampV",Range(0.0,1))= 0.0

        //Rim
        _BlendRim("BlendRim",Range(0.0,1.0)) = 0.0
        _RimColor("RimColor",Color) = (0.0,0.0,0.0,0.0)
        _RimPow("RimPower",Range(0.0,10.0)) = 4
        _RimStep("RimStep",Range(0.0,1.0)) = 0.5
        _RimFeather("RimFeather",Range(0.0,1.0)) = 0

        // HairSpecular
        [ToggleOff] _EnableHairSpecular("HairSpecular", Float) = 0.0
        _SpecularShiftMap("SpecularShiftMap",2D) = "white"{}
        _SpecularShiftIntensity("SpecularShiftIntensity",Range(0.0,3.0)) = 1.0
        _SpecularShift1("SpecularShift",Float) = 0.0
        _SpecularShift2("SpecularShiftSec",Float)= 0.0
        _Specular2Mul ("SpecularSecMul", Range (0.0,1.0) ) = 0.5

        //Outline
        [ToggleOff] _EnableOutline("Enable Outline",Float) = 1.0
        [ToggleOff] _UseSmoothNormal("UseSmoothNormal",Float) = 0.0
        _OutlineColor("OutlineColor",Color)= (0.0,0.0,0.0,0.0)
        _OutlineWidth("OutlineWidth",Range(0.0,5.0)) = 0.5

        // Advanced Options
        [ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 0.0
        _RenderQueue("Render Queue", Float) = 2000
        
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

    }
    SubShader
    {
        // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
        // no LightMode tag are also rendered by Universal Render Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
        LOD 300

        Pass
        {
            Name "HairShadowMask"
            ZTest Less
            Tags{"LightMode"="HairShadowMask"}
            ZWrite Off
            Cull Back
 
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonHairShadowMaskPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            ZTest Less
            Tags{"LightMode"="Outline"} //Use For CustomForwardRenderer. Default Tags{"LightMode"="SRPDefaultUnlit"} 
            ZWrite On
            Cull Front
            Stencil
            {
                Ref [_StencilChannel]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOp]
            }
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma shader_feature_local_vertex _USESMOOTHNORMAL
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonOutlinePass.hlsl"            
            ENDHLSL
        }
        
       	Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            Blend [_SrcBlend][_DstBlend]
            ZWrite On
            Cull[_Cull]
            Stencil
            {
                Ref[_StencilChannel]
                Comp[_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOp]    
            }
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // Material Keywords
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _DIFFUSERAMPMAP
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _HAIRSPECULAR
            #pragma shader_feature_local _RECEIVE_HAIRSHADOWMASK
            
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _HAIRSHADOWMASK

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ToonForwardPassVertex
            #pragma fragment ToonForwardPassFragment

            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonLighting.hlsl"
            #include "../Include/ToonForwardPass.hlsl"
            ENDHLSL
        }
         
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

             // -------------------------------------
            // Material Keywords

            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"   
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex ToonVertexMeta
            #pragma fragment ToonFragmentMeta

            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonLighting.hlsl"
            #include "../Include/ToonMetaPass.hlsl"
            
            ENDHLSL
        }

    }
    SubShader
    {
        // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
        // no LightMode tag are also rendered by Universal Render Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "ShaderModel" = "2.0"}
        LOD 300

        Pass
        {
            Name "HairShadowMask"
            ZTest Less
            Tags{"LightMode"="HairShadowMask"}
            ZWrite Off
            Cull [_Cull]
 
            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonHairShadowMaskPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            ZTest Less
            Tags{"LightMode"="Outline"} //Use For CustomForwardRenderer. Default Tags{"LightMode"="SRPDefaultUnlit"} 
            ZWrite On
            Cull Front
            Stencil
            {
                Ref [_StencilChannel]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOp]
            }
            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma shader_feature_local_vertex _USESMOOTHNORMAL
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonOutlinePass.hlsl"            
            ENDHLSL
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            Blend [_SrcBlend][_DstBlend]
            ZWrite On
            Cull[_Cull]
            Stencil
            {
                Ref[_StencilChannel]
                Comp[_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOp]    
            }
            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            // Material Keywords
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _DIFFUSERAMPMAP
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _HAIRSPECULAR
            #pragma shader_feature_local _RECEIVE_HAIRSHADOWMASK
            
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _HAIRSHADOWMASK

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ToonForwardPassVertex
            #pragma fragment ToonForwardPassFragment

            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonLighting.hlsl"
            #include "../Include/ToonForwardPass.hlsl"
            ENDHLSL
        }
         
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

             // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"   
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "../Include/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma only_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex ToonVertexMeta
            #pragma fragment ToonFragmentMeta

            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _INVERSECLIPMASK
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "../Include/ToonInput.hlsl"
            #include "../Include/ToonFunction.hlsl"
            #include "../Include/ToonLighting.hlsl"
            #include "../Include/ToonMetaPass.hlsl"            
            ENDHLSL
        }

    }

    CustomEditor "ChiliMilk.Toon.Editor.ToonShaderGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
