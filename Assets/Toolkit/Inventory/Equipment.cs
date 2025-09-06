using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Equipment")]
    public class Equipment : MonoBehaviour, IEquipment
    {

        [SerializeField] private EquipmentSlotMask mask = null;
        [SerializeField] private bool destroyEquippedItemOnDestroy = true;
        private Slot[] slots;
        private OnEquipDelegate onEquip;
        private OnUnequipDelegate onUnequip;

        void Awake() {
            // Safety Check
            slots = new Slot[EquipmentSlots];
            // Apply Mask
            if(mask != null) {
                for(int i = 0; i < EquipmentSlotUtility.SLOTS; i++) {
                    slots[i].Enabled = mask.IsSlotEnabled((EquipmentSlot)i + 1);
                }
            }
            else {
                for(int i = 0; i < EquipmentSlotUtility.SLOTS; i++) {
                    slots[i].Enabled = true;
                }
            }
        }

        void OnDestroy() {
            if(destroyEquippedItemOnDestroy) {
                foreach(var slot in slots) {
                    if(slot.equipedItem != null && slot.equipedItem is UnityEngine.Object o) {
                        Destroy(o);
                    }
                }
            }
        }

        public event OnEquipDelegate OnEquip { add => onEquip += value; remove => onEquip -= value; }
        public event OnUnequipDelegate OnUnequip { add => onUnequip += value; remove => onUnequip -= value; }

        public float TotalWeight {
            get {
                float res = 0f;
                for(int i = 0; i < EquipmentSlotUtility.SLOTS; i++) {
                    if(slots[i].equipedItem != null && slots[i].equipedItem is IItemWeight w) {
                        res += w.Weight;
                    }
                }
                return res;
            }
        }
        public int EquipmentSlots => EquipmentSlotUtility.SLOTS;
        public bool CanEquip(IItem item, EquipmentSlot slot) {
            return slots[(int)slot - 1].Enabled && EquipmentSlotUtility.CanEquip(slot, item.Type);
        }

        public bool Equip(IItem item) {
            if(item == null)
                return false;
            var slot = EquipmentSlotUtility.GetPreferredSlot(item.Type);
            if(slot == EquipmentSlot.None) {
                return false;
            }
            var occupied = HasEquipment(slot);

            if(occupied) {
                for(int i = 0; i < EquipmentSlotUtility.SLOTS; i++) {
                    if((int)slot != (i + 1) && CanEquip(item, (EquipmentSlot)(i + 1)) && slots[i].equipedItem == null) {
                        slots[i].equipedItem = item;
                        onEquip?.Invoke(item, (EquipmentSlot)(i + 1));
                        return true;
                    }
                }
            }
            else if(slots[(int)slot - 1].Enabled) {
                slots[(int)slot - 1].equipedItem = item;
                onEquip?.Invoke(item, slot);
                return true;
            }
            return false;
        }

        public bool Equip(IItem item, EquipmentSlot slot) {
            if(slot == EquipmentSlot.None) {
                Debug.LogWarning("Attempting to equip item into a fake slot");
                return false;
            }
            if(item == null) {
                Debug.LogWarning("Attempting to equip item that is null");
                return false;
            }
            if(!CanEquip(item, slot)) {
                Debug.LogWarning("Attempting to equip item to a slot that is not accepted");
                return false;
            }
            var occupied = HasEquipment(slot);
            if(occupied)
                return false;
            slots[(int)slot - 1].equipedItem = item;
            onEquip?.Invoke(item, slot);
            return true;
        }

        public IItem GetEquipment(int index) {
            return slots[index].equipedItem;
        }

        public IItem GetEquipment(EquipmentSlot slot) {
            return slots[(int)slot - 1].equipedItem;
        }

        public EquipmentSlot GetEquipmentSlot(int index) {
            return (EquipmentSlot)(index + 1);
        }

        public bool HasEquipment(EquipmentSlot slot) {
            return slots[(int)slot - 1].equipedItem != null;
        }

        public bool Unequip(EquipmentSlot slot) {
            if(slots[(int)slot - 1].equipedItem != null) {
                var item = slots[(int)slot - 1].equipedItem;
                slots[(int)slot - 1].equipedItem = null;
                onUnequip?.Invoke(item, slot);
                return true;
            }
            return false;
        }

        public bool Unequip(IItem item) {
            for(int i = 0; i < EquipmentSlotUtility.SLOTS; i++) {
                if(slots[i].equipedItem == item) {
                    slots[i].equipedItem = null;
                    onUnequip?.Invoke(item, (EquipmentSlot)(i + 1));
                    return true;
                }
            }
            return false;
        }

        public bool Unequip(int index) {
            if(slots[index].equipedItem != null) {
                var item = slots[index].equipedItem;
                slots[index].equipedItem = null;
                onUnequip?.Invoke(item, (EquipmentSlot)(index + 1));
                return true;
            }
            return false;
        }

        public IItem SetSlotActive(EquipmentSlot slot, bool active) {
            slots[(int)slot - 1].Enabled = active;
            return GetEquipment(slot);
        }

        public IEnumerable<IItem> Find(System.Func<IItem, bool> predicate) {
            for(int i = 0, length = slots.Length; i < length; i++) {
                if(slots[i].equipedItem != null && predicate(slots[i].equipedItem))
                    yield return slots[i].equipedItem;
            }
        }

        public IEnumerable<T> Find<T>(System.Func<IItem, T> findFunction) {
            for(int i = 0, length = slots.Length; i < length; i++) {
                if(slots[i].equipedItem != null) {
                    var res = findFunction(slots[i].equipedItem);
                    if(res != null)
                        yield return res;
                }
            }
        }

        public struct Slot
        {
            public bool Enabled;
            public IItem equipedItem;
        }
    }
}
