using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio {
    public static class AudioUtility {

        private const string TAG = "[Toolkit.Audio.AudioUtility] - ";

        #region Volume Conversion

        public static float VolumeToDecibel(float volume) => Mathf.Log(Mathf.Clamp(volume, 0.001f, 2f)) * 20f;
        public static float DecibelToVolume(float dB) => Mathf.Pow(Mathematics.MathUtility.E, (dB / 20f));

        #endregion

        #region Scene

        private static UnityEngine.SceneManagement.Scene audioScene;
        public static UnityEngine.SceneManagement.Scene AudioScene {
            get {
                if(!audioScene.isLoaded) {
                    audioScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Audio");
                    if(!audioScene.isLoaded)
                        audioScene = UnityEngine.SceneManagement.SceneManager.CreateScene("Audio");
                }
                return audioScene;
            }
        }

        #endregion

        #region Internal PCM

        private static System.Reflection.EventInfo PCMSetPositionCallback;
        private static System.Reflection.MethodInfo PCMSetPositionCallback_add;
        private static System.Reflection.MethodInfo PCMSetPositionCallback_remove;

        private static System.Reflection.EventInfo PCMReaderCallback;
        private static System.Reflection.MethodInfo PCMReaderCallback_add;
        private static System.Reflection.MethodInfo PCMReaderCallback_remove;

        private static void InitializePCMCallbacks() {
            if(PCMSetPositionCallback != null)
                return;
            PCMSetPositionCallback = typeof(AudioClip).GetEvent("m_PCMSetPositionCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            PCMSetPositionCallback_add = PCMSetPositionCallback.GetAddMethod(true);
            PCMSetPositionCallback_remove = PCMSetPositionCallback.GetRemoveMethod(true);

            PCMReaderCallback = typeof(AudioClip).GetEvent("m_PCMReaderCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            PCMReaderCallback_add = PCMReaderCallback.GetAddMethod(true);
            PCMReaderCallback_remove = PCMReaderCallback.GetRemoveMethod(true);
        }

        public static void AddPCM(this AudioClip clip, AudioClip.PCMReaderCallback reader, AudioClip.PCMSetPositionCallback setPosition) {
            if(clip == null) {
                Debug.LogError(TAG + "AddPCM: Clip is null");
                return;
            }
            InitializePCMCallbacks();
            if(reader != null)
                PCMReaderCallback_add.Invoke(clip, new object[] { reader });
            if(setPosition != null)
                PCMSetPositionCallback_add.Invoke(clip, new object[] { setPosition });
        }
        public static void RemovePCM(this AudioClip clip, AudioClip.PCMReaderCallback reader, AudioClip.PCMSetPositionCallback setPosition) {
            if(clip == null) {
                Debug.LogError(TAG + "AddPCM: Clip is null");
                return;
            }
            InitializePCMCallbacks();
            if(reader != null)
                PCMReaderCallback_remove.Invoke(clip, new object[] { reader });
            if(setPosition != null)
                PCMSetPositionCallback_remove.Invoke(clip, new object[] { setPosition });
        }

        // public static void AddPCMReader(this AudioClip clip, AudioClip.PCMReaderCallback method) {
        //     InitializePCMCallbacks();
        //     PCMReaderCallback_add.Invoke(clip, new object[] { method });
        // }
        // 
        // public static void RemovePCMReader(this AudioClip clip, AudioClip.PCMReaderCallback method) {
        //     InitializePCMCallbacks();
        //     PCMReaderCallback_remove.Invoke(clip, new object[] { method });
        // }
        // 
        // public static void AddPCMSetPositionCallback(this AudioClip clip, AudioClip.PCMSetPositionCallback method) {
        //     InitializePCMCallbacks();
        //     PCMSetPositionCallback_add.Invoke(clip, new object[] { method });
        // }
        // 
        // public static void RemovePCMSetPositionCallback(this AudioClip clip, AudioClip.PCMSetPositionCallback method) {
        //     InitializePCMCallbacks();
        //     PCMSetPositionCallback_remove.Invoke(clip, new object[] { method });
        // }

        #endregion
    }
}
