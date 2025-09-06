using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [CreateAssetMenu(fileName = "Audio Source Settings", menuName = "Toolkit/Audio/Audio Source Settings")]
    public class AudioSourceSettingsPreset : ScriptableObject, IAudioSourceSettings
    {
        #region Variables

        [SerializeField] private UnityEngine.Audio.AudioMixerGroup output = null;
        [SerializeField] private bool bypassEffects = false;
        [SerializeField] private bool bypassListenerEffects = false;
        [SerializeField] private bool bypassReverbZones = false;
        [SerializeField, Range(0, 255)] private int priority = 128;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
        [SerializeField, Range(-1f, 1f)] private float stereoPan = 0f;
        [SerializeField, Range(0f, 1f)] private float spatialBlend = 0f;
        [SerializeField, Range(0f, 1.1f)] private float reverbZoneMix = 1f;
        [SerializeField, Range(0f, 5f)] private float dopplerLevel = 1f;
        [SerializeField, Range(0f, 360f)] private float spread = 0f;
        [SerializeField] private AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;
        [SerializeField] private MinMax distance = new MinMax(1, 500);
        [SerializeField] private AnimationCurve volumeRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        #endregion

        #region Properties

        public UnityEngine.Audio.AudioMixerGroup Output {
            get => output;
            set => output = value;
        }

        public bool BypassEffects {
            get => bypassEffects;
            set => bypassEffects = value;
        }

        public bool BypassListenerEffects {
            get => bypassListenerEffects;
            set => bypassListenerEffects = value;
        }

        public bool BypassReverbZones {
            get => bypassReverbZones;
            set => bypassReverbZones = value;
        }

        public int Priority {
            get => priority;
            set => priority = Mathf.Clamp(value, 0, 255);
        }

        public float Volume {
            get => volume;
            set => volume = Mathf.Clamp01(value);
        }

        public float Pitch {
            get => pitch;
            set => pitch = Mathf.Clamp(value, -3f, 3f);
        }

        public float StereoPan {
            get => stereoPan;
            set => stereoPan = Mathf.Clamp(value, -1f, 1f);
        }

        public float SpatialBlend {
            get => spatialBlend;
            set => spatialBlend = Mathf.Clamp01(value);
        }

        public float ReverbZoneMix {
            get => reverbZoneMix;
            set => reverbZoneMix = Mathf.Clamp(value, 0f, 1.1f);
        }

        public float DopplerLevel {
            get => dopplerLevel;
            set => dopplerLevel = Mathf.Clamp(value, 0f, 5f);
        }

        public float Spread {
            get => spread;
            set => Mathf.Clamp(value, 0f, 360f);
        }

        public AudioRolloffMode RolloffMode {
            get => volumeRolloff;
            set => volumeRolloff = value;
        }

        public float MinDistance {
            get => distance.min;
            set => distance.min = value;
        }

        public float MaxDistance {
            get => distance.max;
            set => distance.max = value;
        }

        public MinMax Distance {
            get => distance;
            set => distance = value;
        }

        public AnimationCurve VolumeRolloffCurve {
            get => volumeRolloffCurve;
            set => volumeRolloffCurve = value;
        }

        #endregion

        #region Apply

        public void ApplyTo(AudioSource source) {

            source.outputAudioMixerGroup = output;
            source.bypassEffects = bypassEffects;
            source.bypassListenerEffects = bypassListenerEffects;
            source.bypassReverbZones = bypassReverbZones;

            source.priority = priority;
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo = stereoPan;
            source.spatialBlend = spatialBlend;
            source.reverbZoneMix = reverbZoneMix;

            source.dopplerLevel = dopplerLevel;
            source.spread = spread;
            source.rolloffMode = volumeRolloff;
            source.minDistance = distance.min;
            source.maxDistance = distance.max;

            if(source.rolloffMode == AudioRolloffMode.Custom) {
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeRolloffCurve);
            }
        }

        #endregion

        #region Copy

        public void Copy(AudioSource source) {
            output = source.outputAudioMixerGroup;
            bypassEffects = source.bypassEffects;
            bypassListenerEffects = source.bypassListenerEffects;
            bypassReverbZones = source.bypassReverbZones;

            priority = source.priority;
            volume = source.volume;
            pitch = source.pitch;
            stereoPan = source.panStereo;
            spatialBlend = source.spatialBlend;
            reverbZoneMix = source.reverbZoneMix;

            dopplerLevel = source.dopplerLevel;
            spread = source.spread;
            volumeRolloff = source.rolloffMode;
            distance = new MinMax(source.minDistance, source.maxDistance);

            if(source.rolloffMode == AudioRolloffMode.Custom) {
                volumeRolloffCurve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            }
        }

        public void Copy(IAudioSourceSettings source) {
            output = source.Output;
            bypassEffects = source.BypassEffects;
            bypassListenerEffects = source.BypassListenerEffects;
            bypassReverbZones = source.BypassReverbZones;

            priority = source.Priority;
            volume = source.Volume;
            pitch = source.Pitch;
            stereoPan = source.StereoPan;
            spatialBlend = source.SpatialBlend;
            reverbZoneMix = source.ReverbZoneMix;

            dopplerLevel = source.DopplerLevel;
            spread = source.Spread;
            volumeRolloff = source.RolloffMode;
            distance = source.Distance;

            if(source.RolloffMode == AudioRolloffMode.Custom) {
                volumeRolloffCurve = source.VolumeRolloffCurve;
            }
        }

        #endregion
    }
}
