using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(DegToRadAttribute))]
    public class DegToRadAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var degtorad = attribute as DegToRadAttribute;
            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    switch(degtorad.mode) {
                        case DegToRadAttribute.Mode.Value:
                            property.floatValue = EditorGUI.FloatField(position, property.floatValue * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                            break;
                        case DegToRadAttribute.Mode.Slider:
                            property.floatValue = EditorGUI.Slider(position, label, property.floatValue * Mathf.Rad2Deg, degtorad.min, degtorad.max) * Mathf.Deg2Rad;
                            break;
                        case DegToRadAttribute.Mode.SliderStep:
                            position.SplitHorizontal(out Rect labelArea, out Rect valueArea, out Rect fieldArea, 0.35f, 0.5f, 2f);
                            EditorGUI.LabelField(labelArea, label);
                            property.floatValue = GUI.Slider(valueArea.Pad(20, -20, 0, 0),
                                property.floatValue * Mathf.Rad2Deg, degtorad.step, degtorad.min, degtorad.max + 0.0001f,
                                GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, (int)position.y + (int)position.x, null) * Mathf.Deg2Rad;
                            property.floatValue = EditorGUI.FloatField(fieldArea, property.floatValue * Mathf.Rad2Deg).Snap(degtorad.step) * Mathf.Deg2Rad;
                            break;
                    }
                    break;
                default:
                    EditorGUI.HelpBox(position, "Only float fields supported!", MessageType.Error);
                    break;
            }
        }
    }
}
