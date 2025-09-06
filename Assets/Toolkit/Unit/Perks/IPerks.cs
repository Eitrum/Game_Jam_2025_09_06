using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IPerks {
        /// <summary>
        /// All active perks
        /// </summary>
        IReadOnlyDictionary<int, IPerk> Active { get; }
        /// <summary>
        /// Perks currently not active
        /// </summary>
        IList<IPerk> Inactive { get; }

        void AddPerk(IPerk perk);
        void Remove(IPerk perk);
    }
}
