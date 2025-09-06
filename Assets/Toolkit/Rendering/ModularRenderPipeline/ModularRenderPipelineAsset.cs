using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public class ModularRenderPipelineAsset : RenderPipelineAsset<ModularRenderPipelineInstance> {

        [SerializeField] private string uid;
        [SerializeField] private MRPAsset customAsset;
        public MRPAsset CustomAsset => customAsset;

        protected override RenderPipeline CreatePipeline() {

            return new ModularRenderPipelineInstance(this);
        }
    }

    public class ModularRenderPipelineInstance : RenderPipeline {

        private ModularRenderPipelineAsset asset;

        public ModularRenderPipelineInstance(ModularRenderPipelineAsset asset) {
            this.asset = asset;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            ModularRenderPipelinePreview.Dispose();
            ModularRenderPipelineSceneView.Dispose();
            if(asset && asset.CustomAsset)
                asset.CustomAsset.Dispose();
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras) {
            var cameraCount = cameras.Count;
            if(cameraCount == 0) {
                context.Submit();
                return;
            }

            var c = cameras[0];
            var type = c.cameraType;
            switch(type) {
                case CameraType.SceneView:
                    ModularRenderPipelineSceneView.Draw(context, c);
                    return;
                case CameraType.Preview:
                    ModularRenderPipelinePreview.Draw(context, c);
                    return;
                case CameraType.Game: {
                        if(asset && asset.CustomAsset) {
                            asset.CustomAsset.Render(context, cameras);
                        }
                        else {
                            CommandBuffer draw = CommandBufferPool.Get("Draw");
                            context.SetupCameraProperties(cameras[0]);
                            draw.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                            draw.ClearRenderTarget(true, true, Color.clear);
                            context.ExecuteCommandBuffer(draw);
                            CommandBufferPool.Release(draw);
                        }
                    }
                    break;
            }

            context.Submit();
        }
    }
}
