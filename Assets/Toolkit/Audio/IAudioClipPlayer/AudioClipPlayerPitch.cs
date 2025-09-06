using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio.ACP {
    [NSOFile("Pitch", typeof(IAudioClipPlayer))]
    public class AudioClipPlayerPitch : ScriptableObject, IAudioClipPlayer {
        #region Variables

        [SerializeField] private AudioClip clip;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float pitchModifer = 0.05f;

        #endregion

        #region IAudioClipPlayer Impl

        public AudioClip Clip => clip;
        public bool UseDynamicUpdate => false;

        public void Prepare(AudioSource source, IAudioSourceSettings settings) {
            source.pitch += UnityEngine.Random.Range(-pitchModifer, pitchModifer);
        }

        public void DynamicUpdate(AudioSource source, IAudioSourceSettings settings, float dt, float totalTime) { }

        public void Restore(AudioSource source, IAudioSourceSettings settings) {
            source.pitch = settings.Pitch;
        }

        #endregion
    }
}
