using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health.Utility
{
    [AddComponentMenu("Toolkit/Health/Utility/Destroy On Death")]
    public class DestroyOnDeath : MonoBehaviour
    {
        #region Variables

        [SerializeField, Min(0f)] private float delay = 0f;
        private IHealth health;

        #endregion

        #region Init

        private void Awake() {
            health = GetComponentInParent<IHealth>();
        }

        private void OnEnable() {
            if(health != null)
                health.OnDeath += OnDeath;
        }

        private void OnDisable() {
            if(health != null)
                health.OnDeath -= OnDeath;
        }

        #endregion

        #region Death Callback

        [ContextMenu("OnDeath Callback")]
        public void ForceCallback()
            => OnDeath();

        private void OnDeath() {
            if(this == null)
                return;
            Destroy(gameObject, delay);
        }

        #endregion
    }
}
