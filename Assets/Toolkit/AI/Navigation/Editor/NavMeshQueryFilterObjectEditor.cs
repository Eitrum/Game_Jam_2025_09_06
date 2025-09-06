using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.AI.Navigation
{
    [CustomEditor(typeof(NavMeshQueryFilterObject))]
    public class NavMeshQueryFilterObjectEditor : Editor
    {
        private SerializedProperty idProperty;
        private SerializedProperty areaMaskProperty;
        private SerializedProperty multiplierProperty;

        private void OnEnable() {
            idProperty = serializedObject.FindProperty("agentTypeId");
            areaMaskProperty = serializedObject.FindProperty("areaMask");
            multiplierProperty = serializedObject.FindProperty("areaCostMultiplier");
        }

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Query Filter", EditorStyles.boldLabel);
                var names = NavMeshGeneratorSettingsEditor.RuleTypes.ToArray();
                var newId = EditorGUILayout.Popup("Generation", idProperty.intValue, names);
                if(newId < 0 || newId >= names.Length)
                    newId = 0;
                idProperty.intValue = newId;

                areaMaskProperty.intValue = EditorGUILayout.MaskField("Area Mask", areaMaskProperty.intValue, UnityEngine.AI.NavMesh.GetAreaNames());
            }

            EditorGUILayout.Space(4);
            NavigationAreaCostEditor.DrawCustomLayout(multiplierProperty);
            EditorGUILayout.HelpBox("Note: The area cost multiplier fields is what being saved and not the individual cost.", MessageType.Warning);
            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
