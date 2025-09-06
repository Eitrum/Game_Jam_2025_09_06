using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(RagdollDynamic))]
    public class DynamicRagdollInspector : Editor
    {
        private SerializedProperty profile;

        private void OnEnable() {
            profile = serializedObject.FindProperty("profile");
        }


        public override void OnInspectorGUI() {
            serializedObject.Update();

            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(profile);
                if(profile.objectReferenceValue == null) {
                    if(GUILayout.Button("new", GUILayout.Width(50f))) {
                        var t = (RagdollDynamic)target;
                        var bones = t.GetComponentInChildren<Toolkit.Utility.BoneReferences>();
                        if(!bones) {
                            Debug.LogError("No bone references found!");
                            return;
                        }
                        var prof = CreateInstance<RagdollProfile>();
                        prof.CreateFrom(bones);
                        prof.name = t.name;
                        var path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + t.name + ".asset");
                        AssetDatabase.CreateAsset(prof, path);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        profile.objectReferenceValue = prof;
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
