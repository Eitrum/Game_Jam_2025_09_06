using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Toolkit.Procedural.Terrain
{
    [CustomPropertyDrawer(typeof(Layer))]
    public class LayerPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 120f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.DrawRect(position, new Color(0.2f, 0.2f, 0.2f, 0.4f));
            var splits = position.SplitVertical(7, 0f);
            EditorGUI.LabelField(splits[0], label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var texProp = property.FindPropertyRelative("texture");
            var useTexProp = property.FindPropertyRelative("useTexture");
            var strength = property.FindPropertyRelative("strength");
            var offsetX = property.FindPropertyRelative("offsetX");
            var offsetY = property.FindPropertyRelative("offsetY");
            var scaleX = property.FindPropertyRelative("scaleX");
            var scaleY = property.FindPropertyRelative("scaleY");
            var rotation = property.FindPropertyRelative("rotation");

            // Strenght / Texture
            EditorGUI.PropertyField(splits[1], strength);
            EditorGUI.PropertyField(splits[2], texProp);
            useTexProp.boolValue = texProp.objectReferenceValue != null;

            // Handle Offset
            EditorGUI.PropertyField(splits[3], offsetX);
            EditorGUI.PropertyField(splits[4], offsetY);

            // Handle Scale
            var isEqualKinda = scaleX.floatValue.Equals(scaleY.floatValue, 0.00001f);
            var scaleYVal = scaleY.floatValue;
            EditorGUI.BeginChangeCheck();
            var newScale = EditorGUI.Vector2Field(splits[5], "Scale", new Vector2(scaleX.floatValue, scaleY.floatValue));
            if(EditorGUI.EndChangeCheck()) {
                scaleX.floatValue = newScale.x;
                if(isEqualKinda && scaleYVal.Equals(scaleY.floatValue, 0.00001f))
                    scaleY.floatValue = scaleX.floatValue;
                else
                    scaleY.floatValue = newScale.y;
            }
            // Handle Rotation
            EditorGUI.PropertyField(splits[6], rotation);
            EditorGUI.indentLevel--;
        }
    }
}
