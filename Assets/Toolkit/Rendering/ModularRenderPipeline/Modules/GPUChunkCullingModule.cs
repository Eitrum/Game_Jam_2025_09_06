using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("GPUChunkCulling", typeof(MRPModule))]
    public class GPUChunkCullingModule : MRPModule {

        #region Variables

        [Header("Chunk Culling Setings")]
        [SerializeField] private float chunkSize = 64f;
        [SerializeField] private float chunkOverlap = 4f;
        [SerializeField] private bool doubleBuffer = true;
        [SerializeField] private bool sortByDistance = true;

        [Header("Output")]
        [SerializeField, DefaultString("chunkCullResults")] private string cullingResultsKey = "chunkCullResults";

        private GPUChunkCulling chunkCulling;

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(chunkCulling == null || chunkCulling.IsDisposed)
                chunkCulling = new GPUChunkCulling(chunkSize, chunkOverlap, doubleBuffer, sortByDistance);

            chunkCulling.Dispatch(data.TargetCamera);
            var chunksInFrustum = chunkCulling.ReadResults();
            var chunkCullingResults = chunkCulling.CullingResults;
            data.Set(cullingResultsKey, chunkCullingResults);

            if(debug)
                Debug.Log(this.FormatLog($"Chunks in Frustum: {chunksInFrustum}"));
        }

        [ContextMenu("Dispose")]
        public override void Dispose() {
            chunkCulling?.Dispose();
        }

        #endregion
    }
}
