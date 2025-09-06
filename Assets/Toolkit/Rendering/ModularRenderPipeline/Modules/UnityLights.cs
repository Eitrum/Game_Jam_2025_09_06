using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Unity/Light", typeof(MRPModule))]
    public class UnityLights : MRPModule {

        #region Variables

        [SerializeField, DefaultString("cullResults", true)] private string cullResultsKey = "cullResults";

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(!data.TryGet(cullResultsKey, out CullingResults cullResults)) {
                if(ShowErrors)
                    Debug.LogError(this.FormatLog("No cull results found", cullResultsKey));
                return;
            }

            var lights = cullResults.visibleLights;
            if(lights != null && lights.Length > 0) {
                var screenArea = lights[0].screenRect;
               // Debug.Log(this.FormatLog(screenArea));
            }
        }

        public override void Dispose() { }

        #endregion
    }
}
