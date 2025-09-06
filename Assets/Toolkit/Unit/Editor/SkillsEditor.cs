using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Toolkit.Unit
{
    [CustomPropertyDrawer(typeof(Skill))]
    public class SkillEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return ExperienceDrawerUtility.GetExperienceDrawerHeight(true) + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.SplitVertical(out Rect experienceArea, out Rect temporaryLevelArea, 1f - (EditorGUIUtility.singleLineHeight / position.height));
            GUI.Box(temporaryLevelArea, "");
            var experience = property.FindPropertyRelative("experience");
            var path = (property.propertyPath);
            var split = path.Split('.');
            ISkill skill = FindISkill(split, property.serializedObject.targetObject);

            EditorGUI.PropertyField(experienceArea, experience, label);
            if(skill != null)
                skill.TemporaryLevels = EditorGUI.DelayedIntField(temporaryLevelArea, "Temporary Levels", skill.TemporaryLevels);
            else
                EditorGUI.LabelField(temporaryLevelArea, "Temporary Levels Not Found!");
        }

        private ISkill FindISkill(string[] path, UnityEngine.Object obj) {
            object o = obj;
            for(int i = 0; i < path.Length; i++) {
                var t = o.GetType();
                var field = t.GetField(path[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(field == null) {
                    return null;
                }
                o = field.GetValue(o);
            }
            return o as ISkill;
        }
    }
}
