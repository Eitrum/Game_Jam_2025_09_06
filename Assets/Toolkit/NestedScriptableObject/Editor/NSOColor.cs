using System;
using UnityEngine;

namespace Toolkit
{
    public enum NSOColor
    {
        [InspectorName("None")] None = -1,
        [InspectorName("Default")] Default = 0,
        [InspectorName("Grayscale")] Grayscale = 10,
        [InspectorName("Sepia Filter")] Sepia = 20,
        [InspectorName("Sepia Filter (Variant)")] SepiaVariant = 21,
        [InspectorName("Blue Filter")] Blue = 30,
        [InspectorName("Blue Filter (Variant)")] BlueVariant = 31,
    }
}
