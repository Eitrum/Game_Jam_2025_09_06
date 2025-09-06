using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("DeferredCompositeModule", typeof(MRPModule))]
    public class DeferredCompositeModule : MRPModule {

        [System.Serializable]
        public class TextureMapping {
            public string MRPTexture;
            public string MaterialVariableName;
        }

        #region Variables

        [SerializeField] private string compositeMaterial = "Hidden/CompositeMRTs";
        [SerializeField] private TextureMapping[] mappings = { };

        private Material compositeMaterialInstance;

        #endregion

        #region MRPModule Impl

        protected override bool Initialize() {
            if(compositeMaterialInstance == null)
                compositeMaterialInstance = new Material(Shader.Find(compositeMaterial));
            return true;
        }

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            foreach(var mapping in mappings) {
                if(!data.TryGet<RenderTexture>(mapping.MRPTexture, out var rt))
                    Debug.LogError(this.FormatLog("Failed to find texture: " + mapping.MRPTexture));
                compositeMaterialInstance.SetTexture(mapping.MaterialVariableName, rt);
            }

            var draw = GetCommandBuffer("Composite");

            draw.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            draw.Blit(null, BuiltinRenderTextureType.CameraTarget, compositeMaterialInstance);

            context.ExecuteCommandBuffer(draw);
            Release(draw);
        }

        public override void Dispose() {
            if(compositeMaterialInstance != null) {
                DestroyImmediate(compositeMaterialInstance);
            }
        }

        #endregion
    }
}
