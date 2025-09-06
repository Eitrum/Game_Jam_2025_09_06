using UnityEditor;
using Toolkit;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [CustomEditor(typeof(Camera))]
    [SupportedOnRenderPipeline(typeof(ModularRenderPipelineAsset))]
    public class ModularRenderPipelineCameraInspector : Editor {

        private static bool showDebugMatrix = false;

        public override void OnInspectorGUI() {
            var c = target as Camera;

            using(var inspect = new ToolkitEditorUtility.InspectorScope(this)) {
                EditorGUILayout.LabelField("Field of View", EditorStyles.boldLabel);
                using(new EditorGUI.IndentLevelScope(1)) {
                    c.orthographic = EditorGUILayout.Toggle("Orthographic", c.orthographic);
                    if(c.orthographic)
                        c.orthographicSize = EditorGUILayout.Slider("Size", c.orthographicSize, 0f, 100f);
                    else
                        c.fieldOfView = EditorGUILayout.Slider("FoV", c.fieldOfView, 1f, 179f);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Clip Plane", EditorStyles.boldLabel);
                using(new EditorGUI.IndentLevelScope(1)) {
                    c.nearClipPlane = Mathf.Clamp(EditorGUILayout.FloatField("Near", c.nearClipPlane), 0.0001f, c.farClipPlane);
                    c.farClipPlane = Mathf.Clamp(EditorGUILayout.FloatField("Far", c.farClipPlane), c.nearClipPlane, 10000f);
                }
            }
            EditorGUILayout.Space();
            showDebugMatrix = EditorGUILayout.Foldout(showDebugMatrix, "Matrix", true);
            if(showDebugMatrix) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    using(new EditorGUI.DisabledScope(true)) {
                        ToolkitEditorUtility.GUILayout.DrawMatrix("View Matrix", c.worldToCameraMatrix);
                        ToolkitEditorUtility.GUILayout.DrawMatrix("Projection Matrix", c.projectionMatrix);
                        ToolkitEditorUtility.GUILayout.DrawMatrix("View-Proj Matrix", c.projectionMatrix * c.worldToCameraMatrix);
                    }
                }
            }
            c.GetOrAddComponent<MRPCamera>();
        }
    }
}
