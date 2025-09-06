using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    public static class Texture2DArrayUtility
    {
        #region Variables

        private static Texture2D copyTexture;

        #endregion

        #region Create

        public static Texture2DArray Create(IReadOnlyList<Texture2D> textures) {
            var count = textures.Count;
            if(count == 0)
                throw new System.Exception("Can't build a texture 2d array from no textures!");
            var mainTex = textures[0];
            Texture2DArray array = new Texture2DArray(mainTex.width, mainTex.height, count, GetSupportedFormat(mainTex.format), false);
            for(int i = 0; i < count; i++)
                CopyTextureToArray(array, i, textures[i]);
            array.Apply();
            return array;
        }

        public static Texture2DArray Create(IReadOnlyList<Texture2D> textures, int width, int height) {
            var count = textures.Count;
            if(count == 0)
                throw new System.Exception("Can't build a texture 2d array from no textures!");
            var mainTex = textures[0];
            Texture2DArray array = new Texture2DArray(width, height, count, GetSupportedFormat(mainTex.format), false);
            for(int i = 0; i < count; i++)
                CopyTextureToArray(array, i, textures[i]);
            array.Apply();
            return array;
        }

        public static Texture2DArray Create(IEnumerable<Texture2D> textures) {
            var count = textures.Count();
            if(count == 0)
                throw new System.Exception("Can't build a texture 2d array from no textures!");
            var mainTex = textures.First();
            Texture2DArray array = new Texture2DArray(mainTex.width, mainTex.height, count, GetSupportedFormat(mainTex.format), false);
            textures.Foreach((x, i) => CopyTextureToArray(array, i, x));
            array.Apply();
            return array;
        }

        public static Texture2DArray Create(IEnumerable<Texture2D> textures, int width, int height) {
            var count = textures.Count();
            if(count == 0)
                throw new System.Exception("Can't build a texture 2d array from no textures!");
            var mainTex = textures.First();
            Texture2DArray array = new Texture2DArray(width, height, count, GetSupportedFormat(mainTex.format), false);
            textures.Foreach((x, i) => CopyTextureToArray(array, i, x));
            array.Apply();
            return array;
        }

        #endregion

        #region Utility

        public static void CopyTextureToArray(Texture2DArray array, int layer, Texture source) {
            var mode = source.filterMode;
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTextureUtility.GetTemporary(array.width, array.height);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            if(copyTexture == null || copyTexture.width != array.width || copyTexture.height != array.height) {
                copyTexture = new Texture2D(array.width, array.height);
            }
            copyTexture.ReadPixels(new Rect(0, 0, array.width, array.height), 0, 0);
            RenderTexture.active = null;
            RenderTextureUtility.Release(rt);
            source.filterMode = mode;
            array.SetPixels(copyTexture.GetPixels(), layer);
        }

        public static TextureFormat GetSupportedFormat(TextureFormat format) {
            switch(format) {
                case TextureFormat.DXT1:
                case TextureFormat.DXT1Crunched:
                    return TextureFormat.RGB24;
                case TextureFormat.DXT5:
                case TextureFormat.DXT5Crunched:
                    return TextureFormat.RGBA32;
                case TextureFormat.ASTC_10x10:
                case TextureFormat.ASTC_12x12:
                case TextureFormat.ASTC_4x4:
                case TextureFormat.ASTC_5x5:
                case TextureFormat.ASTC_6x6:
                case TextureFormat.ASTC_8x8:
                    return TextureFormat.RGBA32;
            }
            return format;
        }

        #endregion
    }
}
