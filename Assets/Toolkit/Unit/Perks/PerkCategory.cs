using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Flags]
    public enum PerkCategory {
        /// <summary>
        /// If no perk doesn't have a category
        /// </summary>
        [InspectorName("Uncategorized")] Uncategorized = 0,
        /// <summary>
        /// Used to filter perks by non-removable perks.
        ///  - Quest
        ///  - Story
        ///  - Ireversible choices
        /// </summary>
        [InspectorName("Filter / Permanent")] Permanent = 1 << 0,
        /// <summary>
        /// Used to filter perks that might just be temporarily active.
        ///  - Spells
        ///  - Status Effects
        ///  - Special Events
        ///  - Map/Instance modifiers
        /// </summary>
        [InspectorName("Filter / Temporary")] Temporary = 1 << 1,
        /// <summary>
        /// Hidden from users. If they should be displayed or not.
        /// </summary>
        [InspectorName("Filter / Hidden")] Hidden = 1 << 2,

        // Sources
        [InspectorName("Source / Status Effect")] StatusEffect = 1 << 3,
        [InspectorName("Source / Equipment")] Equipment = 1 << 4,
        [InspectorName("Source / Inventory")] Inventory = 1 << 5,
        [InspectorName("Source / Trait")] Trait = 1 << 6,
        [InspectorName("Source / Skill")] Skill = 1 << 7,
        [InspectorName("Source / Faction")] Faction = 1 << 8,
        [InspectorName("Source / Alignment")] Alignment = 1 << 9,
        [InspectorName("Source / Level")] Level = 1 << 10,
        [InspectorName("Source / Spell")] Spell = 1 << 11,

        FilterMask = Permanent | Temporary | Hidden,
        SourceMask = StatusEffect | Equipment | Inventory | Trait | Skill | Faction | Alignment | Level | Spell,
    }

    public static partial class PerkUtility {

        #region Hidden Utility / IsDisplayed

        public static bool IsDisplayed(this IPerk perk) {
            if(perk == null)
                return false;
            return IsDisplayed(perk.Metadata);
        }

        public static bool IsDisplayed(this IPerkMetadata metaData) {
            if(metaData == null)
                return false;
            return IsDisplayed(PerkCategory.Hidden);
        }

        public static bool IsDisplayed(PerkCategory category) {
            return !category.HasFlag(PerkCategory.Hidden);
        }

        public static bool IsHidden(this PerkCategory category) {
            return category.HasFlag(PerkCategory.Hidden);
        }

        #endregion

        #region Permanent

        public static bool IsPermanent(this PerkCategory category) {
            return category.HasFlag(PerkCategory.Permanent);
        }

        #endregion

        #region Check

        public static bool Is(this PerkCategory category, PerkCategory toCompare)
            => (category & toCompare) == toCompare;

        #endregion

        #region Get Filter/Source

        public static PerkCategory GetFilter(this PerkCategory category)
            => (category & PerkCategory.FilterMask);

        public static PerkCategory GetSource(this PerkCategory category)
            => (category & PerkCategory.SourceMask);

        #endregion

        #region Pre-defined comparers

        public static class Comparer {
            public static bool IsUncategorized(PerkCategory category) => category == PerkCategory.Uncategorized;

            public static bool IsPermanent(PerkCategory category) => Is(category, PerkCategory.Permanent);
            public static bool IsTemporary(PerkCategory category) => Is(category, PerkCategory.Temporary);
            public static bool IsHidden(PerkCategory category) => Is(category, PerkCategory.Hidden);

            // StatusEffect | Equipment | Inventory | Trait | Skill | Faction | Alignment | Level | Spell,
            public static bool IsStatusEffect(PerkCategory category) => Is(category, PerkCategory.StatusEffect);
            public static bool IsEquipment(PerkCategory category) => Is(category, PerkCategory.Equipment);
            public static bool IsInventory(PerkCategory category) => Is(category, PerkCategory.Inventory);
            public static bool IsTrait(PerkCategory category) => Is(category, PerkCategory.Trait);
            public static bool IsSkill(PerkCategory category) => Is(category, PerkCategory.Skill);
            public static bool IsFaction(PerkCategory category) => Is(category, PerkCategory.Faction);
            public static bool IsAlignment(PerkCategory category) => Is(category, PerkCategory.Alignment);
            public static bool IsLevel(PerkCategory category) => Is(category, PerkCategory.Level);
            public static bool IsSpell(PerkCategory category) => Is(category, PerkCategory.Spell);
        }
        #endregion
    }
}
