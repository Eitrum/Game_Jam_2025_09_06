using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Toolkit.SceneManagement;

namespace Toolkit
{
    public static class SceneExtensions
    {
        #region Add Remove

        public static void AddGameObject(this Scene scene, GameObject go) {
            if(scene.IsValid())
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);
        }

        public static void RemoveGameObject(this Scene scene, GameObject go) {
            if(scene.IsValid()) {
                var active = SceneManager.GetActiveScene();
                SceneManager.MoveGameObjectToScene(go, active);
            }
        }

        #endregion

        #region Load

        public static SceneLoadingData Load(this Scene scene)
            => Toolkit.SceneManagement.SceneUtility.Load(scene);

        #endregion

        #region Unload

        public static void Unload(this Toolkit.SceneManagement.SceneObject scene)
            => Unload(scene.Scene);

        public static void Unload(this Scene scene) {
            if(scene.isLoaded)
                Toolkit.SceneManagement.SceneUtility.Unload(scene);
        }

        #endregion
    }
}
