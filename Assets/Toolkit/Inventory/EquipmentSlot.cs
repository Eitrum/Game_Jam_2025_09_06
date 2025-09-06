///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

using UnityEngine;
namespace Toolkit.Inventory {
	public enum EquipmentSlot : int {
		None = 0,
		Chest = 1,
		Helmet = 2,
		Main_Hand = 3,
		Off_Hand = 4,
		Trinket = 5,
		Unknown = 6,
		Legs = 7,
		Consumable = 8,
		Consumable2 = 9,
	}

	public static class EquipmentSlotUtility {
		public const System.Int32 SLOTS = 9;
		public static string ToString(this Toolkit.Inventory.EquipmentSlot slot) {
			switch(slot) {
				case EquipmentSlot.None: return "None";
				case EquipmentSlot.Chest: return "Chest";
				case EquipmentSlot.Helmet: return "Helmet";
				case EquipmentSlot.Main_Hand: return "Main Hand";
				case EquipmentSlot.Off_Hand: return "Off Hand";
				case EquipmentSlot.Trinket: return "Trinket";
				case EquipmentSlot.Unknown: return "Unknown";
				case EquipmentSlot.Legs: return "Legs";
				case EquipmentSlot.Consumable: return "Consumable";
				case EquipmentSlot.Consumable2: return "Consumable2";
			}
			return "Unknown";

		}

		public static bool CanEquip(Toolkit.Inventory.EquipmentSlot slot, Toolkit.Inventory.ItemType type) => CanEquip(type, slot);

		public static bool CanEquip(Toolkit.Inventory.ItemType type, Toolkit.Inventory.EquipmentSlot slot) {
			switch(slot){
				case EquipmentSlot.Chest:
					switch(type){
						case ItemType.Chest:return true;
					}
				break;
				case EquipmentSlot.Helmet:
					switch(type){
						case ItemType.Helmet:return true;
					}
				break;
				case EquipmentSlot.Main_Hand:
					switch(type){
						case ItemType.Sword:return true;
						case ItemType.Dagger:return true;
						case ItemType.Wand:return true;
						case ItemType.Shield:return true;
						case ItemType.Axe:return true;
						case ItemType.Fist:return true;
						case ItemType.Mace:return true;
						case ItemType.Heirloom:return true;
						case ItemType.Artistry:return true;
						case ItemType.Crossbow: return true;
					}
				break;
				case EquipmentSlot.Off_Hand:
					switch(type){
						case ItemType.Sword:return true;
						case ItemType.Dagger:return true;
						case ItemType.Wand:return true;
						case ItemType.Shield:return true;
						case ItemType.Axe:return true;
						case ItemType.Fist:return true;
						case ItemType.Mace:return true;
						case ItemType.Heirloom:return true;
						case ItemType.Artistry:return true;
						case ItemType.Crossbow:return true;
					}
				break;
				case EquipmentSlot.Trinket:
					switch(type){
						case ItemType.Trinket:return true;
					}
				break;
				case EquipmentSlot.Legs:
					switch(type){
						case ItemType.Legs:return true;
					}
				break;
				case EquipmentSlot.Consumable:
					switch(type){
						case ItemType.Potion:return true;
					}
				break;
			}
			return false;

		}

		public static Toolkit.Inventory.EquipmentSlot GetPreferredSlot(Toolkit.Inventory.ItemType itemType) {
			switch(itemType){
				case ItemType.Chest: return EquipmentSlot.Chest;
				case ItemType.Helmet: return EquipmentSlot.Helmet;
				case ItemType.Sword: return EquipmentSlot.Main_Hand;
				case ItemType.Dagger: return EquipmentSlot.Main_Hand;
				case ItemType.Wand: return EquipmentSlot.Main_Hand;
				case ItemType.Shield: return EquipmentSlot.Main_Hand;
				case ItemType.Axe: return EquipmentSlot.Main_Hand;
				case ItemType.Fist: return EquipmentSlot.Main_Hand;
				case ItemType.Mace: return EquipmentSlot.Main_Hand;
				case ItemType.Heirloom: return EquipmentSlot.Main_Hand;
				case ItemType.Artistry: return EquipmentSlot.Main_Hand;
				case ItemType.Trinket: return EquipmentSlot.Trinket;
				case ItemType.Legs: return EquipmentSlot.Legs;
				case ItemType.Potion: return EquipmentSlot.Consumable;
				case ItemType.Crossbow: return EquipmentSlot.Main_Hand;
			}
			return EquipmentSlot.None;

		}
	}
}
