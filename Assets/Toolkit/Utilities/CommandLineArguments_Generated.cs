///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.CommandLineArgumentsEditor.cs

namespace Toolkit {
	public static partial class CommandLineArguments {
		public static bool Debug = false;
		public static bool Save_Enabled = false;
		public static int width = 0;
		public static int height = 0;
		private static void LoadPresets() {
			if(TryGetValue("-debug", out bool Debug))
				UnityEngine.Debug.Log(TAG + "Launching with Debug = " + Debug);
			if(TryGetValue("-save_enabled", out bool Save_Enabled))
				UnityEngine.Debug.Log(TAG + "Launching with Save-Enabled = " + Save_Enabled);
			if(TryGetValue("-width", out int width))
				UnityEngine.Debug.Log(TAG + "Launching with width = " + width);
			if(TryGetValue("-height", out int height))
				UnityEngine.Debug.Log(TAG + "Launching with height = " + height);

		}

		internal static string GetName(int index) {
			switch(index) {
				case 0: return "Debug";
				case 1: return "Save-Enabled";
				case 2: return "width";
				case 3: return "height";
			}
			return "unknown";

		}
	}
}
