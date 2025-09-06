using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Toolkit.PhysicEx
{
    [CustomEditor(typeof(RagdollStatic))]
    public class StaticRagdollInspector : Editor
    {
        #region Variables

        private RagdollProfile profile;

        private SerializedProperty parts;
        private ReorderableList list;

        private GUIContent colliderContent = new GUIContent("", "Disable Collider, unchecking this causes the ragdoll to ignore the collider.");
        private GUIContent rigidbodyContent = new GUIContent("", "Disable Rigidbody, unchecking this causes the ragdoll to ignore the rigidbody.");

        private Vector3 direction = new Vector3(-1, 1, 0);
        private float force = 80f;
        private AnimationClip restoreTarget = null;
        private float restoreDuration = 1f;

        #endregion

        #region Init

        private void OnEnable() {
            parts = serializedObject.FindProperty("parts");
            list = new ReorderableList(serializedObject, parts);
            list.drawElementCallback += DrawElement;
            list.drawHeaderCallback += DrawHeader;
            list.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 4;
        }

        #endregion

        #region Drawing

        private void DrawHeader(Rect rect) {
            parts.isExpanded = EditorGUI.Foldout(rect.Pad(12, 0, 0, 0), parts.isExpanded, "Add Parts", true);
            if(GUI.Button(rect.Pad(rect.width - 110, 0, 0, 0), "Add Parts")) {
                var t = (RagdollStatic)target;
                t.AddParts();
                serializedObject.Update();
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var element = parts.GetArrayElementAtIndex(index);
            rect.Pad(0, 0, 2, 2).SplitVertical(out Rect boneArea, out Rect colliderArea, out Rect bodyArea, 0.333f, 0.333f, 1f);
            var bone = element.FindPropertyRelative("bone");
            var disableCollider = element.FindPropertyRelative("disableCollider");
            var collider = element.FindPropertyRelative("collider");
            var disableRigidbody = element.FindPropertyRelative("disableRigidbody");
            var body = element.FindPropertyRelative("body");

            EditorGUI.PropertyField(boneArea, bone);
            disableCollider.boolValue = EditorGUI.Toggle(colliderArea.Pad(0, colliderArea.width - 20, 0, 0), colliderContent, disableCollider.boolValue);
            using(new EditorGUI.DisabledScope(!disableCollider.boolValue))
                EditorGUI.PropertyField(colliderArea.Pad(20, 0, 0, 0), collider);
            disableRigidbody.boolValue = EditorGUI.Toggle(bodyArea.Pad(0, bodyArea.width - 20, 0, 0), rigidbodyContent, disableRigidbody.boolValue);
            using(new EditorGUI.DisabledScope(!disableRigidbody.boolValue))
                EditorGUI.PropertyField(bodyArea.Pad(20, 0, 0, 0), body);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var t = (RagdollStatic)target;

            using(new EditorGUILayout.HorizontalScope()) {
                profile = EditorGUILayout.ObjectField("Profile", profile, typeof(RagdollProfile), false) as RagdollProfile;
                if(profile == null) {
                    if(GUILayout.Button("new", GUILayout.Width(50f))) {
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
                        profile = prof;
                    }
                }
                else {
                    if(GUILayout.Button("Add Parts", GUILayout.Width(110f))) {
                        t.AddParts(profile);
                        serializedObject.Update();
                        if(!t.gameObject.scene.IsValid())
                            EditorUtility.SetDirty(t);
                        else
                            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
                    }
                    if(GUILayout.Button("Copy", GUILayout.Width(60f))) {
                        var bones = t.GetComponentInChildren<Toolkit.Utility.BoneReferences>();
                        if(!bones) {
                            Debug.LogError("No bone references found!");
                            return;
                        }
                        profile.CreateFrom(bones);
                    }
                }
            }

            if(parts.isExpanded)
                list.DoLayoutList();
            else {
                using(new EditorGUILayout.VerticalScope("box")) {
                    var area = GUILayoutUtility.GetRect(1f, 20);
                    DrawHeader(area);
                }
            }

            if(serializedObject.hasModifiedProperties) {
                serializedObject.ApplyModifiedProperties();
            }

            if(Application.isPlaying) {
                DrawDebugMode();
            }
        }

        private void DrawDebugMode() {
            var t = (RagdollStatic)target;
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Debug", EditorStylesUtility.BoldLabel);
                using(new EditorGUI.IndentLevelScope(1)) {
                    direction = EditorGUILayout.Vector3Field("Direction", direction);
                    force = EditorGUILayout.Slider("Force", force, 0f, 100f);
                    if(GUILayout.Button("Initialize", GUILayout.Width(90f))) {
                        if(force > Mathf.Epsilon)
                            t.Initialize(direction, force);
                        else
                            t.Initialize();
                    }
                    EditorGUILayout.Space();
                    if(t.IsInitialized) {
                        restoreTarget = EditorGUILayout.ObjectField("Restore Target", restoreTarget, typeof(AnimationClip), false) as AnimationClip;
                        restoreDuration = EditorGUILayout.Slider("Restore Duration", restoreDuration, 0f, 5f);
                        if(GUILayout.Button("Restore", GUILayout.Width(90f))) {
                            if(restoreTarget != null && restoreDuration > 0)
                                t.Restore(restoreTarget, restoreDuration);
                            else
                                t.Restore();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
