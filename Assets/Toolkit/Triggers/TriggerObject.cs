using System;
using UnityEngine;

namespace Toolkit.Trigger {
    [CreateAssetMenu(menuName = "Toolkit/Trigger/Stateless Trigger Object")]
    public class TriggerObject : ScriptableObject, ITrigger {
        #region Variables

        private int activations;
        private OnTriggerDelegate onTrigger;

        #endregion

        #region Properties

        public int Activations => activations;
        public bool HasTriggered => false;

        public event OnTriggerDelegate OnTrigger {
            add {
                onTrigger += value;
            }
            remove {
                onTrigger -= value;
            }
        }

        #endregion

        #region Cause Trigger

        [Button("Trigger"), ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                CauseTrigger(s);
        }

        public void CauseTrigger(Source source) {
            activations++;
            onTrigger?.Invoke(source.AddChild(this as ITrigger));
        }

        #endregion
    }
}
