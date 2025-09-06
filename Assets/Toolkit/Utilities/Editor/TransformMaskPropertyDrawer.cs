using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(TransformMask))]
    public class TransformMaskPropertyDrawer : PropertyDrawer
    {
        private static string[] displayValues = new string[]{
            "Position X", "Position Y", "Position Z",
            "Position",
            "Rotation X", "Rotation Y", "Rotation Z",
            "Rotation",
            "Scale X", "Scale Y", "Scale Z",
            "Scale",
            "All",
        };

        private const int POSITION_MASK = 0b0000_0000_0111;
        private const int ROTATION_MASK = 0b0000_0011_1000;
        private const int SCALE_MASK = 0b0001_1100_0000;

        private const int ROTATION_MASK2 = 0b0000_0111_0000;
        private const int SCALE_MASK2 = 0b0111_0000_0000;
        private const int ALL_MASK = 0b0001_0000_0000_0000;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var storedValue = property.intValue;
            storedValue >>= 1;

            storedValue = ((SCALE_MASK & storedValue) << 2) | ((ROTATION_MASK & storedValue) << 1) | ((POSITION_MASK & storedValue));

            if((storedValue & POSITION_MASK) == POSITION_MASK)
                storedValue |= 0b1000;
            if((storedValue & ROTATION_MASK2) == ROTATION_MASK2)
                storedValue |= 0b1000_0000;
            if((storedValue & SCALE_MASK2) == SCALE_MASK2)
                storedValue |= 0b1000_0000_0000;

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.MaskField(position, label, storedValue, displayValues);

            if(EditorGUI.EndChangeCheck()) {
                // Handle Position
                if(((storedValue & 0b1000) == 0b1000) && ((newValue & 0b1000) != 0b1000))
                    newValue = newValue & ~POSITION_MASK;
                else if(((storedValue & 0b1000) != 0b1000) && ((newValue & 0b1000) == 0b1000))
                    newValue = newValue | POSITION_MASK;

                // Handle Rotation
                if(((storedValue & 0b1000_0000) == 0b1000_0000) && ((newValue & 0b1000_0000) != 0b1000_0000))
                    newValue = newValue & ~ROTATION_MASK2;
                else if(((storedValue & 0b1000_0000) != 0b1000_0000) && ((newValue & 0b1000_0000) == 0b1000_0000))
                    newValue = newValue | ROTATION_MASK2;

                // Handle Scale
                if(((storedValue & 0b1000_0000_0000) == 0b1000_0000_0000) && ((newValue & 0b1000_0000_0000) != 0b1000_0000_0000))
                    newValue = newValue & ~SCALE_MASK2;
                else if(((storedValue & 0b1000_0000_0000) != 0b1000_0000_0000) && ((newValue & 0b1000_0000_0000) == 0b1000_0000_0000))
                    newValue = newValue | SCALE_MASK2;

                storedValue = newValue;
                if((storedValue & ALL_MASK) == ALL_MASK) {
                    storedValue = ROTATION_MASK2 | SCALE_MASK2 | POSITION_MASK;
                }
                storedValue = (((storedValue & SCALE_MASK2) >> 2) | ((storedValue & ROTATION_MASK2) >> 1) | (storedValue & POSITION_MASK)) << 1;

                property.intValue = storedValue;
            }
        }
    }
}
