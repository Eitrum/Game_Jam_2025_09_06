using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Toolkit.VersionControl {
    public class Git {
        #region Consts

        private const string GIT_ENABLED_PATH = "Toolkit.GitEnabled";
        private const string GIT_WARNING_DISABLE_PATH = "Project Settings -> Toolkit -> Editor -> Git";
        private const string TAG = ColorTable.RichTextTags.NAVY + "[Git]</color> - ";
        private const string ADD_GIT_IGNORE_PATH = "Toolkit/Editor/Version Control/Git/Create .gitignore";
        private const string ADD_GIT_ATTRIBUTES_PATH = "Toolkit/Editor/Version Control/Git/Create .gitattributes";

        #endregion

        #region Verification

        public static bool IsGitWarningsEnabled {
            set => EditorPrefs.SetBool(GIT_ENABLED_PATH, !value);
            get => !EditorPrefs.GetBool(GIT_ENABLED_PATH);
        }

        public static bool IsGitInstalled {
            get {
                var result = CommandUtility.Run("git --version", true);
                return result[1].StartsWith("git version");
            }
        }

        public static bool IsProjectUsingGit => System.IO.Directory.Exists(".git");
        public static bool HasGitIgnore => System.IO.File.Exists(".gitignore");
        public static bool HasGitAttributes => System.IO.File.Exists(".gitattributes");

        [MenuItem("Toolkit/Editor/Version Control/Git/Verify")]
        public static void VerifyGit() {
            var usingGit = IsProjectUsingGit;
            if (!usingGit) {
                Debug.Log(TAG + "Project is not using git");
                return;
            }
            var gitInstalled = IsGitInstalled;
            var hasIgnore = HasGitIgnore;
            var hasAttributes = HasGitAttributes;

            if (!gitInstalled)
                Debug.LogError(TAG + "Git is not installed for this current user.\n\nTo disable these messages: " + GIT_ENABLED_PATH);
            if (!hasIgnore)
                Debug.LogError(TAG + "Git project seems to be missing a .gitignore file, to add a default .gitignore file to the project: " + ADD_GIT_IGNORE_PATH + "\n\nTo disable these messages: " + GIT_ENABLED_PATH);
            if (!hasAttributes)
                Debug.LogError(TAG + "Git project seems to missing a .gitattributes file, to add a default .gitattributes file to the project: " + ADD_GIT_ATTRIBUTES_PATH + "\n\nTo disable these messages: " + GIT_ENABLED_PATH);

        }

        [InitializeOnLoadMethod]
        private static void InitializeEditor() {
            ToolkitProjectSettings.RegisterEditor("Git", 0, (s) => {
                EditorGUI.BeginChangeCheck();
                var gitwarningenabled = EditorGUILayout.ToggleLeft("Enable Git Warnings", IsGitWarningsEnabled);
                if (EditorGUI.EndChangeCheck()) {
                    IsGitWarningsEnabled = gitwarningenabled;
                }
                if (gitwarningenabled) {
                    if (!IsGitInstalled) {
                        EditorGUILayout.HelpBox("Git not installed", MessageType.Error);
                    }
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
                        if (HasGitIgnore)
                            EditorGUILayout.HelpBox("Git ignore found", MessageType.None);
                        else {
                            EditorGUILayout.HelpBox("Git ignore not found", MessageType.Error);
                            if (GUILayout.Button("Create", GUILayout.Width(80))) {
                                CreateGitIgnore();
                            }
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
                        if (HasGitAttributes)
                            EditorGUILayout.HelpBox("Git attributes found", MessageType.None);
                        else {
                            EditorGUILayout.HelpBox("Git attributes (GIT-LFS) not found", MessageType.Warning);
                            if (GUILayout.Button("Create", GUILayout.Width(80))) {
                                CreateGitIgnore();
                            }
                        }
                    }
                }
            });
        }

        [InitializeOnLoadMethod]
        private static void VerifyGitAtStartup() {
            if (!IsGitWarningsEnabled) {
                return;
            }
            var usingGit = IsProjectUsingGit;
            if (!usingGit)
                return;
            var gitInstalled = IsGitInstalled;
            var hasIgnore = HasGitIgnore;
            var hasAttributes = HasGitAttributes;

            if (!gitInstalled)
                Debug.LogError(TAG + "Git is not installed for this current user.\n\nTo disable these messages: " + GIT_WARNING_DISABLE_PATH);
            if (!hasIgnore)
                Debug.LogError(TAG + "Git project is missing a .gitignore file, to add a default .gitignore file to the project: " + ADD_GIT_IGNORE_PATH + "\n\nTo disable these messages: " + GIT_WARNING_DISABLE_PATH);
            if (!hasAttributes)
                Debug.LogWarning(TAG + "Git project is missing a .gitattributes file, to add a default .gitattributes file to the project: " + ADD_GIT_ATTRIBUTES_PATH + "\n\nTo disable these messages: " + GIT_WARNING_DISABLE_PATH);
        }

        #endregion

        #region Default files

        [MenuItem(ADD_GIT_IGNORE_PATH)]
        public static void CreateGitIgnore() {
            if (HasGitIgnore) {
                Debug.LogWarning(TAG + "Already has .gitignore file located at root in project.");
                return;
            }
            var asset = AssetDatabase.FindAssets("t:DefaultAsset default").FirstOrDefault(x => AssetDatabase.GUIDToAssetPath(x).EndsWith(".gitignore"));
            if (string.IsNullOrEmpty(asset)) {
                Debug.LogError(TAG + "Could not find the 'default.gitignore' file in project");
                return;
            }
            var path = AssetDatabase.GUIDToAssetPath(asset);
            System.IO.File.Copy(path, ".gitignore");
            Debug.Log(TAG + ".gitignore created!");
        }

        [MenuItem(ADD_GIT_ATTRIBUTES_PATH)]
        public static void CreateGitAttributes() {
            if (HasGitAttributes) {
                Debug.LogWarning(TAG + "Already has .gitattributes file located at root in project.");
                return;
            }
            var asset = AssetDatabase.FindAssets("t:DefaultAsset default").FirstOrDefault(x => AssetDatabase.GUIDToAssetPath(x).EndsWith(".gitattributes"));
            if (string.IsNullOrEmpty(asset)) {
                Debug.LogError(TAG + "Could not find the 'default.gitattributes' file in project");
                return;
            }
            var path = AssetDatabase.GUIDToAssetPath(asset);
            System.IO.File.Copy(path, ".gitattributes");
            Debug.Log(TAG + ".gitattributes created!");
        }

        #endregion
    }

    [DefaultAssetEditor(".gitignore")]
    [DefaultAssetEditor(".gitattributes")]
    public class GitDefaultFileViewer : Editor {
        private Vector2 scrollPosition;
        private string text;

        private void OnEnable() {
            text = System.IO.File.ReadAllText(AssetDatabase.GetAssetPath(target));
        }

        public override void OnInspectorGUI() {
            using (new EditorGUILayout.ScrollViewScope(scrollPosition)) {
                if (text == null) {
                    return;
                }
                GUILayout.Label(text);
            }
        }
    }
}
