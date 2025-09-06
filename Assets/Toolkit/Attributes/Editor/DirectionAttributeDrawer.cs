using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Toolkit.Mathematics;

namespace Toolkit
{
    [CustomPropertyDrawer(typeof(DirectionAttribute))]
    public class DirectionAttributeDrawer : PropertyDrawer
    {
        private static void OnSceneGUI(SceneView obj) {
            for(int i = caches.Count - 1; i >= 0; i--) {
                try {
                    var c = caches[i];
                    var prop = c.property;
                    var att = c.attribute;


                    var comp = prop.serializedObject.targetObject as Component;
                    var transform = comp.transform;

                    if(prop.vector3Value.sqrMagnitude <= Mathf.Epsilon)
                        prop.vector3Value = Vector3.up;

                    var mtx = att.RelativeToObject ? transform.localToWorldMatrix : Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);

                    using(new Handles.DrawingScope(mtx)) {
                        var dir = prop.vector3Value;
                        var rot = Quaternion.LookRotation(dir, Vector3.up);

                        using(new Handles.DrawingScope(new Color(0.8f, 0.2f, 0.2f, 1f))) {
                            Handles.DrawWireDisc(Vector3.zero, Vector3.right, 1f);
                            Handles.DrawDottedLine(dir, dir.To_YZ(), 4f);
                        }
                        using(new Handles.DrawingScope(new Color(0.2f, 0.8f, 0.2f, 1f))) {
                            Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
                            Handles.DrawDottedLine(dir, dir.To_XZ(), 4f);
                        }
                        using(new Handles.DrawingScope(new Color(0.4f, 0.6f, 1f, 1f))) {
                            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f);
                            Handles.DrawDottedLine(dir, dir.To_XY(), 4f);
                        }
                        Handles.DrawDottedLine(Vector3.zero, dir, 2f);

                        var line = new Line(prop.vector3Value, prop.vector3Value + rot * Vector3.right);

                        // Debug Line drawing
                        // Handles.DrawDottedLine(line.StartPoint, line.EndPoint, 1f);

                        if(att.Normalize) {
                            var h = Handles.Slider(prop.vector3Value, rot * Vector3.right);
                            prop.vector3Value = Quaternion.Euler(0, -line.GetValueFromPointOnLine(line.GetClosestPointOnLine(h)) * 5f, 0) * prop.vector3Value;
                            var vertical = Handles.Slider(prop.vector3Value, rot * Vector3.up);
                            prop.vector3Value = vertical.normalized;
                        }
                        else {
                            prop.vector3Value = Handles.PositionHandle(prop.vector3Value, Quaternion.identity);
                        }
                    }

                    if(prop.serializedObject.hasModifiedProperties)
                        prop.serializedObject.ApplyModifiedProperties();
                }
#pragma warning disable CS0168
                catch(System.Exception e) {
#pragma warning restore CS0168
                    caches[i].attribute.EditMode = false;
                    caches.RemoveAt(i);
                }
            }


            if(caches.Count == 0) {
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }

        private static List<Cache> caches = new List<Cache>();

        private static Texture d_GizmosToggle;
        private static Texture d_GizmosToggle_On;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            VerifyGizmos();
            var att = attribute as DirectionAttribute;
            switch(property.propertyType) {
                case SerializedPropertyType.Vector3:
                    DrawVector3(position, property, label, att);
                    break;
                default:
                    EditorGUI.HelpBox(position, $"{label.text} - Is not of a supported type for direction value.", MessageType.Error);
                    break;
            }
        }

        private static void VerifyGizmos() {
            if(d_GizmosToggle == null) {
                d_GizmosToggle = EditorGUIUtility.IconContent("d_Record Off").image;
                d_GizmosToggle_On = EditorGUIUtility.IconContent("d_Record On").image;
            }
        }

        private static void DrawVector3(Rect position, SerializedProperty property, GUIContent label, DirectionAttribute attribute) {
            position.SplitHorizontal(out Rect propPosition, out Rect sceneGUIArea, 1f - (32f / position.width), 4f);
            EditorGUI.PropertyField(propPosition, property, label);
            if(property.vector3Value.sqrMagnitude < Mathf.Epsilon) {
                property.vector3Value = Vector3.forward;
            }

            if(attribute.Normalize)
                property.vector3Value = property.vector3Value.normalized;


            sceneGUIArea.width = EditorGUIUtility.singleLineHeight;
            GUI.DrawTexture(sceneGUIArea, attribute.EditMode ? d_GizmosToggle_On : d_GizmosToggle);

            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && sceneGUIArea.Contains(ev.mousePosition)) {
                attribute.EditMode = !attribute.EditMode;
                SceneView.RepaintAll();
                EditorWindow.focusedWindow.Repaint();
                if(attribute.EditMode) {
                    caches.Add(new Cache() {
                        so = property.serializedObject,
                        property = property,
                        attribute = attribute,
                    });

                    SceneView.duringSceneGui -= OnSceneGUI;
                    SceneView.duringSceneGui += OnSceneGUI;
                }
                else {
                    for(int i = caches.Count - 1; i >= 0; i--) {
                        if(caches[i].attribute == attribute) {
                            caches.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public class Cache
        {
            public SerializedObject so;
            public SerializedProperty property;
            public DirectionAttribute attribute;
        }
    }
}
