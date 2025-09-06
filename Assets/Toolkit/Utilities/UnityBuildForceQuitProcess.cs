using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Internal {
    internal class UnityBuildForceQuitProcess : MonoBehaviour {

#if !UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            var go = new GameObject("Force Quit Process");
            DontDestroyOnLoad(go);
        }

        void OnDestroy() {
            Application.Quit();
            OnApplicationQuit();
        }

        void OnApplicationQuit() {
            System.Environment.Exit(0);
        }

#endif
    }
}
