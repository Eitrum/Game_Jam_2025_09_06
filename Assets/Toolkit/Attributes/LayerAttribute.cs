using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LayerAttribute : PropertyAttribute
    {
        public bool mask = false;

        public LayerAttribute() { }
        public LayerAttribute(bool mask) {
            this.mask = mask;
        }
    }
}
