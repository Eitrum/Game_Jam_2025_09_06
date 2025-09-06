using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Trigger {
    [CustomPropertyDrawer(typeof(TriggerSources))]
    public class TriggerSourcesPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var sprops = property.FindPropertyRelative("sources");
            return EditorGUI.GetPropertyHeight(sprops, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var sprops = property.FindPropertyRelative("sources");
            EditorGUI.PropertyField(position, sprops, label, true);
        }
    }
}
