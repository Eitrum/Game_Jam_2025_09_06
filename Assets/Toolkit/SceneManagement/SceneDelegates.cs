using UnityEngine.SceneManagement;

namespace Toolkit.SceneManagement {
    public delegate void OnLoadedCallback(SceneLoadingData sceneHelperData);
    public delegate void OnUnloadedCallback(string sceneName, UnloadSceneOptions unloadSceneOptions);

    public delegate void OnReadyToActivate(SceneLoadingData sceneHelperData);
    public delegate void OnCompleted(SceneLoadingData sceneHelperData);
    public delegate void OnCanceled(SceneLoadingData sceneHelperData);
    public delegate void OnFailed(SceneLoadingData sceneHelperData);
}
