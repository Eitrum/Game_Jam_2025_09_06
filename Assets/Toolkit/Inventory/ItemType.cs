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
	public enum ItemType : int {
		None = 0,
		Sword = 1,
		Dagger = 3,
		Wand = 5,
		Helmet = 7,
		Shield = 8,
		Chest = 10,
		Legs = 11,
		Resource = 12,
		Axe = 13,
		Fist = 14,
		Mace = 15,
		Heirloom = 16,
		Potion = 17,
		Artistry = 18,
		Trinket = 19,
		Crossbow = 20,
	}

	public static class ItemTypeUtility {
		public static string ToString(this Toolkit.Inventory.ItemType slot) {
			switch(slot) {
				case ItemType.None: return "None";
				case ItemType.Sword: return "Sword";
				case ItemType.Dagger: return "Dagger";
				case ItemType.Wand: return "Wand";
				case ItemType.Helmet: return "Helmet";
				case ItemType.Shield: return "Shield";
				case ItemType.Chest: return "Chest";
				case ItemType.Legs: return "Legs";
				case ItemType.Resource: return "Resource";
				case ItemType.Axe: return "Axe";
				case ItemType.Fist: return "Fist";
				case ItemType.Mace: return "Force";
				case ItemType.Heirloom: return "Heirloom";
				case ItemType.Potion: return "Potion";
				case ItemType.Artistry: return "Artistry";
				case ItemType.Trinket: return "Trinket";
				case ItemType.Crossbow: return "Crossbow";
			}
			return "Unknown";

		}
	}
}
