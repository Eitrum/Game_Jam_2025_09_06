using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Health {
    [CustomEditor(typeof(Health))]
    public class HealthInspector : Editor {
        #region Variables

        private SerializedProperty full;
        private SerializedProperty current;

        private static bool percentage = true;
        private float healAmount = 0f;
        private float damageAmount = 0f;

        #endregion

        #region Init

        private void OnEnable() {
            full = serializedObject.FindProperty("maxHealth");
            current = serializedObject.FindProperty("current");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings", EditorStylesUtility.BoldLabel);
                EditorGUILayout.PropertyField(full);
                EditorGUILayout.PropertyField(current);
                serializedObject.ApplyModifiedProperties();
            }
            if(Application.isPlaying) {
                using(new EditorGUILayout.VerticalScope("box")) {
                    EditorGUILayout.LabelField("Debug", EditorStylesUtility.BoldLabel);
                    var health = target as IHealth;
                    EditorGUILayout.LabelField("Percentage:", $"{(health.Percentage * 100f):0.0#}%");
                    EditorGUILayout.LabelField("IsAlive:", $"{health.IsAlive}");
                    EditorGUILayout.Space();
                    using(new EditorGUILayout.HorizontalScope()) {
                        if(EditorGUILayout.ToggleLeft("Percentage", percentage)) {
                            if(!percentage) {
                                healAmount = healAmount / health.Full;
                                damageAmount = damageAmount / health.Full;
                            }
                            percentage = true;
                        }
                        if(EditorGUILayout.ToggleLeft("Value", !percentage)) {
                            if(percentage) {
                                healAmount = healAmount * health.Full;
                                damageAmount = damageAmount * health.Full;
                            }
                            percentage = false;
                        }
                    }
                    using(new EditorGUILayout.HorizontalScope()) {
                        healAmount = percentage ?
                            EditorGUILayout.Slider(healAmount, 0f, 1f) :
                            EditorGUILayout.Slider(healAmount, 0f, health.Full);

                        if(health.IsAlive) {
                            if(GUILayout.Button("Heal", GUILayout.Width(80f)))
                                health.Heal(new HealInstance(percentage ? healAmount * health.Full : healAmount));
                        }
                        else {
                            if(GUILayout.Button("Revive", GUILayout.Width(90f)))
                                health.Revive(percentage ? healAmount * health.Full : healAmount);
                        }
                    }

                    using(new EditorGUILayout.HorizontalScope()) {
                        damageAmount = percentage ?
                            EditorGUILayout.Slider(damageAmount, 0f, 1f) :
                            EditorGUILayout.Slider(damageAmount, 0f, health.Full);
                        if(GUILayout.Button("Damage", GUILayout.Width(80f)))
                            health.Damage(new DamageInstance(percentage ? damageAmount * health.Full : damageAmount));
                    }
                }
            }
        }

        #endregion
    }
}
