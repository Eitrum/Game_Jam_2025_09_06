using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Trigger - (Listen Relay)")]
    public class TriggerRelay : MonoBehaviour, ITrigger, ITriggerRelay {
        #region Variables

        [SerializeField] private TriggerSources listenTargets = new TriggerSources(); // Custom editor to check for trigger as parent inside target

        private ITrigger trigger;

        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Properties

        public TriggerSources Target {
            get => listenTargets;
        }

        public bool HasTriggered => trigger?.HasTriggered ?? false;

        IEnumerable<ITrigger> ITriggerRelay.Parents {
            get {
                foreach(var t in listenTargets.Sources)
                    yield return t.Trigger;
            }
        }

        #endregion

        #region Init

        private void OnEnable() {
            if(listenTargets != null)
                listenTargets.OnTrigger += CauseTrigger;
        }

        private void OnDisable() {
            if(listenTargets != null)
                listenTargets.OnTrigger -= CauseTrigger;
        }

        #endregion

        #region ITrigger Implementation

        [Button("Trigger")]
        private void EditorTrigger() {
            using(var t = Source.Create("Editor"))
                CauseTrigger(t);
        }

        public void CauseTrigger(Source source) {
            using(var t = source.AddChild(this as ITrigger))
                OnTrigger?.Invoke(t);
        }

        #endregion

        #region Editor

        void OnDrawGizmos() {
            if(listenTargets != null) {
                foreach(var t in listenTargets.TransformTargets)
                    Gizmos.DrawLine(transform.position, t.position);
            }
        }

        #endregion
    }
}
