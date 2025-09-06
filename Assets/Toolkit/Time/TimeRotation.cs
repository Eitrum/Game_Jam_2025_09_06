using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Day Cycle/Time Rotation")]
    public class TimeRotation : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Vector3 startRotation = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 direction = new Vector3(1f, 0f, 0f);
        [SerializeField] private float multiplier = 180f;

        #endregion

        #region Unity Methods

        private void OnEnable() {
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
        }

        #endregion

        #region Methods

        private void OnTimeUpdate(float dt, float time) {
            transform.localRotation = Quaternion.Euler(startRotation) * Quaternion.AngleAxis(TimeSystem.NormalizedTime * multiplier, direction);
        }

        #endregion

        #region Editor

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }

        #endregion
    }
}
