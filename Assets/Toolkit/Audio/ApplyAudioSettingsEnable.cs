using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Toolkit/Audio/Apply Settings (Enable)")]
    public class ApplyAudioSettingsEnable : MonoBehaviour
    {

        #region Variables

        [SerializeField] private AudioSource source;
        [SerializeField] private AudioSourceSettingsPreset preset;

        private IAudioSourceSettings settings;

        #endregion

        #region Properties

        public AudioSource Source => source;
        public AudioSourceSettingsPreset OriginalPreset => preset;
        public IAudioSourceSettings Settings {
            get => settings;
            set {
                if(value == null)
                    settings = preset;
                else
                    settings = value;
                Apply();
            }
        }

        #endregion

        #region Apply

        private void Awake() {
            settings = preset;
        }

        private void OnEnable() => Apply();

        public void Apply() {
            if(!source)
                source = GetComponent<AudioSource>();
            if(source && settings != null)
                settings.ApplyTo(source);
        }

        #endregion
    }
}
