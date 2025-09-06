using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.Audio;
using System;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Utility/Play Sound (OnTrigger)")]
    public class PlaySoundOnTrigger : MonoBehaviour, IPlaySound {
        #region Variables
        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private AudioFile file = default;
        [SerializeField, Range(0f, 1f)] private float volumeMultiplier = 1f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private bool cancelIfDestroyed = true;

        [SerializeField] private bool followSource = false;
        [SerializeField] private SourceType sourceType = SourceType.None;
        [SerializeField] private bool usePreset = false;
        [SerializeField] private AudioSourceSettingsPreset preset;
        [SerializeField] private bool createSeperateAudioSource = true;

        private AudioSource source;
        private Coroutine routine;
        private ITrigger trigger;

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
            trigger = GetComponentInParent<ITrigger>();
            source = this.GetComponent<AudioSource>();
            if(usePreset && preset) {
                if(createSeperateAudioSource)
                    source = gameObject.AddComponent<AudioSource>();
                preset.ApplyTo(source);
            }
        }

        void OnEnable() {
            if(trigger != null)
                trigger.OnTrigger += OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger += OnTrigger;
        }

        void OnDisable() {
            if(trigger != null)
                trigger.OnTrigger -= OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger -= OnTrigger;

            if(cancelIfDestroyed)
                Timer.Stop(routine);
        }

        void OnDestroy() {
            Timer.Stop(routine);
        }

        #endregion

        #region Trigger Callbacks

        [Button, ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                OnTrigger(s);
        }

        private void OnTrigger(Source source) {
            Transform t;
            if(sourceType != SourceType.None) {
                var ts = source.Find(sourceType);
                t = ts?.Transform ?? transform;
            }
            else
                t = transform;

            if(delay > Mathf.Epsilon) {
                var pos = transform.position; // In-case transform gets destroyed during delay
                routine = Timer.Once(delay, () => Internal_PlayOneShot(file, volumeMultiplier, t, pos));
            }
            else
                Internal_PlayOneShot(file, volumeMultiplier, t);
        }

        private void Internal_PlayOneShot(AudioFile file, float volume, Transform target) {
            if(target == null) {
                target = transform;
            }
            if(file.Type == AudioFileType.Player && file.Reference is AudioPlayer player) {
                if(followSource)
                    player.PlayAndFollow(target);
                else
                    player.PlayAt(target);
            }
            else if(source != null)
                source.PlayOneShot(file, volume * volumeMultiplier);
        }

        private void Internal_PlayOneShot(AudioFile file, float volume, Transform target, Vector3 position) {
            if(target == null) {
                target = transform;
            }
            if(file.Type == AudioFileType.Player && file.Reference is AudioPlayer player) {
                if(target) {
                    if(followSource)
                        player.PlayAndFollow(target);
                    else
                        player.PlayAt(target);
                }
                else
                    player.PlayAt(position);
            }
            else if(source != null)
                source.PlayOneShot(file, volume * volumeMultiplier);
        }

        #endregion

        #region IPlaySound Impl

        [Button]
        public void PlayOneShot()
            => PlayOneShot(file, volumeMultiplier);

        public void PlayOneShot(float volume)
            => PlayOneShot(file, volume);

        public void PlayOneShot(AudioFile file, float volume) {
            if(!enabled)
                return;
            if(file.Type == AudioFileType.Player && file.Reference is AudioPlayer player) {
                if(followSource)
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
            if(enabled && source)
                source.PlayOneShot(clip, volumeMultiplier);
        }

        public void PlayOneShot(AudioClip clip, float volume) {
            if(enabled && source)
                source.PlayOneShot(clip, volume);
        }

        public void PlayOneShot(AudioVariation clip) {
            if(enabled && source)
                source.PlayOneShot(clip, volumeMultiplier);
        }

        public void PlayOneShot(AudioVariation clip, float volume) {
            if(enabled && source)
                source.PlayOneShot(clip, volume);
        }

        #endregion
    }
}
