using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Flags]
    public enum PerkType {
        [InspectorName("None")] None = 0,

        // Modifiers
        [InspectorName("Unit / Attribute")] Attribute = 1 << 0,
        [InspectorName("Unit / Stats")] Stats = 1 << 1,
        [InspectorName("Unit / Status Effect")] StatusEffect = 1 << 2,
        [InspectorName("Unit / Trait")] Trait = 1 << 4,
        [InspectorName("Unit / Skill")] Skill = 1 << 5,
        [InspectorName("Unit / Faction")] Faction = 1 << 6,
        [InspectorName("Unit / Spell")] Spell = 1 << 7,

        // Unique Effects
        [InspectorName("Combat / Combat Passive")] CombatPassive = 1 << 8,
        [InspectorName("Combat / Non-Combat Passive")] NonCombatPassive = 1 << 9,
    }
}
