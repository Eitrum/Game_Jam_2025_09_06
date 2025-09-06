
///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

namespace Toolkit.Health {
	public enum DamageTypeCustom : int {
		None = 0,
		Fire = 1,
		Earth = 2,
		Wind = 4,
		Water = 8
	}

	public static class DamageTypeCustomUtility {
		public static string ToString(this Toolkit.Health.DamageTypeCustom type) {
			switch(type) {
				case DamageTypeCustom.Fire: return "Fire";
				case DamageTypeCustom.Earth: return "Earth";
				case DamageTypeCustom.Wind: return "Wind";
				case DamageTypeCustom.Water: return "Water";
			}
			return "Unknown";

		}
	}
}
