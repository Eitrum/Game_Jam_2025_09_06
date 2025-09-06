using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    [CustomEditor(typeof(SkillsBehaviour))]
    public class SkillsBehaviourEditor : Editor
    {
        public override void OnInspectorGUI() {
            if(targets == null || targets.Length > 1) {
                EditorGUILayout.LabelField("Traits Behaviour does not support multi select editing");
            }

            var skills = (SkillsBehaviour)target;
            var length = SkillType.None.GetLength();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            prop.NextVisible(false);

            var ev = Event.current;

            for(int i = 1; i < length; i++) {
                var type = (SkillType)i;
                var skill = skills.GetSkill(type);
                if(prop.isExpanded) {
                    EditorGUILayout.PropertyField(prop);
                    if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Pad(0, 0, 0, EditorGUIUtility.singleLineHeight * 4).Contains(ev.mousePosition)) {
                        prop.isExpanded = false;
                        ev.Use();
                    }
                    GUILayout.Space(8f);
                }
                else {
                    using(new EditorGUILayout.HorizontalScope("box")) {
                        EditorGUILayout.LabelField(prop.displayName, EditorStyles.boldLabel, GUILayout.Width(200));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField($"Level ({skill.TotalLevel}/{skill.Experience.Level})", GUILayout.Width(80));
                        var experienceBar = GUILayoutUtility.GetRect(200, EditorGUIUtility.singleLineHeight);
                        ExperienceDrawerUtility.DrawExperienceBar(experienceBar, skill.Experience.LevelingPercentage);
                    }
                    if(ev.type == EventType.MouseDown && ev.button == 0 && GUILayoutUtility.GetLastRect().Contains(ev.mousePosition)) {
                        prop.isExpanded = true;
                        ev.Use();
                    }
                }

                prop.NextVisible(false);
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
