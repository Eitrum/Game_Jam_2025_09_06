using System.Collections;
using System.Collections.Generic;
using Toolkit.Audio;
using UnityEngine;

namespace Toolkit.Health
{
    public class PlaySoundOnHealthChanged : MonoBehaviour, IPlaySound
    {
        public enum HealthChangeCheck
        {
            Damage,
            Heal,
            Both
        }

        #region Variables

        [SerializeField] private AudioFile file = default;
        [SerializeField, Range(0f, 1f)] private float volumeMultiplier = 1f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private bool cancelIfDestroyed = true;

        [SerializeField] private HealthChangeCheck check = HealthChangeCheck.Damage;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float minimumPercentage = 0.05f;
        [SerializeField, Min(0f)] private float cooldown = 0.8f;

        [SerializeField] private bool followSource = false;
        [SerializeField] private bool usePreset = false;
        [SerializeField] private AudioSourceSettingsPreset preset;
        [SerializeField] private bool createSeperateAudioSource = true;

        private AudioSource source;
        private Coroutine routine;
        private IHealth health;
        private float lastPlayed;

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
            health = GetComponentInParent<IHealth>();
            source = this.GetComponent<AudioSource>();
            if(usePreset && preset) {
                if(createSeperateAudioSource)
                    source = gameObject.AddComponent<AudioSource>();
                preset.ApplyTo(source);
            }
        }

        void OnEnable() {
            if(health != null)
                health.OnHealthChanged += OnHealthChanged;
        }

        void OnDisable() {
            if(health != null)
                health.OnHealthChanged -= OnHealthChanged;

            if(cancelIfDestroyed)
                Timer.Stop(routine);
        }

        void OnDestroy() {
            Timer.Stop(routine);
        }

        #endregion

        #region Health Changed Callbacks

        private static float GetDelta(HealthChangeCheck check, float old, float n) {
            switch(check) {
                case HealthChangeCheck.Damage: return (n - old);
                case HealthChangeCheck.Heal: return (old - n);
            }
            return Mathf.Abs(n - old);
        }

        private void OnHealthChanged(IHealth source, float old, float newHealth) {
            var t = Time.realtimeSinceStartup;
            if(t < lastPlayed + cooldown)
                return;
            if(GetDelta(check, old, newHealth) < health.Full * minimumPercentage)
                return;

            lastPlayed = t;

            if(delay > Mathf.Epsilon) {
                var pos = transform.position; // In-case transform gets destroyed during delay
                routine = Timer.Once(delay, () => Internal_PlayOneShot(file, volumeMultiplier, transform, pos));
            }
            else
                Internal_PlayOneShot(file, volumeMultiplier, transform);
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
