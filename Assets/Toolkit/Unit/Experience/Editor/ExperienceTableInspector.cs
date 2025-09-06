using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Unit
{
    [CustomEditor(typeof(ExpereinceTable))]
    public class ExperienceTableInspector : Editor
    {
        #region Variables

        private double totalExperience = 0d;
        private SerializedProperty experienceTable;
        private UnityEditorInternal.ReorderableList reorderableList;

        #endregion

        #region Init

        private void OnEnable() {
            experienceTable = serializedObject.FindProperty("experienceTable");
            reorderableList = new UnityEditorInternal.ReorderableList(serializedObject, experienceTable);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.onCanRemoveCallback += OnCanRemove;
        }

        private bool OnCanRemove(ReorderableList list) {
            return list.index > 1 && experienceTable.arraySize > 2;
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                totalExperience = 0;
                reorderableList.DoLayoutList();
            }
        }

        private void DrawHeader(Rect rect) {
            rect.x += 16;
            rect.width -= 16;
            rect.SplitHorizontal(out Rect level, out Rect experienceArea, out Rect totalExperienceArea, 0.1f, 0.5f, 2f);
            EditorGUI.LabelField(level, "Level");
            EditorGUI.LabelField(experienceArea, "Experience");
            EditorGUI.LabelField(totalExperienceArea, "Total Experience");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            rect.SplitHorizontal(out Rect level, out Rect experienceArea, out Rect totalExperienceArea, 0.1f, 0.5f, 2f);
            var prop = experienceTable.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(level, (index).ToString());
            if(index <= 1) {
                EditorGUI.LabelField(experienceArea, prop.doubleValue.ToString());
                totalExperience += prop.doubleValue;
            }
            else {
                EditorGUI.PropertyField(experienceArea, prop, GUIContent.none);
                totalExperience += prop.doubleValue;
            }
            EditorGUI.LabelField(totalExperienceArea, totalExperience.ToString(), EditorStyles.centeredGreyMiniLabel);
        }

        #endregion
    }
}
