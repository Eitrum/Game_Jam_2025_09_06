using System;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(IEEE754Attribute))]
    public class IEEE754AttributeDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    return property.type == "double" ? EditorGUIUtility.singleLineHeight * 5f + 8f : EditorGUIUtility.singleLineHeight * 3f + 6f;
                case SerializedPropertyType.Integer:
                    return (property.type == "long" || property.type == "ulong") ? EditorGUIUtility.singleLineHeight * 5f + 8f : EditorGUIUtility.singleLineHeight * 3f + 6f;
            }

            return EditorGUIUtility.singleLineHeight + 8f;
        }

        unsafe public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            switch(property.propertyType) {
                case SerializedPropertyType.Float:
                    if(property.type == "double")
                        HandleDouble(position, property, label);
                    else
                        HandleFloat(position, property, label);
                    break;
                case SerializedPropertyType.Integer:
                    if(property.type == "long" || property.type == "ulong")
                        HandleLong(position, property, label);
                    else
                        HandleInt(position, property, label);
                    break;
                default:
                    EditorGUI.HelpBox(position, label.text + " is not a float, remove IEEE574 attribute", MessageType.Error);
                    break;
            }
        }

        private unsafe static void HandleInt(Rect position, SerializedProperty property, GUIContent label) {
            const uint BASE = 0b0001;
            EditorGUI.BeginChangeCheck();
            position.ShrinkRef(1f);
            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            position.ShrinkRef(2f);

            position.SplitVertical(out Rect floatPreview, out Rect floatPartNameArea, 0.3333f, 2f);

            var iValue = property.intValue;
            uint binary = *(uint*)&iValue;
            var fValue = *(float*)(&binary);
            fValue = EditorGUI.FloatField(floatPreview, label, fValue);
            binary = *(uint*)&fValue;

            floatPartNameArea.SplitHorizontal(out Rect mantissaArea, out Rect exponentArea, out Rect signArea,
                (IEEE754Attribute.FLOAT_MANTISSA_COUNT * 18f) / floatPartNameArea.width,
                (IEEE754Attribute.FLOAT_EXPONENT_COUNT * 18f) / floatPartNameArea.width, 0f);
            signArea.width = 18f;

            EditorGUI.DrawRect(mantissaArea, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(exponentArea, new Color(0.9f, 0.2f, 0.2f, 0.6f));
            EditorGUI.DrawRect(signArea, new Color(0.2f, 0.2f, 0.9f, 0.6f));

            EditorGUI.LabelField(mantissaArea, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(exponentArea, "Exponent", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(signArea, "S", EditorStylesUtility.CenterAlignedBoldLabel);

            var toggleArea = new Rect(mantissaArea);
            toggleArea.width = 16f;
            toggleArea.height = 16f;
            toggleArea.x += 2f;
            toggleArea.y += 18f;

            for(int i = 0; i < IEEE754Attribute.FLOAT_MANTISSA_COUNT; i++) {
                uint m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            for(int i = IEEE754Attribute.FLOAT_MANTISSA_COUNT; i < 31; i++) {
                uint m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            {
                uint m = BASE << 31;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            if(EditorGUI.EndChangeCheck()) {
                property.intValue = *(int*)&binary;
            }
        }

        private unsafe static void HandleLong(Rect position, SerializedProperty property, GUIContent label) {
            const ulong BASE = 0b0001;
            EditorGUI.BeginChangeCheck();
            position.ShrinkRef(1f);
            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            position.ShrinkRef(2f);

            position.SplitVertical(out Rect doublePreviewArea, out Rect topPart, out Rect botPart, 0.2f, 0.4f, 2f);

            var lValue = property.longValue;
            ulong binary = *(ulong*)(&lValue);

            var dValue = *(double*)&binary;
            dValue = EditorGUI.DoubleField(doublePreviewArea, label, dValue);
            binary = *(ulong*)&dValue;

            topPart.width = 32f * 18f;
            botPart.width = 32f * 18f;

            botPart.SplitHorizontal(out Rect mantissaArea, out Rect exponentArea, out Rect signArea,
                ((IEEE754Attribute.DOUBLE_MANTISSA_COUNT - 32) * 18f) / botPart.width,
                (IEEE754Attribute.DOUBLE_EXPONENT_COUNT * 18f) / botPart.width, 0f);
            signArea.width = 18f;

            EditorGUI.DrawRect(topPart, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(mantissaArea, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(exponentArea, new Color(0.9f, 0.2f, 0.2f, 0.6f));
            EditorGUI.DrawRect(signArea, new Color(0.2f, 0.2f, 0.9f, 0.6f));

            EditorGUI.LabelField(topPart, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(mantissaArea, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(exponentArea, "Exponent", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(signArea, "S", EditorStylesUtility.CenterAlignedBoldLabel);

            var toggleArea = new Rect(topPart);
            toggleArea.width = 16f;
            toggleArea.height = 16f;
            toggleArea.x += 2f;
            toggleArea.y += 18f;

            for(int i = 0; i < 32; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            toggleArea.x = mantissaArea.x + 2f;
            toggleArea.y = mantissaArea.y + 18f;

            for(int i = 32; i < IEEE754Attribute.DOUBLE_MANTISSA_COUNT; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            for(int i = IEEE754Attribute.DOUBLE_MANTISSA_COUNT; i < 63; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            {
                ulong m = BASE << 63;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            if(EditorGUI.EndChangeCheck()) {
                property.longValue = *(long*)&binary;
            }
        }

        private unsafe static void HandleFloat(Rect position, SerializedProperty property, GUIContent label) {
            const uint BASE = 0b0001;
            EditorGUI.BeginChangeCheck();
            position.ShrinkRef(1f);
            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            position.ShrinkRef(2f);

            position.SplitVertical(out Rect floatPreview, out Rect floatPartNameArea, 0.3333f, 2f);

            var fValue = property.floatValue;
            fValue = EditorGUI.FloatField(floatPreview, label, fValue);

            floatPartNameArea.SplitHorizontal(out Rect mantissaArea, out Rect exponentArea, out Rect signArea,
                (IEEE754Attribute.FLOAT_MANTISSA_COUNT * 18f) / floatPartNameArea.width,
                (IEEE754Attribute.FLOAT_EXPONENT_COUNT * 18f) / floatPartNameArea.width, 0f);
            signArea.width = 18f;

            EditorGUI.DrawRect(mantissaArea, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(exponentArea, new Color(0.9f, 0.2f, 0.2f, 0.6f));
            EditorGUI.DrawRect(signArea, new Color(0.2f, 0.2f, 0.9f, 0.6f));

            EditorGUI.LabelField(mantissaArea, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(exponentArea, "Exponent", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(signArea, "S", EditorStylesUtility.CenterAlignedBoldLabel);

            uint binary = *(uint*)(&fValue);

            var toggleArea = new Rect(mantissaArea);
            toggleArea.width = 16f;
            toggleArea.height = 16f;
            toggleArea.x += 2f;
            toggleArea.y += 18f;

            for(int i = 0; i < IEEE754Attribute.FLOAT_MANTISSA_COUNT; i++) {
                uint m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            for(int i = IEEE754Attribute.FLOAT_MANTISSA_COUNT; i < 31; i++) {
                uint m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            {
                uint m = BASE << 31;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            fValue = *(float*)&binary;

            if(EditorGUI.EndChangeCheck()) {
                property.floatValue = fValue;
            }
        }

        private unsafe static void HandleDouble(Rect position, SerializedProperty property, GUIContent label) {
            const ulong BASE = 0b0001;
            EditorGUI.BeginChangeCheck();
            position.ShrinkRef(1f);
            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            position.ShrinkRef(2f);

            position.SplitVertical(out Rect doublePreviewArea, out Rect topPart, out Rect botPart, 0.2f, 0.4f, 2f);

            var dValue = property.doubleValue;
            dValue = EditorGUI.DoubleField(doublePreviewArea, label, dValue);

            topPart.width = 32f * 18f;
            botPart.width = 32f * 18f;

            botPart.SplitHorizontal(out Rect mantissaArea, out Rect exponentArea, out Rect signArea,
                ((IEEE754Attribute.DOUBLE_MANTISSA_COUNT - 32) * 18f) / botPart.width,
                (IEEE754Attribute.DOUBLE_EXPONENT_COUNT * 18f) / botPart.width, 0f);
            signArea.width = 18f;

            EditorGUI.DrawRect(topPart, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(mantissaArea, new Color(0.2f, 0.9f, 0.2f, 0.6f));
            EditorGUI.DrawRect(exponentArea, new Color(0.9f, 0.2f, 0.2f, 0.6f));
            EditorGUI.DrawRect(signArea, new Color(0.2f, 0.2f, 0.9f, 0.6f));

            EditorGUI.LabelField(topPart, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(mantissaArea, "Mantissa", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(exponentArea, "Exponent", EditorStylesUtility.CenterAlignedBoldLabel);
            EditorGUI.LabelField(signArea, "S", EditorStylesUtility.CenterAlignedBoldLabel);

            ulong binary = *(ulong*)(&dValue);

            var toggleArea = new Rect(topPart);
            toggleArea.width = 16f;
            toggleArea.height = 16f;
            toggleArea.x += 2f;
            toggleArea.y += 18f;

            for(int i = 0; i < 32; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            toggleArea.x = mantissaArea.x + 2f;
            toggleArea.y = mantissaArea.y + 18f;

            for(int i = 32; i < IEEE754Attribute.DOUBLE_MANTISSA_COUNT; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            for(int i = IEEE754Attribute.DOUBLE_MANTISSA_COUNT; i < 63; i++) {
                ulong m = BASE << i;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            {
                ulong m = BASE << 63;
                var isChecked = (binary & m) == m;
                var val = EditorGUI.Toggle(toggleArea, isChecked);
                if(isChecked != val) {
                    binary = val ? binary | m : binary ^ m;
                }
                toggleArea.x += 18f;
            }

            dValue = *(double*)&binary;

            if(EditorGUI.EndChangeCheck()) {
                property.doubleValue = dValue;
            }
        }
    }
}
