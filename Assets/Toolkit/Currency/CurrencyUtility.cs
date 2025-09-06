///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Currency.CurrencyEditor.cs

using System;
using UnityEngine;
namespace Toolkit.Currency {
	public static class CurrencyUtility {
		public const int Copper_VALUE = 1;
		public const int Silver_VALUE = 100;
		public const int Gold_VALUE = 10000;
		public const int Platinum_VALUE = 1000000;
		public const int Copper_REM = 100;
		public const int Silver_REM = 100;
		public const int Gold_REM = 100;
		public const int Platinum_REM = int.MaxValue / 1000000;
		public const int CURRENCY_TIERS = 4;
		public static string GetName(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return "Copper";
				case CurrencyType.Silver: return "Silver";
				case CurrencyType.Gold: return "Gold";
				case CurrencyType.Platinum: return "Platinum";
			}
			return "";

		}

		public static string GetSuffix(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return "c";
				case CurrencyType.Silver: return "s";
				case CurrencyType.Gold: return "g";
				case CurrencyType.Platinum: return "p";
			}
			return "";

		}

		public static int GetValue(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return 1;
				case CurrencyType.Silver: return 100;
				case CurrencyType.Gold: return 10000;
				case CurrencyType.Platinum: return 1000000;
			}
			return 1;

		}

		internal static int GetRem(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return Copper_REM;
				case CurrencyType.Silver: return Silver_REM;
				case CurrencyType.Gold: return Gold_REM;
				case CurrencyType.Platinum: return Platinum_REM;
			}
			return int.MaxValue;

		}
	}
}
