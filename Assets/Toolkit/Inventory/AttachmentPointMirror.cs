using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    public class AttachmentPointMirror : MonoBehaviour
    {
        [SerializeField] private EquipmentSlot slot = EquipmentSlot.None;
        [SerializeField] private bool useScale = true;
        [SerializeField] private Vector3 mirrorScale = new Vector3(-1, 1, 1);
        [SerializeField] private Transform container;

        private void Awake() {
            if(!container)
                container = transform;
        }

        public void ActivateSlot(EquipmentSlot slot) {
            if(slot != this.slot)
                return;
            if(container == null)
                return;
            if(useScale) {
                container.localScale = mirrorScale;
            }
        }
    }
}
