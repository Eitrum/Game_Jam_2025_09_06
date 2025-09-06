using System.Collections;
using System.Collections.Generic;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Scale (OnEnable)")]
    public class ScaleOnEnable : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool overrideInitialScale = false;
        [SerializeField] private Vector3 initialScale = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 targetScale = Vector3.one;

        [SerializeField] private bool useConsistentDuration = true;
        [SerializeField, Min(0f)] private float durationPerUnit = 1f;
        [SerializeField] private EaseReference easing = new EaseReference(Ease.Type.Out, Ease.Function.Linear);

        private Coroutine routine = null;

        #endregion

        #region Properties

        public bool OverrideInitialScale {
            get => overrideInitialScale;
            set => overrideInitialScale = value;
        }

        public Vector3 InitialScale {
            get => initialScale;
            set => initialScale = value;
        }

        public Vector3 TargetScale {
            get => targetScale;
            set {
                targetScale = value;
                Play(false);
            }
        }

        public EaseReference Easing {
            get => easing;
            set => easing = value;
        }

        public bool UseConsistentDuration {
            get => useConsistentDuration;
            set => useConsistentDuration = value;
        }

        public float DurationPerUnit {
            get => durationPerUnit;
            set => durationPerUnit = value;
        }

        #endregion

        #region Init

        private void OnEnable() {
            PlayIfNotRunning();
        }

        private void OnDisable() {
            Timer.Stop(routine);
            routine = null;
        }

        #endregion

        #region Play

        [ContextMenu("Play")]
        public void Play()
            => Play(overrideInitialScale);


        public void Play(bool overrideInitialScale) {
            if(durationPerUnit <= Mathf.Epsilon) {
                transform.localScale = targetScale;
                return;
            }

            if(overrideInitialScale)
                transform.localScale = initialScale;

            var l = new Line(transform.localScale, targetScale);
            var dur = useConsistentDuration ? durationPerUnit : (durationPerUnit * l.Length);
            Timer.Animate(dur, Animate, l, easing.Evaluate, Finish, ref routine);
        }

        public void PlayIfNotRunning() {
            if(routine == null)
                Play();
        }

        #endregion

        #region Utility

        public void SetTargetSize(Vector3 size, bool updateSize) {
            this.targetScale = size;
            if(updateSize)
                Play();
        }

        #endregion

        #region Animate

        private void Animate(float t, Line l) {
            transform.localScale = l.Evaluate(t);
        }

        public void Finish() {
            Timer.Stop(routine);
            routine = null;
            transform.localScale = targetScale;
        }

        #endregion
    }
}
