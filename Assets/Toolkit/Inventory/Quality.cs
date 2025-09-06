///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Inventory.CustomQualityEditor.cs

using System;
using UnityEngine;
namespace Toolkit.Inventory {
	public enum Quality : int {
		None = 0,
		Broken = 1,
		Poor = 2,
		Okay = 3,
		Good = 4,
		Very_Good = 5,
		Fantastic = 6,
		Masterpiece = 7,
	}

	public interface IQuality {
		Quality Quality { get; }
		float QualityValue { get; }
	}

	public static class QualityUtility {
		public static Color GetColor(this Quality quality) {
			switch(quality) {
				case Quality.None: return new Color32(0,0,0,255);;
				case Quality.Broken: return new Color32(72,72,72,255);;
				case Quality.Poor: return new Color32(161,161,161,255);;
				case Quality.Okay: return new Color32(233,233,233,255);;
				case Quality.Good: return new Color32(139,214,90,255);;
				case Quality.Very_Good: return new Color32(81,177,248,255);;
				case Quality.Fantastic: return new Color32(205,37,224,255);;
				case Quality.Masterpiece: return new Color32(243,178,45,255);;
			}
			return Color.black;

		}

		public static Quality GetQuality(float value) {
			if(value >= 0.0f) return Quality.None;
			if(value >= 0.0f) return Quality.Broken;
			if(value >= 0.2f) return Quality.Poor;
			if(value >= 0.4f) return Quality.Okay;
			if(value >= 0.6f) return Quality.Good;
			if(value >= 0.8f) return Quality.Very_Good;
			if(value >= 0.9f) return Quality.Fantastic;
			if(value >= 0.98f) return Quality.Masterpiece;
			return Quality.None;

		}

		public static float GetMinimumValue(this Quality quality) {
			switch(quality) {
				case Quality.None: return 0.0f;;
				case Quality.Broken: return 0.0f;;
				case Quality.Poor: return 0.2f;;
				case Quality.Okay: return 0.4f;;
				case Quality.Good: return 0.6f;;
				case Quality.Very_Good: return 0.8f;;
				case Quality.Fantastic: return 0.9f;;
				case Quality.Masterpiece: return 0.98f;;
			}
			return 0f;

		}

		public static string GetName(this Quality quality) {
			switch(quality) {
				case Quality.None: return "None";;
				case Quality.Broken: return "Broken";;
				case Quality.Poor: return "Poor";;
				case Quality.Okay: return "Okay";;
				case Quality.Good: return "Good";;
				case Quality.Very_Good: return "Very Good";;
				case Quality.Fantastic: return "Fantastic";;
				case Quality.Masterpiece: return "Masterpiece";;
			}
			return "";

		}
	}
}
