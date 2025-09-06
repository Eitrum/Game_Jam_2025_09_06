using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health
{
    [AddComponentMenu("Toolkit/Health/Set Active (OnDeath)")]
    public class SetActiveOnDeath : MonoBehaviour
    {
        #region Variables

        private const string TAG = "[SetActiveOnDeath] - ";

        [SerializeField] private bool setInactiveOnAwake = true;
        [SerializeField, Min(0)] private float delay = 0f;
        [SerializeField] private Transform[] toSetActive;
        [SerializeField] private Transform[] toSetInactive;

        private IHealth health;

        #endregion

        #region Properties

        #endregion

        #region Init

        private void Awake() {
            health = GetComponentInParent<IHealth>();
#if UNITY_EDITOR
            if(health != null)
                Debug.LogError(TAG + $"No health found in parents of '{gameObject.name}'");
#endif

            if(setInactiveOnAwake)
                toSetActive.SetGameObjectsActive(false);
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

        private void OnDeath() {
            if(delay > 0f)
                Timer.Once(delay, Activate);
            else
                Activate();
        }

        public void Activate() {
            toSetActive?.SetGameObjectsActiveSafe(true);
            toSetInactive?.SetGameObjectsActiveSafe(false);
        }

        #endregion

        #region Editor

        private void OnDrawGizmosSelected() {
            var pos = transform.position;

            using(new GizmosUtility.ColorScope(ColorTable.LawnGreen)) {
                if(toSetActive != null)
                    foreach(var t in toSetActive) {
                        if(t)
                            Gizmos.DrawLine(pos, t.position);
                    }
            }

            using(new GizmosUtility.ColorScope(ColorTable.IndianRed)) {
                if(toSetInactive != null)
                    foreach(var t in toSetInactive) {
                        if(t)
                            Gizmos.DrawLine(pos, t.position);
                    }
            }
        }

        #endregion
    }
}
