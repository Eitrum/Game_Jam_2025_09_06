using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    public interface IAudioSourceSettings
    {
        UnityEngine.Audio.AudioMixerGroup Output { get; set; }
        bool BypassEffects { get; set; }
        bool BypassListenerEffects { get; set; }
        bool BypassReverbZones { get; set; }

        int Priority { get; set; }
        float Volume { get; set; }
        float Pitch { get; set; }
        float StereoPan { get; set; }
        float SpatialBlend { get; set; }
        float ReverbZoneMix { get; set; }

        float DopplerLevel { get; set; }
        float Spread { get; set; }
        AudioRolloffMode RolloffMode { get; set; }
        MinMax Distance { get; set; }
        AnimationCurve VolumeRolloffCurve { get; set; }

        void ApplyTo(AudioSource source);
    }
}
