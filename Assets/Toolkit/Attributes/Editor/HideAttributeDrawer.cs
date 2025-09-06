using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(HideAttribute))]
    public class HideAttributeDrawer : PropertyDrawer {

        private static bool forceShow;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var att = attribute as HideAttribute;

            if(forceShow && string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
                forceShow = false;

            if(forceShow || (att.ShowIfHoldingAlt && EditorStylesUtility.IsHoldingAlt)) {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var att = attribute as HideAttribute;

            if(att.ShowIfHoldingAlt && EditorStylesUtility.IsHoldingAlt) {
                label.text = "<color=#888888><i><size=10>(Hidden)</size></i></color> " + label.text;
                GUI.SetNextControlName(property.propertyPath);
                EditorGUI.PropertyField(position, property, label, true);
                if(GUI.GetNameOfFocusedControl() == property.propertyPath) {
                    forceShow = true;
                }
            }
            else if(forceShow) {
                if(string.IsNullOrEmpty(GUI.GetNameOfFocusedControl())) {
                    forceShow = false;
                    ToolkitEditorUtility.RepaintInspectorsDelayed();
                }
                else {
                    GUI.SetNextControlName(property.propertyPath);
                    EditorGUI.PropertyField(position, property, label, true);
                    ToolkitEditorUtility.RepaintInspectors();
                }
            }
        }
    }
}
