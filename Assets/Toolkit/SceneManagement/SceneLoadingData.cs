using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.SceneManagement
{
    public class SceneLoadingData
    {

        #region Variables

        private string sceneName = null;
        private bool allowSceneActivation = true;
        private bool setSceneActive = false;
        private bool canceled = false;
        private bool failed = false;
        private Scene? scene = null;
        private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
        private LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.None;

        private AsyncOperation operation;
        private float operationProgression = 0f;
        private float timeout = SceneUtility.TIMEOUT;

        private OnReadyToActivate onReadyToActivate;
        private OnCompleted onCompleted;
        private OnCanceled onCanceled;
        private OnFailed onFailed;

        #endregion

        #region Properties

        public string SceneName => sceneName;
        public bool AllowSceneActivation {
            get => allowSceneActivation;
            set {
                if(value != allowSceneActivation) {
                    allowSceneActivation = value;
                    if(operation != null) {
                        operation.allowSceneActivation = allowSceneActivation;
                    }
                    if(value && Progress >= 1f && !IsDone) {
                        //Check if loaded and activate the scene...
                        Activate();
                    }
                }
            }
        }
        public bool SetSceneActive => setSceneActive;
        public float Timeout {
            get => timeout;
            set => timeout = value;
        }

        // divide by 0.9 to get a range between 0 and 1 when not automatic scene activation is on. Makes this consistent between those two settings.
        public float Progress {
            get {
                if(operation == null)
                    return 0f;
                return operation.allowSceneActivation ? operation.progress : (operation.progress / 0.9f);
            }
        }

        public bool IsDone {
            get {
                if(scene.HasValue) {
                    return scene.Value.isLoaded;
                }
                if(operation == null)
                    return false;
                return operation.isDone;
            }
        }

        public bool IsCanceled {
            get => canceled;
            set {
                if(!canceled && value) {
                    Cancel();
                }
            }
        }

        public bool IsDoneOrCanceled {
            get {
                return canceled || IsDone;
            }
        }

        public bool Failed => failed;

        public Scene Scene {
            get => scene.HasValue ? scene.Value : default;
            internal set {
                scene = value;
                onCompleted?.Invoke(this);
            }
        }

        public LoadSceneMode LoadSceneMode => loadSceneMode;
        public LocalPhysicsMode LocalPhysicsMode => localPhysicsMode;
        public LoadSceneParameters LoadSceneParameters => new LoadSceneParameters(loadSceneMode, localPhysicsMode);

        public event OnReadyToActivate OnReadyToActivate {
            add {
                onReadyToActivate += value;
                if(Progress >= 1f && !IsDone) {
                    onReadyToActivate?.Invoke(this);
                }
            }
            remove => onReadyToActivate -= value;
        }

        public event OnCompleted OnCompleted {
            add {
                onCompleted += value;
                if(IsDone)
                    value(this);
            }
            remove => onCompleted -= value;
        }

        public event OnCanceled OnCanceled {
            add {
                onCanceled += value;
                if(IsCanceled)
                    value(this);
            }
            remove => onCanceled -= value;
        }

        public event OnFailed OnFailed {
            add => onFailed += value;
            remove => onFailed -= value;
        }

        #endregion

        #region Constructor

        internal SceneLoadingData(
            string sceneName,
            bool allowSceneActivation = true,
            bool setSceneActive = false,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive,
            LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.None
            ) {
            this.sceneName = sceneName;
            this.allowSceneActivation = allowSceneActivation;
            this.setSceneActive = setSceneActive;
            this.loadSceneMode = loadSceneMode;
            this.localPhysicsMode = localPhysicsMode;
        }

        #endregion

        #region Help Methods

        public void Activate() {
            if(IsDone && Scene.isLoaded) {
                // Already Has everything loaded and ready.
                return;
            }
            if(!allowSceneActivation && Progress < 1f) {
                // Change to automatic scene loading as soon as its done loading.
                AllowSceneActivation = true;
                return;
            }
            // This will trigger the scene to be loaded as soon as possible.
            operation.allowSceneActivation = true;
        }

        public void Unload() => Cancel();

        public void Cancel() {
            if(canceled) {
                return;
            }
            canceled = true;
            onCanceled?.Invoke(this);
            if(IsDone) {
                SceneUtility.Unload(Scene);
            }
        }

        #endregion

        #region Internals

        internal IEnumerator Load() { // takes care of all the loading...
            var validationScene = SceneManager.GetSceneByName(SceneName);
            if(validationScene.IsValid() && validationScene.isLoaded) {
                Debug.LogError($"<color=orange>[Scene Loading]</color> - <color=red>Could not load level as level was already loaded.</color>");
                failed = true;
                onFailed?.Invoke(this);
                yield break;
            }

            operation = SceneManager.LoadSceneAsync(SceneName, LoadSceneParameters);
            operation.allowSceneActivation = AllowSceneActivation;
            float startTime = Time.unscaledTime;
            
            while(!IsDone) {
                if(startTime + Timeout < Time.unscaledTime) { // did timeout
                    Debug.LogError($"<color=orange>[Scene Loading]</color> - <color=red>Could not load level, timeout after {Timeout:00.##} seconds</color>");
                    failed = true;
                    onFailed?.Invoke(this);
                    yield break;
                }
                Update();
                yield return null;
            }

            yield return null;

            var scene = SceneManager.GetSceneByName(SceneName);
            this.scene = scene;

            if(IsCanceled) {
                SceneUtility.Unload(scene);
                yield break;
            }
            yield return null;

            if(SetSceneActive) {
                SceneUtility.SetSceneActive(scene);
            }
            yield return null;
            onCompleted?.Invoke(this);
        }

        internal void Update() {
            if(!allowSceneActivation) {
                var currOperationProgress = operation.progress / 0.9f;
                if(operationProgression < 1.0f && currOperationProgress >= 1f) {
                    onReadyToActivate?.Invoke(this);
                }
                operationProgression = currOperationProgress;
            }
        }

        #endregion
    }
}
