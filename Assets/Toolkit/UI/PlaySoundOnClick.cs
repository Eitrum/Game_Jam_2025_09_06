using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Toolkit.UI.Components {
    public class PlaySoundOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
        #region Variables

        [SerializeField] private bool onClick = true;
        [SerializeField] private AudioPlayer overrideClick;
        [SerializeField] private bool onPointerDown = false;
        [SerializeField] private AudioPlayer overridePointerDown;
        [SerializeField] private bool onPointerUp = false;
        [SerializeField] private AudioPlayer overridePointerUp;

        #endregion

        #region IPointer Impl

        public void OnPointerClick(PointerEventData eventData) {
            if(!onClick)
                return;
            if(overrideClick)
                overrideClick.Play();
            else
                UIDefaultSounds.PlayOnClick();
        }

        public void OnPointerDown(PointerEventData eventData) {
            if(!onPointerDown)
                return;
            if(overridePointerDown)
                overridePointerDown.Play();
            else
                UIDefaultSounds.PlayOnPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData) {
            if(!onPointerUp)
                return;
            if(overridePointerUp)
                overridePointerUp.Play();
            else
                UIDefaultSounds.PlayOnPointerUp();
        }

        #endregion
    }
}
