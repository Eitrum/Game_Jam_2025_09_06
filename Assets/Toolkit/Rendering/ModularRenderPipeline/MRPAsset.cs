using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [CreateAssetMenu(fileName = "MRPAsset", menuName = "Toolkit/Rendering/MRPAsset")]
    public class MRPAsset : ScriptableObject {
        #region Variables

        public NSOReferenceArray<Modules.MRPCameraModule> modules;

        #endregion

        #region Render

        public void Render(in ScriptableRenderContext context, List<Camera> cameras) {
            foreach(var mod in modules) {
                using(var data = new MRPContext(context, cameras))
                    mod.HandleModule(context, data);
            }
        }

        public void Dispose() {
            foreach(var mod in modules) {
                mod.Dispose();
            }
        }

        #endregion
    }
}
