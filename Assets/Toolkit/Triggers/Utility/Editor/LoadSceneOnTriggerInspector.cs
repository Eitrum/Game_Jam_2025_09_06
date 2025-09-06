using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Trigger
{
    [CustomEditor(typeof(LoadSceneOnTrigger))]
    public class LoadSceneOnTriggerInspector : Editor
    {
        #region Variables
        
        private SerializedProperty optionalSources;
        private SerializedProperty sceneToLoad;

        private SerializedProperty unloadMode;
        private SerializedProperty unloadDelay;
        private SerializedProperty unloadCurrent;
        private SerializedProperty otherScenesToUnload;

        #endregion

        #region Init

        private void OnEnable() {
            optionalSources = serializedObject.FindProperty("optionalSources");
            sceneToLoad = serializedObject.FindProperty("sceneToLoad");
            unloadMode = serializedObject.FindProperty("unloadMode");
            unloadDelay = serializedObject.FindProperty("unloadDelay");
            unloadCurrent = serializedObject.FindProperty("unloadCurrent");
            otherScenesToUnload = serializedObject.FindProperty("otherScenesToUnload");
        }

        #endregion

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                TriggerEditorUtility.CheckForTriggerWithOptionalSources(target, optionalSources);
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(sceneToLoad);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(unloadMode);
                    if(unloadMode.intValue > 0) {
                        using(new EditorGUI.IndentLevelScope(1)) {
                            EditorGUILayout.PropertyField(unloadCurrent);
                            EditorGUILayout.PropertyField(unloadDelay);
                            EditorGUILayout.PropertyField(otherScenesToUnload);
                        }
                    }
                }
            }
        }
    }
}
