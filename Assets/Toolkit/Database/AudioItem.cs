using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [CreateAssetMenu(fileName = "Audio Item", menuName = Database.MENU_PATH + "Audio Item")]
    public class AudioItem : Item {

        // Write as override??? 
        // Fix Audio Pooling
        // 
        [Header("Audio Settings")]
        public AudioSettings settings;

        [Header("Clip Settings")]
        public bool variation = true;
        public AudioClipSettings[] audioClipSettings = { };

        public AudioClip Clip => variation ? audioClipSettings.RandomElement().audioClip : audioClipSettings[0].audioClip;
        public AudioClip GetClip(int index) => audioClipSettings[index].audioClip;
        public AudioSettings GetAudioSettings(int index) => audioClipSettings[index].settings;
    }

    [System.Serializable]
    public struct AudioClipSettings {
        public AudioClip audioClip;
        public AudioSettings settings;
    }
    [System.Serializable]
    public struct AudioSettings {
        public float volume;
        public float pitch;
    }
}
