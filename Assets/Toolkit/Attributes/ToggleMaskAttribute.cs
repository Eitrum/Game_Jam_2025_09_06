using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ToggleMaskAttribute : PropertyAttribute
    {
        public ToggleMaskAttribute() { }
    }
}
