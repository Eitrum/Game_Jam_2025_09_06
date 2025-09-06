using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Callback/Log on Trigger")]
    public class LogOnTrigger : MonoBehaviour {
        #region Variables

        [SerializeField] private TriggerSources optionalSources;
        [SerializeField] private string message = "Trigger excecuted by {source}";
        private ITrigger trigger;

        #endregion

        #region Init

        private void Awake() {
            trigger = GetComponent<ITrigger>();
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
                optionalSources.OnTrigger += OnTrigger;
        }

        #endregion

        #region Callbacks

        private void OnTrigger(Source source) {
            Debug.Log($"[Trigger] - {message.Replace("{source}", $"({source.ToString(true)})")}");
        }

        #endregion
    }
}
