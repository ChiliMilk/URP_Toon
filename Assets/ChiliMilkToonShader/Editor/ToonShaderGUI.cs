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

        private struct Styles
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
            public static readonly GUIContent EnableRampMap = new GUIContent("UseRampMapShadow");
            public static readonly GUIContent DiffuseRampMap = new GUIContent("DiffuseRampMap VOffset");
            public static readonly GUIContent Shadow1Step = new GUIContent("Shadow1Step");
            public static readonly GUIContent Shadow1Feather = new GUIContent("Shadow1Feather");
            public static readonly GUIContent Shadow2Step = new GUIContent("Shadow2Step");
            public static readonly GUIContent Shadow2Feather = new GUIContent("Shadow2Feather");
            public static readonly GUIContent EnableInShadowMap = new GUIContent("EnableInShadowMap");
            public static readonly GUIContent InShadowMap = new GUIContent("InShadowMap");
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
            public static readonly GUIContent SpecularSecMul = new GUIContent("SpecularSecMul");
            public static readonly GUIContent SpecularHighlights = new GUIContent("Enable Specular Highlights");

            //Outline
            public static readonly GUIContent EnableOutline = new GUIContent("EnableOutline");
            public static readonly GUIContent UseSmoothNormal = new GUIContent("UseSmoothNormal");
            public static readonly GUIContent OutlineWidth = new GUIContent("OutlineWidth");

            //Rim
            public static readonly GUIContent EnableRim = new GUIContent("EnableRim");
            public static readonly GUIContent BlendRim = new GUIContent("BlendRim");
            public static readonly GUIContent RimColor = new GUIContent("RimColor");
            public static readonly GUIContent RimPow = new GUIContent("RimPow");
            public static readonly GUIContent RimStep = new GUIContent("RimStep");
            public static readonly GUIContent RimFeather = new GUIContent("RimFeather");
        }

        private struct MPropertyNames
        {
            public static readonly string WorkflowMode = "_WorkflowMode";
            public static readonly string SurfaceType = "_SurfaceType";
            public static readonly string Cull = "_Cull";
            public static readonly string AlphaClip = "_AlphaClip";
            public static readonly string InverseClipMask = "_InverseClipMask";
            public static readonly string ClipMask = "_ClipMask";
            public static readonly string Cutoff = "_Cutoff";
            public static readonly string EnableStencil = "_EnableStencil";
            public static readonly string StencilType = "_StencilType";
            public static readonly string StencilChannel = "_StencilChannel";
            public static readonly string EnvironmentReflections = "_EnvironmentReflections";
            public static readonly string RenderQueue = "_RenderQueue";
            
            //Diffuse
            public static readonly string BaseMap = "_BaseMap";
            public static readonly string BaseColor = "_BaseColor";
            public static readonly string Shadow1Color = "_Shadow1Color";
            //public static readonly string Shadow1Map = "_Shadow1Map";
            public static readonly string Shadow1Step = "_Shadow1Step";
            public static readonly string Shadow1Feather = "_Shadow1Feather";
            public static readonly string Shadow2Color = "_Shadow2Color";
            //public static readonly string Shadow2Map = "_Shadow2Map";
            public static readonly string Shadow2Step = "_Shadow2Step";
            public static readonly string Shadow2Feather = "_Shadow2Feather";
            public static readonly string EnableInShadowMap = "_EnableInShadowMap";
            public static readonly string InShadowMap = "_InShadowMap";
            public static readonly string InShadowMapStrength = "_InShadowMapStrength";
            public static readonly string EnableRampMap = "_EnableRampMap";
            public static readonly string DiffuseRampMap = "_DiffuseRampMap";
            public static readonly string DiffuseRampV = "_DiffuseRampV";
            public static readonly string ReceiveShadows = "_ReceiveShadows";
            
            //Specular
            public static readonly string SpecularHighlights = "_SpecularHighlights";
            public static readonly string Metallic = "_Metallic";
            public static readonly string SpecColor = "_SpecColor";
            public static readonly string MetallicGlossMap = "_MetallicGlossMap";
            public static readonly string SpecGlossMap = "_SpecGlossMap";
            public static readonly string SpecStep = "_SpecularStep";
            public static readonly string SpecFeather = "_SpecularFeather";
            public static readonly string Smoothness = "_Smoothness";
            public static readonly string EnableHairSpecular = "_EnableHairSpecular";
            public static readonly string SpecularShiftMap = "_SpecularShiftMap";
            public static readonly string SpecularShiftIntensity = "_SpecularShiftIntensity";
            public static readonly string SpecularShift1 = "_SpecularShift1";
            public static readonly string SpecularShift2 = "_SpecularShift2";
            public static readonly string Specular2Mul = "_Specular2Mul";
            
            //Base
            public static readonly string BumpMap = "_BumpMap";
            public static readonly string BumpScale = "_BumpScale";
            public static readonly string OcclusionMap = "_OcclusionMap";
            public static readonly string OcclusionStrength = "_OcclusionStrength";
            public static readonly string EmissionMap = "_EmissionMap";
            public static readonly string EmissionColor = "_EmissionColor";
            
            //Outline
            public static readonly string EnableOutline = "_EnableOutline";
            public static readonly string UseSmoothNormal = "_UseSmoothNormal";
            public static readonly string OutlineColor = "_OutlineColor";
            public static readonly string OutlineWidth = "_OutlineWidth";
            
            //Rim
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

        private const string EditorPrefKey = "ChiliMilk:ToonShaderGUI:";

        // Foldouts
        private bool m_SurfaceOptionsFoldout;
        private bool m_AdvancedOptionsFoldout;
        private bool m_BaseFoldout;
        private bool m_ShadowFoldout;
        private bool m_SpecularFoldout;
        private bool m_RimFoldout;
        private bool m_OutlineFoldout;

        // Properties
        private MaterialProperty m_WorkflowModeProp;
        private MaterialProperty m_SurfaceTypeProp;
        private MaterialProperty m_CullProp;
        private MaterialProperty m_AlphaClipProp;
        private MaterialProperty m_InverseClipMaskProp;
        private MaterialProperty m_ClipMaskProp;
        private MaterialProperty m_CutoffProp;
        private MaterialProperty m_EnvironmentReflectionsProp;
        private MaterialProperty m_RenderQueueProp;
        private MaterialProperty m_EnableStencilProp;
        private MaterialProperty m_StencilTypeProp;
        private MaterialProperty m_StencilChannelProp;
        
        //Diffuse
        private MaterialProperty m_BaseMapProp;
        private MaterialProperty m_BaseColorProp;
        private MaterialProperty m_Shadow1ColorProp;
        //private MaterialProperty m_Shadow1MapProp;
        private MaterialProperty m_Shadow1StepProp;
        private MaterialProperty m_Shadow1FeatherProp;
        private MaterialProperty m_Shadow2ColorProp;
        //private MaterialProperty m_Shadow2MapProp;
        private MaterialProperty m_Shadow2StepProp;
        private MaterialProperty m_Shadow2FeatherProp;
        private MaterialProperty m_EnableRampMapProp;
        private MaterialProperty m_DiffuseRampMapProp;
        private MaterialProperty m_DiffuseRampVProp;
        private MaterialProperty m_EnableInShadowMapProp;
        private MaterialProperty m_InShadowMapProp;
        private MaterialProperty m_InShadowMapStrengthProp;
        private MaterialProperty m_ReceiveShadowsProp;
        
        //Base
        private MaterialProperty m_BumpMapProp;
        private MaterialProperty m_BumpScaleProp;
        private MaterialProperty m_OcclusionMapProp;
        private MaterialProperty m_OcclusionStrengthProp;
        private MaterialProperty m_EmissionMapProp;
        private MaterialProperty m_EmissionColorProp;
        
        //Specular
        private MaterialProperty m_MetallicProp;
        private MaterialProperty m_SpecColorProp;
        private MaterialProperty m_MetallicGlossMapProp;
        private MaterialProperty m_SpecGlossMapProp;
        private MaterialProperty m_SpecStepProp;
        private MaterialProperty m_SpecFeatherProp;
        private MaterialProperty m_SmoothnessProp;
        private MaterialProperty m_SpecularHighlightsProp;
        private MaterialProperty m_EnableHairSpecularProp;
        private MaterialProperty m_SpeculatShiftMapProp;
        private MaterialProperty m_SpecularShiftIntensityProp;
        private MaterialProperty m_SpecularShift1Prop;
        private MaterialProperty m_SpecularShift2Prop;
        private MaterialProperty m_Specular2MulProp;

        //Outline
        private MaterialProperty m_EnableOutlineProp;
        private MaterialProperty m_UseSmoothNormalProp;
        private MaterialProperty m_OutlineColorProp;
        private MaterialProperty m_OutlineWidthProp;
        
        //Rim
        private MaterialProperty m_EnableRimProp;
        private MaterialProperty m_BlendRimProp;
        private MaterialProperty m_RimColorProp;
        private MaterialProperty m_RimPowProp;
        private MaterialProperty m_RimStepProp;
        private MaterialProperty m_RimFeatherProp;
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

            m_WorkflowModeProp = FindProperty(MPropertyNames.WorkflowMode, properties, false);
            m_SurfaceTypeProp = FindProperty(MPropertyNames.SurfaceType, properties, false);
            m_CullProp = FindProperty(MPropertyNames.Cull, properties, false);
            m_AlphaClipProp = FindProperty(MPropertyNames.AlphaClip, properties, false);
            m_InverseClipMaskProp = FindProperty(MPropertyNames.InverseClipMask, properties, false);
            m_ClipMaskProp = FindProperty(MPropertyNames.ClipMask,properties,false);
            m_CutoffProp = FindProperty(MPropertyNames.Cutoff, properties, false);
            m_EnvironmentReflectionsProp = FindProperty(MPropertyNames.EnvironmentReflections, properties, false);
            m_RenderQueueProp = FindProperty(MPropertyNames.RenderQueue, properties, false);
            m_EnableStencilProp = FindProperty(MPropertyNames.EnableStencil, properties, false);
            m_StencilTypeProp = FindProperty(MPropertyNames.StencilType, properties, false);
            m_StencilChannelProp = FindProperty(MPropertyNames.StencilChannel, properties, false);
            
            //Diffuse
            m_BaseMapProp = FindProperty(MPropertyNames.BaseMap, properties, false);
            m_BaseColorProp = FindProperty(MPropertyNames.BaseColor, properties, false);
            m_Shadow1ColorProp = FindProperty(MPropertyNames.Shadow1Color, properties, false);
            //m_Shadow1MapProp = FindProperty(MPropertyNames.Shadow1Map, properties, false);
            m_Shadow1StepProp = FindProperty(MPropertyNames.Shadow1Step, properties, false);
            m_Shadow1FeatherProp = FindProperty(MPropertyNames.Shadow1Feather, properties, false);
            m_Shadow2ColorProp = FindProperty(MPropertyNames.Shadow2Color, properties, false);
            //m_Shadow2MapProp = FindProperty(MPropertyNames.Shadow2Map, properties, false);
            m_Shadow2StepProp = FindProperty(MPropertyNames.Shadow2Step, properties, false);
            m_Shadow2FeatherProp = FindProperty(MPropertyNames.Shadow2Feather, properties, false);
            m_EnableRampMapProp = FindProperty(MPropertyNames.EnableRampMap, properties, false);
            m_DiffuseRampMapProp = FindProperty(MPropertyNames.DiffuseRampMap, properties, false);
            m_DiffuseRampVProp = FindProperty(MPropertyNames.DiffuseRampV, properties, false);
            m_EnableInShadowMapProp = FindProperty(MPropertyNames.EnableInShadowMap, properties, false);
            m_InShadowMapProp = FindProperty(MPropertyNames.InShadowMap, properties, false);
            m_InShadowMapStrengthProp = FindProperty(MPropertyNames.InShadowMapStrength, properties, false);
            m_ReceiveShadowsProp = FindProperty(MPropertyNames.ReceiveShadows, properties, false);
            
            //Specular
            m_MetallicProp = FindProperty(MPropertyNames.Metallic, properties);
            m_SpecColorProp = FindProperty(MPropertyNames.SpecColor, properties, false);
            m_MetallicGlossMapProp = FindProperty(MPropertyNames.MetallicGlossMap, properties);
            m_SpecGlossMapProp = FindProperty(MPropertyNames.SpecGlossMap, properties, false);
            m_SpecStepProp = FindProperty(MPropertyNames.SpecStep, properties, false);
            m_SpecFeatherProp = FindProperty(MPropertyNames.SpecFeather, properties, false);
            m_SmoothnessProp = FindProperty(MPropertyNames.Smoothness, properties, false);
            m_SpeculatShiftMapProp = FindProperty(MPropertyNames.SpecularShiftMap, properties, false);
            m_SpecularShiftIntensityProp = FindProperty(MPropertyNames.SpecularShiftIntensity, properties, false);
            m_SpecularShift1Prop = FindProperty(MPropertyNames.SpecularShift1, properties, false);
            m_SpecularShift2Prop = FindProperty(MPropertyNames.SpecularShift2, properties, false);
            m_Specular2MulProp = FindProperty(MPropertyNames.Specular2Mul, properties, false);
            m_SpecularHighlightsProp = FindProperty(MPropertyNames.SpecularHighlights, properties, false);
            
            //Base
            m_BumpMapProp = FindProperty(MPropertyNames.BumpMap, properties, false);
            m_BumpScaleProp = FindProperty(MPropertyNames.BumpScale, properties, false);
            m_OcclusionMapProp = FindProperty(MPropertyNames.OcclusionMap, properties, false);
            m_OcclusionStrengthProp = FindProperty(MPropertyNames.OcclusionStrength, properties, false);
            m_EmissionMapProp = FindProperty(MPropertyNames.EmissionMap, properties, false);
            m_EmissionColorProp = FindProperty(MPropertyNames.EmissionColor, properties, false);
            m_EnableHairSpecularProp = FindProperty(MPropertyNames.EnableHairSpecular, properties, false);
            
            //Outline
            m_EnableOutlineProp = FindProperty(MPropertyNames.EnableOutline, properties, false);
            m_UseSmoothNormalProp = FindProperty(MPropertyNames.UseSmoothNormal, properties, false);
            m_OutlineColorProp = FindProperty(MPropertyNames.OutlineColor, properties, false);
            m_OutlineWidthProp = FindProperty(MPropertyNames.OutlineWidth, properties, false);
            
            //Rim
            m_EnableRimProp = FindProperty(MPropertyNames.EnableRimLight, properties, false);
            m_BlendRimProp = FindProperty(MPropertyNames.BlendRim, properties, false);
            m_RimColorProp = FindProperty(MPropertyNames.RimColor, properties, false);
            m_RimPowProp = FindProperty(MPropertyNames.RimPow, properties, false);
            m_RimStepProp = FindProperty(MPropertyNames.RimStep, properties, false);
            m_RimFeatherProp = FindProperty(MPropertyNames.RimFeather, properties, false);

            EditorGUI.BeginChangeCheck();
            DrawProperties(materialEditor);
            if (EditorGUI.EndChangeCheck())
            {
                SetMaterialKeywords(materialEditor.target as Material);
            }
        }
        #endregion

        #region Keywords
        
        private void SetKeyword(Material material, string keyword, bool value)
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

        private void SetMaterialKeywords(Material material)
        {
            // Reset
            material.shaderKeywords = null;

            // WorkflowMode
            if (material.HasProperty(MPropertyNames.WorkflowMode))
            {
                SetKeyword(material,"_SPECULAR_SETUP", material.GetFloat(MPropertyNames.WorkflowMode) == 0);
            }
            
            //Alpha clip
            bool alphaClip = material.GetFloat(MPropertyNames.AlphaClip) == 1;
            if (alphaClip)
            {
                SetKeyword(material,"_INVERSECLIPMASK",material.GetFloat(MPropertyNames.InverseClipMask) == 1);
                material.EnableKeyword("_ALPHATEST_ON");
                //material.SetOverrideTag("RenderType", "TransparentCutout");
            }
            else
            {
                material.DisableKeyword("_ALPHATEST_ON");
                //material.SetOverrideTag("RenderType", "Opaque");
            }

            SurfaceType surfaceType = (SurfaceType)material.GetFloat(MPropertyNames.SurfaceType);
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
                material.SetShaderPassEnabled("ShadowCaster", true);
            }
            else
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // General Transparent Material Settings
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetShaderPassEnabled("ShadowCaster", false);
            }

            // Receive Shadows
            if (material.HasProperty(MPropertyNames.ReceiveShadows))
            {
                SetKeyword(material,"_RECEIVE_SHADOWS_OFF", material.GetFloat(MPropertyNames.ReceiveShadows) == 0.0f);
            }

            // Highlights
            if (material.HasProperty(MPropertyNames.SpecularHighlights))
            {
                SetKeyword(material,"_SPECULARHIGHLIGHTS_OFF", material.GetFloat(m_SpecularHighlightsProp.name) == 0.0f);
            }

            // Reflections
            if (material.HasProperty(MPropertyNames.EnvironmentReflections))
            {
                SetKeyword(material,"_ENVIRONMENTREFLECTIONS_OFF", material.GetFloat(m_EnvironmentReflectionsProp.name) == 0.0f);
            }

            // Metallic Specular
            var isSpecularWorkFlow = (WorkflowMode)material.GetFloat(MPropertyNames.WorkflowMode) == WorkflowMode.Specular;
            var hasGlossMap = false;
            if (isSpecularWorkFlow)
                hasGlossMap = material.GetTexture(MPropertyNames.SpecGlossMap) != null;
            else
                hasGlossMap = material.GetTexture(MPropertyNames.MetallicGlossMap) != null;
            SetKeyword(material,"_METALLICSPECGLOSSMAP", hasGlossMap);

            // Normal
            SetKeyword(material,"_NORMALMAP", material.GetTexture(MPropertyNames.BumpMap) != null);

            // Occlusion
            SetKeyword(material,"_OCCLUSIONMAP", material.GetTexture(MPropertyNames.OcclusionMap) != null);

            // Emission
            bool hasEmissionMap = material.GetTexture(MPropertyNames.EmissionMap) != null;
            Color emissionColor = material.GetColor(MPropertyNames.EmissionColor);
            SetKeyword(material,"_EMISSION", hasEmissionMap || emissionColor != Color.black);

            // HairSpecular
            SetKeyword(material,"_HAIRSPECULAR", material.GetFloat(MPropertyNames.EnableHairSpecular) == 1.0f);
            SetKeyword(material,"_SPECULARSHIFTMAP", material.GetTexture(MPropertyNames.SpecularShiftMap) != null);

            //InShadowMap
            SetKeyword(material,"_INSHADOWMAP", material.GetFloat(MPropertyNames.EnableInShadowMap)==1.0);

            //Rim
            SetKeyword(material,"_RIMLIGHT", material.GetFloat(MPropertyNames.EnableRimLight) == 1.0f);
            SetKeyword(material,"_BLENDRIM", material.GetFloat(MPropertyNames.BlendRim) == 1.0f);

            //Outline
            SetKeyword(material,"_USESMOOTHNORMAL", material.GetFloat(MPropertyNames.UseSmoothNormal) == 1.0);
            material.SetShaderPassEnabled("Outline", material.GetFloat(MPropertyNames.EnableOutline) == 1.0f&&surfaceType == SurfaceType.Opaque);

            //RampMap
            SetKeyword(material,"_DIFFUSERAMPMAP", material.GetFloat(MPropertyNames.EnableRampMap)==1.0f);
        }
        #endregion

        #region Properties

        private void DrawProperties(MaterialEditor materialEditor)
        {
            // Surface Options
            var surfaceOptionsFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceOptionsFoldout, Styles.SurfaceOptionsFold);
            if (surfaceOptionsFold)
            {
                DrawSurfaceOptions(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("SurfaceOptions", m_SurfaceOptionsFoldout, surfaceOptionsFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Base
            var baseFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_BaseFoldout, Styles.BaseFold);
            if (baseFold)
            {
                EditorGUILayout.Space();
                DrawBaseProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Base", m_BaseFoldout, baseFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Shadow
            var shadowFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShadowFoldout, Styles.ShadowFold);
            if (shadowFold)
            {
                EditorGUILayout.Space();
                DrawShadowProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Shadow", m_ShadowFoldout, shadowFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Specular
            var specularFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_SpecularFoldout, Styles.SpecularFold);
            if (specularFold)
            {
                EditorGUILayout.Space();
                DrawSpecularProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Specular", m_SpecularFoldout, specularFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Rim
            var rimFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_RimFoldout, Styles.RimFold);
            if (rimFold)
            {
                EditorGUILayout.Space();
                DrawRimProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Rim", m_RimFoldout, rimFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Outline
            var OutlineFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_OutlineFoldout, Styles.OutlineFold);
            if (OutlineFold)
            {
                EditorGUILayout.Space();
                DrawOutlineProperties(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("Outline", m_OutlineFoldout, OutlineFold);
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Advanced Options
            var advancedOptionsFold = EditorGUILayout.BeginFoldoutHeaderGroup(m_AdvancedOptionsFoldout, Styles.AdvancedOptionsFold);
            if (advancedOptionsFold)
            {
                EditorGUILayout.Space();
                DrawAdvancedOptions(materialEditor);
                EditorGUILayout.Space();
            }
            SetFoldoutState("AdvancedOptions", m_AdvancedOptionsFoldout, advancedOptionsFold);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Workflow Mode
            DoPopup(Styles.WorkflowMode, m_WorkflowModeProp, Enum.GetNames(typeof(WorkflowMode)),materialEditor);

            //// Render Face
            //if (material.HasProperty(MPropertyNames.Cull))
            //{
            //    EditorGUI.showMixedValue = m_CullProp.hasMixedValue;
            //    EditorGUI.BeginChangeCheck();
            //    int renderFace = EditorGUILayout.Popup(Styles.RenderFace, (int)m_CullProp.floatValue, Enum.GetNames(typeof(RenderFace)));
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        materialEditor.RegisterPropertyChangeUndo(Styles.RenderFace.text);
            //        m_CullProp.floatValue = renderFace;
            //        material.doubleSidedGI = (RenderFace)m_CullProp.floatValue != RenderFace.Front;
            //    }
            //    EditorGUI.showMixedValue = false;
            //}

            if (material.HasProperty(MPropertyNames.SurfaceType))
            {
                EditorGUI.showMixedValue = m_SurfaceTypeProp.hasMixedValue;
                EditorGUI.BeginChangeCheck();
                var surface = EditorGUILayout.Popup(Styles.SurfaceType, (int)m_SurfaceTypeProp.floatValue, Enum.GetNames(typeof(SurfaceType)));
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(Styles.SurfaceType.text);
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
                EditorGUI.showMixedValue = false;
            }

            // Render Face
            if (material.HasProperty(MPropertyNames.Cull))
            {
                if ((SurfaceType)material.GetFloat(MPropertyNames.SurfaceType) == SurfaceType.Opaque)
                {
                    EditorGUI.showMixedValue = m_CullProp.hasMixedValue;
                    EditorGUI.BeginChangeCheck();
                    int renderFace = EditorGUILayout.Popup(Styles.RenderFace, (int)m_CullProp.floatValue, Enum.GetNames(typeof(RenderFace)));
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(Styles.RenderFace.text);
                        m_CullProp.floatValue = renderFace;
                        material.doubleSidedGI = (RenderFace)m_CullProp.floatValue != RenderFace.Front;
                    }
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    m_CullProp.floatValue = (float)RenderFace.Front;
                    material.doubleSidedGI = false;
                }
            }

            // AlphaClip
            if (material.HasProperty(MPropertyNames.AlphaClip) && material.HasProperty(MPropertyNames.Cutoff))
            {
                EditorGUI.BeginChangeCheck();
                var enableAlphaClip = EditorGUILayout.Toggle(Styles.AlphaClipping, m_AlphaClipProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    m_AlphaClipProp.floatValue = enableAlphaClip ? 1 : 0;
                    m_RenderQueueProp.floatValue = enableAlphaClip ? (int)UnityEngine.Rendering.RenderQueue.AlphaTest : (int)UnityEngine.Rendering.RenderQueue.Geometry;
                }
                if (m_AlphaClipProp.floatValue == 1)
                {
                    materialEditor.ShaderProperty(m_InverseClipMaskProp, Styles.InverseClipMask);
                    materialEditor.TexturePropertySingleLine(Styles.AlphaClippingMask, m_ClipMaskProp, m_CutoffProp);
                }
            }

            //Stencil
            if (material.HasProperty(MPropertyNames.EnableStencil) && material.HasProperty(MPropertyNames.StencilChannel))
            {
                EditorGUI.BeginChangeCheck();
                var enableStencil = EditorGUILayout.Toggle(Styles.EnableStencil, m_EnableStencilProp.floatValue == 1);
                if (EditorGUI.EndChangeCheck())
                {
                    m_EnableStencilProp.floatValue = enableStencil ? 1 : 0;
                }
                if (m_EnableStencilProp.floatValue == 1)
                {
                    EditorGUI.showMixedValue = m_StencilTypeProp.hasMixedValue;
                    EditorGUI.BeginChangeCheck();
                    var stencilType = EditorGUILayout.Popup(Styles.StencilType, (int)m_StencilTypeProp.floatValue, Enum.GetNames(typeof(StencilType)));
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(Styles.StencilType.text);
                        m_StencilTypeProp.floatValue = stencilType;
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
                    materialEditor.ShaderProperty(m_StencilChannelProp, Styles.StencilChannel);
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Disabled);
                    material.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
                }
            }
        }

        private void DrawBaseProperties(MaterialEditor materialEditor)
        {
            materialEditor.TexturePropertySingleLine(Styles.Color, m_BaseMapProp, m_BaseColorProp);

            // Normal
            materialEditor.TexturePropertySingleLine(Styles.Normal, m_BumpMapProp, m_BumpScaleProp);

            // Occlusion
            materialEditor.TexturePropertySingleLine(Styles.Occlusion, m_OcclusionMapProp,
                m_OcclusionMapProp.textureValue != null ? m_OcclusionStrengthProp : null);

            // Emission
            var hadEmissionTexture = m_EmissionMapProp.textureValue != null;
            materialEditor.TexturePropertyWithHDRColor(Styles.Emission, m_EmissionMapProp,
                m_EmissionColorProp, false);

            // If texture was assigned and color was black set color to white
            var brightness = m_EmissionColorProp.colorValue.maxColorComponent;
            if (m_EmissionMapProp.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                m_EmissionColorProp.colorValue = Color.white;

            // TilingOffset
            materialEditor.TextureScaleOffsetProperty(m_BaseMapProp);
        }

        private void DrawShadowProperties(MaterialEditor materialEditor)
        {
            var material = materialEditor.target as Material;

            //Diffuse
            materialEditor.ShaderProperty(m_EnableRampMapProp, Styles.EnableRampMap);
            if (m_EnableRampMapProp.floatValue == 1.0)
            {
                //Use Ramp
                materialEditor.TexturePropertySingleLine(Styles.DiffuseRampMap, m_DiffuseRampMapProp, m_DiffuseRampVProp);
            }
            else
            {
                //materialEditor.TexturePropertySingleLine(Styles.Shadow1Map, m_Shadow1MapProp, m_Shadow1ColorProp);
                //materialEditor.TexturePropertySingleLine(Styles.Shadow2Map, m_Shadow2MapProp, m_Shadow2ColorProp);
                materialEditor.ColorProperty(m_Shadow1ColorProp, "Shadow1Color");
                materialEditor.ColorProperty(m_Shadow2ColorProp, "Shadow2Color");
                EditorGUI.BeginChangeCheck();
                var step1 = EditorGUILayout.Slider(Styles.Shadow1Step, m_Shadow1StepProp.floatValue, 0f, 1f);
                var feather1 = EditorGUILayout.Slider(Styles.Shadow1Feather,m_Shadow1FeatherProp.floatValue,0f,1f);
                var step2 = EditorGUILayout.Slider(Styles.Shadow2Step, m_Shadow2StepProp.floatValue, 0f, 1f);
                var feather2 = EditorGUILayout.Slider(Styles.Shadow2Feather,m_Shadow2FeatherProp.floatValue,0f,1f);
                if (EditorGUI.EndChangeCheck())
                {
                    m_Shadow1StepProp.floatValue = step1;
                    m_Shadow1FeatherProp.floatValue = feather1;
                    m_Shadow2StepProp.floatValue = step2;
                    m_Shadow2FeatherProp.floatValue = feather2;
                }
            }
            //InShadowMap
            materialEditor.ShaderProperty(m_EnableInShadowMapProp, Styles.EnableInShadowMap);
            if (m_EnableInShadowMapProp.floatValue == 1.0)
            {
                materialEditor.TexturePropertySingleLine(Styles.InShadowMap, m_InShadowMapProp, m_InShadowMapStrengthProp);
            }
            //ReceiveShadows
            if (material.HasProperty(MPropertyNames.ReceiveShadows))
            {
                EditorGUI.BeginChangeCheck();
                var receiveShadows = EditorGUILayout.Toggle(Styles.ReceiveShadows, m_ReceiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(Styles.ReceiveShadows.text);
                    m_ReceiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                }
            }
        }

        private void DrawSpecularProperties(MaterialEditor materialEditor)
        {
            var material = materialEditor.target as Material;
            // MetallicSpecular
            bool hasGlossMap = false;
            bool specularWork = material.GetFloat(MPropertyNames.WorkflowMode) == 0;
            if (specularWork)
            {
                hasGlossMap = m_SpecGlossMapProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(Styles.Specular, m_SpecGlossMapProp, hasGlossMap ? null : m_SpecColorProp);
            }
            else
            {
                hasGlossMap = m_MetallicGlossMapProp.textureValue != null;
                materialEditor.TexturePropertySingleLine(Styles.Metallic, m_MetallicGlossMapProp, hasGlossMap ? null : m_MetallicProp);
            }

            //Specular
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel += 2;
            var specStep = EditorGUILayout.Slider(Styles.SpecularStep, m_SpecStepProp.floatValue, 0f, 1f);
            var specFeather = EditorGUILayout.Slider(Styles.SpecularFeather, m_SpecFeatherProp.floatValue, 0f, 1f);
            EditorGUI.indentLevel -= 2;
            if (EditorGUI.EndChangeCheck())
            {
                m_SpecStepProp.floatValue = specStep;
                m_SpecFeatherProp.floatValue = specFeather;
            }

            // Smoothness
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel+=2;
            var smooth = EditorGUILayout.Slider(Styles.Smoothness, m_SmoothnessProp.floatValue, 0f, 1f);
            EditorGUI.indentLevel-=2;
            if (EditorGUI.EndChangeCheck())
            {
                m_SmoothnessProp.floatValue = smooth;
            }
            
            // HairSpecular
            materialEditor.ShaderProperty(m_EnableHairSpecularProp, Styles.EnableHairSpecular);
            if (m_EnableHairSpecularProp.floatValue == 1.0)
            {
                materialEditor.TexturePropertySingleLine(Styles.SpecularShiftMap, m_SpeculatShiftMapProp, m_SpecularShiftIntensityProp);
                materialEditor.TextureScaleOffsetProperty(m_SpeculatShiftMapProp);
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                var shift1 = EditorGUILayout.Slider(Styles.SpecularShift, m_SpecularShift1Prop.floatValue, -1f, 1f);
                var shift2 = EditorGUILayout.Slider(Styles.SpecularShiftSec, m_SpecularShift2Prop.floatValue, -1f, 1f);
                var specular2mul = EditorGUILayout.Slider(Styles.SpecularSecMul, m_Specular2MulProp.floatValue, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    m_SpecularShift1Prop.floatValue = shift1;
                    m_SpecularShift2Prop.floatValue = shift2;
                    m_Specular2MulProp.floatValue = specular2mul;
                }
                EditorGUI.indentLevel -= 2;
            }
            // Highlights
            if (material.HasProperty(MPropertyNames.SpecularHighlights))
            {
                materialEditor.ShaderProperty(m_SpecularHighlightsProp, Styles.SpecularHighlights);
            }
        }

        private void DrawRimProperties(MaterialEditor materialEditor)
        {
            //Rim
            materialEditor.ShaderProperty(m_EnableRimProp, Styles.EnableRim);
            if (m_EnableRimProp.floatValue == 1.0)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 2;
                materialEditor.ShaderProperty(m_BlendRimProp, Styles.BlendRim);
                materialEditor.ColorProperty(m_RimColorProp, Styles.RimColor.text);
                var rimPow = EditorGUILayout.Slider(Styles.RimPow, m_RimPowProp.floatValue, 0f, 10f);
                var rimStep = EditorGUILayout.Slider(Styles.RimStep, m_RimStepProp.floatValue, 0f, 1f);
                var rimFeather = EditorGUILayout.Slider(Styles.RimFeather, m_RimFeatherProp.floatValue, 0f, 1f);
                EditorGUI.indentLevel -= 2;
                if (EditorGUI.EndChangeCheck())
                {
                    m_RimPowProp.floatValue = rimPow;
                    m_RimStepProp.floatValue = rimStep;
                    m_RimFeatherProp.floatValue = rimFeather;
                }
            }
        }

        private void DrawOutlineProperties(MaterialEditor materialEditor)
        {
            materialEditor.ShaderProperty(m_EnableOutlineProp, Styles.EnableOutline);
            if (m_EnableOutlineProp.floatValue == 1.0)
            {
                materialEditor.ShaderProperty(m_UseSmoothNormalProp, Styles.UseSmoothNormal);
                materialEditor.ColorProperty(m_OutlineColorProp, "OutlineColor");
                EditorGUI.BeginChangeCheck();
                var OutlineWidth = EditorGUILayout.Slider(Styles.OutlineWidth, m_OutlineWidthProp.floatValue, 0f, 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    m_OutlineWidthProp.floatValue = OutlineWidth;
                }
            }
        }

        private void DrawAdvancedOptions(MaterialEditor materialEditor)
        {
            // Get Material
            var material = materialEditor.target as Material;

            // Reflections
            if (material.HasProperty(MPropertyNames.EnvironmentReflections))
            {
                materialEditor.ShaderProperty(m_EnvironmentReflectionsProp, Styles.EnvironmentReflections);
            }

            materialEditor.EnableInstancingField();

            // RenderQueue
            if (material.HasProperty(MPropertyNames.RenderQueue))
            {
                EditorGUI.BeginChangeCheck();
                var RenderQueue = EditorGUILayout.IntSlider(Styles.RenderQueue, (int)m_RenderQueueProp.floatValue, -1, 5000);
                if (EditorGUI.EndChangeCheck())
                {
                    m_RenderQueueProp.floatValue = RenderQueue;
                }
                material.renderQueue = (int)m_RenderQueueProp.floatValue;
            }
        }
        #endregion

        #region EditorPrefs

        private bool GetFoldoutState(string name)
        {
            // Get value from EditorPrefs
            return EditorPrefs.GetBool($"{EditorPrefKey}.{name}");
        }

        private void SetFoldoutState(string name, bool field, bool value)
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
    


}
