using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    public enum FactionState
    {
        /// <summary>
        /// Unknown Faction state
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Hostile, attackable, AI auto engage.
        /// </summary>
        Hostile = 1,

        /// <summary>
        /// Neutral, attackable, AI engages  if attacked
        /// </summary>
        Neutral = 2,

        /// <summary>
        /// Friendly, not attackable, AI would not attack back.
        /// </summary>
        Friendly = 3,
    }
}
