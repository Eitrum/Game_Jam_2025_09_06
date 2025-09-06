using UnityEditor;
using UnityEngine;

namespace Toolkit.InputSystem {
    [CustomEditor(typeof(SetCursorOnEnable))]
    public class SetCursorOnEnableInspector : Editor {

        #region Variables

        private SerializedProperty setVisibility;
        private SerializedProperty visibility;

        private SerializedProperty setLockMode;
        private SerializedProperty lockMode;

        private SerializedProperty setTexture;
        private SerializedProperty texture;

        #endregion

        #region Init

        private void OnEnable() {
            setVisibility = serializedObject.FindProperty("setVisibility");
            visibility = serializedObject.FindProperty("visibility");

            setLockMode = serializedObject.FindProperty("setLockMode");
            lockMode = serializedObject.FindProperty("lockMode");

            setTexture = serializedObject.FindProperty("setTexture");
            texture = serializedObject.FindProperty("texture");
        }

        #endregion

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {

                using(new EditorGUILayout.HorizontalScope()) {
                    setVisibility.boolValue = GUILayout.Toggle(setVisibility.boolValue, GUIContent.none, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!setVisibility.boolValue))
                        EditorGUILayout.PropertyField(visibility);
                }

                using(new EditorGUILayout.HorizontalScope()) {
                    setLockMode.boolValue = GUILayout.Toggle(setLockMode.boolValue, GUIContent.none, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!setLockMode.boolValue))
                        EditorGUILayout.PropertyField(lockMode);
                }

                using(new EditorGUILayout.HorizontalScope()) {
                    setTexture.boolValue = GUILayout.Toggle(setTexture.boolValue, GUIContent.none, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!setTexture.boolValue))
                        EditorGUILayout.PropertyField(texture);
                }

            }
        }
    }
}
