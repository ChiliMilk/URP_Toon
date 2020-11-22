using System;
using UnityEngine;
using UnityEditor;

namespace ChiliMilk.Toon.Editor
{
    /// <summary>
    /// ToonGUI class for shaders.
    /// </summary>
    public class ToonShaderGUI : ShaderGUI
    {
        #region Structs
        struct GUIContents
        {
            // Foldouts
            public static readonly GUIContent SurfaceOptionsFold = new GUIContent("Surface Options");
            public static readonly GUIContent OutlineFold = new GUIContent("Outline");
            public static readonly GUIContent AdvancedOptionsFold = new GUIContent("Advanced Options");
            public static readonly GUIContent BaseFold = new GUIContent("Base");
            public static readonly GUIContent ShadowFold = new GUIContent("Shadow");
            public static readonly GUIContent SpecularFold = new GUIContent("Specular");
            public static readonly GUIContent RimFold = new GUIContent("Rim");

            // Properies
            public static readonly GUIContent WorkflowMode = new GUIContent("Workflow Mode");
            public static readonly GUIContent SurfaceType = new GUIContent("SurfaceType");
            public static readonly GUIContent BlendMode = new GUIContent("BlendMode");
            public static readonly GUIContent RenderFace = new GUIContent("Render Face");
            public static readonly GUIContent AlphaClipping = new GUIContent("Alpha Clipping");
            public static readonly GUIContent InverseClipMask = new GUIContent("Inverse ClipMask");
            public static readonly GUIContent AlphaClippingMask = new GUIContent("ClipMask Threshold");
            public static readonly GUIContent EnvironmentReflections = new GUIContent("Environment Reflections");
            public static readonly GUIContent RenderQueue = new GUIContent("RenderQueue");
            public static readonly GUIContent EnableStencil = new GUIContent("Stencil");
            public static readonly GUIContent StencilType = new GUIContent("Stencil Type");
            public static readonly GUIContent StencilChannel = new GUIContent("Stencil Channel");

            public static readonly GUIContent Color = new GUIContent("Color");
            public static readonly GUIContent Normal = new GUIContent("Normal");
            public static readonly GUIContent Occlusion = new GUIContent("Occlusion");
            public static readonly GUIContent Emission = new GUIContent("Emission");

            //Shadow
            public static readonly GUIContent ShadeMap = new GUIContent("ShadeMap", "Use for GlobalIllumination,Default use Diffuse for GlobalIllumination");
            public static readonly GUIContent EnableRampMap = new GUIContent("UseRampMapShadow");
            public static readonly GUIContent DiffuseRampMap = new GUIContent("DiffuseRampMap VOffset");
            public static readonly GUIContent ShadowMinus = new GUIContent("ShadowMinus");
            public static readonly GUIContent ShadowStep = new GUIContent("ShadowStep");
            public static readonly GUIContent ShadowFeather = new GUIContent("ShadowFeather");
            public static readonly GUIContent EnableInShadowMap = new GUIContent("EnableInShadowMap");
            public static readonly GUIContent InShadowMap = new GUIContent("InShadowMap");
            public static readonly GUIContent InShadowMapStrength = new GUIContent("InShadowMapStrength");
            public static readonly GUIContent ReceiveShadows = new GUIContent("Receive Shadows");

            //Specular
            public static readonly GUIContent SpecularStep = new GUIContent("SpecularStep");
            public static readonly GUIContent SpecularFeather = new GUIContent("SpecularFeather");
            public static readonly GUIContent Specular = new GUIContent("Specular");
            public static readonly GUIContent Metallic = new GUIContent("Metallic");
            public static readonly GUIContent Smoothness = new GUIContent("Smoothness");
            public static readonly GUIContent EnableHairSpecular = new GUIContent("HairSpecular");
            public static readonly GUIContent SpecularShiftMap = new GUIContent("HairShiftMap");
            public static readonly GUIContent SpecularShift = new GUIContent("SpecularShift");
            public static readonly GUIContent SpecularShiftSec = new GUIContent("SpecularShiftSec");
            public static readonly GUIContent SmoothnessSecMul = new GUIContent("SmoothnessSecMul");
            public static readonly GUIContent SpecularSecMul = new GUIContent("SpecularSecMul");
            public static readonly GUIContent SpecularHighlights = new GUIContent("Enable Specular Highlights");

            //Outline
            public static readonly GUIContent EnableOutline = new GUIContent("EnableOutline");
            public static readonly GUIContent UseSmoothNormal = new GUIContent("UseSmoothNormal");
            public static readonly GUIContent OutlineColor = new GUIContent("OutlineColor");
            public static readonly GUIContent OutlineWidth = new GUIContent("OutlineWidth");

            //Rim
            public static readonly GUIContent EnableRim = new GUIContent("EnableRim");
            public static readonly GUIContent BlendRim = new GUIContent("BlendRim");
            public static readonly GUIContent RimColor = new GUIContent("RimColor");
            public static readonly GUIContent RimPow = new GUIContent("RimPow");
            public static readonly GUIContent RimStep = new GUIContent("RimStep");
            public static readonly GUIContent RimFeather = new GUIContent("RimFeather");
        }

        struct PropertyNames
        {
            public static readonly string WorkflowMode = "_WorkflowMode";
            public static readonly string SurfaceType = "_SurfaceType";
            public static readonly string BlendMode = "_Blend";
            public static readonly string Cull = "_Cull";
            public static readonly string AlphaClip = "_AlphaClip";
            public static readonly string InverseClipMask = "_InverseClipMask";
            public static readonly string ClipMask = "_ClipMask";
            public static readonly string Cutoff = "_Cutoff";
            public static readonly string ReceiveShadows = "_ReceiveShadows";
            public static readonly string SpecularHighlights = "_SpecularHighlights";
            public static readonly string EnvironmentReflections = "_EnvironmentReflections";
            public static readonly string RenderQueue = "_RenderQueue";
            public static readonly string EnableStencil = "_EnableStencil";
            public static readonly string StencilType = "_StencilType";
            public static readonly string StencilChannel = "_StencilChannel";
            public static readonly string BaseMap = "_BaseMap";
            public static readonly string BaseColor = "_BaseColor";
            public static readonly string ShadeMap = "_ShadeMap";
            public static readonly string EnabeRampMap = "_EnableRampMap";
            public static readonly string DiffuseRampMap = "_DiffuseRampMap";
            public static readonly string DiffuseRampV = "_DiffuseRampV";
            public static readonly string ShadowMinus = "_ShadowMinus";
            public static readonly string ShadowStep = "_ShadowStep";
            public static readonly string ShadowFeather = "_ShadowFeather";
            public static readonly string EnableInShdowMap = "_EnableInShadowMap";
            public static readonly string InShadowMap = "_InShadowMap";
            public static readonly string InShadowMapStrength = "_InShadowMapStrength";
            public static readonly string Metallic = "_Metallic";
            public static readonly string SpecColor = "_SpecColor";
            public static readonly string MetallicGlossMap = "_MetallicGlossMap";
            public static readonly string SpecGlossMap = "_SpecGlossMap";
            public static readonly string SpecStep = "_SpecularStep";
            public static readonly string SpecFeather = "_SpecularFeather";
            public static readonly string SmoothnessMap = "_SmoothnessMap";
            public static readonly string Smoothness = "_Smoothness";
            public static readonly string BumpMap = "_BumpMap";
            public static readonly string BumpScale = "_BumpScale";
            public static readonly string OcclusionMap = "_OcclusionMap";
            public static readonly string OcclusionStrength = "_OcclusionStrength";
            public static readonly string EmissionMap = "_EmissionMap";
            public static readonly string EmissionColor = "_EmissionColor";
            public static readonly string EnableHairSpecular = "_EnableHairSpecular";
            public static readonly string SpecularShiftMap = "_SpecularShiftMap";
            public static readonly string SpecularShiftIntensity = "_SpecularShiftIntensity";
            public static readonly string SpecularShift1 = "_SpecularShift1Add";
            public static readonly string SpecularShift2 = "_SpecularShift2Add";
            public static readonly string Smoothness2Mul = "_Smoothness2Mul";
            public static readonly string Specular2Mul = "_Specular2Mul";
            public static readonly string EnableOutline = "_EnableOutline";
            public static readonly string UseSmoothNormal = "_UseSmoothNormal";
            public static readonly string OutlineColor = "_OutlineColor";
            public static readonly string OutlineWidth = "_OutlineWidth";
            public static readonly string EnableRimLight = "_EnableRim";
            public static readonly string BlendRim = "_BlendRim";
            public static readonly string RimColor = "_RimColor";
            public static readonly string RimPow = "_RimPow";
            public static readonly string RimStep = "_RimStep";
            public static readonly string RimFeather = "_RimFeather";
        }
        #endregion

        #region Enum

        public enum WorkflowMode
        {
            Specular,
            Metallic,
        }

        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        public enum BlendMode
        {
            Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
            Additive,
            Multiply
        }

        public enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        public enum StencilType
        {
            Mask = 0,
            Out = 1
        }

        #endregion
        #region Fields
        const string EditorPrefKey = "ChiliMilk:ToonShaderGUI:";

        // Foldouts
        bool m_SurfaceOptionsFoldout;
        bool m_AdvancedOptionsFoldout;
        bool m_BaseFoldout;
        bool m_ShadowFoldout;
        bool m_SpecularFoldout;
        bool m_RimFoldout;
        bool m_OutlineFoldout;

        // Properties
        MaterialProperty m_WorkflowModeProp;
        MaterialProperty m_SurfaceTypeProp;
        MaterialProperty m_BlendModeProp;
        MaterialProperty m_CullProp;
        MaterialProperty m_AlphaClipProp;
        MaterialProperty m_InverseClipMaskProp;
        MaterialProperty m_ClipMaskProp;
        MaterialProperty m_CutoffProp;
        MaterialProperty m_ReceiveShadowsProp;
        MaterialProperty m_SpecularHighlightsProp;
        MaterialProperty m_EnvironmentReflectionsProp;
        MaterialProperty m_RenderQueueProp;
        MaterialProperty m_EnableStencilProp;
        MaterialProperty m_StencilTypeProp;
        MaterialProperty m_StencilChannelProp;
        MaterialProperty m_BaseMapProp;
        MaterialProperty m_BaseColorProp;
        MaterialProperty m_ShadeMapProp;
        MaterialProperty m_EnableRampMapProp;
        MaterialProperty m_DiffuseRampMapProp;
        MaterialProperty m_DiffuseRampVProp;
        MaterialProperty m_ShadowMinusProp;
        MaterialProperty m_ShadowStepProp;
        MaterialProperty m_ShadowFeatherProp;
        MaterialProperty m_EnableInShadowMapProp;
        MaterialProperty m_InShadowMapProp;
        MaterialProperty m_InShadowMapStrength;
        MaterialProperty m_MetallicProp;
        MaterialProperty m_SpecColorProp;
        MaterialProperty m_MetallicGlossMapProp;
        MaterialProperty m_SpecGlossMapProp;
        MaterialProperty m_SpecStep;
        MaterialProperty m_SpecFeather;
        MaterialProperty m_SmoothnessMapProp;
        MaterialProperty m_SmoothnessProp;
        MaterialProperty m_BumpMapProp;
        MaterialProperty m_BumpScaleProp;
        MaterialProperty m_OcclusionMapProp;
        MaterialProperty m_OcclusionStrengthProp;
        MaterialProperty m_EmissionMapProp;
        MaterialProperty m_EmissionColorProp;
        MaterialProperty m_EnableHairSpecularProp;
        MaterialProperty m_SpeculatShiftMapProp;
        MaterialProperty m_SpecularShiftIntensityProp;
        MaterialProperty m_SpecularShift1Prop;
        MaterialProperty m_SpecularShift2Prop;
        MaterialProperty m_Smoothness2MulProp;
        MaterialProperty m_Specular2MulProp;
        MaterialProperty m_EnableOutlineProp;
        MaterialProperty m_UseSmoothNormalProp;
        MaterialProperty m_OutlineColorProp;
        MaterialProperty m_OutlineWidthProp;
        MaterialProperty m_EnableRimProp;
        MaterialProperty m_BlendRimProp;
        MaterialProperty m_RimColorProp;
        MaterialProperty m_RimPowProp;
        MaterialProperty m_RimStepProp;
        MaterialProperty m_RimFeatherProp;
        #endregion

        #region GUI
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            m_SurfaceOptionsFoldout = GetFoldoutState("SurfaceOptions");
            m_AdvancedOptionsFoldout = GetFoldoutState("AdvancedOptions");
            m_BaseFoldout = GetFoldoutState("Base");
            m_ShadowFoldout = GetFoldoutState("Shadow");
            m_SpecularFoldout = GetFoldoutState("Specular");
            m_RimFoldout = GetFoldoutState("Rim");
            m_OutlineFoldout = GetFoldoutState("Outline");

            m_WorkflowModeProp = FindProperty(PropertyNames.WorkflowMode, properties, false);
            m_SurfaceTypeProp = FindProperty(PropertyNames.SurfaceType, properties, false);
            m_BlendModeProp = FindProperty(PropertyNames.BlendMode, properties, false);
            m_CullProp = FindProperty(PropertyNames.Cull, properties, false);
            m_AlphaClipProp = FindProperty(PropertyNames.AlphaClip, properties, false);
            m_InverseClipMaskProp = FindProperty(PropertyNames.InverseClipMask, properties, false);
            m_ClipMaskProp = FindProperty(PropertyNames.ClipMask,properties,false);
            m_CutoffProp = FindProperty(PropertyNames.Cutoff, properties, false);
            m_ReceiveShadowsProp = FindProperty(PropertyNames.ReceiveShadows, properties, false);
            m_SpecularHighlightsProp = FindProperty(PropertyNames.SpecularHighlights, properties, false);
            m_EnvironmentReflectionsProp = FindProperty(PropertyNames.EnvironmentReflections, properties, false);
            m_RenderQueueProp = FindProperty(PropertyNames.RenderQueue, properties, false);
            m_EnableStencilProp = FindProperty(PropertyNames.EnableStencil, properties, false);
            m_StencilTypeProp = FindProperty(PropertyNames.StencilType, properties, false);
            m_StencilChannelProp = FindProperty(PropertyNames.StencilChannel, properties, false);
            m_BaseMapProp = FindProperty(PropertyNames.BaseMap, properties, false);
            m_BaseColorProp = FindProperty(PropertyNames.BaseColor, properties, false);
            m_ShadeMapProp = FindProperty(PropertyNames.ShadeMap, properties, false);
            m_EnableRampMapProp = FindProperty(PropertyNames.EnabeRampMap, properties, false);
            m_DiffuseRampMapProp = FindProperty(PropertyNames.DiffuseRampMap, properties, false);
            m_DiffuseRampVProp = FindProperty(PropertyNames.DiffuseRampV, properties, false);
            m_ShadowMinusProp = FindProperty(PropertyNames.ShadowMinus, properties, false);
            m_ShadowStepProp = FindProperty(PropertyNames.ShadowStep, properties, false);
            m_ShadowFeatherProp = FindProperty(PropertyNames.ShadowFeather, properties, false);
            m_EnableInShadowMapProp = FindProperty(PropertyNames.EnableInShdowMap, properties, false);
            m_InShadowMapProp = FindProperty(PropertyNames.InShadowMap, properties, false);
            m_InShadowMapStrength = FindProperty(PropertyNames.InShadowMapStrength, properties, false);
            m_MetallicProp = FindProperty(PropertyNames.Metallic, properties);
            m_SpecColorProp = FindProperty(PropertyNames.SpecColor, properties, false);
            m_MetallicGlossMapProp = FindProperty(PropertyNames.MetallicGlossMap, properties);
            m_SpecGlossMapProp = FindProperty(PropertyNames.SpecGlossMap, properties, false);
            m_SpecStep = FindProperty(PropertyNames.SpecStep, properties, false);
            m_SpecFeather = FindProperty(PropertyNames.SpecFeather, properties, false);
            m_SmoothnessMapProp = FindProperty(PropertyNames.SmoothnessMap, properties, false);
            m_SmoothnessProp = FindProperty(PropertyNames.Smoothness, properties, false);
            m_BumpMapProp = FindProperty(PropertyNames.BumpMap, properties, false);
            m_BumpScaleProp = FindProperty(PropertyNames.BumpScale, properties, false);
            m_OcclusionMapProp = FindProperty(PropertyNames.OcclusionMap, properties, false);
            m_OcclusionStrengthProp = FindProperty(PropertyNames.OcclusionStrength, properties, false);
            m_EmissionMapProp = FindProperty(PropertyNames.EmissionMap, properties, false);
            m_EmissionColorProp = FindProperty(PropertyNames.EmissionColor, properties, false);
            m_EnableHairSpecularProp = FindProperty(PropertyNames.EnableHairSpecular, properties, false);
            m_SpeculatShiftMapProp = FindProperty(PropertyNames.SpecularShiftMap, properties, false);
            m_SpecularShiftIntensityProp = FindProperty(PropertyNames.SpecularShiftIntensity, properties, false);
            m_SpecularShift1Prop = FindProperty(PropertyNames.SpecularShift1, properties, false);
            m_SpecularShift2Prop = FindProperty(PropertyNames.SpecularShift2, properties, false);
            m_Smoothness2MulProp = FindProperty(PropertyNames.Smoothness2Mul, properties, false);
            m_Specular2MulProp = FindProperty(PropertyNames.Specular2Mul, properties, false);
            m_EnableOutlineProp = FindProperty(PropertyNames.EnableOutline, properties, false);
            m_UseSmoothNormalProp = FindProperty(PropertyNames.UseSmoothNormal, properties, false);
            m_OutlineColorProp = FindProperty(PropertyNames.OutlineColor, properties, false);
            m_OutlineWidthProp = FindProperty(PropertyNames.OutlineWidth, properties, false);
            m_EnableRimProp = FindProperty(PropertyNames.EnableRimLight, properties, false);
            m_BlendRimProp = FindProperty(PropertyNames.BlendRim, properties, false);
            m_RimColorProp = FindProperty(PropertyNames.RimColor, properties, false);
            m_RimPowProp = FindProperty(PropertyNames.RimPow, properties, false);
            m_RimStepProp = FindProperty(PropertyNames.RimStep, properties, false);
            m_RimFeatherProp = FindProperty(PropertyNames.RimFeather, properties, false);

            EditorGUI.BeginChangeCheck();
            DrawProperties(materialEditor);
            if (EditorGUI.EndChangeCheck())
            {
                SetMaterialKeywords(materialEditor.target as Material);
            }
        }
        #endregion

        #region Keywords

        public void SetupMaterialBlendMode(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            bool alphaClip = material.GetFloat("_AlphaClip") == 1;
            material.SetKeyword("_INVERSECLIPMASK", alphaClip && material.GetFloat(PropertyNames.InverseClipMask) == 1);
            if (alphaClip)
            {
                material.EnableKeyword("_ALPHATEST_ON");
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
            }

            SurfaceType surfaceType = (SurfaceType)material.GetFloat(PropertyNames.SurfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                if (alphaClip)
                {
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                }
                else
                {
                    material.SetOverrideTag("RenderType", "Opaque");
                }
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            else
            {
                BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");
                // Specific Transparent Mode Settings
                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Premultiply:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Additive:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                    case BlendMode.Multiply:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.EnableKeyword("_ALPHAMODULATE_ON");
                        break;
                }
                // General Transparent Material Settings
                material.SetOverrideTag("RenderType", "Transparent");
            }
        }

        void SetMaterialKeywords(Material material)
        {
            // Reset
            material.shaderKeywords = null;

            // WorkflowMode
            if (material.HasProperty(PropertyNames.WorkflowMode))
            {
                material.SetKeyword("_SPECULAR_SETUP", material.GetFloat("_WorkflowMode") == 0);
            }

            SetupMaterialBlendMode(material);

            // Receive Shadows
            if (material.HasProperty(PropertyNames.ReceiveShadows))
            {
                material.SetKeyword("_RECEIVE_SHADOWS_OFF", material.GetFloat(PropertyNames.ReceiveShadows) == 0.0f);
            }

            // Highlights
            if (material.HasProperty(PropertyNames.SpecularHighlights))
            {
                material.SetKeyword("_SPECULARHIGHLIGHTS_OFF", material.GetFloat(m_SpecularHighlightsProp.name) == 0.0f);
            }

            // Reflections
            if (material.HasProperty(PropertyNames.EnvironmentReflections))
            {
                material.SetKeyword("_ENVIRONMENTREFLECTIONS_OFF", material.GetFloat(m_EnvironmentReflectionsProp.name) == 0.0f);
            }

            // Metallic Specular
            var isSpecularWorkFlow = (WorkflowMode)material.GetFloat("_WorkflowMode") == WorkflowMode.Specular;
            var hasGlossMap = false;
            if (isSpecularWorkFlow)
                hasGlossMap = material.GetTexture(PropertyNames.SpecGlossMap) != null;
            else
                hasGlossMap = material.GetTexture(PropertyNames.MetallicGlossMap) != null;
            material.SetKeyword("_METALLICSPECGLOSSMAP", hasGlossMap);

            //Shade
            material.SetKeyword("_SHADEMAP", material.GetTexture(PropertyNames.ShadeMap) != null);

            // Normal
            material.SetKeyword("_NORMALMAP", material.GetTexture(PropertyNames.BumpMap) != null);

            // Occlusion
            material.SetKeyword("_OCCLUSIONMAP", material.GetTexture(PropertyNames.OcclusionMap) != null);

            // Emission
            bool hasEmissionMap = material.GetTexture(PropertyNames.EmissionMap) != null;
            Color emissionColor = material.GetColor(PropertyNames.EmissionColor);
            material.SetKeyword("_EMISSION", hasEmissionMap || emissionColor != Color.black);

            // HairSpecular
            material.SetKeyword("_HAIRSPECULAR", material.GetFloat(PropertyNames.EnableHairSpecular) == 1.0f);
            material.SetKeyword("_SPECULARSHIFTMAP", material.GetTexture(PropertyNames.SpecularShiftMap) != null);

            //InShadowMap
            material.SetKeyword("_INSHADOWMAP", material.GetFloat(PropertyNames.EnableInShdowMap)==1.0);

            //Rim
            material.SetKeyword("_RIMLIGHT", material.GetFloat(PropertyNames.EnableRimLight) == 1.0f);
            material.SetKeyword("_BLENDRIM", material.GetFloat(PropertyNames.BlendRim) == 1.0f);

            //Outline
            material.SetKeyword("_USESMOOTHNORMAL", material.GetFloat(PropertyNames.UseSmoothNormal) == 1.0);
            material.SetShaderPassEnabled("SRPDefaultUnlit", material.GetFloat(PropertyNames.EnableOutline) == 1.0f);

            //RampMap
            material.SetKeyword("_DIFFUSERAMPMAP", material.GetFloat(PropertyNames.EnabeRampMap)==1.0f);
        }
        #endregion

        #region Properties

        void DrawProperties(MaterialEditor materialEditor)
        {
            // Surface Options
            var surfaceOptionsFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceOptionsFoldout, GUIContents.SurfaceOptionsFold);
            if (surfaceOptionsFold)
            {
                DrawSurfaceOptions(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("SurfaceOptions", m_SurfaceOptionsFoldout, surfaceOptionsFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Base
            var baseFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_BaseFoldout, GUIContents.BaseFold);
            if (baseFold)
            {
                EditorGUILayout.Space();
                DrawBaseProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Base", m_BaseFoldout, baseFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Shadow
            var shadowFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShadowFoldout, GUIContents.ShadowFold);
            if (shadowFold)
            {
                EditorGUILayout.Space();
                DrawShadowProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Shadow", m_ShadowFoldout, shadowFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Specular
            var specularFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_SpecularFoldout, GUIContents.SpecularFold);
            if (specularFold)
            {
                EditorGUILayout.Space();
                DrawSpecularProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Specular", m_SpecularFoldout, specularFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Rim
            var rimFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_RimFoldout, GUIContents.RimFold);
            if (rimFold)
            {
                EditorGUILayout.Space();
                DrawRimProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Rim", m_RimFoldout, rimFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Outline
            var OutlineFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_OutlineFoldout, GUIContents.OutlineFold);
            if (OutlineFold)
            {
                EditorGUILayout.Space();
                DrawOutlineProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Outline", m_OutlineFoldout, OutlineFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Advanced Options
            var advancedOptionsFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_AdvancedOptionsFoldout, GUIContents.AdvancedOptionsFold);
            if (advancedOptionsFold)
            {
                EditorGUILayout.Space();
                DrawAdvancedOptions(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("AdvancedOptions", m_AdvancedOptionsFoldout, advancedOptionsFold);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Workflow Mode
            DoPopup(GUIContents.WorkflowMode, m_WorkflowModeProp, Enum.GetNames(typeof(WorkflowMode)),materialEditor);

            //SufaceType
            if (material.HasProperty(PropertyNames.SurfaceType))
            {
                EditorGUI.BeginChangeCheck();
                var surface = EditorGUILayout.Popup(GUIContents.SurfaceType, (int)m_SurfaceTypeProp.floatValue, Enum.GetNames(typeof(SurfaceType)));
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(GUIContents.SurfaceType.text);
                    if ((SurfaceType)surface == SurfaceType.Opaque)
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    }
                    else
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    m_RenderQueueProp.floatValue = material.renderQueue;
                    m_SurfaceTypeProp.floatValue = surface;
                }
            }

            //BlendMode
            if ((SurfaceType)material.GetFloat(PropertyNames.SurfaceType) == SurfaceType.Transparent)
            {
                DoPopup(GUIContents.BlendMode, m_BlendModeProp, Enum.GetNames(typeof(BlendMode)),materialEditor);
            }

            // Render Face
            if (material.HasProperty(PropertyNames.Cull))
            {
                EditorGUI.BeginChangeCheck();
                var renderFace = EditorGUILayout.Popup(GUIContents.RenderFace, (int)m_CullProp.floatValue, Enum.GetNames(typeof(RenderFace)));
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(GUIContents.RenderFace.text);
                    m_CullProp.floatValue = renderFace;
                    material.doubleSidedGI = (RenderFace)m_CullProp.floatValue != RenderFace.Front;
                }
            }

            // AlphaClip
            if (material.HasProperty(PropertyNames.AlphaClip) && material.HasProperty(PropertyNames.Cutoff))
            {
                EditorGUI.BeginChangeCheck();
                var enableAlphaClip = EditorGUILayout.Toggle(GUIContents.AlphaClipping, m_AlphaClipProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    m_AlphaClipProp.floatValue = enableAlphaClip ? 1 : 0;
                    m_RenderQueueProp.floatValue = enableAlphaClip ? (int)UnityEngine.Rendering.RenderQueue.AlphaTest : (int)UnityEngine.Rendering.RenderQueue.Geometry;
                }
                if (m_AlphaClipProp.floatValue == 1)
                {
                    materialEditor.ShaderProperty(m_InverseClipMaskProp, GUIContents.InverseClipMask);
                    materialEditor.TexturePropertySingleLine(GUIContents.AlphaClippingMask, m_ClipMaskProp, m_CutoffProp);
                }
            }

            //Stencil
            if (material.HasProperty(PropertyNames.EnableStencil) && material.HasProperty(PropertyNames.StencilChannel))
            {
                EditorGUI.BeginChangeCheck();
                var enableStencil = EditorGUILayout.Toggle(GUIContents.EnableStencil, m_EnableStencilProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    m_EnableStencilProp.floatValue = enableStencil ? 1 : 0;
                }
                if (m_EnableStencilProp.floatValue == 1)
                {
                    if (material.HasProperty(PropertyNames.StencilType))
                    {
                        EditorGUI.BeginChangeCheck();
                        var stencilType = EditorGUILayout.Popup(GUIContents.StencilType, (int)m_StencilTypeProp.floatValue, Enum.GetNames(typeof(StencilType)));
                        if (EditorGUI.EndChangeCheck())
                        {
                            materialEditor.RegisterPropertyChangeUndo(GUIContents.StencilType.text);
                            m_StencilTypeProp.floatValue = stencilType;
                        }
                        if (m_StencilTypeProp.floatValue == 0)
                        {
                            material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Always);
                            material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Replace);
                        }
                        else if (m_StencilTypeProp.floatValue == 1)
                        {
                            material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
                            material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                        }
                    }
                    materialEditor.ShaderProperty(m_StencilChannelProp, GUIContents.StencilChannel);
                }
                else
                {
                    material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Disabled);
                    material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                    m_StencilChannelProp.floatValue = 0f;
                }
            }
        }

        void DrawBaseProperties(MaterialEditor materialEditor)
        {
            //Diffuse
            materialEditor.TexturePropertySingleLine(GUIContents.Color, m_BaseMapProp, m_BaseColorProp);

            // Normal
            materialEditor.TexturePropertySingleLine(GUIContents.Normal, m_BumpMapProp, m_BumpScaleProp);

            // Occlusion
            materialEditor.TexturePropertySingleLine(GUIContents.Occlusion, m_OcclusionMapProp,
                m_OcclusionMapProp.textureValue != null ? m_OcclusionStrengthProp : null);

            // Emission
            var hadEmissionTexture = m_EmissionMapProp.textureValue != null;
            materialEditor.TexturePropertyWithHDRColor(GUIContents.Emission, m_EmissionMapProp,
                m_EmissionColorProp, false);

            // If texture was assigned and color was black set color to white
            var brightness = m_EmissionColorProp.colorValue.maxColorComponent;
            if (m_EmissionMapProp.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                m_EmissionColorProp.colorValue = Color.white;

            // TilingOffset
            materialEditor.TextureScaleOffsetProperty(m_BaseMapProp);
        }

        void DrawShadowProperties(MaterialEditor materialEditor)
        {
            var material = materialEditor.target as Material;
            //shade
            materialEditor.TexturePropertySingleLine(GUIContents.ShadeMap, m_ShadeMapProp, null);
            materialEditor.ShaderProperty(m_EnableRampMapProp, GUIContents.EnableRampMap);
            if (m_EnableRampMapProp.floatValue == 1.0)
            {
                //Use Ramp
                materialEditor.TexturePropertySingleLine(GUIContents.DiffuseRampMap, m_DiffuseRampMapProp, m_DiffuseRampVProp);
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                var shadowMinus = EditorGUILayout.Slider(GUIContents.ShadowMinus, m_ShadowMinusProp.floatValue, 0.0f, 1.0f);
                if (EditorGUI.EndChangeCheck())
                {
                    m_ShadowMinusProp.floatValue = shadowMinus;
                }
            }
            else
            {
                //Costom Shadow
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                var shadowMinus = EditorGUILayout.Slider(GUIContents.ShadowMinus, m_ShadowMinusProp.floatValue, 0.0f, 1.0f);
                var shadowStep = EditorGUILayout.Slider(GUIContents.ShadowStep, m_ShadowStepProp.floatValue, 0.0f, 1f);
                var shadowFeather = EditorGUILayout.Slider(GUIContents.ShadowFeather, m_ShadowFeatherProp.floatValue, 0f, 1f);
                EditorGUI.indentLevel -= 2;
                if (EditorGUI.EndChangeCheck())
                {
                    m_ShadowMinusProp.floatValue = shadowMinus;
                    m_ShadowStepProp.floatValue = shadowStep;
                    m_ShadowFeatherProp.floatValue = shadowFeather;
                }
            }
            //InShadowMap
            materialEditor.ShaderProperty(m_EnableInShadowMapProp, GUIContents.EnableInShadowMap);
            if (m_EnableInShadowMapProp.floatValue == 1.0)
            {
                materialEditor.TexturePropertySingleLine(GUIContents.InShadowMap, m_InShadowMapProp, m_InShadowMapStrength);
            }
            //ReceiveShadows
            if (material.HasProperty(PropertyNames.ReceiveShadows))
            {
                EditorGUI.BeginChangeCheck();
                var receiveShadows = EditorGUILayout.Toggle(GUIContents.ReceiveShadows, m_ReceiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(GUIContents.ReceiveShadows.text);
                    m_ReceiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                }
            }
        }

        void DrawSpecularProperties(MaterialEditor materialEditor)
        {
            var material = materialEditor.target as Material;
            // MetallicSpecular
            bool hasGlossMap = false;
            if (material.GetFloat("_WorkflowMode") == 0)
            {
                hasGlossMap = m_SpecGlossMapProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(GUIContents.Specular, m_SpecGlossMapProp, hasGlossMap ? null : m_SpecColorProp);
            }
            else
            {
                hasGlossMap = m_MetallicGlossMapProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(GUIContents.Metallic, m_MetallicGlossMapProp, hasGlossMap ? null : m_MetallicProp);
            }

            //Specular
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel += 2;
            var specStep = EditorGUILayout.Slider(GUIContents.SpecularStep, m_SpecStep.floatValue, 0f, 1f);
            var specFeather = EditorGUILayout.Slider(GUIContents.SpecularFeather, m_SpecFeather.floatValue, 0f, 1f);
            EditorGUI.indentLevel -= 2;
            if (EditorGUI.EndChangeCheck())
            {
                m_SpecStep.floatValue = specStep;
                m_SpecFeather.floatValue = specFeather;
            }

            // Smoothness
            materialEditor.TexturePropertySingleLine(GUIContents.Smoothness, m_SmoothnessMapProp, m_SmoothnessProp);

            // HairSpecular
            materialEditor.ShaderProperty(m_EnableHairSpecularProp, GUIContents.EnableHairSpecular);
            if (m_EnableHairSpecularProp.floatValue == 1.0)
            {
                materialEditor.TexturePropertySingleLine(GUIContents.SpecularShiftMap, m_SpeculatShiftMapProp, m_SpecularShiftIntensityProp);
                materialEditor.TextureScaleOffsetProperty(m_SpeculatShiftMapProp);
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                var shift1 = EditorGUILayout.Slider(GUIContents.SpecularShift, m_SpecularShift1Prop.floatValue, -1f, 1f);
                var shift2 = EditorGUILayout.Slider(GUIContents.SpecularShiftSec, m_SpecularShift2Prop.floatValue, -1f, 1f);
                var specular2mul = EditorGUILayout.Slider(GUIContents.SpecularSecMul, m_Specular2MulProp.floatValue, 0f, 1f);
                var smoothness2mul = EditorGUILayout.Slider(GUIContents.SmoothnessSecMul, m_Smoothness2MulProp.floatValue, 0f, 1f);             
                if (EditorGUI.EndChangeCheck())
                {
                    m_SpecularShift1Prop.floatValue = shift1;
                    m_SpecularShift2Prop.floatValue = shift2;
                    m_Smoothness2MulProp.floatValue = smoothness2mul;
                    m_Specular2MulProp.floatValue = specular2mul;
                }
                EditorGUI.indentLevel -= 2;
            }
            // Highlights
            if (material.HasProperty(PropertyNames.SpecularHighlights))
            {
                materialEditor.ShaderProperty(m_SpecularHighlightsProp, GUIContents.SpecularHighlights);
            }
        }

        void DrawRimProperties(MaterialEditor materialEditor)
        {
            //Rim
            materialEditor.ShaderProperty(m_EnableRimProp, GUIContents.EnableRim);
            if (m_EnableRimProp.floatValue == 1.0)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                materialEditor.ShaderProperty(m_BlendRimProp, GUIContents.BlendRim);
                materialEditor.ColorProperty(m_RimColorProp, GUIContents.RimColor.text);
                var rimPow = EditorGUILayout.Slider(GUIContents.RimPow, m_RimPowProp.floatValue, 0f, 10f);
                var rimStep = EditorGUILayout.Slider(GUIContents.RimStep, m_RimStepProp.floatValue, 0f, 1f);
                var rimFeather = EditorGUILayout.Slider(GUIContents.RimFeather, m_RimFeatherProp.floatValue, 0f, 1f);
                EditorGUI.indentLevel -= 2;
                if (EditorGUI.EndChangeCheck())
                {
                    m_RimPowProp.floatValue = rimPow;
                    m_RimStepProp.floatValue = rimStep;
                    m_RimFeatherProp.floatValue = rimFeather;
                }
            }
        }

        void DrawOutlineProperties(MaterialEditor materialEditor)
        {
            materialEditor.ShaderProperty(m_EnableOutlineProp, GUIContents.EnableOutline);
            if (m_EnableOutlineProp.floatValue == 1.0)
            {
                materialEditor.ShaderProperty(m_UseSmoothNormalProp, GUIContents.UseSmoothNormal);
                materialEditor.ColorProperty(m_OutlineColorProp, GUIContents.OutlineColor.text);
                EditorGUI.BeginChangeCheck();
                var OutlineWidth = EditorGUILayout.Slider(GUIContents.OutlineWidth, m_OutlineWidthProp.floatValue, 0f, 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    m_OutlineWidthProp.floatValue = OutlineWidth;
                }
            }
        }

        void DrawAdvancedOptions(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Reflections
            if (material.HasProperty(PropertyNames.EnvironmentReflections))
            {
                materialEditor.ShaderProperty(m_EnvironmentReflectionsProp, GUIContents.EnvironmentReflections);
            }

            materialEditor.EnableInstancingField();

            // RenderQueue
            if (material.HasProperty(PropertyNames.RenderQueue))
            {
                EditorGUI.BeginChangeCheck();
                var RenderQueue = EditorGUILayout.IntSlider(GUIContents.RenderQueue, (int)m_RenderQueueProp.floatValue, -1, 5000);
                if (EditorGUI.EndChangeCheck())
                {
                    m_RenderQueueProp.floatValue = RenderQueue;
                }
                material.renderQueue = (int)m_RenderQueueProp.floatValue;
            }
        }
        #endregion

        #region EditorPrefs
        bool GetFoldoutState(string name)
        {
            // Get value from EditorPrefs
            return EditorPrefs.GetBool($"{EditorPrefKey}.{name}");
        }

        void SetFoldoutState(string name, bool field, bool value)
        {
            if (field == value)
                return;

            // Set value to EditorPrefs and field
            EditorPrefs.SetBool($"{EditorPrefKey}.{name}", value);
        }

        public static void DoPopup(GUIContent label, MaterialProperty property, string[] options, MaterialEditor materialEditor)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            EditorGUI.showMixedValue = property.hasMixedValue;

            var mode = property.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(label, (int)mode, options);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(label.text);
                property.floatValue = mode;
            }

            EditorGUI.showMixedValue = false;
        }
        #endregion
    }

    public static class ToonShaderGUIExtensions
    {
        #region Keywords
        public static void SetKeyword(this Material material, string keyword, bool value)
        {
            if (value)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
        #endregion
    }


}
