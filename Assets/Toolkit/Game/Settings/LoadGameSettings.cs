using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings {
    [AddComponentMenu("Toolkit/Game/Load Game Settings")]
    public class LoadGameSettings : MonoBehaviour {

        public enum InvokeType {
            None = -1,
            Awake = 0,
            OnEnable = 1,
            Start = 2,
        }

        #region Variables

        private const string TAG = "[Toolkit.Game.LoadGameSettings] - ";
        [SerializeField] private InvokeType load = InvokeType.Awake;
        [SerializeField] private string customFilePath = string.Empty;
        [SerializeField] private SettingsList[] settingsToInitialize = { };

        #endregion

        #region Init

        private void Awake() {
            if(load == InvokeType.Awake) {
                InitializeSettings();
                Load();
            }
        }

        private void OnEnable() {
            if(load == InvokeType.OnEnable) {
                InitializeSettings();
                Load();
            }
        }

        void Start() {
            if(load == InvokeType.Start) {
                InitializeSettings();
                Load();
            }
        }

        #endregion

        #region Save / load Methods

        [Button]
        public void AssignNewFileLocation() {
            if(!string.IsNullOrEmpty(customFilePath)) {
                string path = customFilePath.StartsWith('/') || customFilePath.StartsWith('\\') ?
                        (Application.persistentDataPath + customFilePath) :
                        (Application.persistentDataPath + "/" + customFilePath);
                Debug.Log(TAG + $"Assigning a new file path location: '{path}'");
                InGameSettings.FileLocation = path;
            }
            else
                InGameSettings.FileLocation = null;
        }

        [Button]
        public void Save() {
            var result = InGameSettings.Save();
            Debug.Log(TAG + "Saved the game settings " + (result ? "succesfully" : "unsuccesfully"));
        }

        [Button]
        public void InitializeSettings() {
            foreach(var s in settingsToInitialize)
                if(s != null)
                    s.Initialize();
        }

        [Button]
        public void Load() {
            if(!string.IsNullOrEmpty(customFilePath)) {
                string path = customFilePath.StartsWith('/') || customFilePath.StartsWith('\\') ?
                        (Application.persistentDataPath + customFilePath) :
                        (Application.persistentDataPath + "/" + customFilePath);
                Debug.Log(TAG + $"Assigning a new file path location: '{path}'");
                InGameSettings.FileLocation = path;
            }
            var result = InGameSettings.Load();
            Debug.Log(TAG + "Loaded the game settings " + (result ? "succesfully" : "unsuccesfully"));
        }

        [Button]
        public void OpenFile() {
            Application.OpenURL(InGameSettings.FileLocation);
        }

        [Button]
        public void OpenFileLocation() {
            try {
                var path = System.IO.Path.GetDirectoryName(InGameSettings.FileLocation);
                Application.OpenURL(path);
            }
            catch {

            }
        }

        [Button]
        private void PrintSettings() {
            var node = InGameSettings.Serialize();
            var text = Toolkit.IO.TML.TMLParser.ToString(node, true);
            Debug.Log(TAG + text);
        }

        [DebugView]
        private string GetDebugInfo() {
            return InGameSettings.FileLocation;
        }

        #endregion
    }
}
