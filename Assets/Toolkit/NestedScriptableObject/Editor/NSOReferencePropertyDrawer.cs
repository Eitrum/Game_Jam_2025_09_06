using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(NSOReference), true)]
    public class NSOReferencePropertyDrawer : NSOReferenceAttributePropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return NSOEditor.CalculateHeight(property);
        }

        private SerializedProperty FindProperProperty(SerializedProperty property, out Type type) {
            var nsor = fieldInfo.GetValue(GetParent(property));// as NestedScriptableObjectReference;
            var nsorType = nsor.GetType();
            type = null;
            if(nsorType.IsArray) {
                var tar = (object[])nsor;
                if(tar != null && tar.Length > 0) {
                    type = (tar[0] as NSOReference)?.NestedScriptableObjectType;
                }
            }
            else
                type = (nsor as NSOReference)?.NestedScriptableObjectType;
            property = property.FindPropertyRelative("scriptable");
            return property;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.isExpanded = true;
            property = FindProperProperty(property, out Type type);
            NSOEditor.DrawProperty(position, property, label, type);
        }

        #region Utility

        public object GetParent(SerializedProperty prop) {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            for(int i = 0, length = elements.Length - 1; i < length; i++) {
                var element = elements[i];
                if(element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public object GetValue(object source, string name) {
            if(source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f == null) {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public object GetValue(object source, string name, int index) {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while(index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }

        #endregion
    }
}
