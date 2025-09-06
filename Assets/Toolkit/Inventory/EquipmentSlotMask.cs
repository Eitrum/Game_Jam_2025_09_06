///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

namespace Toolkit.Inventory {
	public class EquipmentSlotMask : UnityEngine.ScriptableObject {
		public bool Chest = true;
		public bool Helmet = true;
		public bool Main_Hand = true;
		public bool Off_Hand = true;
		public bool Trinket = true;
		public bool Unknown = true;
		public bool Legs = true;
		public bool Consumable1 = true;
		public bool Consumable2 = true;
		public bool IsSlotEnabled(EquipmentSlot slot) {
			switch(slot){
				case EquipmentSlot.Chest: return Chest;
				case EquipmentSlot.Helmet: return Helmet;
				case EquipmentSlot.Main_Hand: return Main_Hand;
				case EquipmentSlot.Off_Hand: return Off_Hand;
				case EquipmentSlot.Trinket: return Trinket;
				case EquipmentSlot.Unknown: return Unknown;
				case EquipmentSlot.Legs: return Legs;
				case EquipmentSlot.Consumable: return Consumable1;
				case EquipmentSlot.Consumable2: return Consumable2;
			}
			return false;

		}
	}
}
