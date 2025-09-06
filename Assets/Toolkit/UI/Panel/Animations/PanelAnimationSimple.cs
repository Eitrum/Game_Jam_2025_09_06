using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem.Animations {
    [DefaultExecutionOrder(200)]
    public class PanelAnimationSimple : MonoBehaviour, IPanelAnimation {

        public enum Side {
            None,
            Left,
            Right,
            Top,
            Bottom,
        }

        #region Variables

        private const string TAG = "[PanelAnimationSimple] - ";

        [Header("Show")]
        [SerializeField] private Side enterSide = Side.Left;
        [SerializeField, Min(0f)] private float enterDuration = 0.1f;
        [Header("Hide")]
        [SerializeField] private Side exitSide = Side.Left;
        [SerializeField, Min(0f)] private float exitDuration = 0.1f;

        [SerializeField] private bool autoShow = true;

        private Canvas root;
        private Vector2 anchor;
        private Vector2 size;
        private Vector2 target;
        private RectTransform rt;
        private float distancePerSecond;

        #endregion

        #region Properties

        public bool IsPlaying { get; private set; }
        public bool IsComplete { get; private set; }

        #endregion

        #region Init

        void Awake() {
            var c = GetComponentInParent<Canvas>();
            if(c == null) {
                Debug.LogError(TAG + "No canvas found");
            }
            root = c.rootCanvas;
            anchor = transform.ToRectTransform().anchoredPosition;
        }

        void OnEnable() {
            size = root.transform.ToRectTransform().rect.size;
            rt = transform.ToRectTransform();
            rt.anchoredPosition = anchor + GetOffset(enterSide);
        }

        void OnDisable() {
            if(rt)
                rt.anchoredPosition = target;
        }

        void Start() {
            if(autoShow && !IsComplete)
                Show();
        }

        #endregion

        #region Update

        void LateUpdate() {
            if(IsComplete)
                return;
            IsPlaying = true;
            rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, target, Time.deltaTime * distancePerSecond);
            if((rt.anchoredPosition - target).sqrMagnitude <= Mathf.Epsilon) {
                rt.anchoredPosition = target;
                IsComplete = true;
                IsPlaying = false;
            }
        }

        #endregion

        #region Show / Hide

        public void Cancel() {
            IsPlaying = false;
            IsComplete = true;
        }

        [ContextMenu("Show")]
        public void Show() {
            target = anchor;
            if(enterDuration <= Mathf.Epsilon) {
                rt.anchoredPosition = target;
                IsComplete = true;
            }
            else {
                distancePerSecond = Vector2.Distance(rt.anchoredPosition, target) / enterDuration;
                IsPlaying = true;
                IsComplete = false;
            }
        }

        [ContextMenu("Hide")]
        public void Hide() {
            target = anchor + GetOffset(exitSide);
            if(exitDuration <= Mathf.Epsilon) {
                rt.anchoredPosition = target;
                IsComplete = true;
            }
            else {
                distancePerSecond = Vector2.Distance(anchor, target) / exitDuration;
                IsPlaying = true;
                IsComplete = false;
            }
        }

        #endregion

        #region Util

        public Vector2 GetOffset(Side side) {
            switch(side) {
                case Side.Left: return new Vector2(-size.x, 0);
                case Side.Right: return new Vector2(size.x, 0);
                case Side.Top: return new Vector2(0, -size.y);
                case Side.Bottom: return new Vector2(0, size.y);
            }
            return anchor;
        }

        #endregion
    }
}
