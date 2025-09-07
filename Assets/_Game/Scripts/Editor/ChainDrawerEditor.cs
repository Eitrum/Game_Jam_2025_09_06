using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolkit;
using Toolkit.Mathematics;

namespace Spaghetti
{
    [CustomEditor(typeof(ChainDrawer))]
    public class ChainDrawerEditor : Editor
    {
        private static bool isEditMode = false;
        private List<AnimationCurve> curves = new List<AnimationCurve>();

        public override void OnInspectorGUI() {
            var t = (ChainDrawer)target;

            isEditMode = EditorGUILayout.ToggleLeft("Edit", isEditMode);
            if(GUILayout.Button("Bake Matrix")) {
                t.UpdateCurve(true);
            }

            base.OnInspectorGUI();
            EditorGUILayout.Space();
            for(int i = 0, length = curves.Count; i < length; i++) {
                EditorGUILayout.CurveField(curves[i]);
            }
        }


        void OnSceneGUI() {
            if(!isEditMode)
                return;
            var t = (ChainDrawer)target;
            var poses = t.poses;
            var strengths = t.handleStrength;

            var mtx = Handles.matrix;
            Handles.matrix = t.transform.localToWorldMatrix;

            for(int i = strengths.Count; i < poses.Count; i++) {
                strengths.Add(1f);
            }

            for(int i = strengths.Count - 1; i >= poses.Count; i--) {
                strengths.RemoveAt(i);
            }

            for(int i = 0, length = poses.Count; i < length; i++) {
                var p = poses[i];

                p.rotation = p.rotation.normalized;

                Handles.TransformHandle(ref p.position, ref p.rotation);
                //p.rotation = Handles.RotationHandle(p.rotation, p.position);

                poses[i] = p;
            }

            for(int i = 0; i < strengths.Count; i++) {
                strengths[i] = 0f;
            }

            for(int i = 0, length = poses.Count - 1; i < length; i++) {
                strengths[i] = Vector3.Distance(poses[i].position, poses[i + 1].position) / 2f;
            }

            for(int i = poses.Count - 1; i >= 1; i--) {
                strengths[i] = Mathf.Max(strengths[i], Vector3.Distance(poses[i].position, poses[i - 1].position) / 2f);
            }

            Handles.matrix = mtx;
        }
    }
}
