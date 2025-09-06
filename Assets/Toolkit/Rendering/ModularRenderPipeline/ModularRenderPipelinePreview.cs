using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public static class ModularRenderPipelinePreview {

        #region Variables

        public static List<ShaderTagId> shaderTagIds = new List<ShaderTagId>
        {
            new("SRPDefaultUnlit"),
            new("UniversalForward"),
            new("ForwardBase"),
            new("ForwardLit"),
            new("Unlit"),
        };

        #endregion

        #region Draw

        public static void Draw(in ScriptableRenderContext context, Camera c) {
            ScriptableRenderContext.EmitGeometryForCamera(c);
            //context.Submit();
            //return;

            context.SetupCameraProperties(c);
            // Use normal culling and drawing
            if(c.TryGetCullingParameters(out var cullingParams)) {
                var buffer = CommandBufferPool.Get("Preview");
                buffer.Clear();

                if(c.targetTexture != null)
                    buffer.SetRenderTarget(c.targetTexture);

                var clearFlags = c.clearFlags;
                buffer.ClearRenderTarget(
                    (clearFlags & CameraClearFlags.Depth) != 0,
                    (clearFlags & CameraClearFlags.Color) != 0,
                    c.backgroundColor
                );
                var cullingResults = context.Cull(ref cullingParams);

                var drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), new SortingSettings(c));
                var filterSettings = new FilteringSettings(RenderQueueRange.all);
                var renderListParams = new RendererListParams(cullingResults, drawSettings, filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                buffer.DrawRendererList(renderList);
                context.ExecuteCommandBuffer(buffer);
                CommandBufferPool.Release(buffer);

                context.DrawGizmos(c, GizmoSubset.PreImageEffects);
                context.DrawGizmos(c, GizmoSubset.PostImageEffects);
            }

            context.Submit();
        }

        #endregion

        #region Dispose

        public static void Dispose() { }

        #endregion
    }
}
