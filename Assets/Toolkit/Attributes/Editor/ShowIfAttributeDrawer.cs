using System;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(ShowRender(property))
                return EditorGUI.GetPropertyHeight(property, label, true);
            else
                return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(ShowRender(property))
                using(new EditorGUI.IndentLevelScope(1))
                    EditorGUI.PropertyField(position, property, label, true);
        }

        private bool ShowRender(SerializedProperty property) {
            var att = attribute as ShowIfAttribute;
            var thisPath = property.propertyPath;
            var targetPath = thisPath.Remove(thisPath.Length - property.name.Length) + att.PropertyName;
            var targetProperty = property.serializedObject.FindProperty(targetPath);

            if(targetProperty == null) {
                // Debug.Log(this.FormatLog("Failed to find target property", targetPath));
                return true;
            }

            try {
                var typeCode = System.Convert.GetTypeCode(att.Value);
                // Debug.Log(this.FormatLog("TypeCode: " + typeCode));
                switch(typeCode) {
                    // If no typecode added check if boolean value
                    case TypeCode.Empty: {
                            if(targetProperty.propertyType == SerializedPropertyType.Boolean)
                                return targetProperty.boolValue;
                            return true;
                        }
                    case TypeCode.Boolean: return targetProperty.boolValue == att.IgnoreCase;
                    /// INT
                    case TypeCode.Int32: {
                            var val = (int)att.Value;
                            var cval = targetProperty.intValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    case TypeCode.UInt32: {
                            var val = (uint)att.Value;
                            var cval = targetProperty.uintValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    /// LONG
                    case TypeCode.Int64: {
                            var val = (long)att.Value;
                            var cval = targetProperty.longValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    case TypeCode.UInt64: {
                            var val = (ulong)att.Value;
                            var cval = targetProperty.ulongValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    /// FLOATING POINTS
                    case TypeCode.Single: {
                            var val = (float)att.Value;
                            var cval = targetProperty.floatValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    case TypeCode.Double: {
                            var val = (double)att.Value;
                            var cval = targetProperty.doubleValue;
                            return Mathematics.ComparitorOperatorUtility.Is(att.OperatorType, cval, val);
                        }
                    case TypeCode.String: {
                            var val = (string)att.Value;
                            var cval = targetProperty.stringValue;
                            return cval.Equals(val, att.IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
                        }
                    default:
                        return true;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return true;
            }
        }
    }
}
