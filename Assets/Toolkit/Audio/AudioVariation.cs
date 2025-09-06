using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio {
    [CreateAssetMenu(fileName = "Audio Variation", menuName = "Toolkit/Audio/Audio Variation")]
    public class AudioVariation : ScriptableObject, IAudioVariation {
        #region Variables

        [SerializeField] private string path;
        [SerializeField] private List<Data> clips = new List<Data>();

        // Cache
        [System.NonSerialized] private IReadOnlyList<float> probability;
        [System.NonSerialized] private float maximumProbability;

        #endregion

        #region Properties

        public string Path => path;
        private IReadOnlyList<float> Probability {
            get {
                if(probability == null) {
                    var t = clips.RandomProbablityCache(Weight);
                    probability = t.Item1;
                    maximumProbability = t.Item2;
                }
                return probability;
            }
        }
        public AudioClip Clip => clips.RandomElement(Probability, maximumProbability).clip;
        public int Count => clips.Count;
        public AudioClip this[int index] => clips[index].clip;

        #endregion

        #region Methods

        public AudioClip GetRandom() => Clip;
        public AudioClip GetRandom(System.Random random) => clips.RandomElement(Probability, maximumProbability, random).clip;
        public AudioClip GetClip(int index) => clips[index].clip;

        #endregion

        #region Conversions

        public static implicit operator AudioClip(AudioVariation variation) => variation != null ? variation.Clip : null;

        #endregion

        private static float Weight(Data d) => d.weight;
        [System.Serializable]
        struct Data {
            public AudioClip clip;
            public float weight;
        }
    }
}
