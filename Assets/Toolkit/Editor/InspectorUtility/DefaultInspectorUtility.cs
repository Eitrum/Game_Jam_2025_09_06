using UnityEngine;
using UnityEditor;

namespace Toolkit {
    public static class DefaultInspectorUtility {
        #region Variables

        private const string TAG = ColorTable.RichTextTags.CYAN + "[DefaultInspectorUtility]</color> - ";
        private static bool drawHeader = false;

        #endregion

        #region Properties

        public static bool DrawHeader {
            get => drawHeader;
            set {
                if(drawHeader == value) return;
                drawHeader = value;
                EditorPrefs.SetBool("toolkit.editor.drawheader", value);
            }
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Init() {
            drawHeader = EditorPrefs.GetBool("toolkit.editor.drawheader", false);
            ToolkitProjectSettings.RegisterEditor("Inspector", -10000, OnRenderProjectSettings);
        }

        #endregion

        #region Project Settings

        private static void OnRenderProjectSettings(string filter) {
            DrawHeader = EditorGUILayout.Toggle("Draw script field on components", drawHeader);
        }

        #endregion

        #region Draw Default

        public static bool DrawInspectorWithUserSettings(SerializedObject obj) {
            return
                drawHeader ?
                    DrawDefaultInspector(obj) :
                    DrawDefaultInspectorWithoutScriptReference(obj);
        }

        /// <summary>
        /// Unity's default inspector drawer copied from Editor.cs
        /// </summary>
        /// <returns>Returns true if and serialized property was changed.</returns>
        public static bool DrawDefaultInspector(SerializedObject obj) {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while(iterator.NextVisible(enterChildren)) {
                using(new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath)) {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                enterChildren = false;
            }

            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        public static bool DrawDefaultInspectorWithoutScriptReference(SerializedObject obj) {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
            iterator.NextVisible(true);
            bool enterChildren = true;
            while(iterator.NextVisible(enterChildren)) {
                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }

            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        #endregion
    }
}
