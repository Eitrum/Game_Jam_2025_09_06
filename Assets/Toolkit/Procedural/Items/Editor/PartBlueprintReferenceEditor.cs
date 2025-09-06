using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Procedural.Items
{
    [CustomEditor(typeof(PartBlueprintReference))]
    public class PartBlueprintReferenceEditor : Editor
    {
        public override void OnInspectorGUI() {
            var bpRef = target as PartBlueprintReference;

            if(bpRef != null && bpRef.Blueprint != null) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUILayout.TextArea(bpRef.Blueprint.ToString(bpRef.Assembly), GUILayout.ExpandHeight(true));
                }
            }
        }
    }
}
