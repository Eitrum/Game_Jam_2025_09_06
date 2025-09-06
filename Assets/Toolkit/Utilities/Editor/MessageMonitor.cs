using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    /// <summary>
    /// Message Monitor to track who is subscribed to the message system and how many calls been excecuted.
    /// </summary>
    public class MessageMonitor : EditorWindow {

        private const string MENU_PATH = "Toolkit/Monitor/Messages";
        private const int MENU_PRIORITY = 100001;

        [MenuItem(MENU_PATH, priority = MENU_PRIORITY)]
        public static void Open() {
            var w = GetWindow<MessageMonitor>("Messages Monitor", true);
            w.Show();
        }

        private Vector2 scroll = default;
        private Vector2 scroll2 = default;
        private Type selectedType = default;
        private IMessageTargetData[] targets = { };

        private void OnGUI() {
            var area = new Rect(Vector2.zero, position.size);
            area.ShrinkRef(5f);
            var activeTypes = Message.messageTypesActive;
            if(selectedType != null) {
                area.SplitHorizontal(out Rect left, out Rect right, 200f / area.width, 5f);
                DrawArray(left, activeTypes);
                DrawSelected(right, selectedType);
            }
            else {
                DrawArray(area, activeTypes);
            }
            if(focusedWindow == this && !UnityEditor.EditorApplication.isCompiling) {
                Repaint();
            }
        }

        void DrawArray(Rect area, IList<Type> types) {
            GUI.Box(area, "Message Types");
            GUILayout.BeginArea(area);
            GUILayout.Space(16);
            scroll = GUILayout.BeginScrollView(scroll);
            for(int i = 0; i < types.Count; i++) {
                EditorGUILayout.LabelField($"{i}. {types[i].GetGenericArguments()[0].FullName} x{SubscriberCount(types[i])}");
                var last = GUILayoutUtility.GetLastRect();
                if(Event.current.type == EventType.MouseDown && last.Contains(Event.current.mousePosition)) {
                    selectedType = types[i];
                    targets = Targets(selectedType);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawSelected(Rect area, Type selectedType) {
            GUI.Box(area, "Selected");
            GUILayout.BeginArea(area);
            GUILayout.Space(16);
            scroll2 = GUILayout.BeginScrollView(scroll2);
            EditorGUILayout.LabelField($"{selectedType.GetGenericArguments()[0].FullName}");
            EditorGUILayout.LabelField($"Total published messages x({PublishedMessages(selectedType)})");
            EditorGUILayout.LabelField($"Subscribed Entities x({SubscriberCount(selectedType)})");
            EditorGUILayout.Space();
            for(int i = 0; i < targets.Length; i++) {
                var t = targets[i];
                if(t.Nullable.IsNull) {
                    EditorGUILayout.LabelField($"Object {i} in the list is currently null!");
                    continue;
                }
                if(t.Nullable is UnityEngine.Object obj && obj != null) {
                    EditorGUILayout.LabelField($"{obj.name}+{t.MethodInfo.Name} with {t.Calls} calls since subscribed.");
                }
                else {
                    EditorGUILayout.LabelField($"unknown+{t.MethodInfo.Name} with {t.Calls} calls since subscribed.");
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static int PublishedMessages(Type type) {
            return (int)type.GetField("publishedMessages", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }

        private static int SubscriberCount(Type type) {
            return (int)type.GetProperty("SubscriberCount").GetValue(null);
        }

        private static IMessageTargetData[] Targets(Type type) {
            return (IMessageTargetData[])type.GetProperty("Targets").GetValue(null);
        }

    }
}
