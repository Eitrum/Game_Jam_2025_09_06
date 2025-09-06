using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(NSOReferenceArray), true)]
    public class NSOReferenceArrayDrawer : PropertyDrawer {

        #region Variables

        private Dictionary<string, ReorderableList> reorderableCache = new Dictionary<string, ReorderableList>();

        #endregion

        #region Reorderable Tech

        private ReorderableList GetReorderableList(SerializedProperty prop) {
            SerializedProperty listProperty = prop.FindPropertyRelative("scriptables");

            if(reorderableCache.TryGetValue(listProperty.propertyPath, out ReorderableList list)) {
                list.serializedProperty = listProperty;
                return list;
            }

            list = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);
            reorderableCache[listProperty.propertyPath] = list;

            var types = fieldInfo.FieldType.GenericTypeArguments;
            var type = types[0];

            string displayname = prop.displayName;
            list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, displayname);
            list.drawElementCallback += (rect, index, isActive, isFocused) => {
                var prop = list.serializedProperty.GetArrayElementAtIndex(index);
                var objRef = prop.objectReferenceValue;
                var content = objRef != null ? EditorGUIUtility.TrTempContent(System.IO.Path.GetFileName(objRef.name)) : EditorGUIUtility.TrTempContent($"Element {index}");
                NSOEditor.DrawProperty(new Rect(rect.x + 16, rect.y, rect.width - 16, rect.height), prop, content, type);
            };
            list.elementHeightCallback += index => NSOEditor.CalculateHeight(list.serializedProperty.GetArrayElementAtIndex(index));
            list.onAddCallback = OnAdd;
            list.onRemoveCallback = OnRemove;

            return list;
        }

        private void OnRemove(ReorderableList list) {
            var index = list.index;
            var prop = list.serializedProperty.GetArrayElementAtIndex(index);
            if(prop.objectReferenceValue != null)
                NSOUtility.VerifyAndDestroy(prop);
            list.serializedProperty.DeleteArrayElementAtIndex(index);
        }

        private void OnAdd(ReorderableList list) {
            var len = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            var newElement = list.serializedProperty.GetArrayElementAtIndex(len);
            newElement.objectReferenceValue = null;
        }

        #endregion

        #region PropertyDrawer Impl (Draw / GetHeight)

        public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label) {
            ReorderableList list = GetReorderableList(serializedProperty);
            list.DoList(EditorGUI.IndentedRect(rect));
        }

        public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
            return GetReorderableList(serializedProperty).GetHeight();
        }

        #endregion
    }
}
