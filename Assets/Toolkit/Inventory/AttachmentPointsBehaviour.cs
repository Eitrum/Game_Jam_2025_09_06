///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Inventory.CustomAttachmentPointsEditor.cs

using UnityEngine;
namespace Toolkit.Inventory {
	[AddComponentMenu("Toolkit/Inventory/Attachment Points")]
	public class AttachmentPointsBehaviour : MonoBehaviour, IAttachmentPoints {
		[SerializeField]private Transform helmet = null;
		[SerializeField]private Transform chest = null;
		[SerializeField]private Transform legs = null;
		[SerializeField]private Transform main_hand = null;
		[SerializeField]private Transform off_hand = null;
		[SerializeField]private Transform backpack = null;
		[SerializeField]private Transform cape = null;
		public Transform Helmet => helmet;

		public Transform Chest => chest;

		public Transform Legs => legs;

		public Transform Main_Hand => main_hand;

		public Transform Off_Hand => off_hand;

		public Transform Backpack => backpack;

		public Transform Cape => cape;

		public Transform this[AttachmentPoint point] { get => GetAttachment(point); set => SetAttachment(point, value); }

		public Transform GetAttachment(Toolkit.Inventory.AttachmentPoint point) {
			switch(point) {
				case AttachmentPoint.Helmet: return helmet;
				case AttachmentPoint.Chest: return chest;
				case AttachmentPoint.Legs: return legs;
				case AttachmentPoint.Main_Hand: return main_hand;
				case AttachmentPoint.Off_Hand: return off_hand;
				case AttachmentPoint.Backpack: return backpack;
				case AttachmentPoint.Cape: return cape;
			}
			return null;

		}

		public bool SetAttachment(Toolkit.Inventory.AttachmentPoint point, Transform transform) {
			switch(point) {
				case AttachmentPoint.Helmet: helmet = transform;
				return true;
				case AttachmentPoint.Chest: chest = transform;
				return true;
				case AttachmentPoint.Legs: legs = transform;
				return true;
				case AttachmentPoint.Main_Hand: main_hand = transform;
				return true;
				case AttachmentPoint.Off_Hand: off_hand = transform;
				return true;
				case AttachmentPoint.Backpack: backpack = transform;
				return true;
				case AttachmentPoint.Cape: cape = transform;
				return true;
			}
			return false;

		}

		public bool HasAttachment(Toolkit.Inventory.AttachmentPoint point) => GetAttachment(point) != null;
	}
}
