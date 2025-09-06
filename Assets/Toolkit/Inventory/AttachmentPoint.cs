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
	public enum AttachmentPoint : int {
		None = 0,
		Helmet = 1,
		Chest = 2,
		Legs = 3,
		Main_Hand = 4,
		Off_Hand = 5,
		Backpack = 7,
		Cape = 8,
	}

	public interface IAttachmentPoints {
		Transform this[AttachmentPoint point] { get; set; }
		Transform GetAttachment(AttachmentPoint point);
		bool SetAttachment(AttachmentPoint point, Transform transform);
		bool HasAttachment(AttachmentPoint point);
	}

	public static class AttachmentPointsUtility {
		public static EquipmentSlot GetEquipmentSlot(this AttachmentPoint point) {
			switch(point) {
				case AttachmentPoint.Helmet: return EquipmentSlot.Helmet;
				case AttachmentPoint.Chest: return EquipmentSlot.Chest;
				case AttachmentPoint.Legs: return EquipmentSlot.Legs;
				case AttachmentPoint.Main_Hand: return EquipmentSlot.Main_Hand;
				case AttachmentPoint.Off_Hand: return EquipmentSlot.Off_Hand;
			}
			return EquipmentSlot.None;

		}

		public static AttachmentPoint GetAttachmentPoint(this EquipmentSlot slot) {
			switch(slot) {
				case EquipmentSlot.Helmet: return AttachmentPoint.Helmet;
				case EquipmentSlot.Chest: return AttachmentPoint.Chest;
				case EquipmentSlot.Legs: return AttachmentPoint.Legs;
				case EquipmentSlot.Main_Hand: return AttachmentPoint.Main_Hand;
				case EquipmentSlot.Off_Hand: return AttachmentPoint.Off_Hand;
			}
			return AttachmentPoint.None;

		}

		public static bool IsEquipmentSlot(this AttachmentPoint point) {
			switch(point) {
				case AttachmentPoint.None: return false;
				case AttachmentPoint.Helmet: return true;
				case AttachmentPoint.Chest: return true;
				case AttachmentPoint.Legs: return true;
				case AttachmentPoint.Main_Hand: return true;
				case AttachmentPoint.Off_Hand: return true;
				case AttachmentPoint.Backpack: return false;
				case AttachmentPoint.Cape: return false;
			}
			return false;

		}

		public static string ToString(this AttachmentPoint point) {
			switch(point) {
				case AttachmentPoint.None: return "None";
				case AttachmentPoint.Helmet: return "Helmet";
				case AttachmentPoint.Chest: return "Chest";
				case AttachmentPoint.Legs: return "Legs";
				case AttachmentPoint.Main_Hand: return "Main Hand";
				case AttachmentPoint.Off_Hand: return "Off Hand";
				case AttachmentPoint.Backpack: return "Backpack";
				case AttachmentPoint.Cape: return "Cape";
			}
			return "Unknown";

		}
	}
}
