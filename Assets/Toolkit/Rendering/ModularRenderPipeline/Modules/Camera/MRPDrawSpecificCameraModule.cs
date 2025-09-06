using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Draw Specific Camera", typeof(MRPCameraModule))]
    public class MRPDrawSpecificCameraModule : MRPCameraModule {

        #region Variables

        [SerializeField] private string targetIdentifier = "Main Camera";
        [SerializeField] private NSOReferenceArray<MRPModule> modules;
        private bool debuggingTargetFound = false;

        #endregion

        #region Properties

        public string TargetIdentifier => targetIdentifier;
        public bool DebuggingTargetFound => debuggingTargetFound;

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            if(!MRPCamera.TryGet(targetIdentifier, out MRPCamera instance)) {
                debuggingTargetFound = false;
                return;
            }
            debuggingTargetFound = true;

            if(debug)
                Debug.Log(this.FormatLog("Run: " + targetIdentifier));

            // Set the camera target to instance
            data.SetTarget(instance);
            data.SetSize(Screen.width, Screen.height);

            HandleModules(modules, context, data);
        }

        public override void Dispose() {
            if(debug)
                Debug.Log(this.FormatLog("Dispose: " + targetIdentifier));
            Dispose(modules);
        }

        #endregion
    }
}
