using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.UI.Components {
    [CustomEditor(typeof(PlaySoundOnClick))]
    public class PlaySoundOnClickInspector : Editor {

        #region Variables

        private SerializedProperty onClick;
        private SerializedProperty overrideClick;
        private SerializedProperty onPointerDown;
        private SerializedProperty overridePointerDown;
        private SerializedProperty onPointerUp;
        private SerializedProperty overridePointerUp;

        private static GUIContent onclickcontent = new GUIContent("Click");
        private static GUIContent onPointerDowncontent = new GUIContent("Pointer Down");
        private static GUIContent onPointerUpcontent = new GUIContent("Pointer Up");
        private static GUIContent enableDisable = new GUIContent("Mode");
        private static GUIContent optionalOverride = new GUIContent("Override");

        #endregion

        #region Init

        private void OnEnable() {
            onClick = serializedObject.FindProperty("onClick");
            overrideClick = serializedObject.FindProperty("overrideClick");
            onPointerDown = serializedObject.FindProperty("onPointerDown");
            overridePointerDown = serializedObject.FindProperty("overridePointerDown");
            onPointerUp = serializedObject.FindProperty("onPointerUp");
            overridePointerUp = serializedObject.FindProperty("overridePointerUp");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.LabelField(enableDisable);
                    EditorGUILayout.LabelField("", "Override");
                    //EditorGUILayout.LabelField(optionalOverride, EditorStylesUtility.RightAlignedLabel);
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    onClick.boolValue = EditorGUILayout.Toggle(onClick.boolValue, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!onClick.boolValue))
                        EditorGUILayout.PropertyField(overrideClick, onclickcontent);
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    onPointerDown.boolValue = EditorGUILayout.Toggle(onPointerDown.boolValue, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!onPointerDown.boolValue))
                        EditorGUILayout.PropertyField(overridePointerDown, onPointerDowncontent);
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    onPointerUp.boolValue = EditorGUILayout.Toggle(onPointerUp.boolValue, GUILayout.Width(18));
                    using(new EditorGUI.DisabledScope(!onPointerUp.boolValue))
                        EditorGUILayout.PropertyField(overridePointerUp, onPointerUpcontent);
                }
            }
        }

        #endregion
    }
}
