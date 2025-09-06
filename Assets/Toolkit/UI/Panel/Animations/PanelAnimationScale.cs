using System.Collections;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Animations {
    public class PanelAnimationScale : MonoBehaviour, IPanelAnimationObject {

        #region Variables

        [Header("Config")]
        [SerializeField] private Vector3 showScale;
        [SerializeField] private Vector3 hideScale;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private bool reverseOnHide = true;

        [Header("Overrides")]
        [SerializeField] private bool overrideDuration = false;
        [SerializeField, Min(0f)] private float duration = 0.4f;
        [SerializeField] private bool dontOverrideInitialScale = false;

        private Coroutine routine;
        private Vector3 startScale;

        #endregion

        #region Properties

        public bool IsComplete { get; private set; }
        public bool IsEnabled => gameObject.activeSelf;

        #endregion

        #region Init

        void Awake() {
            if(!dontOverrideInitialScale)
                transform.localScale = hideScale;
        }

        void OnDisable() {
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
            transform.localScale = Vector2.Lerp(startScale, showScale, t);
        }

        private void AnimateHide(float t) {
            if(reverseOnHide)
                t = 1f - curve.Evaluate(1f - t);
            else
                t = curve.Evaluate(t);

            transform.localScale = Vector2.Lerp(startScale, hideScale, t);
        }

        void Complete() {
            IsComplete = true;
        }

        #endregion

        #region IPanelAnim Impl

        public void Cancel() {
            Timer.Stop(routine);
            IsComplete = true;
        }

        void IPanelAnimationObject.Show(float duration) {
            if(!IsEnabled)
                return;
            Cancel();
            IsComplete = false;
            startScale = transform.localScale;
            Timer.Animate(overrideDuration ? this.duration : duration, AnimateShow, Complete, ref routine);
        }

        void IPanelAnimationObject.Hide(float duration) {
            if(!IsEnabled)
                return;
            Cancel();
            IsComplete = false;
            startScale = transform.localScale;
            Timer.Animate(overrideDuration ? this.duration : duration, AnimateHide, Complete, ref routine);
        }

        #endregion
    }
}
