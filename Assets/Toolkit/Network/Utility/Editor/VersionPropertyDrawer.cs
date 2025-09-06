using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Network
{
    [CustomPropertyDrawer(typeof(Version))]
    public class VersionPropertyDrawer : PropertyDrawer
    {
        private static GUIStyle style;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            if(style == null) {
                style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                style.alignment = TextAnchor.UpperRight;
            }
            EditorGUI.LabelField(position, StringFromProperty(property), style);
        }

        public static Version VersionFromProperty(SerializedProperty property) {
            int major, minor, patch = 0;
            property.NextVisible(true);
            major = property.intValue;
            property.NextVisible(true);
            minor = property.intValue;
            property.NextVisible(true);
            patch = property.intValue;
            return new Version(major, minor, patch);
        }

        public static string StringFromProperty(SerializedProperty property) {
            int major, minor, patch = 0;
            property.NextVisible(true);
            major = property.intValue;
            property.NextVisible(true);
            minor = property.intValue;
            property.NextVisible(true);
            patch = property.intValue;
            return $"v{major}.{minor}.{patch}";
        }
    }
}
