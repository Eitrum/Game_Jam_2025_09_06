///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Unit.CustomSkillsEditor.cs

using System;
using UnityEngine;
using System.Linq;
namespace Toolkit.Unit
{
    public enum SkillType : int
    {
        None = 0,
        Illusion = 1,
        Conjuration = 2,
        Destruction = 3,
        Restoration = 4,
        Alteration = 5,
        Enchanting = 6,
        Smithing = 7,
        Heavy_Armor = 8,
        Block = 9,
        Two_Handed = 10,
        One_Handed = 11,
        Archery = 12,
        Light_Armor = 13,
        Sneak = 14,
        Lockpicking = 15,
        Pickpocket = 16,
        Speech = 17,
        Alchemy = 18,
    }

    [Flags]
    public enum SkillCategory : int
    {
        None = 0,
        Mage = 1,
        Warrior = 2,
        Thief = 4,
    }

    public static class SkillUtility
    {
        public const int SKILL_COUNT = 18;
        public static SkillCategory GetSkillCategory(SkillType type) {
            switch(type) {
                case SkillType.Illusion: return SkillCategory.Mage;
                case SkillType.Conjuration: return SkillCategory.Mage;
                case SkillType.Destruction: return SkillCategory.Mage;
                case SkillType.Restoration: return SkillCategory.Mage;
                case SkillType.Alteration: return SkillCategory.Mage;
                case SkillType.Enchanting: return SkillCategory.Mage | SkillCategory.Warrior;
                case SkillType.Smithing: return SkillCategory.Warrior;
                case SkillType.Heavy_Armor: return SkillCategory.Warrior;
                case SkillType.Block: return SkillCategory.Warrior;
                case SkillType.Two_Handed: return SkillCategory.Warrior;
                case SkillType.One_Handed: return SkillCategory.Warrior;
                case SkillType.Archery: return SkillCategory.Warrior | SkillCategory.Thief;
                case SkillType.Light_Armor: return SkillCategory.Thief;
                case SkillType.Sneak: return SkillCategory.Thief;
                case SkillType.Lockpicking: return SkillCategory.Thief;
                case SkillType.Pickpocket: return SkillCategory.Thief;
                case SkillType.Speech: return SkillCategory.Thief;
                case SkillType.Alchemy: return SkillCategory.Mage | SkillCategory.Thief;
            }
            return SkillCategory.None;

        }

        public static bool IsSkillCategory(SkillType type, SkillCategory category) {
            var skillCat = GetSkillCategory(type);
            return (skillCat == SkillCategory.None && category == SkillCategory.None) || ((skillCat & category) == category);

        }

        public static string GetName(SkillType type) {
            switch(type) {
                case SkillType.Illusion: return "Illusion";
                case SkillType.Conjuration: return "Conjuration";
                case SkillType.Destruction: return "Destruction";
                case SkillType.Restoration: return "Restoration";
                case SkillType.Alteration: return "Alteration";
                case SkillType.Enchanting: return "Enchanting";
                case SkillType.Smithing: return "Smithing";
                case SkillType.Heavy_Armor: return "Heavy Armor";
                case SkillType.Block: return "Block";
                case SkillType.Two_Handed: return "Two-Handed";
                case SkillType.One_Handed: return "One-Handed";
                case SkillType.Archery: return "Archery";
                case SkillType.Light_Armor: return "Light-Armor";
                case SkillType.Sneak: return "Sneak";
                case SkillType.Lockpicking: return "Lockpicking";
                case SkillType.Pickpocket: return "Pickpocket";
                case SkillType.Speech: return "Speech";
                case SkillType.Alchemy: return "Alchemy";
            }
            return "";

        }

        public static string GetName(SkillCategory category) {
            switch(category) {
                case SkillCategory.Mage: return "Mage";
                case SkillCategory.Warrior: return "Warrior";
                case SkillCategory.Thief: return "Thief";
            }
            return "";

        }
    }
}
