using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(PhysicsSettings))]
    public class PhysicsSettingsInspector : Editor
    {
        #region Variables

        private SerializedProperty simulationMode;
        private SerializedProperty updatesPerSecond;
        private SerializedProperty autoSyncTransform;
        private SerializedProperty reuseCollisionCallbacks;

        private SerializedProperty gravity;

        private SerializedProperty bounceThreshold;
        private SerializedProperty sleepThreshold;

        private SerializedProperty defaultMaxDepenetrationVelocity;
        private SerializedProperty defaultContactOffset;
        private SerializedProperty defaultSolverIterations;
        private SerializedProperty defaultSolverVelocityIterations;
        private SerializedProperty defaultMaxAngularSpeed;

        private SerializedProperty queriesHitBackfaces;
        private SerializedProperty queriesHitTriggers;

        private SerializedProperty worldBounds;
        private SerializedProperty worldSubdivisions;

        private SerializedProperty clothGravity;
        private SerializedProperty interCollisionEnabled;
        private SerializedProperty interCollisionDistance;
        private SerializedProperty interCollisionStiffness;

        private SerializedProperty defaultMaterial;
        private SerializedProperty enableAdaptiveForce;
        private SerializedProperty contactPairsMode;
        private SerializedProperty broadphaseType;
        private SerializedProperty frictionType;
        private SerializedProperty improvedPatchFriction;
        private SerializedProperty enableEnhancedDeterminism;
        private SerializedProperty enableUnifiedHeightmaps;
        private SerializedProperty solverType;
        private SerializedProperty contactsGeneration;

        #endregion

        #region Init

        private void OnEnable() {
            simulationMode = serializedObject.FindProperty("simulationMode");
            updatesPerSecond = serializedObject.FindProperty("updatesPerSecond");
            autoSyncTransform = serializedObject.FindProperty("autoSyncTransform");
            reuseCollisionCallbacks = serializedObject.FindProperty("reuseCollisionCallbacks");

            gravity = serializedObject.FindProperty("gravity");

            bounceThreshold = serializedObject.FindProperty("bounceThreshold");
            sleepThreshold = serializedObject.FindProperty("sleepThreshold");

            defaultMaxDepenetrationVelocity = serializedObject.FindProperty("defaultMaxDepenetrationVelocity");
            defaultContactOffset = serializedObject.FindProperty("defaultContactOffset");
            defaultSolverIterations = serializedObject.FindProperty("defaultSolverIterations");
            defaultSolverVelocityIterations = serializedObject.FindProperty("defaultSolverVelocityIterations");
            defaultMaxAngularSpeed = serializedObject.FindProperty("defaultMaxAngularSpeed");

            queriesHitBackfaces = serializedObject.FindProperty("queriesHitBackfaces");
            queriesHitTriggers = serializedObject.FindProperty("queriesHitTriggers");

            worldBounds = serializedObject.FindProperty("worldBounds");
            worldSubdivisions = serializedObject.FindProperty("worldSubdivisions");

            clothGravity = serializedObject.FindProperty("clothGravity");
            interCollisionEnabled = serializedObject.FindProperty("interCollisionEnabled");
            interCollisionDistance = serializedObject.FindProperty("interCollisionDistance");
            interCollisionStiffness = serializedObject.FindProperty("interCollisionStiffness");

            defaultMaterial = serializedObject.FindProperty("defaultMaterial");
            enableAdaptiveForce = serializedObject.FindProperty("enableAdaptiveForce");
            contactPairsMode = serializedObject.FindProperty("contactPairsMode");
            broadphaseType = serializedObject.FindProperty("broadphaseType");
            frictionType = serializedObject.FindProperty("frictionType");
            improvedPatchFriction = serializedObject.FindProperty("improvedPatchFriction");
            enableEnhancedDeterminism = serializedObject.FindProperty("enableEnhancedDeterminism");
            enableUnifiedHeightmaps = serializedObject.FindProperty("enableUnifiedHeightmaps");
            solverType = serializedObject.FindProperty("solverType");
            contactsGeneration = serializedObject.FindProperty("contactsGeneration");
        }

        #endregion

        #region Drawing

        public override void OnInspectorGUI() {
            using(new ToolkitEditorUtility.InspectorScope(this)) {
                DrawSimulationSettings();
                DrawGravity();
                DrawThresholds();
                DrawDefaults();
                DrawQueries();
                DrawBroadphase();
                DrawFrictionSettings();
                DrawCloths();
            }
        }

        private void DrawSimulationSettings() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Simulation", EditorStylesUtility.BoldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(simulationMode);

                EditorGUI.indentLevel++;
                var simMode = (PhysicsSettings.SimulationMode)simulationMode.intValue;
                switch(simMode) {
                    case PhysicsSettings.SimulationMode.FixedUpdate:
                        EditorGUILayout.PropertyField(updatesPerSecond);
                        EditorGUILayout.PropertyField(autoSyncTransform);
                        break;
                    case PhysicsSettings.SimulationMode.Manual:
                    case PhysicsSettings.SimulationMode.Update:
                        EditorGUILayout.PropertyField(autoSyncTransform);
                        break;
                    case PhysicsSettings.SimulationMode.Toolkit:
                        EditorGUILayout.PropertyField(updatesPerSecond);
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(reuseCollisionCallbacks);
                EditorGUILayout.LabelField("Compile time settings", EditorStylesUtility.BoldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(enableAdaptiveForce);
                EditorGUILayout.PropertyField(contactsGeneration);
                EditorGUILayout.PropertyField(contactPairsMode);
                EditorGUILayout.PropertyField(solverType);
                EditorGUILayout.PropertyField(enableEnhancedDeterminism);
                EditorGUILayout.PropertyField(enableUnifiedHeightmaps);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        private void DrawFrictionSettings() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(frictionType);
                if(frictionType.intValue == 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(improvedPatchFriction);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.Space();
        }

        private void DrawGravity() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(gravity);
            }
            EditorGUILayout.Space();
        }

        private void DrawThresholds() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Threshold", EditorStylesUtility.BoldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(bounceThreshold);
                EditorGUILayout.PropertyField(sleepThreshold);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        private void DrawDefaults() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Defaults", EditorStylesUtility.BoldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(defaultMaterial);
                EditorGUILayout.PropertyField(defaultMaxDepenetrationVelocity);
                EditorGUILayout.PropertyField(defaultContactOffset);
                EditorGUILayout.PropertyField(defaultSolverIterations);
                EditorGUILayout.PropertyField(defaultSolverVelocityIterations);
                EditorGUILayout.PropertyField(defaultMaxAngularSpeed);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        private void DrawQueries() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Queries", EditorStylesUtility.BoldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(queriesHitBackfaces);
                EditorGUILayout.PropertyField(queriesHitTriggers);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        private void DrawBroadphase() {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.PropertyField(broadphaseType);
                if(broadphaseType.intValue > 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(worldBounds);
                    EditorGUILayout.PropertyField(worldSubdivisions);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.Space();
        }

        private void DrawCloths() {
            using(new EditorGUILayout.VerticalScope("box")) {
                clothGravity.isExpanded = EditorGUILayout.Foldout(clothGravity.isExpanded, "Cloth", true, EditorStyles.foldoutHeader);
                EditorGUI.indentLevel++;
                if(clothGravity.isExpanded) {
                    EditorGUILayout.PropertyField(clothGravity);
                    EditorGUILayout.PropertyField(interCollisionEnabled);
                    if(interCollisionEnabled.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(interCollisionDistance);
                        EditorGUILayout.PropertyField(interCollisionStiffness);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }

        #endregion
    }
}
