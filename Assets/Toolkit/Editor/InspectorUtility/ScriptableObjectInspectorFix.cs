using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ScriptableObjectInspectorFix : Editor {

        private ButtonDrawer buttonDrawer;
        private DebugViewDrawer debugViewDrawer;

        private void OnEnable() {
            if(serializedObject == null || serializedObject.targetObject == null) {
                return;
            }
            buttonDrawer = new ButtonDrawer(serializedObject);
            debugViewDrawer = new DebugViewDrawer(serializedObject);
        }

        public override void OnInspectorGUI() {
            if(serializedObject == null || serializedObject.targetObject == null) {
                EditorGUILayout.HelpBox("Serialized Object Missing", MessageType.Error);
                DefaultInspectorUtility.DrawInspectorWithUserSettings(serializedObject);
                return;
            }
            DefaultInspectorUtility.DrawInspectorWithUserSettings(serializedObject);
            buttonDrawer?.DrawLayout(serializedObject);
            debugViewDrawer?.DrawLayout(serializedObject);
        }
    }
}
