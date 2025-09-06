using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [NSOFile("SetTargetRenderer/Output", typeof(MRPModule))]
    public class SetTargetRendererOutput : MRPModule {

        #region Variables

        [SerializeField] private string depthTexture;
        private RenderTargetIdentifier depthIdentifier;

        #endregion

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            var buffer = GetCommandBuffer("Set Render Target (Output)");

            if(string.IsNullOrEmpty(depthTexture)) {
                buffer.SetRenderTarget(BuiltinRenderTextureType.CurrentActive);
            }
            else {
                if(!data.TryGet<RenderTexture>(depthTexture, out var depthRt)) {
                    Debug.LogError(this.FormatLog("Failed to find depth texture", depthTexture));
                    return;
                }
                depthIdentifier = depthRt;
                buffer.SetRenderTarget(BuiltinRenderTextureType.CurrentActive, depthIdentifier);
            }
            context.ExecuteCommandBuffer(buffer);
            Release(buffer);
        }

        public override void Dispose() { }
    }
}
