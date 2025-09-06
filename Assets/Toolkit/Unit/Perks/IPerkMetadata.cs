using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IPerkMetadata {
        /// <summary>
        /// The perk id used for database lookup and saving.
        /// </summary>
        int PerkId { get; }
        /// <summary>
        /// Perk Display Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Perk Description
        /// </summary>
        string Description { get; }
        /// <summary>
        /// PerkType to filter by what type of perk it is.
        /// </summary>
        PerkType Type { get; }
        /// <summary>
        /// PerkCategory to filter by hidden or sources.
        /// </summary>
        PerkCategory Category { get; }
        /// <summary>
        /// Display Icon of the perk.
        /// </summary>
        Texture2D Icon { get; }
    }
}
