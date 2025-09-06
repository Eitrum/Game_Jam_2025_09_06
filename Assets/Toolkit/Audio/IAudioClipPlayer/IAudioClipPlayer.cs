using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio {
    public interface IAudioClipPlayer {
        AudioClip Clip { get; }
        bool UseDynamicUpdate { get; }

        // Returns the clip in cases of variations
        void Prepare(AudioSource source, IAudioSourceSettings settings);
        void DynamicUpdate(AudioSource source, IAudioSourceSettings settings, float dt, float totalTime);
        void Restore(AudioSource source, IAudioSourceSettings settings);
    }
}
