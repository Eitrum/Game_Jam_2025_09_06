using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public static class ModularRenderPipelineSceneView {

        #region Variables

        public static List<ShaderTagId> shaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("ForwardLit"),
            new ShaderTagId("Unlit"),
        };

        #endregion

        #region Draw

        public static void Draw(in ScriptableRenderContext context, Camera c) {
#if UNITY_EDITOR
            ScriptableRenderContext.EmitWorldGeometryForSceneView(c);
#endif
            // Setup command buffer
            ScriptableCullingParameters cullingParams;
            if(!c.TryGetCullingParameters(out cullingParams))
                return;

            // Set up camera properties
            context.SetupCameraProperties(c);
            ScriptableRenderContext.EmitGeometryForCamera(c);
            cullingParams.cullingMask = uint.MaxValue;
            CullingResults cullResults = context.Cull(ref cullingParams);

            CommandBuffer drawBuffer = CommandBufferPool.Get("Opaque" );

            foreach(var sTagId in shaderTagIds) {
                // Draw opaque objects
                var drawSettings = new DrawingSettings(
                sTagId,
                new SortingSettings(c));
                var filterSettings = new FilteringSettings(RenderQueueRange.all);

                var renderListParams = new RendererListParams(cullResults, drawSettings, filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                drawBuffer.DrawRendererList(renderList);
            }

            // Skybox
            var skybox = context.CreateSkyboxRendererList(c);
            drawBuffer.DrawRendererList(skybox);


            context.ExecuteCommandBuffer(drawBuffer);
            CommandBufferPool.Release(drawBuffer);


#if UNITY_EDITOR
            // REQUIRED: Draw gizmos in Scene View
            if(UnityEditor.Handles.ShouldRenderGizmos()) {
                context.DrawGizmos(c, GizmoSubset.PreImageEffects);
                context.DrawGizmos(c, GizmoSubset.PostImageEffects);
            }
#endif
            context.Submit();
        }

        #endregion

        #region Dispose

        public static void Dispose() { }

        #endregion
    }
}
