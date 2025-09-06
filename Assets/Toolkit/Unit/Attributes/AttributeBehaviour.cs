///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'
/*
using UnityEngine;
namespace Toolkit.Unit {
	[AddComponentMenu("Toolkit/Unit/Attributes")]
	public class AttributeBehaviour : MonoBehaviour, IAttributes {
		public Toolkit.Stat Strength = new Stat(0f, 1f, 1f);
		public Toolkit.Stat Toughness = new Stat(0f, 1f, 1f);
		public Toolkit.Stat Agility = new Stat(0f, 1f, 1f);
		public Toolkit.Stat Ancestry = new Stat(0f, 1f, 1f);
		public Toolkit.Stat Intelligence = new Stat(0f, 1f, 1f);
		public Toolkit.Stat Talent = new Stat(0f, 1f, 1f);
		public Toolkit.Stat this[AttributeType type] { get => GetStat(type); set => SetStat(type, value); }

		public Toolkit.Stat this[int index] { get => GetStat((AttributeType)(index-1)); set => SetStat((AttributeType)(index-1), value); }

		public Toolkit.Stat GetStat(AttributeType type) {
			switch(type) {
				case AttributeType.Strength: return Strength;
				case AttributeType.Toughness: return Toughness;
				case AttributeType.Agility: return Agility;
				case AttributeType.Ancestry: return Ancestry;
				case AttributeType.Intelligence: return Intelligence;
				case AttributeType.Talent: return Talent;
			}
#if UNITY_EDITOR
			throw new System.Exception("Attribute Type not supported");
#else
			return default;
#endif
		}

		public void SetStat(AttributeType type, Toolkit.Stat value) {
			switch(type) {
				case AttributeType.Strength: Strength = value;
				break;
				case AttributeType.Toughness: Toughness = value;
				break;
				case AttributeType.Agility: Agility = value;
				break;
				case AttributeType.Ancestry: Ancestry = value;
				break;
				case AttributeType.Intelligence: Intelligence = value;
				break;
				case AttributeType.Talent: Talent = value;
				break;
			}
		}
	}
}
*/
