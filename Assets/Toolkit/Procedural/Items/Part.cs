///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

namespace Toolkit.Procedural.Items {
	public enum Part : int {
		None = 0,
		Sword__Hilt = 1,
		Sword__Blade = 2,
		Dagger__Hilt = 4,
		Dagger__Blade = 5,
		Wand__Handle = 6,
		Wand__Crystal = 7,
		Fist__Handle = 8,
		Fist__Blade = 9,
		Shield__Core = 10,
		Shield__Decoration = 11,
		Artistry__Core = 13,
		Artistry__Decoration = 14,
		Axe__Shaft = 17,
		Axe__Blade = 18,
		Mace__Handle = 19,
		Mace__Head = 20,
	}

	public static class PartUtility {
		public static string GetPath(Part part) {
			switch(part) {
				case Part.Sword__Hilt: return "Sword/Hilt";
				case Part.Sword__Blade: return "Sword/Blade";
				case Part.Dagger__Hilt: return "Dagger/Hilt";
				case Part.Dagger__Blade: return "Dagger/Blade";
				case Part.Wand__Handle: return "Wand/Handle";
				case Part.Wand__Crystal: return "Wand/Crystal";
				case Part.Fist__Handle: return "Fist/Handle";
				case Part.Fist__Blade: return "Fist/Blade";
				case Part.Shield__Core: return "Shield/Core";
				case Part.Shield__Decoration: return "Shield/Decoration";
				case Part.Artistry__Core: return "Artistry/Core";
				case Part.Artistry__Decoration: return "Artistry/Decoration";
				case Part.Axe__Shaft: return "Axe/Shaft";
				case Part.Axe__Blade: return "Axe/Blade";
				case Part.Mace__Handle: return "Mace/Handle";
				case Part.Mace__Head: return "Mace/Head";
			}
			return "None";

		}
	}
}
