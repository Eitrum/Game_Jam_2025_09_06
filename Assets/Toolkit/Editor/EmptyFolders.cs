using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Toolkit
{
    public static class EmptyFolders
    {
        private const string TAG = ColorTable.RichTextTags.CYAN + "[Empty Folders]</color> - ";
        private const string DELETE_ALL_PATH = "Toolkit/Editor/Empty Folders/Delete All";
        private const string ADD_EMPTY_FILE_PATH = "Toolkit/Editor/Empty Folders/Add Empty Files";
        private const string CLEAR_EMPTY_FILE_PATH = "Toolkit/Editor/Empty Folders/Clear All Empty Files";
        private const string DISABLE_WARNING_PATH = "Toolkit/Editor/Empty Folders/Disable Warnings";
        private const string LOG_EMPTY_FOLDERS = "Toolkit/Editor/Empty Folders/Print";

        public static bool IsWarningEnabled {
            set => Menu.SetChecked(DISABLE_WARNING_PATH, !value);
            get => !Menu.GetChecked(DISABLE_WARNING_PATH);
        }

        [MenuItem(DELETE_ALL_PATH)]
        public static void DeleteAll() {
            var paths = GetAllEmptyFoldersRelativePath;
            paths.Foreach(x => AssetDatabase.DeleteAsset(x));
            AssetDatabase.Refresh();
            Debug.Log(TAG + "deleted all empty folders at:\n" + paths.CombineToString());
        }

        [MenuItem(ADD_EMPTY_FILE_PATH)]
        public static void AddEmptyFiles() {
            var paths = GetAllEmptyFoldersRelativePath;
            paths.Foreach(x => System.IO.File.WriteAllText(System.IO.Path.Combine(x, "empty.empty"), ""));
            AssetDatabase.Refresh();
            Debug.Log(TAG + "'empty.empty' files added to all empty folders at:\n" + paths.CombineToString());
        }

        [MenuItem(CLEAR_EMPTY_FILE_PATH)]
        public static void ClearEmptyFiles() {
            AssetDatabase.FindAssets("t:DefaultAsset empty")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Where(x => x.EndsWith("empty.empty"))
                .Foreach(x => AssetDatabase.DeleteAsset(x));
            Debug.Log(TAG + "All 'empty.empty' files deleted!");
        }

        [MenuItem(DISABLE_WARNING_PATH)]
        public static void ToggleWarnings() {
            IsWarningEnabled = !IsWarningEnabled;
        }

        [MenuItem(LOG_EMPTY_FOLDERS)]
        public static void Print() {
            var paths = GetAllEmptyFoldersRelativePath;
            if(paths.Count > 0) {
                Debug.LogWarning(TAG + $"{paths.Count} empty folder{(paths.Count > 1 ? "s found at:\n" : " found at: ")}{paths.CombineToString(paths.Count > 1)}");
            }
            else {
                Debug.Log(TAG + $"No empty folders found in project.");
            }
        }

        [InitializeOnLoadMethod]
        private static void Warning() {
            if(!IsWarningEnabled) {
                return;
            }
            System.Threading.Thread thread = new System.Threading.Thread(ThreadedWarning);
            thread.Start();
        }

        private static void ThreadedWarning() {
            var paths = GetAllEmptyFoldersRelativePath;
            if(paths.Count > 0) {
                Debug.LogWarning(TAG + $"{paths.Count} empty folder{(paths.Count > 1 ? "s found at:\n" : " found at: ")}{paths.CombineToString(paths.Count > 1)}");
            }
        }

        public static IReadOnlyList<string> GetAllEmptyFoldersAbsolutePath {
            get {
                List<string> paths = new List<string>();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets"));
                FindEmptyDirectoriesRecursive(di, paths);
                return paths;
            }
        }

        public static IReadOnlyList<string> GetAllEmptyFoldersRelativePath {
            get {
                List<string> paths = new List<string>();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets"));
                FindEmptyDirectoriesRecursive(di, paths);
                var projectPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets");
                for(int i = 0; i < paths.Count; i++) {
                    paths[i] = paths[i].Replace(projectPath, "Assets");
                }
                return paths;
            }
        }

        private static int FindEmptyDirectoriesRecursive(System.IO.DirectoryInfo dir, List<string> paths) {
            var dirs = dir.GetDirectories();
            var files = dir.GetFiles().Where(x => !x.Name.EndsWith(".meta"));
            if(dirs.Count() == 0 && files.Count() == 0) {
                paths.Add(dir.FullName);
                return 0;
            }
            else {
                int filesFound = files.Count();
                foreach(var d in dirs) {
                    filesFound += FindEmptyDirectoriesRecursive(d, paths);
                }
                if(filesFound == 0) {
                    paths.Add(dir.FullName);
                }
                return filesFound;
            }
        }

    }
}
