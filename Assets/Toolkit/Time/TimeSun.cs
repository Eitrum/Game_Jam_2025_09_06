using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Day Cycle/Time Sun")]
    public class TimeSun : MonoBehaviour
    {
        #region Variables

        [Header("Sun Color")]
        [SerializeField] private bool applySunColor = true;
        [SerializeField] private Gradient color = new Gradient();
        [SerializeField] private float intensityModifier = 1f;
        [SerializeField] private bool isMainSun = false;
        [Header("Rotation")]
        [SerializeField] private bool invert = false;
        private Light source;

        #endregion

        #region Properties

        public Light Source => source;
        public float SunRotation => invert ? 180f - TimeSystem.SunRotation : TimeSystem.SunRotation;
        public bool IsSunUp => TimeSystem.IsSunUp;
        public bool IsMainSun => isMainSun;

        #endregion

        #region Unity Methods

        private void OnEnable() {
            if(source == null) {
                source = GetComponentInChildren<Light>();
            }
            if(isMainSun && source != null && RenderSettings.sun == null) {
                RenderSettings.sun = source;
            }
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
        }

        #endregion

        #region Methods

        private void OnTimeUpdate(float dt, float time) {
            if(source) {
                source.enabled = TimeSystem.IsSunUp;
                if(applySunColor) {
                    source.color = color.Evaluate(TimeSystem.NormalizedTime);
                    source.intensity = TimeSystem.Intesity * intensityModifier;
                }
            }
            transform.localRotation = Quaternion.Euler(invert ? 180f - TimeSystem.SunRotation : TimeSystem.SunRotation, 0, 0);
        }

        #endregion

        #region Editor

        private void OnDrawGizmos() {
            var pos = transform.position;
            Gizmos.DrawLine(pos, pos + transform.forward);
            Gizmos.DrawLine(pos, pos + Quaternion.Euler(20, 0, 0) * transform.forward);
            Gizmos.DrawLine(pos, pos + Quaternion.Euler(-20, 0, 0) * transform.forward);
            GizmosUtility.DrawCircle(pos, transform.rotation * Quaternion.Euler(-90, 90, 0), 0.2f);
        }

        #endregion
    }
}
