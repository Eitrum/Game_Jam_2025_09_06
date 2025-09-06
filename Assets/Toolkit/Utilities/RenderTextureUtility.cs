using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public static class RenderTextureUtility
    {
        #region Variables

        private static List<int> addedFrameIndex = new List<int>();
        private static List<RenderTexture> unused = new List<RenderTexture>();
        private static List<RenderTexture> released = new List<RenderTexture>();
        private static Coroutine routine;
        private static int index = 0;
        private static Nullable nullable = new Nullable();
        private static bool isUpdating = false;

        #endregion

        #region Get Temporary

        public static RenderTexture GetTemporary(int width, int height)
            => GetTemporary(width, height, 32);
        public static RenderTexture GetTemporary(int width, int height, int depth) {
            for(int i = unused.Count - 1; i >= 0; i--) {
                var tex = unused[i];
                if(tex == null) {
                    unused.RemoveAt(i);
                    continue;
                }
                if(tex.width == width && tex.height == height && tex.depth == depth) {
                    unused.RemoveAt(i);
                    return tex;
                }
            }

            return new RenderTexture(width, height, depth);
        }

        #endregion

        #region Release

        public static void Release(RenderTexture texture) {
            if(Application.isPlaying) {
                if(Time.frameCount > index) {
                    UpdateTextures();
                    index = Time.frameCount;
                }
                released.Add(texture);
                Timer.NextFrame(UpdateTextures, ref routine);
                if(!isUpdating) {
                    UpdateSystem.Subscribe(nullable, 1f, Update);
                    isUpdating = true;
                }
            }
            else {
                Object.DestroyImmediate(texture);
            }
        }

        #endregion

        #region Updates

        private static void UpdateTextures() {
            var currentFrame = Time.frameCount;
            for(int i = 0, length = released.Count; i < length; i++) {
                unused.Add(released[i]);
                addedFrameIndex.Add(currentFrame);
            }
            released.Clear();
        }

        private static void Update(float dt) {
            UpdateTextures();
            var frame = Time.frameCount - 1000;
            for(int i = 0; i < unused.Count; i++) {
                if(addedFrameIndex[i] < frame) {
                    addedFrameIndex.RemoveAt(i);
                    unused.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        #region private class

        private class Nullable : INullable
        {
            public bool IsNull { get; set; } = false;
        }

        #endregion
    }
}
