using System;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NavAreaAttribute : PropertyAttribute
    {
        public bool IsMask = false;

        public NavAreaAttribute() { }
        public NavAreaAttribute(bool mask) => IsMask = mask;
    }
}
