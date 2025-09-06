using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.DayCycle;
using System;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (Time Change)")]
    public class TriggerOnTimeChange : MonoBehaviour, ITrigger {
        #region Variables

        [SerializeField] private bool repeatable = true;
        [SerializeField, Time] private List<float> triggerAtTime = new List<float>();

        private bool triggered;

        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Properties

        public bool Repeatable {
            get => repeatable;
            set => repeatable = value;
        }

        public bool HasTriggered {
            get => triggered;
            set => triggered = value;
        }

        public List<float> TriggerAtTime => triggerAtTime;

        #endregion

        #region Init

        private void OnEnable() {
            Toolkit.DayCycle.TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            Toolkit.DayCycle.TimeSystem.OnTimeUpdate -= OnTimeUpdate;
        }

        #endregion

        #region Time Update

        private void OnTimeUpdate(float dt, float time) {
            if(dt > 0) {
                for(int i = triggerAtTime.Count - 1; i >= 0; i--) {
                    if(triggerAtTime[i] < time && triggerAtTime[i] > time - dt) {
                        CauseTrigger();
                    }
                }
            }
            else {
                for(int i = triggerAtTime.Count - 1; i >= 0; i--) {
                    if(triggerAtTime[i] > time && triggerAtTime[i] < time - dt) {
                        CauseTrigger();
                    }
                }
            }
        }

        #endregion

        #region ITrigger Impl

        [Button]
        public void CauseTrigger()
            => Handle(Source.Create(this as ITrigger));

        public void CauseTrigger(Source source)
            => Handle(source.AddChild(this as ITrigger));

        private void Handle(Source source) {
            if(!repeatable && triggered)
                return;
            try {
                triggered = true;
                OnTrigger?.Invoke(source);
            }
            finally {
                source.Dispose();
            }
        }

        #endregion
    }
}
