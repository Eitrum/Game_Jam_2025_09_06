using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit;
using System;

namespace Toolkit.UI.LoadingScreen {
    [CustomEditor(typeof(TipsTable))]
    public class TipsTableInspector : Editor {
        #region Variables

        private SerializedProperty tableId;
        private SerializedProperty tips;

        private UnityEditorInternal.ReorderableList list;

        #endregion

        #region Init

        private void OnEnable() {
            tableId = serializedObject.FindProperty("tableId");
            tips = serializedObject.FindProperty("tips");
            list = new UnityEditorInternal.ReorderableList(serializedObject, tips);
            list.elementHeight = EditorGUIUtility.singleLineHeight * 2f + 4f;
            list.drawElementCallback += OnDrawElement;
            list.headerHeight = 0;
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.PropertyField(tableId);
                EditorGUILayout.Space(8);
                list.DoLayoutList();
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.Pad(0, 0, 2f, 2f).SplitHorizontal(out Rect numberArea, out Rect labelArea, 20f / rect.width, 2f);
            GUI.Label(numberArea, $"{index}", EditorStylesUtility.CenterAlignedLabel);
            var prop = tips.GetArrayElementAtIndex(index);
            EditorGUI.BeginChangeCheck();
            var strnew = EditorGUI.TextArea(labelArea, prop.stringValue);
            if(EditorGUI.EndChangeCheck()) {
                prop.stringValue = strnew;
            }
        }

        #endregion
    }
}
