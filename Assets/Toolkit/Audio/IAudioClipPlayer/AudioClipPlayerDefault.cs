using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio.ACP {
    [NSOFile("Default", typeof(IAudioClipPlayer))]
    public class AudioClipPlayerDefault : ScriptableObject, IAudioClipPlayer {
        #region Variables

        [SerializeField] private AudioClip clip;

        #endregion

        #region IAudioClipPlayer Impl

        public AudioClip Clip => clip;
        public bool UseDynamicUpdate => false;

        public void Prepare(AudioSource source, IAudioSourceSettings settings) { }

        public void DynamicUpdate(AudioSource source, IAudioSourceSettings settings, float dt, float totalTime) { }

        public void Restore(AudioSource source, IAudioSourceSettings settings) { }

        #endregion
    }
}
