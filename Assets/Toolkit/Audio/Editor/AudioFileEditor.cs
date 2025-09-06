using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Audio
{
    [CustomPropertyDrawer(typeof(AudioFile))]
    public class AudioFileEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            var refProperty = property.FindPropertyRelative("reference");
            label.text = $"{label.text} - ({GetExtraName(refProperty.objectReferenceValue)})";
            var newObj = EditorGUI.ObjectField(position, label, refProperty.objectReferenceValue, typeof(UnityEngine.Object), true);

            if(EditorGUI.EndChangeCheck()) {
                AudioFile af = default;
                if(newObj is GameObject go) {
                    af = AudioFile.FindInChildren(go);
                }
                else if(newObj is Component com) {
                    af = AudioFile.FindInChildren(com);
                }
                else if(newObj is UnityEngine.Object o) {
                    af = AudioFile.Find(o);
                }

                if(af.Type != AudioFileType.None) {
                    refProperty.objectReferenceValue = af.Reference;
                }
                else {
                    refProperty.objectReferenceValue = null;
                }
            }
        }

        public static string GetExtraName(UnityEngine.Object o) {
            if(o is AudioClip)
                return "Audio Clip";
            if(o is IAudioVariation)
                return "IAudioVariation";
            if(o is AudioPlayer)
                return "Audio Player";
            return "null";
        }
    }
}
