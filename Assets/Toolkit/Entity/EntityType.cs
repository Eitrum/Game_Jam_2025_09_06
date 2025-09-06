///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

namespace Toolkit {
	public enum EntityType : int {
		Uncategorized = 0,
		Player = 1,
		NPC = 2,
		Enemy = 3,
		Item = 4,
		Projectile = 5,
		Prop = 6,
	}

	public static class EntityTypeUtility {
		public static string GetName(EntityType type) {
			switch(type) {
				case EntityType.Uncategorized: return "Uncategorized";
				case EntityType.Player: return "Player";
				case EntityType.NPC: return "NPC";
				case EntityType.Enemy: return "Enemy";
				case EntityType.Item: return "Item";
				case EntityType.Projectile: return "Projectile";
				case EntityType.Prop: return "Prop";
			}
			return "";

		}
	}
}
