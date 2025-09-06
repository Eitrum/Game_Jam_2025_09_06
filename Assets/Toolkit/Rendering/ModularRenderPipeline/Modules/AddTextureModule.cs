using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("AddTextureModule", typeof(MRPModule))]
    public class AddTextureModule : MRPModule {

        public enum TextureDepth {
            _0,
            _16,
            _24,
            _32,
        }

        [System.Serializable]
        public class MRPTextureConfig {
            public string Name = "_TextureName";
            public bool UseCameraSize;
            public Vector2Int CustomSize;
            public RenderTextureFormat TextureFormat = RenderTextureFormat.ARGB32;
            public TextureDepth Depth = TextureDepth._0;
            public RenderTextureReadWrite RTReadWrite = RenderTextureReadWrite.Default;


            public void Create(MRPContext context, ref RenderTexture rt) {
                var w = context.Width;
                var h = context.Height;
                if(rt == null) {
                    rt = new RenderTexture(w, h, GetDepth(Depth), TextureFormat, 4);
                    rt.name = Name;
                    rt.Create();
                }

                context.Set(Name, rt);
            }
        }

        public static int GetDepth(TextureDepth depth) {
            switch(depth) {
                case TextureDepth._16: return 16;
                case TextureDepth._24: return 24;
                case TextureDepth._32: return 32;
                default: return 0;
            }
        }

        [SerializeField, Readonly(true)] private MRPTextureConfig[] textures = { };
        [SerializeField] private RenderTexture[] renderTextures = { };

        protected override bool Initialize() {
            if(renderTextures == null || renderTextures.Length != textures.Length) {
                Dispose();
                renderTextures = new RenderTexture[textures.Length];
            }
            return true;
        }

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            for(int i = 0, length = textures.Length; i < length; i++) {
                textures[i].Create(data, ref renderTextures[i]);
            }
        }

        [ContextMenu("Clear Cache")]
        public override void Dispose() {
            for(int i = renderTextures.Length - 1; i >= 0; i--) {
                if(renderTextures[i] != null) {
                    DestroyImmediate(renderTextures[i]);
                }
            }
        }
    }
}
