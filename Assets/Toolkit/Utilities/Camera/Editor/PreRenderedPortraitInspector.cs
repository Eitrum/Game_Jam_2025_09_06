using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Utility
{
    [CustomEditor(typeof(PreRenderedPortrait))]
    public class PreRenderedPortraitInspector : Editor
    {
        private SerializedProperty texture;

        private void OnEnable() {
            texture = serializedObject.FindProperty("texture");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(texture);

            if(texture.objectReferenceValue is Texture tex) {
                var area = GUILayoutUtility.GetRect(tex.width, tex.height);
                var w = area.width;
                var minX = Mathf.Min(tex.width, area.width);
                var minY = Mathf.Min(tex.height, area.height);
                area.size = new Vector2(minX, minY);
                GUI.DrawTexture(area, tex);
            }

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
