using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Clear Render Targets", typeof(MRPModule))]
    public class ClearRenderTargetModule : MRPModule {

        #region Variables

        [SerializeField] private bool clearDepth = true;
        [SerializeField] private bool clearColor = true;
        [SerializeField] private Color[] backgroundColor = new Color[]{ Color.white };

        [Header("Advanced")]
        [SerializeField, Range(0f, 1f)] private float depth = 1f;
        [SerializeField] private bool clearStencil = true;
        [SerializeField, Range(0, 255)] private uint stencil;

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            var buffer = GetCommandBuffer("Clear Buffer");

            RTClearFlags flag = RTClearFlags.None;
            if(clearDepth)
                flag |= RTClearFlags.Depth;
            if(clearColor)
                flag |= RTClearFlags.Color;
            if(clearStencil)
                flag |= RTClearFlags.Stencil;

            buffer.ClearRenderTarget(flag, backgroundColor, depth, stencil);

            context.ExecuteCommandBuffer(buffer);
            Release(buffer);
        }

        public override void Dispose() { }

        #endregion
    }
}
