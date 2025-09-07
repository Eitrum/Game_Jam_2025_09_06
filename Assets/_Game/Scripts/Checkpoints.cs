using UnityEngine;
using Toolkit;
using Toolkit.Trigger;
using System;
using System.Collections.Generic;
using Toolkit.Unit;
using Toolkit.Health;

namespace Game {
    public class Checkpoints : MonoBehaviour {

        public static List<Checkpoints> allCheckpoints = new List<Checkpoints>();
        public static Checkpoints lastUsedCheckpoint;

        public Transform[] toEnableOrDisable;
        public GameObject toSpawnOnTrigger;

        private ITrigger trigger;

        private void Awake() {
            trigger = GetComponent<ITrigger>();
            trigger.OnTrigger += OnTrigger;
            SetActive(false);
        }

        private void OnTrigger(Source source) {
            if(lastUsedCheckpoint == this) {
                return;
            }
            lastUsedCheckpoint?.SetActive(false);
            lastUsedCheckpoint = this;
            SetActive(true);

            var body = source.Find(SourceType.Rigidbody);
            if(body != null && body.TryGet(out Rigidbody rb) && rb.TryGetComponent<IHealth>(out IHealth health)) {
                health.Restore(true);
            }
        }

        public void SetActive(bool active) {
            foreach(var t in toEnableOrDisable) {
                t.SetActive(active);
            }
        }
    }
}
