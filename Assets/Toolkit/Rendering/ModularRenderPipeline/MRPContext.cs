using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public class MRPContext : IDisposable {
        #region Variables

        private int width;
        private int height;

        private ScriptableRenderContext renderContext;

        private Camera targetCamera;
        private List<Camera> cameras;

        private Dictionary<string, object> data;

        private static FastPool<Dictionary<string, object>> dataPool = new FastPool<Dictionary<string, object>>();

        #endregion

        #region Properties

        public int Width => width;
        public int Height => height;

        public Camera TargetCamera => targetCamera;
        public IReadOnlyList<Camera> Cameras => cameras;
        public ScriptableRenderContext RenderContext => renderContext;

        #endregion

        #region Constructor

        public MRPContext(in ScriptableRenderContext context, List<Camera> cameras) {
            this.renderContext = context;
            this.cameras = cameras;
            data = dataPool.Pop();
            CopyScreenSize();
        }

        ~MRPContext() {
            Dispose();
        }

        #endregion

        #region Disposable

        public void Dispose() {
            if(data != null) {
                data.Clear();
                dataPool.Push(data);
                data = null;
            }
        }

        #endregion

        #region Camera

        public void SetTarget(Camera camera) {
            this.targetCamera = camera;
        }

        #endregion

        #region Size

        public void CopyScreenSize() {
            this.width = Screen.width;
            this.height = Screen.height;
        }

        public void SetWidth(int width) { this.width = width; }

        public void SetHeight(int height) { this.height = height; }

        public void SetSize(int width, int height) {
            this.width = width;
            this.height = height;
        }

        #endregion

        #region Generic Data

        public void Set<T>(string key, T value) {
            data[key] = value;
        }

        public void Remove(string key) {
            data.Remove(key);
        }

        public bool TryGet<T>(string key, out T value) {
            if(data.TryGetValue(key, out object obj) && obj is T tobj) {
                value = tobj;
                return true;
            }
            value = default;
            return false;
        }

        #endregion
    }
}
