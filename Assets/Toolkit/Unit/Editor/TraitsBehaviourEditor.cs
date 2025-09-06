using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Unit {
    [CustomEditor(typeof(TraitsBehaviour))]
    public class TraitsBehaviourEditor : Editor {
        public override void OnInspectorGUI() {
            if(targets == null || targets.Length > 1) {
                EditorGUILayout.LabelField("Traits Behaviour does not support multi select editing");
            }
            var length = TraitType.None.GetLength();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            for(int i = 1; i < length; i++) {
                var type = (TraitType)i;
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(TraitsUtility.GetNegativeName(type), GUILayout.Width(120f));
                prop.NextVisible(false);
                var newValue = 0f;
                EditorGUI.BeginChangeCheck();
                newValue = EditorGUILayout.Slider(prop.floatValue, TraitsUtility.MIN, TraitsUtility.MAX);
                if(EditorGUI.EndChangeCheck())
                    prop.floatValue = EditorStylesUtility.IsHoldingAlt ? Mathf.RoundToInt(newValue) : newValue;
                EditorGUILayout.LabelField(TraitsUtility.GetPositiveName(type), GUILayout.Width(120f));
                EditorGUILayout.EndHorizontal();
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
