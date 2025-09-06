///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.AI.Navigation.NavMeshCodeGenerator.cs

using UnityEngine;
namespace Toolkit.AI.Navigation {
	public class NavMeshBuildRuleMask : ScriptableObject {
		[SerializeField]private bool Default = true;
		public bool IsActive(Toolkit.AI.Navigation.NavMeshBuildRuleType type) {
			switch(type) {
				case NavMeshBuildRuleType.Default: return Default;
			}
			return false;

		}
	}
}
