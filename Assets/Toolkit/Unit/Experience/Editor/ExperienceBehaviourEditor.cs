using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Unit
{
    [CustomEditor(typeof(ExperienceBehaviour))]
    public class ExperienceBehaviourEditor : Editor
    {
        public override bool RequiresConstantRepaint() {
            return true;
        }

        public override void OnInspectorGUI() {
            var tableProperty = serializedObject.FindProperty("experienceTable");
            var exp = target as IExperience;
            ExperienceDrawerUtility.DrawLayout(exp);
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(tableProperty);
            if(!Application.isPlaying && serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
