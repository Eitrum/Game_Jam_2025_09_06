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
	public enum Rarity : int {
		None = 0,
		Common = 1,
		Uncommon = 2,
		Rare = 3,
		Epic = 4,
		Legendary = 5,
		Unique = 6,
		NPC = 7,
	}

	public static class RarityUtility {
		public static UnityEngine.Color32 ToColor(this Toolkit.Inventory.Rarity rarity) {
			switch(rarity) {
				case Rarity.Common: return new Color32(207,165,128,255);
				case Rarity.Uncommon: return new Color32(110,242,106,255);
				case Rarity.Rare: return new Color32(63,129,255,255);
				case Rarity.Epic: return new Color32(219,51,252,255);
				case Rarity.Legendary: return new Color32(255,183,27,255);
				case Rarity.Unique: return new Color32(128,234,255,255);
				case Rarity.NPC: return new Color32(0,0,0,0);
			}
			return new Color32(0,0,0,0);

		}

		public static string ToHex(this Toolkit.Inventory.Rarity rarity) {
			switch(rarity) {
				case Rarity.Common: return "CFA580FF";
				case Rarity.Uncommon: return "6EF26AFF";
				case Rarity.Rare: return "3F81FFFF";
				case Rarity.Epic: return "DB33FCFF";
				case Rarity.Legendary: return "FFB71BFF";
				case Rarity.Unique: return "80EAFFFF";
				case Rarity.NPC: return "00000000";
			}
			return "00000000";

		}

		public static string ToString(this Toolkit.Inventory.Rarity rarity) {
			switch(rarity) {
				case Rarity.Common: return "Common";
				case Rarity.Uncommon: return "Uncommon";
				case Rarity.Rare: return "Rare";
				case Rarity.Epic: return "Epic";
				case Rarity.Legendary: return "Legendary";
				case Rarity.Unique: return "Unique";
				case Rarity.NPC: return "NPC";
			}
			return "";

		}
	}
}
