using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Toolkit.Interactables.UI {
    public class ExamineUIDragToRotate : MonoBehaviour, IDragHandler, IScrollHandler {

        private ExamineUI examineUI;

        void Awake() {
            examineUI = GetComponentInParent<ExamineUI>();
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            var delta = eventData.delta;
            examineUI.Rotate(delta);
            examineUI.AddVelocity(delta);
        }

        void IScrollHandler.OnScroll(PointerEventData eventData) {
            var delta = eventData.scrollDelta.y * 15f;
            examineUI.RotateZ(delta);
            examineUI.AddVelocity(new Vector3(0, 0, delta));
        }
    }
}
