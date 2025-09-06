///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Unit.CustomStatsEditor.cs

using UnityEngine;
using System;
namespace Toolkit.Unit {
	public enum StatType : int {
		None = 0,
		Critical_Strike_Chance = 1,
		Critical_Strike_Damage = 2,
		Damage = 3,
		Max_Health = 4,
		Health_Regen_Percentage = 5,
		Health_Regen_Flat = 6,
		Initiative = 7,
		Accuracy = 8,
		Dodge = 9,
		Resistance = 10,
		Armor = 11,
		Fortitude = 12,
		Creativity = 13,
	}

	public static partial class StatsUtility {
		public const int TYPES = 13;
		public static string GetName(StatType type) {
			switch(type) {
				case StatType.Critical_Strike_Chance: return "Critical Strike Chance";
				case StatType.Critical_Strike_Damage: return "Critical Strike Damage";
				case StatType.Damage: return "Damage";
				case StatType.Max_Health: return "Max Health";
				case StatType.Health_Regen_Percentage: return "Health Regen Percentage";
				case StatType.Health_Regen_Flat: return "Health Regen Flat";
				case StatType.Initiative: return "Initiative";
				case StatType.Accuracy: return "Accuracy";
				case StatType.Dodge: return "Dodge";
				case StatType.Resistance: return "Resistance";
				case StatType.Armor: return "Armor";
				case StatType.Fortitude: return "Fortitude";
				case StatType.Creativity: return "Creativity";
			}
			return "";

		}

		public static Color32 GetColor(StatType type) {
			switch(type) {
				case StatType.Critical_Strike_Chance: return new Color32(204, 147, 237, 255);;
				case StatType.Critical_Strike_Damage: return new Color32(237, 147, 180, 255);;
				case StatType.Damage: return new Color32(235, 2, 2, 255);;
				case StatType.Max_Health: return new Color32(245, 83, 83, 255);;
				case StatType.Health_Regen_Percentage: return new Color32(250, 72, 72, 255);;
				case StatType.Health_Regen_Flat: return new Color32(233, 58, 58, 255);;
				case StatType.Initiative: return new Color32(184, 255, 253, 255);;
				case StatType.Accuracy: return new Color32(223, 236, 166, 255);;
				case StatType.Dodge: return new Color32(101, 176, 99, 255);;
				case StatType.Resistance: return new Color32(233, 237, 111, 255);;
				case StatType.Armor: return new Color32(176, 191, 212, 255);;
				case StatType.Fortitude: return new Color32(243, 225, 224, 255);;
				case StatType.Creativity: return new Color32(255, 168, 117, 255);;
			}
			return Color.white;

		}
	}
}
