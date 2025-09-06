using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Toolkit/Day Cycle/Time Light Color")]
    public class TimeLightColor : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Gradient color = new Gradient();
        [SerializeField] private AnimationCurve intensity = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        private Light source;

        #endregion

        #region Unity Methods

        private void Awake() {
            source = GetComponent<Light>();
        }

        private void OnEnable() {
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
        }

        #endregion

        #region Methods

        private void OnTimeUpdate(float dt, float time) {
            source.color = color.Evaluate(TimeSystem.NormalizedTime);
            source.intensity = intensity.Evaluate(TimeSystem.NormalizedTime);
        }

        #endregion
    }
}
