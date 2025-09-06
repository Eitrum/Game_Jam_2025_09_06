using System.Collections;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Animations {
    public class PanelAnimationLerp : MonoBehaviour, IPanelAnimationObject {

        #region Variables

        [Header("Config")]
        [SerializeField] private Vector2 showLocation;
        [SerializeField] private Vector2 hideLocation;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private bool reverseOnHide = true;

        [Header("Overrides")]
        [SerializeField] private bool overrideDuration = false;
        [SerializeField, Min(0f)] private float duration = 0.4f;
        [SerializeField] private bool dontOverrideInitialPosition = false;

        private Coroutine routine;
        private Vector2 startLocation;
        private RectTransform rectTransform;

        #endregion

        #region Properties

        public bool IsComplete { get; private set; } = false;
        public bool IsEnabled => gameObject.activeSelf;

        #endregion

        #region Init

        void Awake() {
            rectTransform = transform.ToRectTransform();
            if(!dontOverrideInitialPosition)
                rectTransform.anchoredPosition = hideLocation;
        }

        private void OnDisable() {
            Cancel();
        }

        void OnDestroy() {
            Timer.Stop(routine);
        }

        #endregion

        #region Animation

        void IPanelAnimationObject.Update(float dt) { }

        private void AnimateShow(float t) {
            t = curve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(startLocation, showLocation, t);
        }

        private void AnimateHide(float t) {
            if(reverseOnHide)
                t = 1f - curve.Evaluate(1f - t);
            else
                t = curve.Evaluate(t);

            rectTransform.anchoredPosition = Vector2.Lerp(startLocation, hideLocation, t);
        }


        void Complete() => IsComplete = true;

        #endregion

        #region IPanelAnim Impl

        public void Cancel() {
            Timer.Stop(routine);
            Complete();
        }

        void IPanelAnimationObject.Show(float duration) {
            if(!gameObject.activeSelf)
                return;
            Cancel();
            IsComplete = false;
            startLocation = rectTransform.anchoredPosition;
            Timer.Animate(overrideDuration ? this.duration : duration, AnimateShow, Complete, ref routine);
        }

        void IPanelAnimationObject.Hide(float duration) {
            if(!gameObject.activeSelf)
                return;
            Cancel();
            IsComplete = false;
            startLocation = rectTransform.anchoredPosition;
            Timer.Animate(overrideDuration ? this.duration : duration, AnimateHide, Complete, ref routine);
        }

        #endregion
    }
}
