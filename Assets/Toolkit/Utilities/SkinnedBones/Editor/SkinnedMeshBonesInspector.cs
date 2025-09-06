using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(SkinnedMeshBones))]
    public class SkinnedMeshBonesInspector : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            var smb = target as SkinnedMeshBones;
            var bones = smb.Bones;

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField($"Bones: {bones.Count}", EditorStyles.boldLabel);
                using(new EditorGUI.IndentLevelScope(1)) {
                    for(int i = 0; i < bones.Count; i++) {
                        EditorGUILayout.LabelField($"Bone ({i}) : {bones[i]}");
                    }
                }
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
