using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem.Animations {
    [DefaultExecutionOrder(200)]
    public class PanelAnimationEase : MonoBehaviour, IPanelAnimation {

        public enum Side {
            None,
            Left,
            Right,
            Top,
            Bottom,
        }

        #region Variables

        private const string TAG = "[PanelAnimationEase] - ";

        [Header("Show")]
        [SerializeField] private Side enterSide = Side.Left;
        [SerializeField, Min(0f)] private float enterDuration = 0.2f;
        [SerializeField] private Toolkit.Mathematics.EaseReference enterEaseFunction = new Toolkit.Mathematics.EaseReference(Toolkit.Mathematics.Ease.Function.Exponential, Toolkit.Mathematics.Ease.Type.Out);
        [Header("Hide")]
        [SerializeField] private Side exitSide = Side.Left;
        [SerializeField, Min(0f)] private float exitDuration = 0.2f;
        [SerializeField] private Toolkit.Mathematics.EaseReference exitEaseFunction = new Toolkit.Mathematics.EaseReference(Toolkit.Mathematics.Ease.Function.Exponential, Toolkit.Mathematics.Ease.Type.Out);

        [Header("Other")]
        [SerializeField] private bool overrideInitialSize = false;
        [SerializeField] private Vector2 overrideSize = new Vector2();
        [SerializeField] private bool autoShow = true;

        private Canvas root;
        private Vector2 anchor;
        private Vector2 size;
        private Vector2 target;
        private RectTransform rt;
        private float t;
        private Vector2 startAnchorPoint;

        private bool isEntering;

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
            if(overrideInitialSize)
                size = overrideSize;
            else
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
            t = Mathf.Clamp01(t + Time.deltaTime / (isEntering ? enterDuration : exitDuration));
            rt.anchoredPosition = Vector2.LerpUnclamped(startAnchorPoint, target, isEntering ? enterEaseFunction.Evaluate(t) : exitEaseFunction.Evaluate(t));
            if(t >= 1f) {
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
            startAnchorPoint = rt.anchoredPosition;
            isEntering = true;
            if(enterDuration <= Mathf.Epsilon) {
                rt.anchoredPosition = target;
                IsComplete = true;
            }
            else {
                t = 0;
                IsPlaying = true;
                IsComplete = false;
            }
        }

        [ContextMenu("Hide")]
        public void Hide() {
            target = anchor + GetOffset(exitSide);
            startAnchorPoint = rt.anchoredPosition;
            isEntering = false;
            if(exitDuration <= Mathf.Epsilon) {
                rt.anchoredPosition = target;
                IsComplete = true;
            }
            else {
                t = 0;
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
