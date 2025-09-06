///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

using UnityEngine;
using Toolkit.Health;
using Toolkit.Inventory;
using Toolkit.Unit;
namespace Toolkit.Unit {
	public partial interface IUnit {
		System.String Name { get; }
		System.Int32 Id { get; }
		UnitControllerType ControllerType { get; }
		MonoBehaviour Behaviour { get; }
		Vector3 Position { get; }
		Quaternion Rotation { get; }
		IHealth Health { get; }
		IInventory Inventory { get; }
		IEquipment Equipment { get; }
		IExperience Experience { get; }
		IAttributes Attributes { get; }
		ITrait Traits { get; }
		IPerks Perks { get; }
		IStatusEffectList StatusEffects { get; }
		IStats Stats { get; }
	}
}
