using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public abstract class MRPModule : ScriptableObject {

        #region Variables

        [SerializeField, Hide(true)] protected bool disable = false;
        [SerializeField, Hide(true)] protected bool debug = false;
        [SerializeField, Hide(true), PerformanceResults] private long ticks = 0;
        private CircularBuffer<long> tickHistory = new CircularBuffer<long>(8);
        private long timeStamp;
        public long Ticks {
            get => ticks;
            private set {
                tickHistory.Write(value);
                ticks = tickHistory.Average();
            }
        }
        public static bool ShowErrors = true;

        #endregion

        #region Run

        public void HandleModule(in ScriptableRenderContext context, MRPContext data) {
            if(disable)
                return;
            if(!Initialize())
                return;
            timeStamp = PerformanceUtility.Timestamp;
            Run(context, data);
            Ticks = PerformanceUtility.GetElapsedTicks(timeStamp);
        }

        protected virtual bool Initialize() => true;

        protected abstract void Run(in ScriptableRenderContext context, MRPContext data);

        #endregion

        #region Dispose

        public abstract void Dispose();

        #endregion

        #region Helpers

        public static void Initialize(NSOReferenceArray<MRPModule> modules) {
            foreach(var m in modules)
                if(m != null)
                    m.Initialize();
        }

        public static void HandleModules(NSOReferenceArray<MRPModule> modules, in ScriptableRenderContext context, MRPContext data) {
            foreach(var m in modules)
                if(m != null)
                    m.HandleModule(context, data);
        }

        public static void Dispose(NSOReferenceArray<MRPModule> modules) {
            foreach(var m in modules)
                if(m != null)
                    m.Dispose();
        }

        public static CommandBuffer GetCommandBuffer() => CommandBufferPool.Get(string.Empty);
        public static CommandBuffer GetCommandBuffer(string name) => CommandBufferPool.Get(name);
        public static void Release(CommandBuffer buffer) => CommandBufferPool.Release(buffer);

        #endregion
    }
}
