using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.Trigger;
using UnityEngine;

namespace Toolkit.Trigger {
    [DefaultExecutionOrder(-10000)]
    [AddComponentMenu("Toolkit/Trigger/Utility/Set Active (OnTrigger)")]
    public class SetActiveOnTrigger : MonoBehaviour {
        #region Variables

        private const string TAG = "[SetActiveOnTrigger] - ";

        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private bool setInactiveOnAwake = true;
        [SerializeField, Min(0)] private float delay = 0f;
        [SerializeField] private Transform[] toSetActive;
        [SerializeField] private Transform[] toSetInactive;

        private ITrigger trigger;

        #endregion

        #region Properties

        #endregion

        #region Init

        private void Awake() {
            trigger = GetComponentInParent<ITrigger>();
#if UNITY_EDITOR
            if(trigger != null)
                Debug.LogError(TAG + $"No trigger found in parents of '{gameObject.name}'");
#endif

            if(setInactiveOnAwake)
                toSetActive.SetGameObjectsActive(false);
        }

        private void OnEnable() {
            if(trigger != null)
                trigger.OnTrigger += OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger += OnTrigger;
        }

        private void OnDisable() {
            if(trigger != null)
                trigger.OnTrigger -= OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger -= OnTrigger;
        }

        #endregion

        #region Trigger Callback

        [Button, ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                OnTrigger(s);
        }

        private void OnTrigger(Source source) {
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
