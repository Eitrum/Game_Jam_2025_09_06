using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Toolkit.InputSystem {
    public class CursorSystemEditorWindow : SimpleEditorWindow {

        protected override string MENU_ITEM_PATH => $"Toolkit/Monitor/Cursor System";

        private static List<(int type, object o, int order, INullable nullable)> cachedList = new List<(int type, object o, int order, INullable nullable)>();
        private static List<(CursorLockModeSetting setting, INullable nullable)> lockMode = new List<(CursorLockModeSetting setting, INullable nullable)>();
        private static List<(bool? visible, INullable nullable)> visible = new List<(bool? visible, INullable nullable)>();
        private static List<(CursorObject texture, INullable nullable)> textures = new List<(CursorObject texture, INullable nullable)>();

        protected override void DrawLayout() {
            RebuildCaches();

            using(new EditorGUILayout.HorizontalScope()) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
                    foreach(var lm in lockMode) {
                        if(lm.nullable is UnityEngine.Object obj)
                            GUILayout.Label(obj.name);
                        else if(lm.nullable is UnityObjectNullable uon && uon.obj != null)
                            GUILayout.Label(uon.obj.name);
                        else if(lm.nullable != null)
                            GUILayout.Label(lm.nullable.ToString());
                        else
                            GUILayout.Label("null");
                    }
                }
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Lock Mode", EditorStyles.boldLabel);
                    foreach(var lm in lockMode)
                        GUILayout.Label(lm.setting.ToStringFast());
                }
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Visible", EditorStyles.boldLabel);
                    foreach(var v in visible) {
                        if(v.visible.HasValue)
                            GUILayout.Label(v.visible.Value ? "Show" : "Hide");
                        else
                            GUILayout.Label(" --- ");
                    }
                }
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                    foreach(var t in textures) {
                        if(t.texture == null)
                            GUILayout.Label(" --- ");
                        else
                            using(new EditorGUI.DisabledScope(true))
                                EditorGUILayout.ObjectField(t.texture.Texture, typeof(Texture), false);
                    }
                }
            }
        }

        private void RebuildCaches() {
            cachedList.Clear();
            cachedList.AddRange(CursorSystem.GetAll());
            cachedList.Sort((a, b) => a.order.CompareTo(b.order));

            lockMode.Clear();
            visible.Clear();
            textures.Clear();

            INullable previousNullable = null;
            lockMode.Add((CursorLockModeSetting.None, null));
            visible.Add((null, null));
            textures.Add((null, null));

            foreach(var item in cachedList) {
                if(previousNullable != item.nullable) {
                    lockMode.Add((CursorLockModeSetting.None, item.nullable));
                    visible.Add((null, item.nullable));
                    textures.Add((null, item.nullable));
                }
                previousNullable = item.nullable;

                int index = lockMode.Count - 1;
                switch(item.type) {
                    case 0: {
                            var t = lockMode[index];
                            t.setting = (CursorLockModeSetting)item.o;
                            lockMode[index] = t;
                        }
                        break;
                    case 1: {
                            var t = visible[index];
                            t.visible = (bool)item.o;
                            visible[index] = t;
                        }
                        break;
                    case 2: {
                            var t = textures[index];
                            t.texture = (CursorObject)item.o;
                            textures[index] = t;
                        }
                        break;
                }
            }
        }
    }
}
