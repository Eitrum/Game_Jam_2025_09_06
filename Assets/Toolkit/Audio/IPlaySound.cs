using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    public interface IPlaySound
    {
        float VolumeMultiplier { get; set; }

        void PlayOneShot();
        void PlayOneShot(float volume);
        void PlayOneShot(AudioFile file, float volume);
    }
}
