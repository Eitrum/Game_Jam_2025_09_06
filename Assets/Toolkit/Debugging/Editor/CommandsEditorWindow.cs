using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Debugging {
    public class CommandsEditorWindow : EditorWindow {

        #region Menu Item

        [MenuItem("Toolkit/Editor/Commands")]
        public static void OpenCommandsWindow() {
            var w = GetWindow<CommandsEditorWindow>();
            w.Show();
        }

        #endregion

        #region Styles

        private class Styles {
            public static GUIStyle ValidStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState(){textColor = ColorTable.LawnGreen } };
            public static GUIStyle PossibleStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState(){textColor = ColorTable.DimGray } };
            public static GUIStyle ErrorStyle = new GUIStyle(EditorStyles.label) { normal = new GUIStyleState(){textColor = ColorTable.IndianRed } };
        }

        #endregion

        #region Variables

        private Vector2 scrollPosition;
        private bool drawAllCommands;
        public CommandHelper CommandHelper { get; private set; } = new CommandHelper(Privilege.Debug);
        private List<string> baseCommands = new List<string>();

        #endregion


        private void OnGUI() {
            var size = position.size;
            try {
                GUILayout.BeginArea(new Rect(Vector2.zero, size));
                using(var scrollscope = new EditorGUILayout.ScrollViewScope(scrollPosition)) {
                    drawAllCommands = EditorGUILayout.ToggleLeft("Draw All Commands", drawAllCommands);
                    if(drawAllCommands) {
                        var allCommands = Commands.AllCommands;
                        foreach(var c in allCommands) {
                            EditorGUILayout.LabelField(c.FullCommandVisual);
                            using(new EditorGUI.IndentLevelScope(1)) {
                                foreach(var p in c.parameters)
                                    EditorGUILayout.LabelField($"{p.Name} - {p.TypeName}");
                            }
                            EditorGUILayout.Space();
                        }
                        var lineArea = GUILayoutUtility.GetRect(1, 4000, 4, 4);
                        EditorGUI.DrawRect(lineArea, Color.grey);
                    }


                    scrollPosition = scrollscope.scrollPosition;
                    using(new EditorGUILayout.HorizontalScope("box")) {
                        GUILayout.Label("Command:", GUILayout.Width(80));
                        if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
                            if(GUI.GetNameOfFocusedControl() == "commandsinput") {
                                CommandHelper.Run();
                                Event.current.Use();
                                GUI.FocusControl("commandsinput");
                                CommandHelper.Current = "";

                                EditorApplication.delayCall += () => {
                                    GUI.FocusControl("commandsinput");
                                    EditorGUIUtility.editingTextField = true;
                                    Repaint();
                                };
                            }
                        }
                        GUI.SetNextControlName("commandsinput");
                        CommandHelper.Current = EditorGUILayout.TextField(CommandHelper.Current);
                        CommandHelper.Privilege = (Privilege)EditorGUILayout.EnumPopup(CommandHelper.Privilege, GUILayout.Width(80));

                        GUI.SetNextControlName("commandsrun");
                        if(GUILayout.Button("Run", GUILayout.Width(80))) {
                            CommandHelper.Run();
                        }
                    }
                    EditorGUILayout.Space();
                    if(!string.IsNullOrEmpty(CommandHelper.Current) && CommandHelper.PossibleCommands.Count > 0) {
                        DrawPossibleCommands();
                    }
                    else {
                        DrawAllBaseCommands();
                    }
                }
            }
            finally {
                GUILayout.EndArea();
            }
        }

        private void DrawPossibleCommands() {
            EditorGUILayout.LabelField("Possible Commands:", EditorStylesUtility.GrayItalicLabel);

            if(CommandHelper.IsValid) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    var possibleCommands = CommandHelper.PossibleCommands;
                    var validCommands = CommandHelper.ValidCommands;
                    int validIndex = 0;
                    for(int i = 0, len = possibleCommands.Count; i < len; i++) {
                        bool isValid = false;
                        if(validIndex < validCommands.Count) {
                            if(validCommands[validIndex] == possibleCommands[i]) {
                                validIndex++;
                                isValid = true;
                            }
                        }

                        EditorGUILayout.LabelField(possibleCommands[i].FullCommandVisual, isValid ? Styles.ValidStyle : Styles.PossibleStyle);
                    }
                }
            }
            else {
                using(new EditorGUILayout.VerticalScope("box")) {
                    var possibleCommands = CommandHelper.PossibleCommands;
                    for(int i = 0, len = possibleCommands.Count; i < len; i++) {
                        EditorGUILayout.LabelField(possibleCommands[i].FullCommandVisual, Styles.ErrorStyle);
                    }
                }
            }
        }

        private void DrawAllBaseCommands() {
            EditorGUILayout.LabelField("Base Commands:", EditorStylesUtility.GrayItalicLabel);
            using(new EditorGUILayout.VerticalScope("box")) {
                Commands.FindBaseCommands(baseCommands, CommandHelper.Privilege);
                foreach(var b in baseCommands) {
                    EditorGUILayout.LabelField(b, Styles.PossibleStyle);
                }
            }
        }
    }
}
