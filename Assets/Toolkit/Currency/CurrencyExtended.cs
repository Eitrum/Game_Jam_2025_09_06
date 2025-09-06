///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Currency.CurrencyEditor.cs

namespace Toolkit.Currency {
	public partial struct Currency {
		public int Copper => (value ) % 100;

		public float TotalCopper => value / 1f;

		public int Silver => (value / 100) % 100;

		public float TotalSilver => value / 100f;

		public int Gold => (value / 10000) % 100;

		public float TotalGold => value / 10000f;

		public int Platinum => (value / 1000000);

		public float TotalPlatinum => value / 1000000f;

		public void Add(CurrencyType type, int amount) {
			switch(type){
			case CurrencyType.Copper: value += amount * 1; break;
			case CurrencyType.Silver: value += amount * 100; break;
			case CurrencyType.Gold: value += amount * 10000; break;
			case CurrencyType.Platinum: value += amount * 1000000; break;
			}

		}

		public void Remove(CurrencyType type, int amount) {
			switch(type){
			case CurrencyType.Copper: value -= amount * 1; break;
			case CurrencyType.Silver: value -= amount * 100; break;
			case CurrencyType.Gold: value -= amount * 10000; break;
			case CurrencyType.Platinum: value -= amount * 1000000; break;
			}

		}

		public int GetAmount(CurrencyType type) {
			switch(type) {
				case CurrencyType.Copper: return Copper;
				case CurrencyType.Silver: return Silver;
				case CurrencyType.Gold: return Gold;
				case CurrencyType.Platinum: return Platinum;
			}
			return value;

		}

		public override string ToString() => $"{Copper}c {Silver}s {Gold}g {Platinum}p";
	}
}
