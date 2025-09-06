using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Toolkit.Mathematics
{
    [CustomPropertyDrawer(typeof(AnimationCurve))]
    public class AnimationCurveEditor : PropertyDrawer
    {
        static Rect target;
        static AnimationCurve curve;
        static bool change = false;

        public static void Apply(AnimationCurve curve) {
            AnimationCurveEditor.curve = curve;
            change = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var width = 65f;
            var curveField = new Rect(position);
            curveField.width -= width;
            EditorGUI.BeginChangeCheck();
            var newCurve = EditorGUI.CurveField(curveField, label.text, property.animationCurveValue);
            if(EditorGUI.EndChangeCheck()) {
                property.animationCurveValue = newCurve;
            }
            var selectButton = new Rect(position);
            selectButton.x += curveField.width;
            selectButton.width = width;
            if(GUI.Button(selectButton, "Generate")) {
                target = position;
                change = false;
                PopupWindow.Show(position, new AnimationCurveGeneratorPopup(property));
            }
            if(target == position && change) {
                property.animationCurveValue = curve;
                change = false;
            }
        }
    }

    public class AnimationCurveGeneratorPopup : PopupWindowContent
    {
        public SerializedProperty property;

        private Ease.Function easeFunction;
        private Ease.Type easeType;
        private int keyFrames = 8;
        private bool invert = false;

        public AnimationCurveGeneratorPopup(SerializedProperty property) {
            this.property = property;
        }

        public override Vector2 GetWindowSize() {
            return new Vector2(250f, 122f);
        }

        public override void OnGUI(Rect rect) {
            rect.ShrinkRef(5f);
            GUILayout.BeginArea(rect);
            GUILayout.Label("Animation Curve Generator", EditorStyles.boldLabel);

            easeFunction = (Ease.Function)EditorGUILayout.EnumPopup("Ease Function", easeFunction);
            easeType = (Ease.Type)EditorGUILayout.EnumPopup("Ease Type", easeType);
            keyFrames = Mathf.Clamp(EditorGUILayout.IntField("Key Frames", keyFrames), 4, 100);
            invert = EditorGUILayout.ToggleLeft("Invert", invert);

            if(GUILayout.Button("Generate")) {
                AnimationCurveEditor.Apply(Ease.GetAnimationCurve(Ease.GetEaseFunction(easeFunction, easeType), keyFrames, invert));
            }
            GUILayout.EndArea();
        }
    }
}
