using System.Collections;
using System.Collections.Generic;
using Toolkit.Audio;
using UnityEngine;

namespace Toolkit.UI {
    public class UIDefaultSounds : ScriptableSingleton<UIDefaultSounds> {
        #region Variables

        [SerializeField] private AudioPlayer onClick;
        [SerializeField] private AudioPlayer onPointerDown;
        [SerializeField] private AudioPlayer onPointerUp;

        #endregion

        #region Methods

        public static void PlayOnClick() {
            if(Instance.onClick)
                Instance.onClick.Play();
        }

        public static void PlayOnPointerDown() {
            if(Instance.onPointerDown)
                Instance.onPointerDown.Play();
        }

        public static void PlayOnPointerUp() {
            if(Instance.onPointerUp)
                Instance.onPointerUp.Play();
        }

        #endregion
    }
}
