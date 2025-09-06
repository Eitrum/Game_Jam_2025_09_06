using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit.Utility
{
    public static class Wireframe
    {
        #region Consts

        private const string ENABLED_PREFS_PATH = "Toolkit.Wireframe";
        private const string COLOR_PREFS_PATH = "Toolkit.Wireframe_Color";

        #endregion

        #region Variables

        private static bool enabled = false;
        private static Color color = Color.magenta;

        #endregion

        #region Properties

        public static bool Enabled {
            get => enabled;
            set {
                if(value != enabled) {
                    EditorPrefs.SetBool(ENABLED_PREFS_PATH, value);
                    enabled = value;
                }
            }
        }

        public static Color Color {
            get => color;
            set {
                if(!value.Equals(color)) {
                    EditorPrefs.SetInt(COLOR_PREFS_PATH, value.ToInt());
                    color = value;
                }
            }
        }

        #endregion

        #region Setup

        [InitializeOnLoadMethod]
        private static void Initialize() {
            enabled = EditorPrefs.GetBool(ENABLED_PREFS_PATH, false);
            color = EditorPrefs.GetInt(COLOR_PREFS_PATH, Color.magenta.ToInt()).ToColor();

            ToolkitProjectSettings.RegisterEditor("Wireframe", 2, ProjectSettingsGUI);
        }

        #endregion

        #region GUI

        private static void ProjectSettingsGUI(string obj) {
            Enabled = EditorGUILayout.Toggle("Enabled", Enabled);
            Color = EditorGUILayout.ColorField("Color", Color);
        }

        #endregion

        #region Rendering

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void Render(Transform transform, GizmoType type) {
            if(!enabled) {
                return;
            }
            Render(transform);
        }

        public static void Render(Transform transform) {
            Gizmos.color = color;
            if(transform.TryGetComponent(out MeshFilter meshFilter) && meshFilter.sharedMesh != null)
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
            else if(transform.TryGetComponent(out SkinnedMeshRenderer smr))
                Gizmos.DrawWireMesh(smr.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
        }

        #endregion
    }
}
