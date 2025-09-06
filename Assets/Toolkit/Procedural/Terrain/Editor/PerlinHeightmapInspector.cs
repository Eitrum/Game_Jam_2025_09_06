using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CustomEditor(typeof(PerlinHeightmap))]
    public class PerlinHeightmapInspector : Editor
    {
        private SerializedProperty layers;
        private ReorderableList list;

        private void OnEnable() {
            layers = serializedObject.FindProperty("layers");
            list = new ReorderableList(serializedObject, layers);
            list.elementHeight = 124f;
            list.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "Layers", EditorStyles.boldLabel);
            list.drawElementCallback += (r, i, o, o2) => EditorGUI.PropertyField(r.Pad(0, 0, 2, 2), layers.GetArrayElementAtIndex(i));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            prop.NextVisible(false);

            do {
                if(prop.isArray)
                    break;
                EditorGUILayout.PropertyField(prop, true);
            } while(prop.NextVisible(false));

            EditorGUILayout.Space();
            list.DoLayoutList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
