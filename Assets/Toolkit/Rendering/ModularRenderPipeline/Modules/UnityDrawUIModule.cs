using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Unity/Draw UI", typeof(MRPModule))]
    public class UnityDrawUIModule : MRPModule {

        #region Variables

        [SerializeField, DefaultString("cullResults", true)] private string cullResultsKey;
        [SerializeField] private string[] shaderTags = { "SRPDefaultUnlit" };
        [HideInInspector] private List<ShaderTagId> shaderTagIds = new List<ShaderTagId>();

        #endregion

        #region MRPModule Impl

        protected override bool Initialize() {
            if(shaderTagIds == null || shaderTagIds.Count != shaderTags.Length) {
                shaderTagIds.Clear();
                foreach(var s in shaderTags)
                    shaderTagIds.Add(new ShaderTagId(s));
            }
            return true;
        }

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(!data.TryGet<CullingResults>(cullResultsKey, out var cullResults)) {
                if(ShowErrors)
                    Debug.LogError(this.FormatLog("No cull results found", cullResultsKey));
                return;
            }

            var draw = GetCommandBuffer("UI");

            foreach(var sTagId in shaderTagIds) {
                // Draw opaque objects
                var drawSettings = new DrawingSettings(sTagId, new SortingSettings(data.TargetCamera){ criteria = SortingCriteria.CanvasOrder });
                var filterSettings = new FilteringSettings(RenderQueueRange.transparent,layerMask: LayerMask.GetMask("UI"));

                var renderListParams = new RendererListParams(cullResults, drawSettings, filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                draw.DrawRendererList(renderList);
            }

            context.ExecuteCommandBuffer(draw);
            Release(draw);
        }

        public override void Dispose() { }

        #endregion
    }
}
