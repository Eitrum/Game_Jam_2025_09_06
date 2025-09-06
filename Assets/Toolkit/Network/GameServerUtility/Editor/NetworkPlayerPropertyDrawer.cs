using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Network {
    [CustomPropertyDrawer(typeof(NetworkPlayer))]
    public class NetworkPlayerPropertyDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if(!property.isExpanded)
                return;

            var netPlayer = GetTargetObjectOfProperty(property) as NetworkPlayer;

            using(var iscope = new EditorGUI.IndentLevelScope(1)) {
                if(netPlayer.HasAddress) {
                    EditorGUI.LabelField(position.NextLine(), $"{netPlayer.Address.Join('.')}:{netPlayer.Port} [{netPlayer.Id}]");
                }
                else
                    EditorGUI.LabelField(position.NextLine(), $"[{netPlayer.Id}]");

                EditorGUI.LabelField(position.NextLine(), $"'{netPlayer.DisplayName}'");
            }
        }


        public static object GetTargetObjectOfProperty(SerializedProperty prop) {
            if(prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');

            foreach(var element in elements) {
                if(element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(
                element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", "")
            );

                    obj = GetValue(obj, elementName, index);
                }
                else {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue(object source, string name) {
            if(source == null)
                return null;
            var type = source.GetType();

            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if(p != null)
                return p.GetValue(source, null);

            return null;
        }

        private static object GetValue(object source, string name, int index) {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if(enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for(int i = 0; i <= index; i++) {
                if(!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
