using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("DrawUIOverlayModule", typeof(MRPModule))]
    public class DrawUIOverlayModule : MRPModule {
        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(debug)
                Debug.Log(this.FormatLog("Run"));

            // Unity already calls this by default built-in
            // context.DrawUIOverlay(data.TargetCamera);
        }

        public override void Dispose() { }

        #endregion
    }
}
