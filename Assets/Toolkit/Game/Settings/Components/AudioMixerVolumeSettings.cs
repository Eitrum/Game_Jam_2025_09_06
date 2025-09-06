using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Audio/Audio Mixer", typeof(ISettings))]
    public class AudioMixerVolumeSettings : ScriptableObject, ISettings {

        [System.Serializable]
        private class VolumeSettings {
            #region Variables

            [Tooltip("exposed mixer volume name")]
            [SerializeField] private string exposedVariable;
            [SerializeField] private string overrideSettingsName = "";
            [SerializeField, RangeEx(0f, 1f, 0.05f)] private float defaultVolume = 0.7f;
            [SerializeField] private MinMax range = new MinMax(0f, 1f);

            /// <summary>
            /// Used to fix initialization issues.
            /// </summary>
            public Coroutine Routine;

            #endregion

            #region Properties

            public string ExposedVariableName => exposedVariable;
            public string SettingsName => string.IsNullOrEmpty(overrideSettingsName) ? exposedVariable : overrideSettingsName;
            public float DefaultVolume => defaultVolume;
            public float Min => range.min;
            public float Max => range.max;

            #endregion

            public VolumeSettings() {
                defaultVolume = 0.7f;
            }
        }

        #region Variables

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private VolumeSettings[] volumeSettings;

        #endregion

        #region Initialize

        public void Initialize(string groupName) {
            foreach(var vs in volumeSettings)
                Initialize(groupName, vs);
        }

        private void Initialize(string groupName, VolumeSettings setting) {
            InGameSettings.RegisterRange(groupName, setting.SettingsName, setting.DefaultVolume, setting.Min, setting.Max, (float volume) => {
                Debug.Log($"Audio mixer updating : " + setting.SettingsName + " " + volume);
                bool result = mixer.SetFloat(setting.ExposedVariableName, Audio.AudioUtility.VolumeToDecibel(volume));
                if(!result) {
                    Debug.LogError("Failed to update volume: " + setting.SettingsName);
                    return;
                }
                mixer.GetFloat(setting.ExposedVariableName, out float newlyAssigned);
                if(!Mathf.Approximately(volume, newlyAssigned)) {
                    Timer.NextFrame(() => mixer.SetFloat(setting.ExposedVariableName, Audio.AudioUtility.VolumeToDecibel(volume)), ref setting.Routine);
                }
            });
        }

        #endregion
    }
}
