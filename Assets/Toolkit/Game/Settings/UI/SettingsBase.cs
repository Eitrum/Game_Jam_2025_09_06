using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.UI {
    public class SettingsBase : MonoBehaviour {

        public delegate void OnUpdateDelegate(InGameSettings.Entry entry);

        #region Variables

        private InGameSettings.Entry entry;
        private OnUpdateDelegate onUpdate;

        #endregion

        #region Properties

        public InGameSettings.Entry Entry {
            get => entry;
            protected set {
                this.entry = value;
                onUpdate?.Invoke(entry);
            }
        }

        public event OnUpdateDelegate OnIsDirtyUpdate {
            add {
                onUpdate += value;
                if(entry != null)
                    value?.Invoke(entry);
            }
            remove => onUpdate -= value;
        }

        #endregion

        protected void InvokeUpdate() {
            onUpdate?.Invoke(entry);
        }

        public void Do(SettingsOperationType operationType) {
            if(entry == null)
                return;
            switch(operationType) {
                case SettingsOperationType.Apply: entry.ApplyChange(); break;
                case SettingsOperationType.Discard: entry.DiscardChanges(); break;
                case SettingsOperationType.Reset: entry.Reset(); break;
            }
        }
    }
}
