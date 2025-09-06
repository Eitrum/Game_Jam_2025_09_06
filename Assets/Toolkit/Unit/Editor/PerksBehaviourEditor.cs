using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit {
    [CustomEditor(typeof(PerksBehaviour))]
    public class PerksBehaviourEditor : Editor {

        private SerializedProperty initalPerks;

        private void OnEnable() {
            initalPerks = serializedObject.FindProperty("initalPerks");
            initalPerks.isExpanded = false;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            using(new Toolkit.ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUI.DisabledScope(Application.isPlaying))
                    EditorGUILayout.PropertyField(initalPerks);
            }
            var perks = target as PerksBehaviour;

            if(perks.Owner == null) {
                EditorGUILayout.HelpBox("No attached unit!\nThis might cause some errors in runtime.", MessageType.Error);
            }
            else {
                EditorGUILayout.LabelField("Attached Unit: " + perks.Owner.Name);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Active Perks", EditorStylesUtility.BoldLabel);
                var currentPerks = perks.Active;
                if(currentPerks.Count == 0) {
                    EditorGUILayout.LabelField("No perks", EditorStylesUtility.ItalicLabel);
                }
                else {
                    foreach(var (_, perk) in currentPerks) {
                        EditorGUILayout.LabelField($"{perk.Metadata.Name} ({perk.Metadata.PerkId}, {perk.Metadata.Type}, {perk.Metadata.Category})");
                    }
                }
                if(Application.isPlaying) {
                    var obj = EditorGUILayout.ObjectField("Add perk", null, typeof(IPerkBuilder), false);
                    if(obj != null && obj is IPerkBuilder newPeek) {
                        perks.AddPerk(newPeek.Create(perks.Owner));
                    }
                }
            }
        }
    }
}
