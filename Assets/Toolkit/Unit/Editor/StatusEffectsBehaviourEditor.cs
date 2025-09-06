using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    [CustomEditor(typeof(StatusEffectsBehaviour))]
    public class StatusEffectsBehaviourEditor : Editor
    {

        private StatusEffectType type = StatusEffectType.None;

        public override void OnInspectorGUI() {
            var statuses = (StatusEffectsBehaviour)target;
            var list = statuses.Effects;

            var prop = serializedObject.FindProperty("updateMode");

            var newValue = (UpdateModeMask)EditorGUILayout.EnumFlagsField("Update Mode", (UpdateModeMask)prop.enumValueIndex);

            if(Application.isPlaying) {
                if(prop.enumValueIndex != (int)newValue) {
                    UpdateSystem.Unsubscribe(target as StatusEffectsBehaviour, (UpdateModeMask)prop.enumValueIndex);
                    prop.enumValueIndex = (int)newValue;
                    serializedObject.ApplyModifiedProperties();
                    UpdateSystem.Subscribe(target as StatusEffectsBehaviour, (UpdateModeMask)newValue);
                }
            }
            else {
                if(serializedObject.hasModifiedProperties) {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.BeginVertical("box");

            for(int i = 0, length = list.Count; i < length; i++) {
                GUILayout.Label($"{list[i].Name} - Duration ({list[i].Duration})");
            }

            if(list.Count == 0) {
                GUILayout.Label("None Active");
            }

            GUILayout.EndVertical();

            if(Application.isPlaying) {
                GUILayout.BeginHorizontal("box");

                type = (StatusEffectType)EditorGUILayout.EnumPopup("Add Status Effect", type);
                if(GUILayout.Button("Add", GUILayout.Width(60f))) {
                    statuses.Add(new BasicStatusEffect(type, 2f));
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}
