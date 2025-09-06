using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Toolkit.Interactables {
    public class InteractableGeneric : MonoBehaviour, IInteractableMany {

        public enum Config {
            Default,
            UseOnce,
            UseOnceAndHide,
        }

        [System.Serializable]
        public class Option : IInteractableOption {
            #region Variables

            [field: SerializeField] public string Name { get; private set; } = "Interact";
            [field: SerializeField] public string Description { get; private set; }
            [field: SerializeField] public int Order { get; private set; }

            [SerializeField] private Config useConfig = Config.Default;
            [SerializeField] private UnityEvent<Source> callback;
            [System.NonSerialized] private bool hasInteracted;

            #endregion

            #region Properties

            public bool HasInteracted {
                get => hasInteracted;
                set => hasInteracted = value;
            }

            #endregion

            #region GetState

            public OptionState GetState(Source source) {
                switch(useConfig) {
                    case Config.UseOnce: return hasInteracted ? OptionState.Disabled : OptionState.None;
                    case Config.UseOnceAndHide: return hasInteracted ? OptionState.Hidden : OptionState.None;
                    default: return OptionState.None;
                }
            }

            #endregion

            #region Interact

            public void Interact(Source source) {
                if(useConfig != Config.Default && hasInteracted) {
                    Debug.Log(TAG + "Attempting to interact with an disabled interactable");
                    return;
                }
                hasInteracted = true;

                if(callback == null) {
                    Debug.LogError(TAG + "Callback is null!");
                    return;
                }
                if(callback.GetPersistentEventCount() == 0) {
                    Debug.Log(TAG + $"Interacted with '{Name}'");
                    return;
                }
                callback.Invoke(source);
            }

            #endregion
        }

        #region Variables

        private const string TAG = "[Toolkit.Interactables.InteractableGeneric] - ";
        [SerializeField] private List<Option> options = new List<Option>();

        #endregion

        #region Proeprties

        public IReadOnlyList<IInteractableOption> Options => options;

        #endregion

        #region Interact Impl

        public void Interact(Source source) {
            options[0].Interact(source);
        }

        #endregion

        #region Editor

        [Button]
        private void ResetHasInteracted() {
            foreach(var o in options)
                o.HasInteracted = false;
        }

        #endregion
    }
}
