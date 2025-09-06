using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    public class MessageOnTrigger : MonoBehaviour {

        #region Variables

        private const string TAG = "[Toolkit.Trigger.MessageOnTrigger] - ";
        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private string eventId = "event0";
        [SerializeField] private string payload = "";

        private ITrigger trigger;

        #endregion

        #region Init

        private void Awake() {
            trigger = GetComponentInParent<ITrigger>();
#if UNITY_EDITOR
            if(trigger == null && optionalSources.IsAnyValid)
                Debug.LogError(TAG + $"No trigger found in parents of '{gameObject.name}'");
#endif
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

        #region OnTrigger

        [Button, ContextMenu("Trigger")]
        private void Trigger() {
            OnTrigger(null);
        }

        private void OnTrigger(Source source) {
            Message.Publish(eventId, payload);
        }

        #endregion
    }
}
