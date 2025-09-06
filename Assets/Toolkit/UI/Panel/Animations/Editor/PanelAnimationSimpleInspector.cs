using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem.Animations {
    [CustomEditor(typeof(PanelAnimationSimple))]
    public class PanelAnimationSimpleInspector : Editor {
        #region Variables

        private SerializedProperty enterSide;
        private SerializedProperty enterDuration;
        private SerializedProperty exitSide;
        private SerializedProperty exitDuration;

        #endregion

        #region Init

        void OnEnable() {
            enterSide = serializedObject.FindProperty("enterSide");
            enterDuration = serializedObject.FindProperty("enterDuration");
            exitSide = serializedObject.FindProperty("exitSide");
            exitDuration = serializedObject.FindProperty("exitDuration");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(enterSide);
                EditorGUILayout.PropertyField(enterDuration);
                EditorGUILayout.PropertyField(exitSide);
                EditorGUILayout.PropertyField(exitDuration);
            }

            if(Application.isPlaying) {
                var t = (PanelAnimationSimple)target;
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Debug:", EditorStylesUtility.BoldLabel);
                    using(new EditorGUILayout.HorizontalScope()) {
                        if(GUILayout.Button("Show", GUILayout.Width(80))) {
                            t.Show();
                        }
                        if(GUILayout.Button("Hide", GUILayout.Width(80))) {
                            t.Hide();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
