using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(Internal.IObjRef), true)]
    public class IObjRefPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            System.Type filter = GetFilter(fieldInfo);

            if(filter == null) {
                var isArray = property.propertyPath.EndsWith(']');
                if(isArray) {
                    filter = fieldInfo?.FieldType?
                        .GenericTypeArguments?
                        .FirstOrDefault()?
                        .GenericTypeArguments
                        .FirstOrDefault() ?? typeof(Object);
                }
                else {
                    var method = fieldInfo.FieldType.GetMethod("GetImplType", BindingFlags.Static | BindingFlags.Public);
                    filter = method?.Invoke(null, null) as System.Type ?? typeof(Object);
                }
            }
            property = property.FindPropertyRelative("reference");

            EditorGUI.BeginChangeCheck(); ;

            var obj = (label == GUIContent.none) ?
                    EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(UnityEngine.Object), true) :
                    EditorGUI.ObjectField(position, EditorGUIUtility.TrTempContent($"{label.text}<{filter.Name}>"), property.objectReferenceValue, typeof(UnityEngine.Object), true);
            if(obj != null) {
                if(obj is GameObject go && go.TryGetComponent(filter, out Component component)) {
                    obj = component;
                }
                else if(!filter.IsAssignableFrom(obj.GetType())) {
                    obj = null;
                }
            }
            if(EditorGUI.EndChangeCheck())
                property.objectReferenceValue = obj;
        }

        public static System.Type GetFilter(FieldInfo fieldInfo) {
            if(fieldInfo == null)
                return null;
            System.Type result = null;
            try {
                if(fieldInfo.FieldType.IsArray) {
                    result = fieldInfo.FieldType.GetElementType()?.GetGenericArguments()?[0];
                }
                else if(typeof(IList).IsAssignableFrom(fieldInfo.FieldType)) {
                    result = fieldInfo.FieldType?.GetGenericArguments()?[0].GetGenericArguments()?[0] ?? null;
                }
                else {
                    result = fieldInfo.FieldType.GetGenericArguments()[0];
                }
            }
            catch { }

            return result;
        }
    }
}
