using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolkit {
    public class DebugViewAttributeDrawer {

        #region Variables

        private static Stopwatch sw = new Stopwatch();
        private bool isStatic;
        private GUIContent content;
        public DebugViewAttribute attribute;
        public MethodInfo method;
        private object[] invokeParams;
        private int selectedIndex = 0;

        public class Styles {
            public static GUIStyle Label = new GUIStyle(EditorStyles.label){ stretchHeight = true, fixedHeight = 0, wordWrap = true };
        }

        #endregion

        #region Constructor

        public DebugViewAttributeDrawer(DebugViewAttribute attribute, MethodInfo method) {
            isStatic = method.IsStatic;
            this.content = string.IsNullOrEmpty(attribute.Header) ? new GUIContent(method.Name) : new GUIContent(attribute.Header);
            if(isStatic)
                this.content.text = $"*{this.content.text}";
            this.attribute = attribute;
            this.method = method;
            var parameters = method.GetParameters();
            invokeParams = parameters
                .Select(x => Activator.CreateInstance(x.ParameterType))
                .ToArray();
        }

        #endregion

        #region Draw

        public void Draw(SerializedObject obj) {
            var targets = obj.targetObjects;
            if(attribute.Foldout && targets.Length > 1) {
                using(new EditorGUILayout.HorizontalScope()) {
                    attribute.Foldout = EditorGUILayout.Foldout(attribute.Foldout, content, true);
                    EditorGUI.BeginChangeCheck();
                    var tselected = EditorGUILayout.IntSlider(selectedIndex, 0, targets.Length-1);
                    if(EditorGUI.EndChangeCheck()) {
                        selectedIndex = tselected;
                    }
                }
            }
            else {
                attribute.Foldout = EditorGUILayout.Foldout(attribute.Foldout, content, true);
            }
            if(!attribute.Foldout)
                return;
            EditorGUI.indentLevel++;
            var sel = Mathf.Clamp(selectedIndex, 0, targets.Length);
            var target = targets[sel];
            var output = (string)method?.Invoke(target, invokeParams);

            if(attribute.Truncate && output.Length > 1024) {
                var cut = output.IndexOf('\n', 1023, output.Length - 1023);
                if(cut > 0)
                    output = output.Remove(cut) + "\n<...>";
            }
            EditorGUILayout.LabelField(output, Styles.Label);
            EditorGUI.indentLevel--;
        }

        #endregion
    }

    public class DebugViewDrawer {

        private static Dictionary<Type, List<DebugViewAttributeDrawer>> cached = new Dictionary<Type, List<DebugViewAttributeDrawer>>();
        private List<DebugViewAttributeDrawer> drawers =new List<DebugViewAttributeDrawer>();

        public DebugViewDrawer(SerializedObject serializedObject) {
            var t = serializedObject.targetObject.GetType();
            if(cached.TryGetValue(t, out var list)) {
                drawers = list;
                return;
            }
            drawers = CreateAndCache(serializedObject);
        }

        public void DrawLayout(SerializedObject serializedObject) {
            if(drawers.Count == 0)
                return;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Debug", EditorStylesUtility.BoldLabel);
                foreach(var drawer in drawers)
                    drawer.Draw(serializedObject);
            }
        }

        private static List<DebugViewAttributeDrawer> CreateAndCache(SerializedObject serializedObject) {
            var t = serializedObject.targetObject.GetType();
            var drawers = new List<DebugViewAttributeDrawer>();
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach(var m in methods) {
                var att = m.GetCustomAttributes<DebugViewAttribute>();
                if(att.Count() == 0) continue;

                drawers.AddRange(att.Select(x => new DebugViewAttributeDrawer(x, m)));
            }
            cached.Add(t, drawers);
            return drawers;
        }

        public static void DrawLayoutFromCache(SerializedObject serializedObject) {
            if(serializedObject == null)
                return;
            if(serializedObject.targetObject == null)
                return;
            var t = serializedObject.targetObject.GetType();
            if(!cached.TryGetValue(t, out var drawers))
                drawers = CreateAndCache(serializedObject);

            if(drawers.Count == 0)
                return;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Methods", EditorStylesUtility.BoldLabel);
                foreach(var drawer in drawers)
                    drawer.Draw(serializedObject);
            }
        }
    }
}
