using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;

namespace Toolkit.CodeAnalysis {
    public static class CodeStatistics {
        #region Variables

        private const string TAG = "<color=#00FFFF>[Code Statistics]</color> - ";
        private const string LOCAL_STORAGE = "Library/Toolkit/CodeAnalytics/CodeSize.data";
        private const string LOCAL_STORAGE_SETTINGS = "Library/Toolkit/CodeAnalytics/CodeStatistics.cfg";

        private const string ASSET_PREFS_PATH = "Toolkit.CodeStatistics_Enabled";
        private const string ASSET_INCLUDE_PREFS_PATH = "Toolkit.CodeStatistics_Path";
        private const string ASSET_EXLUDE_PREFS_PATH = "Toolkit.CodeStatistics_Exlude";

        private static Container container;
        private static Settings settings = new Settings();
        private static UnityEditorInternal.ReorderableList excludeDrawer;

        public static bool HasLoaded => container != null;
        public static IReadOnlyList<Storage> Storages => container?.storages ?? null;

        [System.Serializable]
        private class Settings {
            #region Variables

            [SerializeField] private bool enabled = false;
            [SerializeField] private string path = "Assets";
            [SerializeField] private List<string> exclude = new List<string>();

            #endregion

            #region Properties

            public bool Enabled {
                get => enabled;
                set {
                    if(enabled != value) {
                        enabled = value;
                        Save();
                    }
                }
            }

            public string Path {
                get => path;
                set {
                    if(path != value) {
                        path = value;
                        Save();
                    }
                }
            }

            public List<string> Exclude => exclude;

            #endregion

            #region Restore

            public void RestorePath() {
                path = "Assets";
                Save();
            }

            public void RestoreAll() {
                enabled = false;
                path = "Assets";
                exclude.Clear();
                Save();
            }

            #endregion

            #region Save / Load

            public void Save() {
                try {
                    var json = JsonUtility.ToJson(this, true);
                    if(Toolkit.IO.IOUtility.PathExistsOrCreate(LOCAL_STORAGE_SETTINGS))
                        System.IO.File.WriteAllText(LOCAL_STORAGE_SETTINGS, json);
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                }
            }

            public void Load() {
                try {
                    if(System.IO.File.Exists(LOCAL_STORAGE_SETTINGS)) {
                        var json = System.IO.File.ReadAllText(LOCAL_STORAGE_SETTINGS);
                        if(!string.IsNullOrEmpty(json)) {
                            JsonUtility.FromJsonOverwrite(json, this);
                        }
                    }
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                }
            }

            #endregion
        }

        #endregion

        #region Properties

        public static bool Enabled {
            get => settings.Enabled;
            set => settings.Enabled = value;
        }

        public static string Path {
            get => settings.Path;
            set {
                if(Path != value) {
                    if(!value.StartsWith("Assets")) {
                        settings.RestorePath();
                        return;
                    }
                    settings.Path = value;
                }
            }
        }

        public static string Exclude {
            get => EditorPrefs.GetString(ASSET_EXLUDE_PREFS_PATH, "Assets/ThirdPartyAssets");
            set => settings.Exclude.Add(value);
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Load() {
            settings.Load();
            if(Enabled) {
                Code code = Code.GenerateFromPath(Path, settings.Exclude);
                code.Print();
            }
            excludeDrawer = new UnityEditorInternal.ReorderableList(settings.Exclude, typeof(string));
            excludeDrawer.drawHeaderCallback += OnExcludeHeader;
            excludeDrawer.drawElementCallback += OnExcludeElement;
            excludeDrawer.onAddCallback += OnExcludeAdd;
            excludeDrawer.onRemoveCallback += OnExcludeRemove;

            ToolkitProjectSettings.RegisterEditor("Code Statistics", 100, ProjectSettingsGUI);

            FolderEditor.RegisterButton("Code Statistics", PrintFolderCodeStatistics);
        }

        private static void PrintFolderCodeStatistics(UnityEngine.Object @object) {
            if(@object == null)
                return;
            var path = AssetDatabase.GetAssetPath(@object);
            Code.GenerateFromPath(path)?.Print(path);
        }

        #endregion

        #region Exclude Drawer

        private static void OnExcludeAdd(ReorderableList list) {
            settings.Exclude.Add("-----");
        }

        private static void OnExcludeRemove(ReorderableList list) {
            if(list.index >= 0)
                settings.Exclude.RemoveAt(list.index);
            else
                settings.Exclude.RemoveLast();
        }

        private static void OnExcludeHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Exclude", EditorStylesUtility.BoldLabel);
        }

        private static void OnExcludeElement(Rect rect, int index, bool isActive, bool isFocused) {
            settings.Exclude[index] = EditorGUI.DelayedTextField(rect, settings.Exclude[index]);
        }

        #endregion

        private static void ProjectSettingsGUI(string obj) {
            Enabled = EditorGUILayout.Toggle("Print", Enabled);
            if(Enabled) {
                using(new EditorGUI.IndentLevelScope(1)) {
                    Path = EditorGUILayout.DelayedTextField("Path", Path);
                    EditorGUI.BeginChangeCheck();
                    excludeDrawer.DoLayoutList();
                    if(EditorGUI.EndChangeCheck()) {
                        settings.Save();
                    }
                    //Exclude = EditorGUILayout.DelayedTextField("Exclude", Exclude);
                }
            }
        }

        [MenuItem("Toolkit/Editor/Print Code Statistics", priority = 1)]
        public static void Print() {
            Code code = Code.GenerateFromPath(Path, settings.Exclude);
            code.Print();
        }

        #region Write to File

        private static void AsyncReadAndWrite(Code code) {
            container = new Container();

            // Read current data
            if(System.IO.File.Exists(LOCAL_STORAGE)) {
                var json = System.IO.File.ReadAllText(LOCAL_STORAGE);
                JsonUtility.FromJsonOverwrite(json, container);
            }
            // verify entries
            var storages = container.storages;
            if(storages.Count == 0) {
                storages.Add(new Storage(code));
                Debug.Log(TAG + "Adding first entry to storage");
            }
            else {
                var last = storages.Last();
                if(last.Date != TKDateTime.Today) {
                    var oldDate = last.Date;
                    var today = TKDateTime.Today;
                    do {
                        oldDate = oldDate.AddTicks(System.TimeSpan.TicksPerDay);
                        storages.Add(new Storage(code, oldDate));
                        Debug.Log(TAG + "Adding new entry to storage from " + oldDate);
                    } while(oldDate != today);
                }
                else {
                    last.code = code;
                }
            }
            var output = JsonUtility.ToJson(container);
            if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(LOCAL_STORAGE))) {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LOCAL_STORAGE));
            }
            System.IO.File.WriteAllText(LOCAL_STORAGE, output);
        }

        #endregion

        #region Drawing

        private static Material mat;
        private static GUIStyle statisticsAlignRight;

        public static void DrawStatistics(Rect area) {
            if(Event.current.type != EventType.Repaint) {
                return;
            }

            if(statisticsAlignRight == null) {
                statisticsAlignRight = new GUIStyle(EditorStyles.label);
                statisticsAlignRight.alignment = TextAnchor.MiddleRight;
                statisticsAlignRight.normal.textColor = Color.black;
            }

            DrawGraphWithData(area);
        }

        private static void DrawGraphWithData(Rect area, float lineLabelSize = 80f, float dateLabelSize = 80f) {
            EditorGUI.DrawRect(area, Color.white);
            DrawGraph(area.Pad(lineLabelSize, 4f, 4f, dateLabelSize));
            DrawLineData(area.Pad(0f, area.width - (lineLabelSize - 6), 4f, lineLabelSize));
            DrawDateData(area.Pad(lineLabelSize, 4f, area.height - (dateLabelSize - 6), 4f));
        }

        private static void DrawLineData(Rect area) {
            var storages = Storages;
            var range = new MinMaxInt(storages.Min(x => x.code.linesOfCode), storages.Max(x => x.code.linesOfCode));
            var diff = (int)((range.max - range.min) * 0.1f);
            range.min -= diff;
            range.max += diff;
            var minRounded = (range.min / 1000) * 1000;
            var delta = range.max - range.min;
            int scale = delta / 10;

            for(int i = 1; i < 10; i++) {
                int value = range.min + scale * i;
                var height = 1f - range.InverseEvaluate(value);
                GUI.Label(new Rect(area.x, area.y + height * area.height - 8f, area.width, 16f), value.ToString(), statisticsAlignRight);
            }
        }

        private static void DrawDateData(Rect area) {
            var storages = Storages;
            var count = storages.Count;
            var sizePerPos = area.width / (count - 1f);
            var skip = 1;
            if(sizePerPos < 3f)
                skip = 14;
            if(sizePerPos < 15f)
                skip = 7;
            for(int i = count - 1; i >= 0; i -= skip) {
                DrawDateLabel(new Vector2(area.x + sizePerPos * i - 10f, area.y), storages[i].Date);
            }
        }

        private static void DrawDateLabel(Vector2 point, TKDateTime date) {
            GUIUtility.RotateAroundPivot(-70f, point);
            GUI.Label(new Rect(point.x - 120f, point.y, 120f, 16f), $"{date.Day} / {date.Month} / {date.Year - 2000}", statisticsAlignRight);
            GUIUtility.RotateAroundPivot(70f, point);
        }

        private static void DrawGraph(Rect area) {
            EditorGUI.DrawRect(area, Color.black);
            area.ShrinkRef(2f);
            EditorGUI.DrawRect(area, Color.white);

            if(!HasLoaded) {
                return;
            }
            if(mat == null) {
                var shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
            }

            var storages = Storages;
            var range = new MinMaxInt(storages.Min(x => x.code.linesOfCode), storages.Max(x => x.code.linesOfCode));

            var diff = (int)((range.max - range.min) * 0.1f);
            range.min -= diff;
            range.max += diff;

            var minRounded = (range.min / 1000) * 1000;
            var delta = range.max - range.min;
            int scale = delta / 10;

            int count = storages.Count;
            float widthPerStorage = area.width / (count - 1f);
            MinMax heightRange = new MinMax(0, area.height);

            GUI.BeginClip(area);
            GL.PushMatrix();
            try {
                GL.Clear(true, false, Color.black);
                mat.SetPass(0);

                Color uLightGray = new Color(0.94f, 0.94f, 0.94f);
                Color lightGray = new Color(0.8f, 0.8f, 0.8f);

                var skip = 1;
                if(widthPerStorage < 3f)
                    skip = 14;
                if(widthPerStorage < 15f)
                    skip = 7;
                for(int i = count; i >= 0; i -= skip) {

                    DrawLine(new Vector2(i * widthPerStorage, heightRange.min), new Vector2(i * widthPerStorage, heightRange.max), uLightGray);
                }

                for(int i = 1; i < 10; i++) {
                    int value = range.min + scale * i;
                    var height = 1f - range.InverseEvaluate(value);
                    DrawLine(new Vector2(0, heightRange.Evaluate(height)), new Vector2(area.width, heightRange.Evaluate(height)), lightGray);
                }

                for(int i = 1; i < count; i++) {
                    var index0 = storages[i - 1];
                    var index1 = storages[i];

                    var height0 = 1f - range.InverseEvaluate(index0.code.linesOfCode);
                    var height1 = 1f - range.InverseEvaluate(index1.code.linesOfCode);
                    DrawLine(new Vector2((i - 1f) * widthPerStorage, heightRange.Evaluate(height0)), new Vector2(i * widthPerStorage, heightRange.Evaluate(height1)), Color.black);
                }
            }
            finally {
                GL.PopMatrix();
                GUI.EndClip();
            }
        }

        private static void DrawLine(Vector2 start, Vector2 end, Color color) {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.End();
        }

        #endregion

        #region Classes

        [Serializable]
        public class Code {
            public int scripts = 0;
            public int linesOfCode = 0;
            public int commentLines = 0;
            public int emptyLines = 0;
            public int codeLines = 0;

            public void Print() {
                var result = TAG;
                result += string.Format("Lines of code\t({0:N0})", linesOfCode);
                result += string.Format("\n - Scripts\t\t\t\t\t({0:N0})", scripts);
                result += string.Format("\n - Lines of actual code\t\t\t({0:N0})", codeLines);
                result += string.Format("\n - Lines of comments\t\t\t({0:N0})", commentLines);
                result += string.Format("\n - Empty Lines\t\t\t\t({0:N0})", emptyLines);
                result += string.Format("\n - Percentage Of Lines is code\t({0:P})", (double)codeLines / (double)linesOfCode);
                Debug.Log(result);
            }

            public void Print(string path) {
                var result = TAG + path;
                result += string.Format(" - Lines of code\t({0:N0})", linesOfCode);
                result += string.Format("\n - Scripts\t\t\t\t\t({0:N0})", scripts);
                result += string.Format("\n - Lines of actual code\t\t\t({0:N0})", codeLines);
                result += string.Format("\n - Lines of comments\t\t\t({0:N0})", commentLines);
                result += string.Format("\n - Empty Lines\t\t\t\t({0:N0})", emptyLines);
                result += string.Format("\n - Percentage Of Lines is code\t({0:P})", (double)codeLines / (double)linesOfCode);
                Debug.Log(result);
            }

            public void Fill(string path) {
                scripts++;
                var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var lines = ta.text.Split('\n');
                var length = lines.Length;
                linesOfCode += length;
                for(int i = 0; i < length; i++) {
                    var line = lines[i].Trim();
                    if(line.StartsWith("#") || line.StartsWith("//"))
                        commentLines++;
                    else if(string.IsNullOrEmpty(line))
                        emptyLines++;
                    else
                        codeLines++;
                }
            }

            public static Code GenerateFromPath(string path) {
                Code code = new Code();
                var assets = AssetDatabase.FindAssets("t:MonoScript", new string[] { path }).Select(x => AssetDatabase.GUIDToAssetPath(x));
                foreach(var assetPath in assets) {
                    code.Fill(assetPath);
                }
                return code;
            }

            public static Code GenerateFromPath(string path, string exclude) {
                Code code = new Code();
                var assets = AssetDatabase.FindAssets("t:MonoScript", new string[] { path }).Select(x => AssetDatabase.GUIDToAssetPath(x));
                foreach(var assetPath in assets) {
                    if(assetPath.StartsWith(exclude))
                        continue;
                    code.Fill(assetPath);
                }
                return code;
            }

            public static Code GenerateFromPath(string path, IReadOnlyList<string> exclude) {
                switch(exclude.Count) {
                    case 0: return GenerateFromPath(path);
                    case 1: return GenerateFromPath(path, exclude[0]);
                }
                Code code = new Code();
                var assets = AssetDatabase.FindAssets("t:MonoScript", new string[] { path }).Select(x => AssetDatabase.GUIDToAssetPath(x));
                foreach(var assetPath in assets) {
                    if(exclude.Any(x => assetPath.StartsWith(x)))
                        continue;
                    code.Fill(assetPath);
                }
                return code;
            }
        }

        [Serializable] // Used for json to work properly
        private class Container {
            public List<Storage> storages = new List<Storage>();
        }

        [Serializable]
        public class Storage {
            public TKDateTime Date {
                get => new TKDateTime(date);
                set => date = value.Ticks;
            }
            [SerializeField] private long date = 0;
            public Code code;

            public Storage() {
                Date = TKDateTime.Today;
                code = new Code();
            }

            public Storage(Code code) {
                Date = TKDateTime.Today;
                this.code = code;
            }

            public Storage(Code code, TKDateTime date) {
                Date = date;
                this.code = code;
            }
        }

        #endregion
    }
}
