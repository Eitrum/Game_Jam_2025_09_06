using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.Interactables;
using System;

namespace Toolkit.Trigger {
    public class TriggerOnInteract : MonoBehaviour, ITrigger, IInteractableMany {
        #region Variables

        [SerializeField] private string interactName = "Interact";
        [SerializeField] private bool repeatable = false;
        [SerializeField] private bool hideOptionIfTriggered = false;

        private InteractableDynamicOption dynamicOption;
        private InteractableDynamicOption[] cachedOptions;

        private bool triggered = false;
        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Init

        void Awake() {

        }

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

        public IReadOnlyList<IInteractableOption> Options {
            get {
                if(cachedOptions == null) {
                    dynamicOption = new InteractableDynamicOption(interactName, Interact, CanInteract);
                    cachedOptions = new InteractableDynamicOption[] { dynamicOption };
                }
                return cachedOptions;
            }
        }

        #endregion

        #region Interact Impl

        private OptionState CanInteract(Source source) {
            if(repeatable && !triggered)
                return OptionState.None;

            return hideOptionIfTriggered ? OptionState.Hidden : OptionState.Disabled;
        }

        #endregion

        public void Interact(Source source) {
            if(!repeatable && triggered) {
                return;
            }
            try {
                triggered = true;
                OnTrigger?.Invoke(source);
            }
            finally {

            }
        }

        #region ITrigger Impl

        public void CauseTrigger(Source source) {
            using(var s = source.AddChild(this))
                Interact(s);
        }

        [Button]
        public void CauseTrigger() {
            using(var s = Source.Create(this))
                Interact(s);
        }

        #endregion
    }
}
