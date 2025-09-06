using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
namespace Toolkit.Unit
{
    [CustomEditor(typeof(AttributeBehaviour))]
    public class AttributesBehaviourEditor : Editor
    {
        public override void OnInspectorGUI() {
            if(targets == null || targets.Length > 1) {
                EditorGUILayout.LabelField("Traits Behaviour does not support multi select editing");
            }

            var length = AttributeType.None.GetLength();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            for(int i = 1; i < length; i++) {
                var type = (AttributeType)i;
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(Attributes.GetFullName(type), GUILayout.Width(120f));
                prop.NextVisible(false);
                switch(prop.propertyType) {
                    case SerializedPropertyType.Integer:
                        prop.intValue = EditorGUILayout.IntField(prop.intValue);
                        break;
                    case SerializedPropertyType.Float:
                        prop.floatValue = EditorGUILayout.FloatField(prop.floatValue);
                        break;
                    case SerializedPropertyType.Generic:
                        // Check for Stat type
                        if(prop.type == "Stat") {
                            prop.NextVisible(true);
                            EditorGUILayout.LabelField("Base", GUILayout.Width(40f));
                            prop.floatValue = EditorGUILayout.FloatField(prop.floatValue);
                            prop.NextVisible(true);
                            EditorGUILayout.LabelField("Base Multiplier", GUILayout.Width(90f));
                            prop.floatValue = EditorGUILayout.FloatField(prop.floatValue);
                            prop.NextVisible(true);
                            EditorGUILayout.LabelField("Multiplier", GUILayout.Width(60f));
                            prop.floatValue = EditorGUILayout.FloatField(prop.floatValue);
                        }
                        break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private enum ValueType
        {
            Stat,
            Int,
            Float
        }

    }

    [CustomPropertyDrawer(typeof(Attributes))]
    public class AttributesEditor : PropertyDrawer
    {

        private static int Length = 0;

        static AttributesEditor() {
            Length = AttributeType.None.GetLength();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * Length : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var area = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(area, property.isExpanded, label, true);
            if(property.isExpanded) {
                EditorGUI.indentLevel++;
                var prop = property;
                for(int i = 1; i < Length; i++) {
                    area.y += EditorGUIUtility.singleLineHeight;
                    var type = (AttributeType)i;
                    GUI.Box(area, "");
                    area.SplitHorizontal(out Rect labelArea, out Rect body, 120f / area.width);
                    EditorGUI.LabelField(labelArea, Attributes.GetFullName(type));
                    prop.NextVisible(true);
                    using(new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
                        switch(prop.propertyType) {
                            case SerializedPropertyType.Integer:
                                prop.intValue = EditorGUI.IntField(body, prop.intValue);
                                break;
                            case SerializedPropertyType.Float:
                                prop.floatValue = EditorGUI.FloatField(body, prop.floatValue);
                                break;
                            case SerializedPropertyType.Generic:
                                // Check for Stat type
                                if(prop.type == "Stat") {
                                    body.SplitHorizontal(out Rect baseArea, out Rect baseMultiArea, out Rect multiArea, 0.25f, 0.45f, 5f);
                                    prop.NextVisible(true);
                                    baseArea.SplitHorizontal(out Rect baseLabelArea, out baseArea, 40f / baseArea.width);
                                    GUI.Label(baseLabelArea, "Base");
                                    prop.floatValue = EditorGUI.FloatField(baseArea, prop.floatValue);
                                    prop.NextVisible(true);
                                    baseMultiArea.SplitHorizontal(out Rect baseMultiLabelArea, out baseMultiArea, 90f / baseMultiArea.width);
                                    GUI.Label(baseMultiLabelArea, "Base Multiplier");
                                    prop.floatValue = EditorGUI.FloatField(baseMultiArea, prop.floatValue);
                                    prop.NextVisible(true);
                                    multiArea.SplitHorizontal(out Rect multiLabelArea, out multiArea, 60f / multiArea.width);
                                    GUI.Label(multiLabelArea, "Multiplier");
                                    prop.floatValue = EditorGUI.FloatField(multiArea, prop.floatValue);
                                }
                                break;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private enum ValueType
        {
            Stat,
            Int,
            Float
        }

    }
}
*/
