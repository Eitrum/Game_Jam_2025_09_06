///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.AI.Navigation.NavMeshCodeGenerator.cs

using UnityEngine.AI;
using System.Collections.Generic;
namespace Toolkit.AI.Navigation {
	public enum NavMeshBuildRuleType : int {
		Default = 0,
	}

	public static class NavMeshBuildRules {
		public static bool INSTANCED = true;
		public const int TILE_SIZE = 16;
		public const float VOXEL_SIZE_PER_AGENT_RADIUS = 4.00f;
		public const int RULES = 1;
		public static NavMeshBuildSettings Default;
		static  NavMeshBuildRules() {
			for(int i = NavMesh.GetSettingsCount() - 1; i >= 1; i--) {
				var setting = NavMesh.GetSettingsByIndex(i);
				NavMesh.RemoveSettings(setting.agentTypeID);
			}
			
			Default = NavMesh.GetSettingsByIndex(0);
			Default.overrideTileSize = true;
			Default.overrideVoxelSize = true;
			Default.tileSize = TILE_SIZE;
			Default.voxelSize = Default.agentRadius / VOXEL_SIZE_PER_AGENT_RADIUS;

		}

		public static IReadOnlyList<NavMeshBuildSettings> AllRules => new NavMeshBuildSettings[]{Default};

		public static NavMeshBuildSettings GetSetting(NavMeshBuildRuleType type) => GetSetting((int)type);

		public static NavMeshBuildSettings GetSetting(int index) {
			switch(index) {
				case 0: return Default;
			}
			return Default;

		}

		public static string GetSettingName(int index) {
			switch(index) {
				case 0: return "Default";
			}
			return "Default";

		}
	}
}
