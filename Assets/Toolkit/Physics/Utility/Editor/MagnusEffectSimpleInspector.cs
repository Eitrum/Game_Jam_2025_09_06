using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(MagnusEffectSimple))]
    public class MagnusEffectSimpleInspector : Editor
    {
        #region Variables

        private SerializedProperty radius;
        private SerializedProperty ignoreBodyMass;
        private SerializedProperty strength;

        #endregion

        #region Init

        private void OnEnable() {
            radius = serializedObject.FindProperty("radius");
            ignoreBodyMass = serializedObject.FindProperty("ignoreBodyMass");
            strength = serializedObject.FindProperty("strength");
        }

        #endregion

        #region Draw

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(ignoreBodyMass);
                    EditorGUILayout.PropertyField(strength);
                    EditorGUILayout.LabelField($"Density: 'Air' (1.225 kg/m^3)", EditorStylesUtility.GrayItalicLabel);
                }

                if(Application.isPlaying) {
                    var me = target as MagnusEffectSimple;
                    using(new EditorGUILayout.VerticalScope("box")) {
                        EditorGUILayout.LabelField("Debug", EditorStylesUtility.BoldLabel);
                        EditorGUILayout.LabelField($"Direction ({me.MagnusEffectForce})");
                        EditorGUILayout.LabelField($"\tForce ({me.MagnusEffectForce.magnitude})");
                    }
                }
            }
        }

        #endregion
    }
}
