using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Audio
{
    public enum AudioFileType
    {
        None = 0,
        Clip = 1,
        Variation = 2,
        Player = 3,
    }

    [System.Serializable, StructLayout(LayoutKind.Explicit)]
    public struct AudioFile : ISerializationCallbackReceiver
    {
        #region Variables

        private const string TAG = "[AudioFile] - ";

        [FieldOffset(0), SerializeField] private UnityEngine.Object reference;
        [FieldOffset(0), System.NonSerialized] private AudioClip clip;
        [FieldOffset(0), System.NonSerialized] private IAudioVariation variation;
        [FieldOffset(0), System.NonSerialized] private AudioPlayer player;
        [FieldOffset(8), System.NonSerialized] private AudioFileType type;

        #endregion

        #region Properties

        public UnityEngine.Object Reference {
            get => reference;
            set {
                reference = value;
                if(reference is AudioClip)
                    type = AudioFileType.Clip;
                else if(reference is IAudioVariation)
                    type = AudioFileType.Variation;
                else if(reference is AudioPlayer)
                    type = AudioFileType.Player;
                else
                    type = AudioFileType.None;
            }
        }

        public AudioClip Clip {
            get {
                switch(type) {
                    case AudioFileType.Clip: return clip;
                    case AudioFileType.Variation: return variation.Clip;
                    case AudioFileType.Player: return player.GetAudioClipPlayer().Clip;
                }
                return null;
            }
            set {
                if(value == null) {
                    type = AudioFileType.None;
                    reference = null;
                    return;
                }
                type = AudioFileType.Clip;
                reference = value;
            }
        }

        public AudioFileType Type => type;

        /// <summary>
        /// Type:
        ///  - Clip, returns clip length.
        ///  - Variation, returns average length between all clips.
        /// </summary>
        public float Length {
            get {
                switch(type) {
                    case AudioFileType.Clip: return clip.length;
                    case AudioFileType.Variation: {
                            float avg = 0f;
                            int count = variation.Count;
                            for(int i = 0; i < count; i++)
                                avg += variation[i].length;
                            return avg;
                        }
                    case AudioFileType.Player: {
                            Debug.LogWarning(TAG + "Unable to retrive the clip length of an AudioPlayer");
                            return 0f;
                        }
                }
                return 0f;
            }
        }

        /// <summary>
        /// Type:
        ///  - Clip, returns clip length.
        ///  - Variation, returns the minimum length and maximum length.
        /// </summary>
        public MinMax LengthRange {
            get {
                switch(type) {
                    case AudioFileType.Clip: return new MinMax(clip.length);
                    case AudioFileType.Variation: {
                            MinMax range = new MinMax(float.MaxValue, 0f);
                            for(int i = 0, count = variation.Count; i < count; i++) {
                                var l = variation[i].length;
                                range.min = Mathf.Min(range.min, l);
                                range.max = Mathf.Max(range.max, l);
                            }
                            return range;
                        }
                    case AudioFileType.Player: {
                            Debug.LogWarning(TAG + "Unable to retrive the clip length of an AudioPlayer");
                            return new MinMax(0f);
                        }
                }
                return new MinMax(0f);
            }
        }

        #endregion

        #region Constructor

        public AudioFile(AudioClip clip) {
            this.clip = null;
            this.variation = null;
            this.player = null;

            this.reference = clip;
            type = AudioFileType.Clip;
        }

        public AudioFile(IAudioVariation variation) {
            this.clip = null;
            this.reference = null;
            this.player = null;

            this.variation = variation;
            type = AudioFileType.Variation;

            if(!(variation is UnityEngine.Object)) {
                Debug.LogWarning(TAG + $"Assigned variation is non-serializable");
            }
        }

        public AudioFile(UnityEngine.Object o) {
            this.clip = null;
            this.variation = null;
            this.player = null;

            this.reference = o;
            if(o is AudioClip)
                type = AudioFileType.Clip;
            else if(o is IAudioVariation)
                type = AudioFileType.Variation;
            else if(o is AudioPlayer)
                type = AudioFileType.Player;
            else {
                type = AudioFileType.None;
                Debug.LogWarning(TAG + $"No supported type provided in audio file.");
            }
        }

        #endregion

        #region Find

        public static AudioFile Find(GameObject go) {
            var variation = go.GetComponent<IAudioVariation>();
            if(variation != null) {
                return new AudioFile(variation);
            }
            var source = go.GetComponent<AudioSource>();
            if(source) {
                return new AudioFile(source.clip);
            }

            return new AudioFile();
        }

        public static AudioFile Find(UnityEngine.Object o) {
            if(o is GameObject go)
                return Find(go);
            else
                return new AudioFile(o);
        }

        public static AudioFile FindInChildren(GameObject go)
           => FindInChildren(go.transform);

        public static AudioFile FindInChildren(Component compoenent)
            => FindInChildren(compoenent.transform);

        public static AudioFile FindInChildren(Transform transform) {
            var variation = transform.GetComponentInChildren<IAudioVariation>();
            if(variation != null) {
                return new AudioFile(variation);
            }
            var source = transform.GetComponentInChildren<AudioSource>();
            if(source) {
                return new AudioFile(source.clip);
            }

            return new AudioFile();
        }

        #endregion

        #region Conversion

        public static implicit operator AudioClip(AudioFile file) => file.Clip;
        public static explicit operator AudioFile(AudioClip clip) => new AudioFile(clip);

        #endregion

        #region ISerializationCallbackReceiver Impl

        void ISerializationCallbackReceiver.OnBeforeSerialize() {

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(reference is AudioClip)
                type = AudioFileType.Clip;
            else if(reference is AudioPlayer)
                type = AudioFileType.Player;
            else if(reference is IAudioVariation)
                type = AudioFileType.Variation;
            else
                type = AudioFileType.None;
        }

        #endregion
    }
}
