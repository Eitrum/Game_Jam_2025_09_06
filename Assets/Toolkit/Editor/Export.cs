using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    public static class Export
    {
        #region Variables

        private const int PRIORITY = 200000000;
        private const string MAIN_PATH = "Assets/Toolkit";
        private const string MAIN_ASSEMBLY_DEFINITION = "Assets/Toolkit/Toolkit.asmdef";

        #endregion

        #region Utility

        public static bool AllAssetsExists(string[] paths) {
            return !paths.Any(x => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(x) == null);
        }

        #endregion

        #region Toolkit Exporting

        [MenuItem("Toolkit/Export/All", priority = PRIORITY)]
        public static void Toolkit() {
            if(!ToolkitValidation()) {
                throw new System.IO.DirectoryNotFoundException("The path 'Assets/Toolkit' was not found");
            }
            BundleMapEditor.UpdateAllBundleMapsIn("Assets/Toolkit");
            AssetDatabase.ExportPackage("Assets/Toolkit", "Toolkit.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
        }

        [MenuItem("Toolkit/Export/All", priority = PRIORITY, validate = true)]
        private static bool ToolkitValidation() {
            return AssetDatabase.IsValidFolder("Assets/Toolkit");
        }

        #endregion

        #region PropertyDrawerLayout Package

        private static string[] GetPropertyDrawerLayoutFiles() {
            return new string[]{
                "Assets/Toolkit/Editor/Toolkit.Editor.asmdef",
                "Assets/Toolkit/Editor/EditorInspectorFix.cs",
                "Assets/Toolkit/Editor/PropertyDrawerLayout.cs",
            };
        }

        [MenuItem("Toolkit/Export/Property Drawer Layout", priority = PRIORITY + 2000)]
        public static void PropertyDrawerLayout() {
            if(!PropertyDrawerLayoutValidation()) {
                throw new System.IO.DirectoryNotFoundException("The path 'Assets/Toolkit' was not found");
            }

            AssetDatabase.ExportPackage(GetPropertyDrawerLayoutFiles(), "PropertyDrawerLayout.unitypackage", ExportPackageOptions.Interactive);
        }

        [MenuItem("Toolkit/Export/Property Drawer Layout", priority = PRIORITY + 2000, validate = true)]
        private static bool PropertyDrawerLayoutValidation() {
            return ToolkitValidation() && AllAssetsExists(GetPropertyDrawerLayoutFiles());
        }

        #endregion

        #region Nested Scriptable Objects

        private static string[] GetNestedScriptableObjects() {
            var allAssets = AssetDatabase.FindAssets("", new string[] { "Assets/Toolkit/NestedScriptableObject" });
            return allAssets
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .AddRange(GetPropertyDrawerLayoutFiles())
                .AddEnumerator(MAIN_ASSEMBLY_DEFINITION)
                .ToArray();
        }

        [MenuItem("Toolkit/Export/Nested Scriptable Objects", priority = PRIORITY + 2001)]
        public static void NestedScriptableObjects() {
            if(!NestedScriptableObjectsValidation()) {
                throw new System.IO.DirectoryNotFoundException("The path 'Assets/Toolkit' was not found");
            }

            AssetDatabase.ExportPackage(GetNestedScriptableObjects(), "NestedScriptableObjects.unitypackage", ExportPackageOptions.Interactive);
        }

        [MenuItem("Toolkit/Export/Nested Scriptable Objects", priority = PRIORITY + 2001, validate = true)]
        private static bool NestedScriptableObjectsValidation() {
            return ToolkitValidation() && AllAssetsExists(GetNestedScriptableObjects());
        }

        #endregion
    }
}
