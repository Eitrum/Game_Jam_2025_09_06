using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CustomEditor(typeof(DiamondSquare))]
    public class DiamondSquareInspector : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}
