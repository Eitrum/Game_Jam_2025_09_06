using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [NSOFile("SetTargetRenderer/Deferred", typeof(MRPModule))]
    public class SetTargetRendererDeferred : MRPModule {

        #region Variables

        [SerializeField] private string[] colorTextures;
        [SerializeField] private string depthTexture;

        private RenderTargetIdentifier[] colorIdentifiers;
        private RenderTargetIdentifier depthIdentifier;

        #endregion

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            var buffer = GetCommandBuffer("Set Render Target (Deferred)");
            var len = colorTextures.Length;
            if(colorIdentifiers == null || colorIdentifiers.Length != len)
                colorIdentifiers = new RenderTargetIdentifier[len];
            if(len == 0) {
                Debug.LogError(this.FormatLog("Minimum 1 color buffer required"));
                return;
            }
            for(var i = 0; i < len; i++) {
                if(!data.TryGet<RenderTexture>(colorTextures[i], out var rt)) {
                    Debug.LogError(this.FormatLog("Failed to find texture", colorTextures[i]));
                    return;
                }
                colorIdentifiers[i] = rt;
            }

            if(!data.TryGet<RenderTexture>(depthTexture, out var depthRt)) {
                Debug.LogError(this.FormatLog("Failed to find depth texture", depthTexture));
                return;
            }
            depthIdentifier = depthRt;

            buffer.SetRenderTarget(colorIdentifiers, depthIdentifier);
            context.ExecuteCommandBuffer(buffer);
            Release(buffer);
        }

        public override void Dispose() { }
    }
}
