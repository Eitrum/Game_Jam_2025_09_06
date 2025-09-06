using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Toolkit.Procedural.Items
{
    [CustomPropertyDrawer(typeof(PartCollection.Entry))]
    public class PartCollectionEntryPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? (EditorGUIUtility.singleLineHeight * 6 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("connections"))) : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.Box(position, "");
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if(property.isExpanded) {
                var prefabProperty = property.FindPropertyRelative("prefab");
                var connectionsProperty = property.FindPropertyRelative("connections");
                var offsetProperty = property.FindPropertyRelative("offset");
                var offsetPositionProperty = offsetProperty.FindPropertyRelative("position");
                var offsetRotationProperty = offsetProperty.FindPropertyRelative("rotation");
                var rarityProperty = property.FindPropertyRelative("rarity");
                var qualityProperty = property.FindPropertyRelative("quality");
                using(new EditorGUI.IndentLevelScope(1)) {
                    position.y += position.height;
                    EditorGUI.PropertyField(position, prefabProperty);
                    position.y += position.height;
                    position.height = EditorGUI.GetPropertyHeight(connectionsProperty);
                    EditorGUI.PropertyField(position, connectionsProperty, true);
                    position.y += position.height;
                    position.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(position, offsetPositionProperty);
                    position.y += position.height;
                    offsetRotationProperty.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(position, "Rotation", offsetRotationProperty.quaternionValue.eulerAngles));
                    position.y += position.height;
                    EditorGUI.PropertyField(position, rarityProperty);
                    position.y += position.height;
                    EditorGUI.PropertyField(position, qualityProperty);
                }

            }
        }
    }
}
