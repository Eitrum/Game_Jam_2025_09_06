using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(EaseReference))]
    public class EaseReferencePropertyDrawer : PropertyDrawer
    {
        private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 8f : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var type = property.FindPropertyRelative("type");
            var function = property.FindPropertyRelative("function");

            if(property.isExpanded) {
                position.SplitVertical(out position, out Rect bot, 1f / 8f);
                Ease.SetAnimationCurve(curve, (Ease.Function)function.intValue, (Ease.Type)type.intValue);
                using(new EditorGUI.DisabledScope(true))
                    EditorGUI.CurveField(bot, curve);
            }
            position.SplitHorizontal(out Rect labelArea, out Rect typeArea, out Rect functionArea, 0.4f, 0.15f, 2f);
            property.isExpanded = EditorGUI.Foldout(labelArea, property.isExpanded, label, true);
            EditorGUI.PropertyField(typeArea, type, GUIContent.none);
            EditorGUI.PropertyField(functionArea, function, GUIContent.none);
        }
    }
}
