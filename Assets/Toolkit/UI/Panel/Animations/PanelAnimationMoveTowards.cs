using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem.Animations {
    public class PanelAnimationMoveTowards : MonoBehaviour, IPanelAnimationObject {

        #region Variables

        [Header("Config")]
        [SerializeField] private Vector2 showLocation;
        [SerializeField] private Vector2 hideLocation;

        [Header("Overrides")]
        [SerializeField] private bool overrideDuration = false;
        [SerializeField, Min(0f)] private float duration = 0.4f;
        [SerializeField] private bool dontOverrideInitialPosition = false;

        private Vector2 target;
        private float distancePerSecond;
        private RectTransform rectTransform;

        #endregion

        #region Properties

        public bool IsComplete { get; private set; }
        public bool IsEnabled => gameObject.activeSelf;

        #endregion

        #region Init

        void Awake() {
            rectTransform = transform.ToRectTransform();
            if(!dontOverrideInitialPosition)
                rectTransform.anchoredPosition = hideLocation;
        }

        #endregion

        #region IPanelAnim Impl

        public void Cancel() {
            IsComplete = true;
        }

        void IPanelAnimationObject.Update(float dt) {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target, dt * distancePerSecond);
            if(Vector2.Distance(rectTransform.anchoredPosition, target) < 0.0001f) {
                rectTransform.anchoredPosition = target;
                IsComplete = true;
            }
        }

        void IPanelAnimationObject.Hide(float duration) {
            Cancel();
            IsComplete = false;
            duration = overrideDuration ? this.duration : duration;
            target = hideLocation;
            if(duration < 0.01f) {
                rectTransform.anchoredPosition = target;
                IsComplete = true;
                return;
            }
            distancePerSecond = Vector2.Distance(showLocation, hideLocation) / duration;
        }

        void IPanelAnimationObject.Show(float duration) {
            Cancel();
            IsComplete = false;
            duration = overrideDuration ? this.duration : duration;
            target = showLocation;
            if(duration < 0.01f) {
                rectTransform.anchoredPosition = target;
                IsComplete = true;
                return;
            }
            distancePerSecond = Vector2.Distance(showLocation, hideLocation) / duration;
        }

        #endregion
    }
}
