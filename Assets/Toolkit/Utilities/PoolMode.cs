using System;
using UnityEngine;

namespace Toolkit
{
    public enum PoolMode
    {
        /// <summary>
        /// Do not use pooling
        /// </summary>
        [Tooltip("Disables pooling all together")]
        None = 0,

        /// <summary>
        /// Create a static amount of objects and only use them.
        /// </summary>
        [Tooltip("Create a static amount of objects and only use them.")]
        Static = 1,

        /// <summary>
        /// Create a default amount of objects and create more as needed.
        /// </summary>
        [Tooltip("Create a default amount of objects and create more as needed.")]
        Dynamic = 2,

        /// <summary>
        /// Create a static amount of objects and reuse the longest living object.
        /// </summary>
        [Tooltip("Create a static amount of objects and reuse the longest living object.")]
        Circular = 3,
    }
}
