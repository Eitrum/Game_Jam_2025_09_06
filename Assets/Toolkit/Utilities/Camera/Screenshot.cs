using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    public static class Screenshot
    {
        public enum Format
        {
            PNG,
            RAW,
            JPG,
            EXR,
            TGA,
        }

        #region Variables

        public const string DEFAULT_DIRECTORY = "screenshots";
        public const string DEFAULT_FILE_NAME = "capture_";
        private const string DEFAULT_FILE_NAME_SEARCH = DEFAULT_FILE_NAME + "*";

        #endregion

        #region Capture To File

        public static string CaptureToFile(Camera camera, int width, int height)
            => WriteToFile(Capture(camera, width, height), DEFAULT_DIRECTORY, Format.PNG);

        public static string CaptureToFile(Camera camera, int width, int height, Format format)
            => WriteToFile(Capture(camera, width, height), DEFAULT_DIRECTORY, format);

        public static string CaptureToFile(Camera camera, int width, int height, string directory)
            => WriteToFile(Capture(camera, width, height), directory, Format.PNG);

        public static string CaptureToFile(Camera camera, int width, int height, string directory, Format format) {
            var tex = Capture(camera, width, height);
            var output = WriteToFile(tex, directory, format);
            if(Application.isPlaying)
                Texture2D.Destroy(tex);
            else
                Texture2D.DestroyImmediate(tex);
            return output;
        }

        #endregion

        #region Capture

        public static Texture2D Capture(Camera camera)
            => Capture(camera, Screen.width, Screen.height);

        public static Texture2D Capture(Camera camera, int width, int height) {
            // Setup
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            var tempRT = RenderTextureUtility.GetTemporary(width, height, 24);

            // Draw
            camera.targetTexture = tempRT;
            camera.Render();

            // Copy
            RenderTexture.active = tempRT;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Restore
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTextureUtility.Release(tempRT);

            return texture;
        }

        public static Texture2D CaptureWithShader(Camera camera, int width, int height, Shader shader, string replacementTag = "") {
            // Setup
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            var tempRT = RenderTextureUtility.GetTemporary(width, height, 24);

            // Draw
            camera.targetTexture = tempRT;
            camera.RenderWithShader(shader, replacementTag);

            // Copy
            RenderTexture.active = tempRT;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Restore
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTextureUtility.Release(tempRT);

            return texture;
        }

        public static Texture2D CaptureTransparent(Camera camera)
            => CaptureTransparent(camera, Screen.width, Screen.height);
        
        public static Texture2D CaptureTransparent(Camera camera, int width, int height) {
            // Setup
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var tempRT = RenderTextureUtility.GetTemporary(width, height, 32);

            // Draw
            camera.targetTexture = tempRT;
            camera.Render();

            // Copy
            RenderTexture.active = tempRT;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Restore
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTextureUtility.Release(tempRT);

            return texture;
        }

        public static Texture2D CaptureTransparentWithShader(Camera camera, int width, int height, Shader shader, string replacementTag = "") {
            // Setup
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var tempRT = RenderTextureUtility.GetTemporary(width, height, 32);

            // Draw
            camera.targetTexture = tempRT;
            camera.RenderWithShader(shader, replacementTag);

            // Copy
            RenderTexture.active = tempRT;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Restore
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTextureUtility.Release(tempRT);

            return texture;
        }

        #endregion

        #region Utility

        public static byte[] Encode(Texture2D texture, Format format) {
            switch(format) {
                case Format.PNG: return texture.EncodeToPNG();
                case Format.JPG: return texture.EncodeToJPG();
                case Format.RAW: return texture.GetRawTextureData();
                case Format.EXR: return texture.EncodeToEXR();
                case Format.TGA: return texture.EncodeToTGA();
            }
            return null;
        }

        public static string GetExtension(Format format) {
            switch(format) {
                case Format.PNG: return "png";
                case Format.JPG: return "jpg";
                case Format.RAW: return "raw";
                case Format.EXR: return "exr";
                case Format.TGA: return "tga";
            }
            return "err";
        }


        public static string WriteToFile(Texture2D texture, Format format)
           => WriteToFile(texture, DEFAULT_DIRECTORY, format);

        public static string WriteToFile(Texture2D texture)
            => WriteToFile(texture, DEFAULT_DIRECTORY, Format.PNG);


        public static string WriteToFile(Texture2D texture, string directory, Format format) {
            var data = Encode(texture, format);
            if(!System.IO.Directory.Exists(directory)) {
                System.IO.Directory.CreateDirectory(directory);
            }
            var files = System.IO.Directory.GetFiles(directory, DEFAULT_FILE_NAME_SEARCH, System.IO.SearchOption.TopDirectoryOnly);
            var index = 0;
            for(int i = 0; i < files.Length; i++) {
                var s = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                if(s.Length > 8) {
                    s = s.Remove(0, 8);
                    if(int.TryParse(s, out int res) && res > index)
                        index = res;
                }
            }
            index++;
            var output = System.IO.Path.Combine(directory, DEFAULT_FILE_NAME + $"{index}.{GetExtension(format)}");
            System.IO.File.WriteAllBytes(output, data);
            Debug.Log($"Screenshot at: " + output);
            return output;
        }

        #endregion

        #region Editor

        public static Texture2D CaptureSceneViewCamera() {
#if UNITY_EDITOR
            var cam = UnityEditor.SceneView.lastActiveSceneView.camera;
            var size = UnityEditor.SceneView.lastActiveSceneView.position.size.FloorToInt();
            if(cam)
                return Screenshot.Capture(cam, size.x, size.y);
            Debug.LogError("Camera does not exists!");
            return null;
#else
            return null;
#endif
        }

        public static Texture2D CaptureSceneViewCamera(int width, int height) {
#if UNITY_EDITOR
            var cam = UnityEditor.SceneView.lastActiveSceneView.camera;
            if(cam)
                return Screenshot.Capture(cam, width, height);
            Debug.LogError("Camera does not exists!");
            return null;
#else
            return null;
#endif
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Toolkit/Editor/Screenshot/Take Screenshot (1x)")]
        private static void EditorScreenshot() {
            var cam = UnityEditor.SceneView.lastActiveSceneView.camera;
            var size = UnityEditor.SceneView.lastActiveSceneView.position.size.FloorToInt();
            if(cam) {
                Screenshot.CaptureToFile(cam, size.x, size.y);
            }
            else {
                Debug.LogError("Camera does not exists!");
            }
        }

        [UnityEditor.MenuItem("Toolkit/Editor/Screenshot/Take Screenshot (4x)")]
        private static void EditorScreenshotUpscale() {
            var cam = UnityEditor.SceneView.lastActiveSceneView.camera;
            var size = UnityEditor.SceneView.lastActiveSceneView.position.size.FloorToInt();
            if(cam) {
                Screenshot.CaptureToFile(cam, Mathf.CeilToInt(size.x * 4f), Mathf.CeilToInt(size.y * 4f));
            }
            else {
                Debug.LogError("Camera does not exists!");
            }
        }
#endif
        #endregion
    }
}
