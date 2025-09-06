using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [AddComponentMenu("Toolkit/Audio/Play Sound (Awake)")]
    public class PlaySoundAwake : MonoBehaviour, IPlaySound
    {
        #region Variables

        [SerializeField] private AudioFile file = default;
        [SerializeField, Range(0f, 1f)] private float volumeMultiplier = 1f;
        [SerializeField] private float delay = 0f;

        [SerializeField] private bool follow = false;
        [SerializeField] private bool usePreset = false;
        [SerializeField] private AudioSourceSettingsPreset preset;
        [SerializeField] private bool createSeperateAudioSource = true;

        private AudioSource source;
        private Coroutine routine;

        #endregion

        #region Properties

        public AudioClip Clip {
            get => file;
            set => file = new AudioFile(value);
        }

        public float VolumeMultiplier {
            get => volumeMultiplier;
            set => volumeMultiplier = Mathf.Clamp(value, 0, 1f);
        }

        #endregion

        #region Unity Awake

        private void Awake() {
            source = this.GetComponent<AudioSource>();
            if(usePreset && preset) {
                if(createSeperateAudioSource)
                    source = gameObject.AddComponent<AudioSource>();
                preset.ApplyTo(source);
            }
            routine = Timer.Once(delay, PlayOneShot);
        }

        private void OnDestroy() {
            Timer.Stop(routine);
        }

        #endregion

        #region IPlaySound Impl

        public void PlayOneShot()
            => PlayOneShot(file, volumeMultiplier);

        public void PlayOneShot(float volume)
            => PlayOneShot(file, volume);

        public void PlayOneShot(AudioFile file, float volume) {
            if(!enabled)
                return;
            if(file.Type == AudioFileType.Player && file.Reference is AudioPlayer player) {
                if(follow)
                    player.PlayAndFollow(transform);
                else
                    player.PlayAt(transform);
            }
            else if(source != null)
                source.PlayOneShot(file, volume * volumeMultiplier);
        }

        #endregion

        #region PlayOneShot

        public void PlayOneShot(AudioClip clip) {
            source.PlayOneShot(clip, volumeMultiplier * source.volume);
        }

        public void PlayOneShot(AudioClip clip, float volume) {
            source.PlayOneShot(clip, volume);
        }

        public void PlayOneShot(AudioVariation clip) {
            source.PlayOneShot(clip, volumeMultiplier * source.volume);
        }

        public void PlayOneShot(AudioVariation clip, float volume) {
            source.PlayOneShot(clip, volume);
        }

        #endregion

        #region Editor

        [ContextMenu("Switch To OnEnable")]
        private void SwitchToOnEnable() {
            var enabled = this.AddComponent<PlaySoundEnabled>();
            this.CopyComponentValuesToSource(enabled);

#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(transform);
            DestroyImmediate(this);
#endif
        }

        #endregion
    }
}
