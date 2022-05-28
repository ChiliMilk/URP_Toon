using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class RenderFrontHairShadowMaskFeature : ScriptableRendererFeature
{
    private RenderFrontHairShadowMaskPass renderFrontHairMaskPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderFrontHairMaskPass);
    }

    public override void Create()
    {
        renderFrontHairMaskPass = new RenderFrontHairShadowMaskPass();
        renderFrontHairMaskPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    class RenderFrontHairShadowMaskPass : ScriptableRenderPass
    {
        static int maskId = Shader.PropertyToID("_HairShadowMask");
        static RenderTargetIdentifier mask_idt = new RenderTargetIdentifier(maskId);
        static string keyword = "_HAIRSHADOWMASK";
        ShaderTagId maskTag = new ShaderTagId("HairShadowMask");
        

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(maskId, new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.R16),FilterMode.Point);
            ConfigureTarget(mask_idt);
            ConfigureClear(ClearFlag.Color, Color.black);
            CoreUtils.SetKeyword(cmd, keyword, true);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(maskTag, ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(maskId);
            cmd.DisableShaderKeyword(keyword);
        }

    }
}
