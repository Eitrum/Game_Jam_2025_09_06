using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI {
    public class UIElementFollowWorldTransform : MonoBehaviour {

        [SerializeField] private Transform globalTransform;
        [SerializeField] private Vector2 anchorOffset = Vector2.zero;

        private RectTransform rt;

        private void Awake() {
            rt = transform as RectTransform;
        }

        private void OnEnable() {
            Application.onBeforeRender += UpdatePosition;
        }

        private void OnDisable() {
            Application.onBeforeRender -= UpdatePosition;
        }

        private void UpdatePosition() {
            if(!globalTransform)
                return;
            var cam = CameraInstance.MainCamera;
            if(!cam)
                return;

            var point = RectTransformUtility.WorldToScreenPoint(cam, globalTransform.position);
            var size = rt.parent.ToRectTransform().rect.size;
            var width = Screen.width;
            var height = Screen.height;

            var xp = size.x / width;
            var yp = size.y / height;
            point.x *= xp;
            point.y *= yp;
            point.x -= size.x * 0.5f;
            point.y -= size.y * 0.5f;
            rt.anchoredPosition = point + anchorOffset;
        }

        public void SetTarget(Component target) {
            globalTransform = target.transform;
            UpdatePosition();
        }
        public void SetTarget(Transform target) {
            globalTransform = target;
            UpdatePosition();
        }
    }
}
