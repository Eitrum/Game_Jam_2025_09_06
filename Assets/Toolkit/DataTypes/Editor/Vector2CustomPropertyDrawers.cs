using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    internal static class VectorPropertyDrawerUtility
    {
        public static GUIContent[] contentsVector2 = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
    }

    #region Byte

    [CustomPropertyDrawer(typeof(Vector2Byte))]
    public class Vector2BytePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(Vector2SByte))]
    public class Vector2SBytePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    #endregion

    #region Short

    [CustomPropertyDrawer(typeof(Vector2Short))]
    public class Vector2ShortPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(Vector2UShort))]
    public class Vector2UShortPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    #endregion

    #region Int

    [CustomPropertyDrawer(typeof(Vector2UInt))]
    public class Vector2UIntPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    #endregion

    #region Long

    [CustomPropertyDrawer(typeof(Vector2Long))]
    public class Vector2LongPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(Vector2ULong))]
    public class Vector2ULongPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var xProp = property.FindPropertyRelative("x");
            var yProp = property.FindPropertyRelative("y");
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
            if(xProp.longValue < 0)
                xProp.longValue = 0;
            if(yProp.longValue < 0)
                yProp.longValue = 0;
        }
    }

    #endregion

    #region Double

    [CustomPropertyDrawer(typeof(Vector2Double))]
    public class Vector2DoublePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.NextVisible(true);
            EditorGUI.MultiPropertyField(position, VectorPropertyDrawerUtility.contentsVector2, property, label);
        }
    }

    #endregion
}
