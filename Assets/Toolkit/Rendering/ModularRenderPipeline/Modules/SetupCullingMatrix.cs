using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("SetupCullingMatrix", typeof(MRPModule))]
    public class SetupCullingMatrix : MRPModule {

        #region Variables

        [SerializeField, DefaultString("cullParameters", true)] private string inputCullParameterKey = "cullParameters";
        [SerializeField, DefaultString("_CameraCullingMatrix", true)] private string cameraCullingMatrixKey = "_CameraCullingMatrix";

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(!data.TryGet<ScriptableCullingParameters>(inputCullParameterKey, out var cullParameters)) {
                var c = data.TargetCamera;
                c.TryGetCullingParameters(out cullParameters);
                data.Set(inputCullParameterKey, cullParameters);
            }
            Shader.SetGlobalMatrix(MRPPropertyIDs.CameraCullingMatrix, cullParameters.cullingMatrix);
            data.Set(cameraCullingMatrixKey, cullParameters.cullingMatrix);
        }

        public override void Dispose() { }

        #endregion
    }
}
