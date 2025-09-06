using System;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TypeFilter : PropertyAttribute {
        public Type type;

        public TypeFilter(Type type) {
            this.type = type;
        }
    }
}
