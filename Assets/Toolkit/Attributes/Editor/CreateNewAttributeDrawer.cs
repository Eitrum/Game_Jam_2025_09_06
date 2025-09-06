using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(CreateNewAttribute))]
    public class CreateNewAttributeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.ObjectReference || !fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject))) {
                EditorGUI.HelpBox(position, $"{label.text} has to be of type ScriptableObject", MessageType.Error);
                return;
            }
            position.SplitHorizontal(out Rect left, out Rect right, 1f - (30 / position.width));
            EditorGUI.PropertyField(left, property, label);
            if(GUI.Button(right, "+")) {
                var so = ScriptableObject.CreateInstance(fieldInfo.FieldType);
                var upath = AssetDatabase.GenerateUniqueAssetPath("Assets/new ToxelWorldData.asset");
                ProjectWindowUtil.CreateAsset(so, upath);
                property.objectReferenceValue = so;
            }
        }
    }
}
