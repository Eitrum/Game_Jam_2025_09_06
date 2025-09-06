using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Toolkit/Audio/Apply Settings (Awake)")]
    public class ApplyAudioSettingsAwake : MonoBehaviour
    {
        #region Variables

        [SerializeField] private AudioSource source;
        [SerializeField] private AudioSourceSettingsPreset preset;

        #endregion

        #region Properties

        public AudioSource Source => source;
        public AudioSourceSettingsPreset Preset => preset;

        #endregion

        #region Apply

        private void Awake() => Apply();

        public void Apply() {
            if(!source)
                source = GetComponent<AudioSource>();
            preset.ApplyTo(source);
        }

        #endregion
    }
}
