using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    [CustomPropertyDrawer(typeof(ExperienceClass))]
    public class ExperienceClassEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return ExperienceDrawerUtility.GetExperienceDrawerHeight(!string.IsNullOrEmpty(label.text));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var split = property.propertyPath.Split('.');
            if(split.Length == 1) {
                var fieldInfo = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var iExperience = fieldInfo.GetValue(property.serializedObject.targetObject) as IExperience;
                ExperienceDrawerUtility.Draw(position, label, iExperience);
            }
            else {
                ExperienceDrawerUtility.Draw(position, label, FindIExperience(split, property.serializedObject.targetObject));
            }
        }

        private IExperience FindIExperience(string[] path, UnityEngine.Object obj) {
            object o = obj;
            for(int i = 0; i < path.Length; i++) {
                var t = o.GetType();
                var field = t.GetField(path[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(field == null) {
                    return null;
                }
                o = field.GetValue(o);
            }
            return o as IExperience;
        }
    }
}
