using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Interactables {
    public class InteractableObject : MonoBehaviour, IInteractableObject, IInteractableMany, IInteractable {
        #region Variables

        [SerializeField] private bool hidden = false;
        [SerializeField] private bool sort = false;
        private static List<IInteractable> interactablesCache = new List<IInteractable>();
        private List<IInteractableOption> options = new List<IInteractableOption>();

        #endregion

        #region Properties

        public bool IsHidden {
            get {
                if(hidden)
                    return true;

                if(options.Count == 0)
                    return true;

                for(int i = 0, length = options.Count; i < length; i++) {
                    if(options[i].GetState(null) != OptionState.Hidden)
                        return false;
                }

                return true;
            }
        }
        public IReadOnlyList<IInteractableOption> Options => options;

        #endregion

        #region Init

        void Awake() {
            GetComponents(interactablesCache);
            var thisinteract = this as IInteractable;
            foreach(var i in interactablesCache) {
                if(thisinteract == i) continue;
                if(i is IInteractableMany many)
                    options.AddRange(many.Options);
                else
                    options.Add(new InteractableOptionWrapper(i));
            }
            if(sort && options.Count > 1)
                options.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        #endregion

        #region Interact

        public void Interact(Source source, int index) {
            if(index < 0 || index >= options.Count) {
                Debug.LogError(this.FormatLog("Out of range"));
                return;
            }
            options[index].Interact(source);
        }

        void IInteractable.Interact(Source source) {
            if(options == null || options.Count == 0)
                return;
            // Element 0 should always be default
            options[0].Interact(source);
        }

        #endregion

        #region Register / Unregister

        public void Register(IInteractableOption option) {
            for(int i = this.options.Count - 1; i >= 0; i--) {
                if(this.options[i].Order > option.Order)
                    continue;
                this.options.Insert(i + 1, option);
                return;
            }
            this.options.Insert(0, option);
        }

        public void Unregister(IInteractableOption option) {
            this.options.Remove(option);
        }

        #endregion
    }
}
