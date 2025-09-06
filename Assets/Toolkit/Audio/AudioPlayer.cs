using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio {
    [CreateAssetMenu(fileName = "Audio Player", menuName = "Toolkit/Audio/Audio Player")]
    public class AudioPlayer : ScriptableObject {
        #region Variables

        public const int MAX_POOL_COUNT = AudioPool.MAX_POOL_COUNT;

        [SerializeField] private AudioSourceSettingsPreset settingsPreset = null;
        [SerializeField] private AudioSourceSettings settings = null;

        //[SerializeField] private AudioVariation variationPreset = null;
        //[SerializeField] private AudioClip[] clips = { };
        [SerializeField] private NSOReferenceArray<IAudioClipPlayer> clips = new NSOReferenceArray<IAudioClipPlayer>();

        [SerializeField] private PoolMode poolingMode = PoolMode.Dynamic;
        [SerializeField, Range(0, MAX_POOL_COUNT)] private int poolingCount = 4;
        private AudioPool pool;

        #endregion

        #region Properties

        public IAudioSourceSettings Settings => settingsPreset != null ? settingsPreset : settings;

        #endregion

        #region Private Methods

        public IAudioClipPlayer GetAudioClipPlayer() {
            return clips.RandomElement();
        }

        private bool GetSource(out AudioPoolObject source) {
            if(!pool)
                pool = AudioPool.Create(name, Settings, poolingMode, poolingCount);
            return pool.GetAudioObject(out source);
        }

        #endregion

        #region Methods

        public float Play() {
            if(GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.Play(audioClipPlayer);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float Play(float volume) {
            if(GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.Play(audioClipPlayer, volume);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAt(Vector3 position) {
            if(GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip =  source.PlayAt(audioClipPlayer, position);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAt(Vector3 position, float volume) {
            if(GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip =  source.PlayAt(audioClipPlayer, position, volume);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAt(Transform transform) {
            if(transform && GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.PlayAt(audioClipPlayer, transform);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAt(Transform transform, float volume) {
            if(transform && GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.PlayAt(audioClipPlayer, transform, volume);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAndFollow(Transform transform) {
            if(transform && GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.PlayAndFollow(audioClipPlayer, transform);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        public float PlayAndFollow(Transform transform, float volume) {
            if(transform && GetSource(out AudioPoolObject source)) {
                var audioClipPlayer = GetAudioClipPlayer();
                var clip = source.PlayAndFollow(audioClipPlayer, transform, volume);
                if(!clip)
                    return 0f;
                return clip.length * source.Source.pitch;
            }
            return 0f;
        }

        #endregion

        #region Delayed Methods

        public Coroutine PlayDelayed(float delay)
            => Timer.Once(delay, () => Play());

        public Coroutine PlayDelayed(float delay, float volume)
            => Timer.Once(delay, () => Play(volume));

        public Coroutine PlayDelayedAt(float delay, Vector3 position)
            => Timer.Once(delay, () => PlayAt(position));

        public Coroutine PlayDelayedAt(float delay, Vector3 position, float volume)
            => Timer.Once(delay, () => PlayAt(position, volume));

        public Coroutine PlayDelayedAt(float delay, Transform transform)
            => Timer.Once(delay, () => PlayAt(transform));

        public Coroutine PlayDelayedAt(float delay, Transform transform, float volume)
            => Timer.Once(delay, () => PlayAt(transform, volume));

        public Coroutine PlayDelayedAndFollow(float delay, Transform transform)
            => Timer.Once(delay, () => PlayAndFollow(transform));

        public Coroutine PlayDelayedAndFollow(float delay, Transform transform, float volume)
            => Timer.Once(delay, () => PlayAndFollow(transform, volume));

        #endregion
    }
}
