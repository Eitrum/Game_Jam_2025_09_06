using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.UI
{
    public class WorkflowOverlayWindow : EditorWindow
    {
        private const string PATH = "Toolkit/WorkflowOverlay";

        [InitializeOnLoadMethod]
        private static void Load() {
            WorkflowOverlay.Transparency = EditorPrefs.GetFloat(PATH, 0.5f);
        }

        public static void OpenEditor() {
            var w = GetWindow<WorkflowOverlayWindow>("Workflow Overlay", true);
        }

        private void OnGUI() {
            var area = new Rect(new Vector2(), position.size).Shrink(10f);
            GUILayout.BeginArea(area);
            EditorGUI.BeginChangeCheck();
            GUILayout.FlexibleSpace();
            var val = EditorGUILayout.Slider("Transparency", WorkflowOverlay.Transparency, 0f, 1f);
            GUILayout.FlexibleSpace();
            if(EditorGUI.EndChangeCheck()) {
                WorkflowOverlay.Transparency = val;
                EditorPrefs.SetFloat(PATH, val);
            }
            GUILayout.EndArea();
        }
    }
}
