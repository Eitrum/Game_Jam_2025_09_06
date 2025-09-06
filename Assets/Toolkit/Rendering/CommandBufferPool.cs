using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering {
    public static class CommandBufferPool {
        #region Variables

        private const string TAG = "[Toolkit.Rendering.CommandBufferPool] - ";
        public const int MAXIMUM_POOLED_BUFFERS = 16;

        private static FastPool<CommandBuffer> pool = new FastPool<CommandBuffer>(MAXIMUM_POOLED_BUFFERS);

        #endregion

        #region Init

        [RecompileCleanup]
        private static void Cleanup() {
            pool.Dispose(x => x.Release());
        }

        #endregion

        #region Get

        public static CommandBuffer Get() {
            var cb = pool.Get();
            cb.Clear();
            cb.name = string.Empty;
            return cb;
        }

        public static CommandBuffer Get(string name) {
            var cb = pool.Get();
            cb.Clear();
            cb.name = name;
            return cb;
        }

        #endregion

        #region Release

        public static void Release(CommandBuffer buffer) {
            if(buffer == null) {
                Debug.LogError(TAG + $"CommandBuffer is null!");
                return;
            }
            buffer.Clear();
            if(!pool.Push(buffer)) // If failed to return buffer, release/dispose it.
                buffer.Release();
        }

        #endregion
    }
}
