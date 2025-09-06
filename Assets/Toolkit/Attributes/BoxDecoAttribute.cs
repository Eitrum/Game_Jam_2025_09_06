using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BoxDecoAttribute : PropertyAttribute {

        public readonly Color Color = Color.black;
        public readonly float Elements = 1f;

        public BoxDecoAttribute() { }

        public BoxDecoAttribute(float elements) => this.Elements = elements;
        public BoxDecoAttribute(float elements, ColorTableType color) : this(color.ToColor(), elements) { }
        public BoxDecoAttribute(float elements, uint color) : this(color.ToColor(), elements) { }

        private BoxDecoAttribute(Color color, float elements) {
            this.Color = color;
            this.Elements = elements;
        }
    }
}
