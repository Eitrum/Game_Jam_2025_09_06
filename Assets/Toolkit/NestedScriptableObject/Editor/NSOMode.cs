using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public enum NSOMode
    {
        [InspectorName("Default (Unity)")]
        Default = 0,

        [InspectorName("Nested Rendering")]
        NestedRendering = 1,

        [InspectorName("File Modifiers Only"),
            Tooltip("Renders only the add, destroy, select and preview options for nested objects")]
        FileModifiersOnly = 2,

    }
}
