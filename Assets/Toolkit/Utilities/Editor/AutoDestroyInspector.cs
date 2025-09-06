using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomEditor(typeof(AutoDestroy))]
    public class AutoDestroyInspector : Editor
    {
        private SerializedProperty useRangeProperty;
        private SerializedProperty minMaxProperty;

        private void OnEnable() {
            useRangeProperty = serializedObject.FindProperty("useRange");
            minMaxProperty = serializedObject.FindProperty("range");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.HorizontalScope("box")) {
                var mode = useRangeProperty.boolValue ? Mode.Random : Mode.Constant;
                mode = (Mode)EditorGUILayout.EnumPopup(mode, GUILayout.Width(80f));
                useRangeProperty.boolValue = mode == Mode.Random;
                if(!useRangeProperty.boolValue) {
                    var minProp = minMaxProperty.FindPropertyRelative("min");
                    minProp.floatValue = EditorGUILayout.DelayedFloatField("Delay", minProp.floatValue);
                }
                else {
                    EditorGUILayout.PropertyField(minMaxProperty, new GUIContent("Delay (Range)"));
                }
            }
            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        public enum Mode
        {
            Constant,
            Random
        }
    }
}
