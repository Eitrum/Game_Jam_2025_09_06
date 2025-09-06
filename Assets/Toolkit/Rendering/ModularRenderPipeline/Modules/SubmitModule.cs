using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("SubmitModule", typeof(MRPModule))]
    public class SubmitModule : MRPModule {

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(debug)
                Debug.Log(this.FormatLog("Run"));
            context.Submit();
        }

        public override void Dispose() { }

        #endregion
    }
}
