using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.SceneManagement
{
    public static class SceneUtility
    {

        #region Variables

        private const string TAG = "[Toolkit.SceneManagement.SceneUtility] - ";

        /// <summary>
        /// Used to block a scene from loading if spending too much time.
        /// </summary>
        public static float TIMEOUT = 120f;
        public static event OnLoadedCallback OnLoaded;
        public static event OnUnloadedCallback OnUnloaded;

        private static List<SceneLoadingData> loadingData = new List<SceneLoadingData>();

        #endregion

        #region Properties

        public static bool IsLoading {
            get {
                VerifyLoading();
                return loadingData.Count > 0;
            }
        }

        #endregion

        #region Helpers

        public static Scene GetActiveScene() => SceneManager.GetActiveScene();
        public static Scene[] GetLoadedScenes() {
            var count = SceneManager.sceneCount;
            Scene[] scenes = new Scene[count];
            for(int i = 0; i < count; i++) {
                scenes[i] = SceneManager.GetSceneAt(i);
            }
            return scenes;
        }

        public static int SceneCount() => SceneManager.sceneCount;
        public static Scene GetScene(string name) => SceneManager.GetSceneByName(name);
        public static Scene GetScene(int index) => SceneManager.GetSceneAt(index);

        public static void SetSceneActive(Scene scene) => UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        public static void SetRootObjectsActive(this Scene scene, bool active) {
            var objs = scene.GetRootGameObjects();
            for(int i = 0; i < objs.Length; i++) {
                objs[i].SetActive(active);
            }
        }

        #endregion

        #region Loading

        public static SceneLoadingData Load(SceneObject sceneObject)
            => Load(sceneObject.Name, true, false, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(SceneObject sceneObject, bool allowSceneActivation)
            => Load(sceneObject.Name, allowSceneActivation, false, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(SceneObject sceneObject, bool allowSceneActivation, bool setSceneActive)
            => Load(sceneObject.Name, allowSceneActivation, setSceneActive, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(SceneObject sceneObject, LoadSceneMode loadSceneMode)
            => Load(sceneObject.Name, true, false, loadSceneMode, LocalPhysicsMode.None);
        public static SceneLoadingData Load(SceneObject sceneObject, bool allowSceneActivation, bool setSceneActive, LoadSceneMode loadSceneMode, LocalPhysicsMode localPhysicsMode)
            => Load(sceneObject.Name, allowSceneActivation, setSceneActive, loadSceneMode, localPhysicsMode);

        public static SceneLoadingData Load(Scene scene)
        => Load(scene, true, false, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(Scene scene, bool allowSceneActivation)
            => Load(scene, allowSceneActivation, false, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(Scene scene, bool allowSceneActivation, bool setSceneActive)
            => Load(scene, allowSceneActivation, setSceneActive, LoadSceneMode.Additive, LocalPhysicsMode.None);
        public static SceneLoadingData Load(Scene scene, LoadSceneMode loadSceneMode)
            => Load(scene, true, false, loadSceneMode, LocalPhysicsMode.None);

        public static SceneLoadingData Load(Scene scene, bool allowSceneActivation, bool setSceneActive, LoadSceneMode loadSceneMode, LocalPhysicsMode localPhysicsMode) {
            VerifyLoading();
            Debug.Log(TAG + $"Attempting to load scene: {scene.name}, and it is valid ({scene.IsValid()})");
            var sceneHelper = new SceneLoadingData(scene.name, allowSceneActivation, setSceneActive, loadSceneMode, localPhysicsMode);
            SceneLoader.Instance.Load(sceneHelper.Load());
            sceneHelper.OnCompleted += OnLoadedCallback;
            loadingData.Add(sceneHelper);
            return sceneHelper;
        }

        public static SceneLoadingData Load(string scene, bool allowSceneActivation, bool setSceneActive, LoadSceneMode loadSceneMode, LocalPhysicsMode localPhysicsMode) {
            VerifyLoading();
            // Check already loading
            foreach(var t in loadingData) {
                if(t.SceneName == scene)
                    return t;
            }
            var sceneHelper = new SceneLoadingData(scene, allowSceneActivation, setSceneActive, loadSceneMode, localPhysicsMode);
            SceneLoader.Instance.Load(sceneHelper.Load());
            sceneHelper.OnCompleted += OnLoadedCallback;
            loadingData.Add(sceneHelper);
            return sceneHelper;
        }

        private static void OnLoadedCallback(SceneLoadingData data) {
            OnLoaded?.Invoke(data);
        }

        private static void VerifyLoading() {
            for(int i = loadingData.Count - 1; i >= 0; i--) {
                if(loadingData[i].IsDoneOrCanceled)
                    loadingData.RemoveAt(i);
            }
        }

        #endregion

        #region Unloading

        public static void Unload(string scene)
            => Unload(GetScene(scene), UnloadSceneOptions.None);
        public static void Unload(string scene, UnloadSceneOptions unloadSceneOptions)
            => Unload(GetScene(scene), unloadSceneOptions);

        public static void Unload(SceneObject sceneObject)
           => Unload(sceneObject.Scene, UnloadSceneOptions.None);
        public static void Unload(SceneObject sceneObject, UnloadSceneOptions unloadSceneOptions)
            => Unload(sceneObject.Scene, unloadSceneOptions);

        public static void Unload(Scene scene)
        => Unload(scene, UnloadSceneOptions.None);

        public static void Unload(Scene scene, UnloadSceneOptions unloadSceneOptions) {
            SceneLoader.Instance.Load(UnloadScene(scene, unloadSceneOptions));
        }

        private static IEnumerator UnloadScene(Scene scene, UnloadSceneOptions unloadSceneOptions) {
            var aop = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene, unloadSceneOptions);
            while(!aop.isDone) {
                yield return null;
            }
            OnUnloaded?.Invoke(scene.name, unloadSceneOptions);
        }

        #endregion
    }
}
