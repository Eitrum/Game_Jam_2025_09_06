using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Simple Item Reference")]
    public class SimpleItemReference : MonoBehaviour, IItemReference
    {
        private IItem item;
        public IItem Item {
            get => item;
            set {
                if(value == null) {
                    Destroy(this.gameObject);
                    return;
                }
                if(item == null || item.IsNull) {
                    item = value;
                }
                else {
                    Debug.LogError("Attempting to assign an item when it already contains a reference");
                }
            }
        }

        void OnDestroy() {
            if(item != null && !item.IsNull) {
                item.DestroyItem();
            }
        }
    }
}
