using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit {
    internal class UpdateTreeViewWindow : EditorWindow {

        #region Styles

        private class Styles {
            public static readonly GUIStyle DefaultTypeHeader = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.BoldAndItalic };
            public static readonly GUIStyle TypeHeader = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            public static readonly GUIStyle DefaultTypeLabel = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };

            public static readonly GUILayoutOption TimeWidth = GUILayout.Width(80f);
            public static readonly GUIStyle CenterAligned = EditorStylesUtility.CenterAlignedLabel;
        }

        #endregion

        #region Variables

        private int frames = 32;
        private static GUIContent Title;
        private Vector2 scroll;
        private bool drawCurrentInternal = false;

        private Dictionary<Type, bool> foldout = new Dictionary<Type, bool>();
        private Dictionary<Type, Data> defaultTypes = new Dictionary<Type, Data>();
        private Dictionary<string, Data> profilerToData = new Dictionary<string, Data>();

        #endregion

        #region Properties

        #endregion

        #region MenuItem

        [MenuItem("Window/Toolkit/Update Tree View")]
        public static void OpenWindow() {
            var w = GetWindow<UpdateTreeViewWindow>();
            w.titleContent = Title;
            w.Show();
        }

        #endregion

        #region Init

        private void OnEnable() {
            Title = new GUIContent("Update Tree View", EditorGUIUtility.IconContent("d_UnityEditor.SceneHierarchyWindow").image);
            titleContent = Title;

            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
            var subsystems = playerLoop.subSystemList;
            for(int i = 0, length = subsystems.Length; i < length; i++)
                RecursiveAddDefault(string.Empty, null, subsystems[i]);
        }

        private void RecursiveAddDefault(string parentName, Data parent, UnityEngine.LowLevel.PlayerLoopSystem playerLoop) {
            var profilerKey = string.IsNullOrEmpty(parentName) ? $"{playerLoop.type.Name}" : $"{parentName}.{playerLoop.type.Name}";
            var data = new Data(playerLoop.type, parent, frames);
            defaultTypes.Add(playerLoop.type, data);
            profilerToData.Add(profilerKey, data);

            if(playerLoop.subSystemList != null)
                for(int i = 0, length = playerLoop.subSystemList.Length; i < length; i++)
                    RecursiveAddDefault(profilerKey, data, playerLoop.subSystemList[i]);
        }

        #endregion

        #region Draw

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);

            using(new GUILayout.AreaScope(area)) {
                using(new EditorGUILayout.HorizontalScope("box")) {
                    using(new EditorGUI.DisabledScope(!drawCurrentInternal))
                        if(GUILayout.Button("Tookit", GUILayout.Width(140))) {
                            drawCurrentInternal = false;
                        }
                    using(new EditorGUI.DisabledScope(drawCurrentInternal))
                        if(GUILayout.Button("Unity", GUILayout.Width(140))) {
                            drawCurrentInternal = true;
                        }
                }
                using(new EditorGUILayout.HorizontalScope("box")) {
                    EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
                }
                var line = GUILayoutUtility.GetRect(area.width, 2f);
                EditorGUI.DrawRect(line, new Color(0.3f, 0.3f, 0.3f, 0.5f));

                using(var s = new GUILayout.ScrollViewScope(scroll)) {
                    scroll = s.scrollPosition;

                    if(drawCurrentInternal) {
                        var subsystems = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop().subSystemList;
                        foreach(var ss in subsystems)
                            Draw(-1, ss);
                    }
                    else {
                        var subsystems = PlayerLoopUtilty.GetCurrentUpdateSystem().SubSystems;
                        foreach(var ss in subsystems)
                            Draw(-1, ss);
                    }
                }
            }

            Repaint();
        }

        private void Draw(int index, PlayerLoopUtilty.Node system) {
            using(var scope = new EditorGUILayout.VerticalScope()) {
                if((index % 2 == 0)) {
                    EditorGUI.DrawRect(scope.rect, new Color(0.3f, 0.3f, 0.3f, 0.3f));
                }
                var ev = Event.current;
                if(scope.rect.Contains(ev.mousePosition)) {
                    EditorGUI.DrawRect(scope.rect, new Color(0.1f, 0.4f, 0.4f, 0.3f));
                }
                EditorGUI.indentLevel++;

                var isDefault = system.Mode != PlayerLoopUtilty.Mode.Managed;
                var subsystems = system.SubSystems;

                if(subsystems != null && subsystems.Count > 0) {
                    bool show = false;
                    using(new EditorGUILayout.HorizontalScope()) {
                        show = foldout[system.Type] = EditorGUILayout.Foldout(foldout.TryGetValue(system.Type, out bool val) ? val : false, $"{system.Name}", true, isDefault ? Styles.DefaultTypeHeader : Styles.TypeHeader);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(system.Enabled ? "Enabled" : "Disabled", GUILayout.Width(60));
                        GUILayout.Label(system.Mode.ToStringFast(), GUILayout.Width(130));
                    }
                    if(show) {
                        if(index < 0) {
                            int childIndex = 0;
                            foreach(var ss in subsystems)
                                Draw(childIndex++, ss);
                        }
                        else {
                            foreach(var ss in subsystems)
                                Draw(index, ss);
                        }
                    }
                }
                else {
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField(system.Name, isDefault ? Styles.DefaultTypeLabel : EditorStyles.label, GUILayout.Width(400));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(system.Enabled ? "Enabled" : "Disabled", GUILayout.Width(60));
                        GUILayout.Label(system.Mode.ToStringFast(), GUILayout.Width(130));
                    }
                }

                EditorGUI.indentLevel--;
            }
        }



        private void Draw(int index, UnityEngine.LowLevel.PlayerLoopSystem system) {
            using(var scope = new EditorGUILayout.VerticalScope()) {
                if((index % 2 == 0)) {
                    EditorGUI.DrawRect(scope.rect, new Color(0.3f, 0.3f, 0.3f, 0.3f));
                }
                var ev = Event.current;
                if(scope.rect.Contains(ev.mousePosition)) {
                    EditorGUI.DrawRect(scope.rect, new Color(0.1f, 0.4f, 0.4f, 0.3f));
                }
                EditorGUI.indentLevel++;

                var t = system.type;
                var isDefault = defaultTypes.TryGetValue(t, out var data);
                var subsystems = system.subSystemList;

                if(subsystems != null && subsystems.Length > 0) {
                    bool show = false;
                    using(new EditorGUILayout.HorizontalScope()) {
                        show = foldout[t] = EditorGUILayout.Foldout(foldout.TryGetValue(t, out bool val) ? val : false, $"{t.Name}", true, isDefault ? Styles.DefaultTypeHeader : Styles.TypeHeader);
                    }
                    if(show) {
                        if(index < 0) {
                            int childIndex = 0;
                            foreach(var ss in subsystems)
                                Draw(childIndex++, ss);
                        }
                        else {
                            foreach(var ss in subsystems)
                                Draw(index, ss);
                        }
                    }
                }
                else {
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField(t.Name, isDefault ? Styles.DefaultTypeLabel : EditorStyles.label, GUILayout.ExpandWidth(true));
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        #endregion

        public class Data {
            #region Variables

            private Type type;
            private CircularBuffer<float> time;
            private Data parent;
            private List<Data> children = new List<Data>();

            #endregion

            #region Properties

            public Type Type => type;

            public bool IsWritten => time.WrittenIndex > 0 || children.Any(x => x.IsWritten);

            public float Min => time.Min() + children.Sum(x => x.Min);
            public float Max => time.Max() + children.Sum(x => x.Max);
            public float Average => time.Average() + children.Sum(x => x.Average);

            public Data Parent => parent;
            public IReadOnlyList<Data> Children => children;

            #endregion

            #region Constructor

            public Data() {
                time = new CircularBuffer<float>(8);
            }

            public Data(Type type, Data parent, int size) {
                this.type = type;
                this.parent = parent;
                if(parent != null)
                    parent.children.Add(this);
                this.time = new CircularBuffer<float>(size);
            }

            #endregion

            #region Methods

            public void Update(float ms) {
                time.Write(ms);
                //UnityEngine.Debug.Log($"{type.Name} {ms:0.00}ms");
            }

            public void SetSize(int size) {
                time.Resize(size);
            }

            #endregion
        }
    }
}
