using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem.Animations {
    [CustomEditor(typeof(PanelAnimation))]
    public class PanelAnimationInspector : Editor {

        #region Variables

        private SerializedProperty playSequenceInReverseOnHide;
        private SerializedProperty show;
        private SerializedProperty hide;

        #endregion

        #region Init

        private void OnEnable() {
            playSequenceInReverseOnHide = serializedObject.FindProperty("playSequenceInReverseOnHide");
            show = serializedObject.FindProperty("show");
            hide = serializedObject.FindProperty("hide");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(playSequenceInReverseOnHide);
                EditorGUILayout.PropertyField(show);
                if(!playSequenceInReverseOnHide.boolValue)
                    EditorGUILayout.PropertyField(hide);
            }

            if(Application.isPlaying) {
                var t = (PanelAnimation)target;
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Debug:", EditorStylesUtility.BoldLabel);
                    using(new EditorGUILayout.HorizontalScope()) {
                        if(GUILayout.Button("Show", GUILayout.Width(80))) {
                            t.Show();
                        }
                        if(GUILayout.Button("Hide", GUILayout.Width(80))) {
                            t.Hide();
                        }
                        if(GUILayout.Button("Cancel", GUILayout.Width(80))) {
                            t.Cancel();
                        }
                    }
                }
                EditorGUILayout.LabelField("IsComplete: " + t.IsComplete);
            }
        }

        #endregion
    }
}
