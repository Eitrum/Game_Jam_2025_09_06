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
	[AddComponentMenu("Toolkit/Unit/Unit")]
	public class BasicUnit : MonoBehaviour, IUnit {
		public System.String Name => name;

		private System.Int32 id;
		private void OnDestroy() => UnitIndexManager.Remove(this, id);

		public System.Int32 Id => id;

		[SerializeField]private UnitControllerType controllerType = UnitControllerType.None;
		public UnitControllerType ControllerType { get => controllerType; set => controllerType = value; }

		public MonoBehaviour Behaviour => this;

		public Vector3 Position => transform.position;

		public Quaternion Rotation => transform.rotation;

		private IHealth health;
		public IHealth Health => health;

		private IInventory inventory;
		public IInventory Inventory => inventory;

		private IEquipment equipment;
		public IEquipment Equipment => equipment;

		private IExperience experience;
		public IExperience Experience => experience;

		private IAttributes attributes;
		public IAttributes Attributes => attributes;

		private ITrait traits;
		public ITrait Traits => traits;

		private IPerks perks;
		public IPerks Perks => perks;

		private IStatusEffectList statusEffects;
		public IStatusEffectList StatusEffects => statusEffects;

		private IStats stats;
		public IStats Stats => stats;

		private void Awake() {
			id = UnitIndexManager.Add(this);
			health = this.GetComponent<IHealth>();
			inventory = this.GetComponent<IInventory>();
			equipment = this.GetComponent<IEquipment>();
			experience = this.GetComponent<IExperience>();
			attributes = this.GetComponent<IAttributes>();
			traits = this.GetComponent<ITrait>();
			perks = this.GetComponent<IPerks>();
			statusEffects = this.GetComponent<IStatusEffectList>();
			stats = this.GetComponent<IStats>();
		}
	}
}
