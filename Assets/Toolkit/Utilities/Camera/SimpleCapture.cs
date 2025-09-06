using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Canera/Capture (Simple)")]
    [RequireComponent(typeof(Camera))]
    public class SimpleCapture : MonoBehaviour
    {
        #region Delegate

        public delegate void OnCaptureToFileCallback(string output);
        public delegate void OnCaptureToTextureCallback(Texture2D output);

        #endregion

        #region Variables

        private const string TAG = "<color=cyan>[Simple Capture]</color> - ";

        [SerializeField] private Screenshot.Format format = Screenshot.Format.PNG;
        [SerializeField] private Mode mode = Mode.CustomValues;
        [SerializeField, Min(1)] private int width = 1920;
        [SerializeField, Min(1)] private int height = 1080;
        [SerializeField, Range(0.001f, 16f)] private float upscale = 2f;
        [SerializeField] private KeyCode captureKey = KeyCode.None;

        private string directory;
        private Camera cam;
        public event OnCaptureToFileCallback OnCaptureToFile;
        public event OnCaptureToTextureCallback OnCaptureToTexture;

        #endregion

        #region Properties

        public Camera Camera {
            get {
                if(!cam)
                    cam = GetComponent<Camera>();
                return cam;
            }
        }

        public Screenshot.Format Format => format;

        public int Width {
            get {
                switch(mode) {
                    case Mode.CustomValues: return width;
                    case Mode.Upscale: return Mathf.CeilToInt(Screen.width * upscale);
                    case Mode.Screen: return Screen.width;
                }
                throw new System.Exception("Capture mode is not set to a supported value");
            }
        }

        public int Height {
            get {
                switch(mode) {
                    case Mode.CustomValues: return height;
                    case Mode.Upscale: return Mathf.CeilToInt(Screen.height * upscale);
                    case Mode.Screen: return Screen.height;
                }
                throw new System.Exception("Capture mode is not set to a supported value");
            }
        }

        /// <summary>
        /// Width / Height
        /// </summary>
        public float Ratio => (float)Width / (float)Height;

        public string Directory {
            get {
                if(string.IsNullOrEmpty(directory))
                    return Screenshot.DEFAULT_DIRECTORY;
                return directory;
            }
            set {
                if(string.IsNullOrEmpty(value))
                    directory = null;
                else if(System.IO.Directory.Exists(value)) {
                    directory = value;
                }
                else {
                    try {
                        var hasExtension = System.IO.Path.HasExtension(value);
                        var tpath = hasExtension ? System.IO.Path.GetDirectoryName(value) : value;
                        directory = tpath;
                    }
                    catch(System.Exception e) {
                        Debug.LogException(e);
                        Debug.LogWarning(TAG + $"Directory '{value}' not found. Applying default directory to capture on '{name}'");
                        directory = "";
                    }
                }
            }
        }

        #endregion

        #region Capture

        public string CaptureToFile() {
            var output = Screenshot.CaptureToFile(Camera, Width, Height, format);
            OnCaptureToFile?.Invoke(output);
            return output;
        }

        public string CaptureToFile(string directory) {
            var output = Screenshot.CaptureToFile(Camera, Width, Height, directory, format);
            OnCaptureToFile?.Invoke(output);
            return output;
        }

        public Texture2D Capture() {
            var output = Screenshot.Capture(Camera, Width, Height);
            OnCaptureToTexture?.Invoke(output);
            return output;
        }

        public Texture2D Capture(bool transparent) {
            var output = transparent ? Screenshot.CaptureTransparent(Camera, Width, Height) : Screenshot.Capture(Camera, Width, Height);
            OnCaptureToTexture?.Invoke(output);
            return output;
        }

        #endregion

        #region Update

        private void Update() {
            if(captureKey == KeyCode.None)
                return;
            if(Input.GetKeyDown(captureKey)) {
                if(string.IsNullOrEmpty(directory))
                    CaptureToFile();
                else
                    CaptureToFile(directory);
            }
        }

        #endregion

        public enum Mode
        {
            Screen,
            Upscale,
            CustomValues,
        }
    }
}
