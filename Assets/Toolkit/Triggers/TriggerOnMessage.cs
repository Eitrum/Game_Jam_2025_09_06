using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    public class TriggerOnMessage : MonoBehaviour, ITrigger {
        #region Variables

        private const string TAG = "[Toolkit.Trigger.TriggerOnMessage] - ";
        [SerializeField, Readonly(true)] private string eventId = "event0";
        [SerializeField] private bool activateOnlyOnce = false;
        [SerializeField] private bool logEventPayload = false;

        private int activations = 0;
        public event OnTriggerDelegate OnTrigger;
        public bool HasTriggered => activations > 0;

        #endregion

        #region Init

        private void OnEnable() {
            Message.Subscribe(eventId, OnReceived);
        }

        private void OnDisable() {
            Message.Unsubscribe(eventId, OnReceived);
        }

        #endregion

        #region Callback

        private void OnReceived(string payload) {
            if(logEventPayload)
                Debug.Log(TAG + $"Received ({eventId}) with payload ({payload})");
            using(var s = Source.Create(payload))
                CauseTrigger(s);
        }

        #endregion

        #region Trigger

        [Button]
        private void TriggerEditor() {
            using(var s = Source.Create("editor"))
                CauseTrigger(s);
        }

        public void CauseTrigger() {
            using(var s = Source.Create(this as ITrigger))
                CauseTrigger(s);
        }

        public void CauseTrigger(Source source) {
            if(activateOnlyOnce && HasTriggered) {
                return;
            }

            using(var s = source?.AddChild(this as ITrigger) ?? Source.Create(this as ITrigger)) {
                activations++;
                OnTrigger?.Invoke(s);
            }
        }

        #endregion
    }
}
