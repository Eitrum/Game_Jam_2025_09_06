using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Audio
{
    [CustomEditor(typeof(AudioListenerInstance))]
    public class AudioListenerInstanceInspector : Editor
    {
        private SerializedProperty priority;

        private void OnEnable() {
            priority = serializedObject.FindProperty("priority");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(priority);

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            if(Application.isPlaying) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Info:", EditorStylesUtility.BoldLabel);
                    var instances = AudioListenerInstance.Instances;
                    var active = AudioListenerInstance.ActiveInstance;

                    var ev = Event.current;

                    for(int i = 0, length = instances.Count; i < length; i++) {
                        var inst = instances[i];
                        EditorGUILayout.LabelField($"{(inst == target ? "-> " : "")} {inst.name} ({inst.Priority})", (inst == active ? EditorStylesUtility.BoldLabel : EditorStylesUtility.Label));

                        var area = GUILayoutUtility.GetLastRect();
                        if(ev != null && ev.type == EventType.MouseDown && ev.button == 0 && area.Contains(ev.mousePosition)) {
                            Selection.activeObject = inst;
                        }
                    }
                }
            }
        }
    }
}
