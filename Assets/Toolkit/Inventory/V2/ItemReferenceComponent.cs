using Toolkit.UI;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public class ItemReferenceComponent : MonoBehaviour, IAssignable {
        #region Variables

        private Item item;
        public event System.Action OnReferenceChanged;

        #endregion

        #region Properties

        public Item Item {
            get => item;
            set {
                item = value;
                OnReferenceChanged?.Invoke();
            }
        }

        #endregion

        #region Assignable Impl

        public void Assign(Item item) {
            this.Item = item;
        }

        #endregion
    }
}
