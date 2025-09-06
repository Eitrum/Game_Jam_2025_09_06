using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class PanelStorageRegister : ScriptableSingleton<PanelStorageRegister> {

        [System.Serializable]
        public class Entry {
            public string name;
            public GameObject panelPrefab;

            public string Name => string.IsNullOrEmpty(name) ? (panelPrefab ? panelPrefab.name : string.Empty) : name;
        }

        #region Singleton

        protected override bool KeepInResources => true;

        protected override void OnSingletonCreated() {
            Initialize();
        }

        protected override void OnSingletonDestroyed() {
            Uninitialize();
        }

        #endregion

        #region Variables

        [SerializeField] private Entry[] entries = new Entry[0];

        #endregion

        #region Methods

        [Button(EditorGUIMode.RuntimeOnly)]
        public void Initialize() {
            foreach(var e in entries)
                if(e.panelPrefab != null)
                    PanelStorage.Register(e.Name, e.panelPrefab, false);
        }

        [Button(EditorGUIMode.RuntimeOnly)]
        public void Uninitialize() {
            foreach(var e in entries)
                PanelStorage.Unregister(e.Name);
        }

        #endregion
    }
}
