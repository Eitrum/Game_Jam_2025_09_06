using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Utility/Load Scene (OnTrigger)")]
    public class LoadSceneOnTrigger : MonoBehaviour {
        #region Variables
        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private SceneObject sceneToLoad;
        [SerializeField] private bool setMainScene;

        [SerializeField] private UnloadMode unloadMode = UnloadMode.Nothing;
        [SerializeField, Min(0f)] private float unloadDelay = 0f;
        [SerializeField] private bool unloadCurrent = true;
        [SerializeField] private SceneObject[] otherScenesToUnload;

        private Scene currentScene;
        private ITrigger trigger;

        #endregion

        #region Init

        private void Awake() {
            trigger = GetComponentInParent<ITrigger>();
            currentScene = gameObject.scene;
        }

        private void OnEnable() {
            if(trigger != null)
                trigger.OnTrigger += OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger += OnTrigger;
        }

        private void OnDisable() {
            if(trigger != null)
                trigger.OnTrigger -= OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger -= OnTrigger;
        }

        #endregion

        #region Callback

        [Button, ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                OnTrigger(s);
        }

        private void OnTrigger(Source source) {
            if(unloadMode == UnloadMode.Before)
                Unload();

            var load = Toolkit.SceneManagement.SceneUtility.Load(sceneToLoad, true, setMainScene);
            if(!load.AllowSceneActivation) {
                load.AllowSceneActivation = true;
            }

            if(unloadMode == UnloadMode.After)
                load.OnCompleted += (t) => Unload();
        }

        void Unload() {
            if(unloadDelay > Mathf.Epsilon)
                Timer.Once(unloadDelay, Internal_Unload);
            else
                Internal_Unload();
        }

        private void Internal_Unload() {
            if(unloadCurrent)
                currentScene.Unload();
            foreach(var s in otherScenesToUnload)
                s.Unload();
        }

        #endregion

        public enum UnloadMode {
            Nothing = 0,
            Before = 1,
            After = 2,
        }
    }
}
