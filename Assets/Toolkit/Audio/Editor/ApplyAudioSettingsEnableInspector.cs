using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Audio
{
    [CustomEditor(typeof(ApplyAudioSettingsEnable))]
    public class ApplyAudioSettingsEnableInspector : Editor
    {
        #region Variables

        private SerializedProperty source;
        private SerializedProperty preset;

        private static AudioSourceSettingsPreset[] presets;
        private static string[] presetNames;

        #endregion

        #region Init

        private void OnEnable() {
            source = serializedObject.FindProperty("source");
            preset = serializedObject.FindProperty("preset");

            presets = AssetDatabaseUtility.LoadAssets<AudioSourceSettingsPreset>();
            presetNames = presets.Select(x => x.name).Insert(0, "none").ToArray();
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            serializedObject.Update();

            using(new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(source);
                if(GUILayout.Button("get", GUILayout.Width(80)) && serializedObject.targetObject is Component comp) {
                    var targetSource = comp.GetComponent<AudioSource>();
                    if(targetSource == null && comp.gameObject.scene.IsValid()) {
                        targetSource = comp.gameObject.AddComponent<AudioSource>();
                    }
                    source.objectReferenceValue = targetSource;
                }
            }

            var area = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(area, null, preset);
            EditorGUI.BeginChangeCheck();

            var obj = preset.objectReferenceValue;
            int index = 0;
            for(int i = 0; i < presets.Length; i++) {
                if(presets[i] == obj) {
                    index = i + 1;
                    break;
                }
            }

            index = EditorGUI.Popup(area, "Preset", index, presetNames) - 1;

            if(EditorGUI.EndChangeCheck()) {
                if(index < 0)
                    preset.objectReferenceValue = null;
                else
                    preset.objectReferenceValue = presets[index];
            }
            EditorGUI.EndProperty();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
