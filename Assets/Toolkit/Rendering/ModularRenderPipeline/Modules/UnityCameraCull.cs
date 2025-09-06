using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Unity/Camera Cull", typeof(MRPModule))]
    public class UnityCameraCull : MRPModule {

        #region Variables

        [SerializeField, DefaultString("cullParameters", true)] private string cullParametersKey;
        [SerializeField, DefaultString("cullResults", true)] private string cullResultsKey;

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            var c = data.TargetCamera;
            c.TryGetCullingParameters(out var cullingParameters);
            CullingResults cullResults = context.Cull(ref cullingParameters);

            data.Set(cullParametersKey, cullingParameters);
            data.Set(cullResultsKey, cullResults);
        }

        public override void Dispose() { }

        #endregion
    }
}
