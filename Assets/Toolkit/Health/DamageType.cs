using System;
using UnityEngine;

namespace Toolkit.Health
{
    /// NOTE : All damage types should be stored as a mask of integer type to make code easier and less clunky.

    /// <summary>
    /// Simple enum to override damage system types.
    /// </summary>
    public enum DamageTypeSystem
    {
        None,
        DungeonsAndDragons,
        Custom
    }

    /// <summary>
    /// Damage type base enum.
    /// </summary>
    [Flags]
    public enum DamageType : int // Base
    {
        None = 0,
        Type0 = 1 << 0,
        Type1 = 1 << 1,
        Type2 = 1 << 2,
        Type3 = 1 << 3,
        Type4 = 1 << 4,
        Type5 = 1 << 5,
        Type6 = 1 << 6,
        Type7 = 1 << 7,
        Type8 = 1 << 8,
        Type9 = 1 << 9,
        Type10 = 1 << 10,
        Type11 = 1 << 11,
        Type12 = 1 << 12,
        Type13 = 1 << 13,
        Type14 = 1 << 14,
        Type15 = 1 << 15,
        Type16 = 1 << 16,
        Type17 = 1 << 17,
        Type18 = 1 << 18,
        Type19 = 1 << 19,
        Type20 = 1 << 20,
        Type21 = 1 << 21,
        Type22 = 1 << 22,
        Type23 = 1 << 23,
        Type24 = 1 << 24,
        Type25 = 1 << 25,
        Type26 = 1 << 26,
        Type27 = 1 << 27,
        Type28 = 1 << 28,
        Type29 = 1 << 29,
        Type30 = 1 << 30,
        Type31 = 1 << 31
    }

    /// <summary>
    /// Damage type based on dungeons and dragons 5e.
    /// </summary>
    [Flags]
    public enum DamageTypeDnD : int
    {
        None = 0,
        Bludgeoning = 1 << 0,
        Piercing = 1 << 1,
        Slashing = 1 << 2,
        Acid = 1 << 3,
        Cold = 1 << 4,
        Fire = 1 << 5,
        Force = 1 << 6,
        Lightning = 1 << 7,
        Necrotic = 1 << 8,
        Poison = 1 << 9,
        Psychic = 1 << 10,
        Radiant = 1 << 11,
        Thunder = 1 << 12
    }
}
