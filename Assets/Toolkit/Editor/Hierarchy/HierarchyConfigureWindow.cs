using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit {
    public class HierarchyConfigureWindow : PopupWindowContent {

        private GameObject targetObject;
        private HierarchyItem item;

        private static string[] backgroundOptions = {
            "User Settings",
            "Small",
            "Medium",
            "Full"
        };

        public static void Configure(Vector2 location, GameObject go) {
            PopupWindow.Show(new Rect(location, new Vector2(0, 0)), new HierarchyConfigureWindow(go));
        }

        public HierarchyConfigureWindow(GameObject targetObject) {
            this.targetObject = targetObject;
            this.item = HierarchyItem.AddTo(targetObject);
        }

        public override Vector2 GetWindowSize() {
            return new Vector2(280, 160);
        }

        public override void OnGUI(Rect rect) {
            if(targetObject == null) {
                editorWindow.Close();
                return;
            }

            try {
                GUILayout.BeginArea(rect.Shrink(8));
                using(new EditorGUILayout.HorizontalScope()) {
                    using(new EditorGUI.DisabledScope(true))
                        EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true);
                    if(GUILayout.Button("Clear", GUILayout.Width(50))) {
                        editorWindow.Close();
                        item.Clear();
                        HierarchyItem.RemoveIfEmpty(targetObject);
                        return;
                    }
                }
                // EditorGUILayout.LabelField($"{targetObject.name}");
                using(new EditorGUILayout.VerticalScope("box")) {
                    using(new EditorGUILayout.HorizontalScope()) {
                        using(new EditorGUILayout.VerticalScope()) {
                            GUILayout.Label("Highlight", EditorStylesUtility.BoldLabel);

                            EditorGUI.BeginChangeCheck();
                            var newBackgroundMode = EditorGUILayout.Popup(item.BackgroundMode, backgroundOptions, GUILayout.Width(120));
                            if(EditorGUI.EndChangeCheck()) {
                                item.BackgroundMode = newBackgroundMode;
                            }

                            EditorGUI.BeginChangeCheck();
                            var newColor = EditorGUILayout.ColorField(item.Color, GUILayout.Width(120));
                            if(EditorGUI.EndChangeCheck()) {
                                item.Color = newColor;
                            }
                        }

                        EditorGUI.BeginChangeCheck();
                        var newTexture = EditorGUILayout.ObjectField(item.Icon, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64)) as Texture2D;
                        if(EditorGUI.EndChangeCheck()) {
                            item.Icon = newTexture;
                        }
                    }

                    EditorGUILayout.Space(8);

                    using(new EditorGUILayout.HorizontalScope()) {
                        GUILayout.Label("Label", EditorStylesUtility.BoldLabel);
                        EditorGUI.BeginChangeCheck();
                        var newFontColor = EditorGUILayout.ColorField(item.CustomFontColor, GUILayout.Width(80));
                        if(EditorGUI.EndChangeCheck()) {
                            item.CustomFontColor = newFontColor;
                        }
                    }

                    // EditorGUILayout.LabelField("Label", EditorStylesUtility.BoldLabel);
                    EditorGUI.BeginChangeCheck();
                    var newName = EditorGUILayout.DelayedTextField(item.CustomText);
                    if(EditorGUI.EndChangeCheck()) {
                        item.CustomText = newName;
                    }
                }
            }
            catch {

            }
            finally {
                GUILayout.EndArea();
            }
        }
    }
}
