using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [System.Serializable]
    public struct TriggerSource {
        #region Variables

        private const string TAG = "[Toolkit.Trigger.TriggerSource] - ";
        [SerializeField] private UnityEngine.Object source;
        private ITrigger trigger;

        #endregion

        #region Properties

        public UnityEngine.Object Source {
            get { return source; }
            set {
                source = value;
                Reload();
            }
        }

        public ITrigger Trigger {
            get {
                Initialize();
                return trigger;
            }
            set {
                trigger = value;
                source = value as UnityEngine.Object;
            }
        }

        public bool IsValid {
            get {
                if(trigger != null)
                    return true;
                Initialize();
                return trigger != null;
            }
        }

        public event OnTriggerDelegate OnTrigger {
            add {
                Initialize();
                if(trigger != null)
                    trigger.OnTrigger += value;
                else
                    Debug.LogWarning(TAG + $"Source is missing trigger reference");
            }
            remove {
                if(trigger != null)
                    trigger.OnTrigger -= value;
            }
        }

        #endregion

        #region Constructor

        public TriggerSource(UnityEngine.Object source) {
            this.source = source;
            trigger = null;
            Reload();
        }

        public TriggerSource(ITrigger trigger) {
            this.source = trigger as UnityEngine.Object;
            this.trigger = trigger;
        }

        #endregion

        #region Methods

        private void Initialize() {
            if(trigger == null)
                Reload();
        }

        public void Assign<T>(T source) where T : UnityEngine.Object {
            this.source = source;
            Reload();
        }

        public void Assign(ITrigger trigger) {
            this.trigger = trigger;
            this.source = trigger as UnityEngine.Object;
        }

        private void Reload() {
            trigger = null;

            if(source == null)
                return;

            switch(source) {
                case Transform transform: {
                        trigger = transform.GetComponentInParent<ITrigger>();
                    }
                    break;
                case GameObject gameObject: {
                        trigger = gameObject.GetComponentInParent<ITrigger>();
                    }
                    break;
                case Component component: {
                        trigger = component.GetComponentInParent<ITrigger>();
                    }
                    break;
                case ScriptableObject scriptableObject: {
                        trigger = scriptableObject as ITrigger;
                    }
                    break;
            }
        }

        #endregion

        #region Static Util

        public static bool TryGetTrigger<T>(T source, out ITrigger trigger) where T : UnityEngine.Object {
            trigger = null;
            if(source == null)
                return false;

            switch(source) {
                case Transform transform: {
                        trigger = transform.GetComponentInParent<ITrigger>();
                    }
                    break;
                case GameObject gameObject: {
                        trigger = gameObject.GetComponentInParent<ITrigger>();
                    }
                    break;
                case Component component: {
                        trigger = component.GetComponentInParent<ITrigger>();
                    }
                    break;
                case ScriptableObject scriptableObject: {
                        trigger = scriptableObject as ITrigger;
                    }
                    break;
            }
            return trigger != null;
        }

        #endregion
    }
}
