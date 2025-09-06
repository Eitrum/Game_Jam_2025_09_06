using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger {
    [CustomEditor(typeof(TriggerRelay))]
    public class TriggerRelayInspector : Editor {
        private SerializedProperty listenTarget;

        private void OnEnable() {
            listenTarget = serializedObject.FindProperty("listenTargets");
        }

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                var t = target as TriggerRelay;

                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(listenTarget);
                }
                TriggerEditorUtility.DrawEditorDebug(this, target);
            }
        }
    }
}
