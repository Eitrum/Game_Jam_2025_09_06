
using System;
using UnityEngine;

namespace Toolkit.Rendering {
    public static class CameraUtility {

        #region Orthographic Size Scope

        public class OrthographicSizeScope : IDisposable {

            #region Variables

            public readonly Camera Camera;
            public readonly float Previous;
            public readonly float Current;

            private bool isDisposed = false;

            #endregion

            #region Constructor

            public OrthographicSizeScope(Camera cam, float orthographicSize) {
                this.Camera = cam;
                this.Current = orthographicSize;
                Previous = cam.orthographicSize;
                cam.orthographicSize = orthographicSize;
            }

            #endregion

            #region Dispose

            ~OrthographicSizeScope() => Dispose();

            public void Dispose() {
                if(isDisposed)
                    return;
                isDisposed = true;
                if(Camera)
                    Camera.orthographicSize = Previous;
            }

            #endregion
        }

        #endregion

        #region Field of View Scope

        public class FieldOfViewScope : IDisposable {

            #region Variables

            public readonly Camera Camera;
            public readonly float Previous;
            public readonly float Current;

            private bool isDisposed = false;

            #endregion

            #region Constructor

            public FieldOfViewScope(Camera cam, float fieldOfView) {
                this.Camera = cam;
                this.Current = fieldOfView;
                Previous = cam.fieldOfView;
                cam.fieldOfView = fieldOfView;
            }

            #endregion

            #region Dispose

            ~FieldOfViewScope() => Dispose();

            public void Dispose() {
                if(isDisposed)
                    return;
                isDisposed = true;
                if(Camera)
                    Camera.fieldOfView = Previous;
            }

            #endregion
        }

        #endregion

        #region FarClipPlane Scope

        public class FarClipPlaneScope : IDisposable {

            #region Variables

            public readonly Camera Camera;
            public readonly float Previous;
            public readonly float Current;

            private bool isDisposed = false;

            #endregion

            #region Constructor

            public FarClipPlaneScope(Camera cam, float distance) {
                this.Camera = cam;
                this.Current = distance;
                Previous = cam.farClipPlane;
                cam.farClipPlane = distance;
            }

            #endregion

            #region Dispose

            ~FarClipPlaneScope() => Dispose();

            public void Dispose() {
                if(isDisposed)
                    return;
                isDisposed = true;
                if(Camera)
                    Camera.farClipPlane = Previous;
            }

            #endregion
        }

        #endregion

        #region NearClipPlane Scope

        public class NearClipPlaneScope : IDisposable {

            #region Variables

            public readonly Camera Camera;
            public readonly float Previous;
            public readonly float Current;

            private bool isDisposed = false;

            #endregion

            #region Constructor

            public NearClipPlaneScope(Camera cam, float distance) {
                this.Camera = cam;
                this.Current = distance;
                Previous = cam.nearClipPlane;
                cam.nearClipPlane = distance;
            }

            #endregion

            #region Dispose

            ~NearClipPlaneScope() => Dispose();

            public void Dispose() {
                if(isDisposed)
                    return;
                isDisposed = true;
                if(Camera)
                    Camera.nearClipPlane = Previous;
            }

            #endregion
        }

        #endregion
    }
}
