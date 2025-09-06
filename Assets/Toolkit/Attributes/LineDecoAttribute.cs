using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class LineDecoAttribute : PropertyAttribute {

        public readonly Color Color = Color.gray;
        public readonly float Thickness = 1f;
        public readonly float Width = 0f;

        public LineDecoAttribute() : this(Color.gray, 1f, 0f) { }
        public LineDecoAttribute(float thickness) : this(Color.gray, thickness, 0f) { }
        public LineDecoAttribute(ColorTableType color, float thickness = 1f, float width = 0f) : this(ColorTable.GetColor(color), thickness, width) { }
        /// <summary>
        /// Example: 0xff0000ff (0xRRGGBBAA)
        /// </summary>
        public LineDecoAttribute(uint color, float thickness = 1f, float width = 0f) : this(color.ToColor(), thickness, width) { }
        private LineDecoAttribute(Color color, float thickness = 1f, float width = 0f) {
            Color = color;
            Thickness = thickness;
            Width = width;
        }
    }
}
