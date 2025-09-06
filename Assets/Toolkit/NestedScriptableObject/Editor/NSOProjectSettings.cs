using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    internal class NSOProjectSettings
    {
        #region Variables

        private const string PATH = "Project/Toolkit/Nested Scriptable Objects";

        #endregion

        #region Unity Init

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider(PATH, SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        #endregion

        #region Drawing

        private static void OnGUI(string obj) {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("User Settings", EditorStyles.boldLabel);
            NSOEditorSettings.Mode = (NSOMode)EditorGUILayout.EnumPopup("Mode", NSOEditorSettings.Mode);
            NSOEditorSettings.ColorMode = (NSOColor)EditorGUILayout.EnumPopup("Color", NSOEditorSettings.ColorMode);
            DrawColors();
            EditorGUI.indentLevel--;
        }

        private static void DrawColors() {
            using(new EditorGUI.IndentLevelScope(1)) {
                var area = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(GUILayout.Height(12f)));
                GUI.Box(area, "");
                area.width = 12f;
                for(int i = 0; i < 16; i++) {
                    EditorGUI.DrawRect(area, NSOUtility.GetColor(i, NSOEditorSettings.ColorMode));
                    area.x += 14f;
                }
            }
        }

        #endregion
    }
}
