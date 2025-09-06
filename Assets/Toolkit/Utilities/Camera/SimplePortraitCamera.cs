using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Toolkit/Camera/Simple Portrait Camera")]
    public class SimplePortraitCamera : MonoSingleton<SimplePortraitCamera>
    {
        #region Singleton

        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        protected override void OnSingletonCreated() {
            if(camReference == null) {
                Debug.LogError("Simple Portrait Camera broke!");
            }
        }

        #endregion

        #region Variables

        [Header("Lights")]
        [SerializeField] private Light mainDirectionalLight = null;
        [SerializeField] private Light subDirectionalLight = null;
        [SerializeField] private Light[] extraLights = null;

        private Camera camReference = null;


        #endregion

        #region Setup

        private void Awake() {
            camReference = GetComponent<Camera>();
            SetLightsEnabled(false);
            if(extraLights == null) {
                extraLights = new Light[0];
            }
        }

        private void Start() {
            camReference.enabled = false;
        }

        #endregion

        #region Rendering

        public static void Render(Texture2D target, Transform location) {
            Instance.transform.Copy(location, Space.World);
            Instance.Render(target);
        }

        public void Render(Texture2D texture) {
            Camera camInstance = camReference;
            SetLightsEnabled(true);
            camInstance.targetTexture = RenderTextureUtility.GetTemporary(texture.width, texture.height);
            camInstance.Render();
            var active = RenderTexture.active;
            RenderTexture.active = camInstance.targetTexture;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = active;
            RenderTextureUtility.Release(camInstance.targetTexture);
            SetLightsEnabled(false);
        }

        public void Render(RenderTexture texture) {
            Camera camInstance = camReference;
            SetLightsEnabled(true);
            camInstance.targetTexture = texture;
            camInstance.Render();
            camInstance.targetTexture = null;
            SetLightsEnabled(false);
        }

        public static void Render(Texture2D texture, Transform location, LayerMask mask)
            => Instance.InternalRender(texture, location, mask);

        public static void Render(Texture2D texture, Transform location, LayerMask mask, CameraClearFlags clearFlags)
            => Instance.InternalRender(texture, location, mask, clearFlags, Color.white);

        public static void Render(Texture2D texture, Transform location, LayerMask mask, CameraClearFlags clearFlags, Color backgroundColor)
            => Instance.InternalRender(texture, location, mask, clearFlags, backgroundColor);

        public static void Render(Texture2D texture, Transform location, LayerMask mask, bool transparent, Color backgroundColor)
            => Instance.InternalRender(texture, location, mask, transparent ? CameraClearFlags.Depth : CameraClearFlags.SolidColor, backgroundColor);

        public static void Render(Texture2D texture, Transform location, float fov, LayerMask mask, bool transparent, Color backgroundColor)
                    => Instance.InternalRender(texture, location, fov, mask, transparent ? CameraClearFlags.Depth : CameraClearFlags.SolidColor, backgroundColor);

        public static void Render(RenderTexture texture, Transform location, float fov, LayerMask mask, bool transparent, Color backgroundColor)
                            => Instance.InternalRender(texture, location, fov, mask, transparent ? CameraClearFlags.Depth : CameraClearFlags.SolidColor, backgroundColor);

        private void InternalRender(Texture2D texture, Transform location, LayerMask mask) {
            // Set Values
            var cm = camReference.cullingMask;
            camReference.cullingMask = mask;
            // Render
            transform.Copy(location, Space.World);
            Render(texture);
            // Revert Values
            camReference.cullingMask = cm;
        }

        private void InternalRender(Texture2D texture, Transform location, LayerMask mask, CameraClearFlags clearFlags, Color backgroundColor) {
            // Set Values
            var bgc = camReference.backgroundColor;
            var cm = camReference.cullingMask;
            var cf = camReference.clearFlags;
            camReference.backgroundColor = backgroundColor;
            camReference.cullingMask = mask;
            camReference.clearFlags = clearFlags;
            // Render
            transform.Copy(location, Space.World);
            Render(texture);
            // Revert Values
            camReference.backgroundColor = bgc;
            camReference.cullingMask = cm;
            camReference.clearFlags = cf;
        }

        private void InternalRender(Texture2D texture, Transform location, float fov, LayerMask mask, CameraClearFlags clearFlags, Color backgroundColor) {
            // Set Values
            var bgc = camReference.backgroundColor;
            var cm = camReference.cullingMask;
            var cf = camReference.clearFlags;
            var oldFov = camReference.fieldOfView;
            camReference.backgroundColor = backgroundColor;
            camReference.cullingMask = mask;
            camReference.clearFlags = clearFlags;
            camReference.fieldOfView = fov;
            // Render
            transform.Copy(location, Space.World);
            Render(texture);
            // Revert Values
            camReference.backgroundColor = bgc;
            camReference.cullingMask = cm;
            camReference.clearFlags = cf;
            camReference.fieldOfView = oldFov;
        }

        private void InternalRender(RenderTexture texture, Transform location, float fov, LayerMask mask, CameraClearFlags clearFlags, Color backgroundColor) {
            // Set Values
            var bgc = camReference.backgroundColor;
            var cm = camReference.cullingMask;
            var cf = camReference.clearFlags;
            var oldFov = camReference.fieldOfView;
            camReference.backgroundColor = backgroundColor;
            camReference.cullingMask = mask;
            camReference.clearFlags = clearFlags;
            camReference.fieldOfView = fov;
            // Render
            transform.Copy(location, Space.World);
            Debug.Log("Rendering with camera @ " + transform.position);
            Render(texture);
            // Revert Values
            camReference.backgroundColor = bgc;
            camReference.cullingMask = cm;
            camReference.clearFlags = cf;
            camReference.fieldOfView = fov;
        }

        #endregion

        #region Light

        public void SetLightsEnabled(bool active) {
            camReference.enabled = active;
            if(mainDirectionalLight)
                mainDirectionalLight.enabled = active;
            if(subDirectionalLight)
                subDirectionalLight.enabled = active;
            if(extraLights != null) {
                foreach(var el in extraLights) {
                    el.enabled = active;
                }
            }
        }

        public void SetDirectionalLightInputAngle(Vector3 forward) =>
            SetDirectionalLightInputAngle(Quaternion.LookRotation(forward, Vector3.up));

        public void SetDirectionalLightInputAngle(Quaternion rotation) {
            if(mainDirectionalLight) {
                mainDirectionalLight.transform.localRotation = rotation;
                if(subDirectionalLight) {
                    var subDir = Vector3.Reflect(rotation * Vector3.forward, Vector3.forward);
                    subDirectionalLight.transform.localRotation = Quaternion.LookRotation(-subDir, Vector3.forward);
                }
            }
        }

        #endregion
    }
}
