using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DirectionAttribute : PropertyAttribute
    {
        public bool RelativeToObject { get; private set; } = false;
        public bool Normalize { get; private set; } = true;
        internal bool EditMode { get; set; } = false;

        public DirectionAttribute() { }

        public DirectionAttribute(bool relativeToObject) {
            this.RelativeToObject = relativeToObject;
            this.Normalize = true;
        }

        public DirectionAttribute(bool relativeToObject = false, bool normalize = true) {
            this.RelativeToObject = relativeToObject;
            this.Normalize = normalize;
        }
    }
}
