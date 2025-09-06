using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Toolkit
{
    public static class SerializedObjectExtensions
    {
        public static SerializedProperty FindScriptableObjectEntry(this SerializedObject so) {
            var scriptProp = so.FindProperty("m_Script");
            if(scriptProp != null) {
                scriptProp.NextVisible(false);
                return scriptProp;
            }
            return null;
        }

        #region Log

        public static void FullLog(this SerializedProperty property) {
            Debug.Log($"Property '{property.propertyPath}'\n\tType: {property.propertyType}\n\t{GetLogByType(property)}");
        }

        private static string GetLogByType(SerializedProperty property) {
            if(property.isArray) {
                StringBuilder sb = new StringBuilder();
                for(int i = 0, length = property.arraySize; i < length; i++) {
                    sb.AppendLine(GetLogByType(property.GetArrayElementAtIndex(i)));
                }
                return sb.ToString();
            }
            switch(property.propertyType) {
                case SerializedPropertyType.Integer: return $"Value: {property.intValue}";
                case SerializedPropertyType.Float: return $"Value: {property.floatValue}";
                case SerializedPropertyType.Vector3: return $"Value: {property.vector3Value}";
                case SerializedPropertyType.Vector3Int: return $"Value: {property.vector3IntValue}";
                case SerializedPropertyType.Vector2: return $"Value: {property.vector2Value}";
                case SerializedPropertyType.Vector2Int: return $"Value: {property.vector2IntValue}";
                case SerializedPropertyType.Vector4: return $"Value: {property.vector4Value}";
                case SerializedPropertyType.Quaternion: return $"Value: {property.quaternionValue}";
                case SerializedPropertyType.String: return $"Value: {property.stringValue}";
                case SerializedPropertyType.Boolean: return $"Value: {property.boolValue}";
                case SerializedPropertyType.Rect: return $"Value: {property.rectValue}";
                case SerializedPropertyType.RectInt: return $"Value: {property.rectIntValue}";
                case SerializedPropertyType.Bounds: return $"Value: {property.boundsValue}";
                case SerializedPropertyType.BoundsInt: return $"Value: {property.boundsIntValue}";
                case SerializedPropertyType.Character: return $"Value: {property.stringValue}";
                case SerializedPropertyType.Color: return $"Value: {property.colorValue}";
                case SerializedPropertyType.LayerMask: return $"Value: {property.intValue}";
                case SerializedPropertyType.Generic: {
                        return $"Actual Type: {property.type}";
                    }
            }
            return "null";
        }

        #endregion
    }
}
