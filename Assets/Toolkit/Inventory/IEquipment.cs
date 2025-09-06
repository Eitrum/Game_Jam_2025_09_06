using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory {

    public delegate void OnEquipDelegate(IItem item, EquipmentSlot slot);
    public delegate void OnUnequipDelegate(IItem item, EquipmentSlot slot);

    public interface IEquipment {
        bool Equip(IItem item);
        bool Equip(IItem item, EquipmentSlot slot);
        bool Unequip(IItem item);
        bool Unequip(EquipmentSlot slot);
        bool Unequip(int index);
        event OnEquipDelegate OnEquip;
        event OnUnequipDelegate OnUnequip;


        float TotalWeight { get; }
        int EquipmentSlots { get; }
        EquipmentSlot GetEquipmentSlot(int index);

        IItem GetEquipment(int index);
        IItem GetEquipment(EquipmentSlot slot);
        bool HasEquipment(EquipmentSlot slot);
        bool CanEquip(IItem item, EquipmentSlot slot);

        IEnumerable<IItem> Find(System.Func<IItem, bool> predicate);
        IEnumerable<T> Find<T>(System.Func<IItem, T> predicate);
    }
}
