using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Toolkit.UI.PanelSystem {
    public class PanelBackground : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {

        public enum Mode {
            Disabled,
            Click,
            PointerDown,
            PointerUp,
        }

        #region Variables

        [SerializeField] private Mode mode = Mode.Click;
        public event System.Action OnClicked;

        #endregion

        #region Show / Hide

        public void Show() {
            if(this == null)
                return;
            gameObject.SetActive(true);
        }

        public void Hide() {
            if(this == null)
                return;
            gameObject.SetActive(false);
        }

        #endregion

        #region IPointer Handler Impl

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if(mode == Mode.Click)
                OnClicked?.Invoke();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(mode == Mode.PointerDown)
                OnClicked?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if(mode == Mode.PointerUp)
                OnClicked?.Invoke();
        }

        #endregion
    }
}
