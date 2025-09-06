using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Trigger {
    [CustomPropertyDrawer(typeof(TriggerSource))]
    public class TriggerSourcePropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var text = label.text;
            var sprop = property.FindPropertyRelative("source");
            EditorGUI.PropertyField(position, sprop, label);
            var obvalue = sprop.objectReferenceValue;
            if(obvalue == null) {
                var success = TriggerSource.TryGetTrigger(obvalue, out var trigger);
                if(success) {
                    return;
                }
                EditorGUI.LabelField(position, text + " <b><color=#f80f0fff>( - Error - )</color></b>", EditorStylesUtility.RichTextLabel);
            }
        }
    }
}
