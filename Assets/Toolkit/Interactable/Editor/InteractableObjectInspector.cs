using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Interactables {
    [CustomEditor(typeof(InteractableObject))]
    public class InteractableObjectInspector : Editor {

        public override void OnInspectorGUI() {
            using(var e = new ToolkitEditorUtility.InspectorScope(this)) {
                e.DrawDefault();
                DrawOptions();
                e.DrawExtras();
            }
        }

        private void DrawOptions() {
            var t = (InteractableObject)target;
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Options:");
                using(var s = Source.Create("editor")) {
                    foreach(var o in t.Options) {
                        using(new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField($"{o.Name} {o.GetState(s)}");
                            if(GUILayout.Button("Interact", GUILayout.Width(80))) {
                                o.Interact(s);
                            }
                        }
                    }
                }
            }
        }
    }
}
