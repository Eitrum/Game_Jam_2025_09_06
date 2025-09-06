using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit
{
    public static class FolderEditor
    {
        #region Variables

        private static List<CustomInspector> inspectors = new List<CustomInspector>();
        private static List<Button> buttons = new List<Button>();

        private static string[] folders;

        #endregion

        #region Properties

        public static IReadOnlyList<CustomInspector> Inspectors => inspectors;
        public static IReadOnlyList<Button> Buttons => buttons;

        public static string[] Folders {
            get {
                if(folders == null) {
                    List<string> temporary = new List<string>();
                    temporary.Add("Assets");
                    var toRemove = Application.dataPath.Replace("Assets", "");
                    RecursiveFolderFind(Application.dataPath, toRemove.Length, temporary);
                    folders = temporary.ToArray();
                }
                return folders;
            }
        }

        private static void RecursiveFolderFind(string path, int toRemove, List<string> temp) {
            var dirs = System.IO.Directory.GetDirectories(path);
            foreach(var d in dirs) {
                temp.Add(d.Remove(0, toRemove).Replace('\\', '/'));
                RecursiveFolderFind(d, toRemove, temp);
            }

        }

        #endregion

        #region Register Button

        public static void RegisterButton(string name, System.Action<Object> callback)
            => buttons.Add(new Button(name, callback));
        public static void RegisterButton(string name, Color color, System.Action<Object> callback)
            => buttons.Add(new Button(name, color, callback));
        public static void RegisterButton(string name, Color color, System.Action<Object> callback, System.Func<Object, bool> verification)
            => buttons.Add(new Button(name, null, color, callback, verification));
        public static void RegisterButton(string name, Texture2D icon, System.Action<Object> callback)
            => buttons.Add(new Button(name, icon, callback));
        public static void RegisterButton(string name, Texture2D icon, System.Action<Object> callback, System.Func<Object, bool> verification)
            => buttons.Add(new Button(name, icon, callback, verification));
        public static void RegisterButton(string name, Texture2D icon, Color color, System.Action<Object> callback, System.Func<Object, bool> verification)
            => buttons.Add(new Button(name, icon, color, callback, verification));

        #endregion

        public abstract class CustomInspector
        {
            #region Variables

            public bool Enable { get; set; } = true;
            public Texture2D Icon { get; protected set; }
            public string Name { get; protected set; }

            #endregion

            #region Constructor

            public CustomInspector(string name) {
                this.Name = name;
            }

            public CustomInspector(string name, Texture2D icon) {
                this.Name = name;
                this.Icon = icon;
            }

            #endregion

            #region Methods

            public abstract void OnEnable();
            public abstract void OnDisable();
            public abstract void OnInspectorGUI();

            #endregion
        }

        public class Button
        {
            #region Variables

            public Color Color { get; private set; } = Color.gray;
            public Texture2D Icon { get; set; } = null;
            public string Name { get; private set; }
            public float Width { get; private set; }

            private System.Action<Object> callback;
            private System.Func<Object, bool> verification;

            #endregion

            #region Constructor

            public Button(string name, System.Action<Object> callback)
                : this(name, null, Color.gray, callback, null) { }

            public Button(string name, Color color, System.Action<Object> callback)
                : this(name, null, color, callback, null) { }

            public Button(string name, Texture2D icon, System.Action<Object> callback)
                : this(name, icon, Color.gray, callback, null) { }

            public Button(string name, Texture2D icon, System.Action<Object> callback, System.Func<Object, bool> verification)
                : this(name, icon, Color.gray, callback, verification) { }

            public Button(string name, Texture2D icon, Color color, System.Action<Object> callback, System.Func<Object, bool> verification) {
                this.Name = name;
                this.Icon = icon;
                this.Color = color;
                this.callback = callback;
                this.verification = verification;
            }

            #endregion

            #region Draw

            public float CalculateWidth() {
                if(Width <= 1f) {
                    Width = EditorStylesUtility.CenterAlignedBoldLabel.CalcSize(new GUIContent(Name)).x + 6;
                    if(Icon != null)
                        Width += 18;
                }
                return Width;
            }

            public void DrawButton(Object folder) {
                var ev = Event.current;

                var button = GUILayoutUtility.GetRect(Width, 18f);
                var canPress = verification?.Invoke(folder) ?? true;
                if(canPress)
                    EditorGUI.DrawRect(button, Color);
                else
                    EditorGUI.DrawRect(button, Color.MultiplyAlpha(0.6f));
                if(Icon != null) {
                    button.SplitHorizontal(out Rect iconArea, out Rect textArea, 18f / button.width, 0f);
                    GUI.DrawTexture(iconArea.Shrink(1f), Icon, ScaleMode.ScaleToFit);
                    EditorGUI.LabelField(textArea, Name, EditorStylesUtility.CenterAlignedBoldLabel);
                }
                else
                    EditorGUI.LabelField(button, Name, EditorStylesUtility.CenterAlignedBoldLabel);
                if(canPress && ev != null && ev.type == EventType.MouseDown && ev.button == 0 && button.Contains(ev.mousePosition)) {
                    callback?.Invoke(folder);
                }
            }

            #endregion
        }
    }
}
