using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderOutlineFeature : ScriptableRendererFeature
{

    private RenderOutlinePass renderOutlinePass;
    public Setting featureSetting = new Setting();

    [System.Serializable]
    public class Setting
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderOutlinePass);
    }

    public override void Create()
    {
        renderOutlinePass = new RenderOutlinePass();
        renderOutlinePass.renderPassEvent = featureSetting.renderPassEvent;
    }

    class RenderOutlinePass : ScriptableRenderPass
    {
        ShaderTagId outlineTag = new ShaderTagId("Outline");

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(outlineTag, ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(renderingData.cullResults,ref drawingSettings,ref filteringSettings);
        }
    }
}
