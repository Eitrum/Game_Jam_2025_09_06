using UnityEditor;
using UnityEngine;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [CustomEditor(typeof(MRPCamera))]
    public class MRPCameraInspector : Editor {

        private SerializedProperty identifier;

        private void OnEnable() {
            identifier = serializedObject.FindProperty("identifier");
        }

        public override void OnInspectorGUI() {
            using(var inspector = new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(identifier);
            }

            var allInstances = MRPCamera.AllInstances;
            if(allInstances == null || allInstances.Count == 0) {
                EditorGUILayout.HelpBox("No Instances Found", MessageType.Error);
                return;
            }
            EditorGUILayout.Space(4);
            var line = GUILayoutUtility.GetRect(1, 1);
            EditorGUI.DrawRect(line, Color.gray);
            EditorGUILayout.LabelField("Instances", EditorStylesUtility.BoldLabel);
            GUI.enabled = false;
            foreach(var instance in allInstances) {
                EditorGUILayout.LabelField($"{instance.Identifier} ({(instance.isActiveAndEnabled ? "Active" : "Inactive")})");
            }
            GUI.enabled = true;

        }
    }
}
