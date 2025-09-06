using System;
using UnityEngine;

namespace Toolkit.Health
{
    /// <summary>
    /// Attribute to convert an integer to DamageType in Editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DamageTypeAttribute : PropertyAttribute
    {
        public bool useMask = false;

        public DamageTypeAttribute() { }
        public DamageTypeAttribute(bool useMask) => this.useMask = useMask;
    }
}
