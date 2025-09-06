using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Unity/Draw", typeof(MRPModule))]
    public class UnityDrawDefaultModule : MRPModule {

        public enum RenderQueueMode {
            None,
            Opaque,
            Transparent,
            All,

            Custom,
        }

        #region Variables

        [SerializeField, DefaultString("cullResults", true)] private string cullResultsKey;
        [SerializeField] private LayerMask layerMask = -1;
        [SerializeField] private SortingCriteria sortingCriteria = SortingCriteria.RenderQueue;
        [SerializeField] private RenderQueueMode renderQueueMode = RenderQueueMode.Opaque;
        [SerializeField, ShowIf("renderQueueMode", RenderQueueMode.Custom)] private MinMaxInt renderQueueRange = new MinMaxInt(0, 5000);

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
            var draw = GetCommandBuffer("Draw");

            if(debug) {
                Debug.Log(this.FormatLog(string.Join(", ", shaderTagIds.Select(x => x.name))));
            }

            var sortingSettings = new SortingSettings(data.TargetCamera){ criteria = sortingCriteria };
            var filterSettings = new FilteringSettings(renderQueueRange: GetRenderQueueRange(), layerMask: layerMask);

            foreach(var sTagId in shaderTagIds) {
                // Draw opaque objects
                var drawSettings = new DrawingSettings(sTagId, sortingSettings);

                var renderListParams = new RendererListParams(cullResults, drawSettings, filterSettings);
                var renderList = context.CreateRendererList(ref renderListParams);
                draw.DrawRendererList(renderList);
            }

            context.ExecuteCommandBuffer(draw);
            Release(draw);
        }

        private RenderQueueRange GetRenderQueueRange() {
            switch(renderQueueMode) {
                case RenderQueueMode.Opaque: return RenderQueueRange.opaque;
                case RenderQueueMode.Transparent: return RenderQueueRange.transparent;
                case RenderQueueMode.Custom: return new RenderQueueRange(renderQueueRange.min, renderQueueRange.max);
                default: return new RenderQueueRange(0, 0);
            }
        }

        [ContextMenu("Reload")]
        public override void Dispose() {
            shaderTagIds.Clear();
        }

        #endregion
    }
}
