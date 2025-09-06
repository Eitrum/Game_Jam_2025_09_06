using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Toolkit {
    internal class ButtonAttributeDrawer {

        #region Variables

        private bool isStatic;
        private GUIContent content;
        public ButtonAttribute attribute;
        public MethodInfo method;
        private object[] invokeParams;

        public class Styles {
            public static GUIStyle Button = new GUIStyle(GUI.skin.button);
        }

        #endregion

        #region Constructor

        public ButtonAttributeDrawer(ButtonAttribute attribute, MethodInfo method) {
            isStatic = method.IsStatic;
            this.content = string.IsNullOrEmpty(attribute.Name) ? new GUIContent(attribute.Name = method.Name.SplitPascalCase()) : new GUIContent(attribute.Name);
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
            if(invokeParams.Length == 0) {
                DrawButton(obj);
                return;
            }

            using(new EditorGUILayout.HorizontalScope()) {
                DrawButton(obj);
            }
        }

        private void DrawButton(SerializedObject obj) {
            var setting = attribute.Setting;
            var preMode = GUI.enabled;
            try {
                switch(setting) {
                    case EditorGUIMode.RuntimeOnly:
                        if(!Application.isPlaying)
                            GUI.enabled = false;
                        break;
                    case EditorGUIMode.EditorOnly:
                        if(Application.isPlaying)
                            GUI.enabled = false;
                        break;
                }
                var cont = GUI.enabled ? content : EditorGUIUtility.TrTempContent($"{content.text} ({setting.ToStringFast()})");
                if(GUILayout.Button(cont, Styles.Button, GUILayout.Width(Styles.Button.CalcSize(cont).x + 8f))) {
                    if(method.IsStatic) {
                        method?.Invoke(null, invokeParams);
                    }
                    else {
                        foreach(var tobj in obj.targetObjects)
                            method?.Invoke(tobj, invokeParams);
                    }
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
            finally {
                GUI.enabled = preMode;
            }
        }

        #endregion
    }

    public class ButtonDrawer {

        private static Dictionary<Type, List<ButtonAttributeDrawer>> cached = new Dictionary<Type, List<ButtonAttributeDrawer>>();
        private List<ButtonAttributeDrawer> drawers;

        public ButtonDrawer(SerializedObject serializedObject) {
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
                EditorGUILayout.LabelField("Methods", EditorStylesUtility.BoldLabel);
                foreach(var drawer in drawers)
                    drawer.Draw(serializedObject);
            }
        }

        private static List<ButtonAttributeDrawer> CreateAndCache(SerializedObject serializedObject) {
            var t = serializedObject.targetObject.GetType();
            var drawers = new List<ButtonAttributeDrawer>();
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach(var m in methods) {
                var att = m.GetCustomAttributes<ButtonAttribute>();
                if(att.Count() == 0) continue;

                drawers.AddRange(att.Select(x => new ButtonAttributeDrawer(x, m)));
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
