using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Utility/Destroy (OnTrigger)")]
    public class DestroyOnTrigger : MonoBehaviour {
        #region Variables

        private const string TAG = "[DestroyOnTrigger] - ";
        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private Transform target;
        [SerializeField, Min(0)] private float delay = 0f;

        private ITrigger trigger;

        #endregion

        #region Properties

        public float Delay {
            get => delay;
            set => delay = Mathf.Max(0f, value);
        }

        public Transform Target {
            get => target;
            set => target = value;
        }

        public ITrigger Trigger {
            get {
                if(trigger == null)
                    trigger = GetComponentInParent<ITrigger>();
                return trigger;
            }
        }

        #endregion

        #region Init

        private void Awake() {
            if(!target)
                target = transform;
            trigger = GetComponentInParent<ITrigger>();
#if UNITY_EDITOR
            if(trigger == null)
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

        #region Trigger Callback

        [Button, ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                OnTrigger(s);
        }

        private void OnTrigger(Source source) {
            if(target)
                Destroy(target.gameObject, delay);
            else
                Debug.LogError(TAG + "No target to destroy!");
        }

        #endregion
    }
}
