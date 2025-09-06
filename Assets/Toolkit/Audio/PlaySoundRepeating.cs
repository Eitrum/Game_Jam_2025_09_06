using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [AddComponentMenu("Toolkit/Audio/Play Sound (Repeat)")]
    public class PlaySoundRepeating : MonoBehaviour, IPlaySound
    {
        #region Variables

        [SerializeField] private AudioFile file = default;
        [SerializeField, Range(0f, 1f)] private float volumeMultiplier = 1f;

        [SerializeField] private MinMax interval = new MinMax(1f, 2f);
        [SerializeField] private bool delayFirstSound = false;

        [SerializeField] private bool follow = false;
        [SerializeField] private bool usePreset = false;
        [SerializeField] private AudioSourceSettingsPreset preset;
        [SerializeField] private bool createSeperateAudioSource = true;

        private AudioSource source;
        private float delay = 0f;

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

        #region Unity Methods

        private void Awake() {
            source = this.GetComponent<AudioSource>();
            if(usePreset && preset) {
                if(createSeperateAudioSource || source == null)
                    source = gameObject.AddComponent<AudioSource>();
                preset.ApplyTo(source);
            }
        }

        void OnEnable() {
            delay = delayFirstSound ? interval.Random : 0f;
        }

        void Update() {
            delay -= Time.deltaTime;
            if(delay <= 0f) {
                delay += interval.Random;
                PlayOneShot();
            }
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
            if(enabled)
                source.PlayOneShot(clip, volumeMultiplier);
        }

        public void PlayOneShot(AudioClip clip, float volume) {
            if(enabled)
                source.PlayOneShot(clip, volume);
        }

        public void PlayOneShot(AudioVariation clip) {
            if(enabled)
                source.PlayOneShot(clip, volumeMultiplier);
        }

        public void PlayOneShot(AudioVariation clip, float volume) {
            if(enabled)
                source.PlayOneShot(clip, volume);
        }

        #endregion
    }
}
