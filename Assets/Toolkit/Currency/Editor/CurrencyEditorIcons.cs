///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Currency.CurrencyEditor.cs

using UnityEngine;
using UnityEditor;
namespace Toolkit.Currency {
	public static class CurrencyEditorIcons {
		private static string CopperIconPath = "Assets/Toolkit/Currency/Editor/copper.png";
		private static Texture2D CopperIcon;
		private static string SilverIconPath = "Assets/Toolkit/Currency/Editor/silver.png";
		private static Texture2D SilverIcon;
		private static string GoldIconPath = "Assets/Toolkit/Currency/Editor/gold.png";
		private static Texture2D GoldIcon;
		private static string PlatinumIconPath = "Assets/Toolkit/Currency/Editor/platinum.png";
		private static Texture2D PlatinumIcon;
		public static Texture2D GetTexture(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return CopperIcon;
				case CurrencyType.Silver: return SilverIcon;
				case CurrencyType.Gold: return GoldIcon;
				case CurrencyType.Platinum: return PlatinumIcon;
			}
			return null;

		}

		[InitializeOnLoadMethod]
		private static void Initialize() {
			CopperIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(CopperIconPath);
			SilverIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(SilverIconPath);
			GoldIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(GoldIconPath);
			PlatinumIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(PlatinumIconPath);

		}
	}
}
