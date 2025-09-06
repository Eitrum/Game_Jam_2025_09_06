using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio.ACP {
    [NSOFile("Variation", typeof(IAudioClipPlayer))]
    public class AudioClipPlayerVariation : ScriptableObject, IAudioClipPlayer {
        #region Variables

        [SerializeField] private AudioVariation variations;

        #endregion

        #region IAudioClipPlayer Impl

        public AudioClip Clip => variations.Clip;
        public bool UseDynamicUpdate => false;

        public void Prepare(AudioSource source, IAudioSourceSettings settings) { }

        public void DynamicUpdate(AudioSource source, IAudioSourceSettings settings, float dt, float totalTime) { }

        public void Restore(AudioSource source, IAudioSourceSettings settings) { }

        #endregion
    }
}
