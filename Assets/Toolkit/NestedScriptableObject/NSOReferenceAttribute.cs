using System;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NSOReferenceAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public NSOReferenceAttribute(Type type) {
            this.Type = type;
        }
    }
}
