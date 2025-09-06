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
using UnityEngine;
using System.Collections.Generic;
namespace Toolkit.AI.Navigation {
	public enum NavMeshQueryFilterType : int {
		Default = 0,
	}

	public static class NavMeshQueryFilterRules {
		public const int RULES = 1;
		public static readonly NavMeshQueryFilter Default = new NavMeshQueryFilter(){ agentTypeID = 0, areaMask = -1};
		private static float[] Default_Multiplier = new float[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1};
		public static IReadOnlyList<NavMeshQueryFilter> AllQueryFilters => new NavMeshQueryFilter[]{Default};

		static  NavMeshQueryFilterRules() {
			for(int i = 0; i < 32; i++) {
				if(Default_Multiplier[i] != 1f)
					Default.SetAreaCost(i, Default_Multiplier[i]);
			}

		}

		public static NavMeshQueryFilter GetQueryFilter(NavMeshQueryFilterType type) => GetQueryFilter((int)type);

		public static NavMeshQueryFilter GetQueryFilter(int index) {
			switch(index) {
				case 0: return Default;
			}
			return Default;

		}

		public static string GetQueryFilterName(NavMeshQueryFilterType type) => GetQueryFilterName((int)type);

		public static string GetQueryFilterName(int index) {
			switch(index) {
				case 0: return "Default";
			}
			return "Default";

		}
	}
}
