using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem {
    [CustomEditor(typeof(PanelManager))]
    public class PanelManagerInspector : Editor {

        private GameObject panelPrefab;

        public override void OnInspectorGUI() {
            DefaultInspectorUtility.DrawInspectorWithUserSettings(serializedObject);

            if(!Application.isPlaying)
                return;

            var t = (PanelManager)target;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Debug: ", EditorStylesUtility.BoldLabel);
                panelPrefab = EditorGUILayout.ObjectField("Panel Prefab", panelPrefab, typeof(GameObject), false) as GameObject;
                if(GUILayout.Button("Add", GUILayout.Width(80))) {
                    t.Add(panelPrefab);
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("CloseById", GUILayout.Width(80))) {
                        var id = panelPrefab.GetComponent<Panel>().PanelId;
                        t.CloseById(id);
                    }
                    if(GUILayout.Button("CloseByIdInstant", GUILayout.Width(140))) {
                        var id = panelPrefab.GetComponent<Panel>().PanelId;
                        t.CloseById(id, true);
                    }
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("CloseAll", GUILayout.Width(80))) {
                        t.CloseAll();
                    }
                    if(GUILayout.Button("CloseAllInstant", GUILayout.Width(140))) {
                        t.CloseAll(true);
                    }
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("CloseAll & Add New", GUILayout.Width(200))) {
                        t.CloseAll();
                        t.Add(panelPrefab);
                    }
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("Close First", GUILayout.Width(160))) {
                        var p = t.GetBottom();
                        t.Close(p, false);
                    }
                    if(GUILayout.Button("Close Last", GUILayout.Width(160))) {
                        var p = t.GetTop();
                        t.Close(p, false);
                    }
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("SetLoading (true)", GUILayout.Width(180))) {
                        t.SetLoading(true);
                    }
                    if(GUILayout.Button("SetLoading (true)", GUILayout.Width(180))) {
                        t.SetLoading(false);
                    }
                }

                EditorGUILayout.LabelField("-----------------");
                var panels = t.Panels;
                foreach(var p in panels)
                    if(p != null)
                        EditorGUILayout.LabelField($"{p.name}");
            }

        }
    }
}
