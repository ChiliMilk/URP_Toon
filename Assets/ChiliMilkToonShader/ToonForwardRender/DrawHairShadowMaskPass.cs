using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal.Internal
{
    public class DrawHairShadowMaskPass : ScriptableRenderPass
    {
        FilteringSettings m_FilteringSettings;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;

        static int maskId = Shader.PropertyToID("_HairShadowMask");
        static RenderTargetIdentifier mask_idt = new RenderTargetIdentifier(maskId);
        static string keyword = "_HAIRSHADOWMASK";
        ShaderTagId maskTag = new ShaderTagId("HairShadowMask");

        public DrawHairShadowMaskPass(string profilerTag, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_ShaderTagIdList.Add(maskTag);
            renderPassEvent = evt;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(maskId, new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.R16));
            ConfigureTarget(mask_idt);
            ConfigureClear(ClearFlag.Color, Color.black);
            CoreUtils.SetKeyword(cmd, keyword, true);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                Camera camera = renderingData.cameraData.camera;
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);

                // Render objects that did not match any shader pass with error shader
                RenderingUtils.RenderObjectsWithError(context, ref renderingData.cullResults, camera, m_FilteringSettings, SortingCriteria.None);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.DisableShaderKeyword(keyword);
        }
    }
}
