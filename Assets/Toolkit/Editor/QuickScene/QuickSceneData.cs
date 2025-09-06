using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.SceneManagement {
    [System.Serializable]
    public class QuickSceneData {
        #region Variables

        public enum Mode {
            Shared,
            Local,
        }

        private const string TAG = "<color=#008080>[Toolkit.SceneManagement.QuickSceneData]</color> - ";
        private const string SHARED_PATH = "ProjectSettings";
        private const string LOCAL_PATH = "UserSettings";
        private const string FOLDER = "QuickScene";
        private const string DATA_CFG_PATH = FOLDER + "/data.cfg";

        private Mode mode;
        public List<SceneData> scenes = new List<SceneData>();

        #endregion

        public Mode CurrentMode => mode;

        public QuickSceneData(Mode mode) {
            this.mode = mode;
        }

        #region Path Util

        public static string GetConfigFilePath(Mode mode) {
            return $"{(mode == Mode.Shared ? SHARED_PATH : LOCAL_PATH)}/{DATA_CFG_PATH}";
        }

        public static string GetDirectoryPath(Mode mode) {
            return $"{(mode == Mode.Shared ? SHARED_PATH : LOCAL_PATH)}/{FOLDER}";
        }

        public string GetTextureFolderPath() {
            return $"{(mode == Mode.Shared ? SHARED_PATH : LOCAL_PATH)}/{FOLDER}";
        }

        #endregion

        #region Save / Load

        public void Load() {
            var path = GetConfigFilePath(mode);
            if(System.IO.File.Exists(path)) {
                var json = System.IO.File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, this);
            }

            foreach(var scene in scenes) {
                scene.Load();
            }

            for(int i = scenes.Count - 1; i >= 0; i--) {
                if(!scenes[i].IsValid)
                    scenes.RemoveAt(i);
            }
        }

        public void Save() {
            for(int i = 0; i < scenes.Count; i++) {
                scenes[i].Save();
            }

            var folderPath = GetDirectoryPath(mode);
            if(!System.IO.Directory.Exists(folderPath)) {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var path = GetConfigFilePath(mode);
            var json = JsonUtility.ToJson(this, true);
            System.IO.File.WriteAllText(path, json);
        }

        #endregion

        [System.Serializable]
        public class SceneData {
            #region Variables

            public string sceneGuid = "";
            public string textureFilePath = "";
            public string description = "";

            public bool IsValid => sceneAsset != null;

            [NonSerialized] public UnityEditor.SceneAsset sceneAsset;
            [NonSerialized] public Texture2D scenePreview;

            #endregion

            #region Save / Load

            public void Load() {
                var path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                if(!string.IsNullOrEmpty(path)) {
                    sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
                if(!string.IsNullOrEmpty(textureFilePath)) {
                    if(System.IO.File.Exists(textureFilePath)) {
                        var data = System.IO.File.ReadAllBytes(textureFilePath);
                        scenePreview = new Texture2D(2, 2);
                        scenePreview.name = sceneGuid;
                        scenePreview.hideFlags = HideFlags.HideAndDontSave;
                        scenePreview.LoadImage(data);
                    }
                    else {
                        Debug.LogError(TAG + "QuickScene texture is missing file but path exists: " + textureFilePath);
                    }
                }
            }

            public void Save() {
                sceneGuid = sceneAsset != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset)) : "";
            }

            #endregion
        }
    }
}
