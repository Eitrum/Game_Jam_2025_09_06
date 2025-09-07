using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;
using Toolkit.Mathematics;

namespace Spaghetti
{
    [ExecuteAlways]
    public class ChainDrawer : MonoBehaviour
    {
        [Header("Settings")]
        public List<Pose> poses = new List<Pose>();
        public List<float> handleStrength = new List<float>();
        public float distancePerSegment = 0.5f;
        public List<Matrix4x4> chainMatricies = new List<Matrix4x4>();
        public float scaleModifier = 1f;

        [Header("Rendering")]
        [SerializeField] private Vector3 rotationFix;
        public Mesh chainMesh;
        public Material chainMaterial;

        private int[] cachedSegments;

        void Awake() {
            cachedSegments = new int[poses.Count - 1];
            Bezier[] curves = new Bezier[poses.Count - 1];
            for(int i = 0; i < curves.Length; i++) {
                var p0 = poses[i];
                var p1 = poses[i + 1];
                var str0 = handleStrength[i];
                var str1 = handleStrength[i + 1];
                curves[i] = new Bezier(p0.position, p0.position + p0.rotation * new Vector3(0, 0, str0), p1.position - p1.rotation * new Vector3(0, 0, str1), p1.position);
            }
            for(int i = 0; i < curves.Length; i++) {
                var l = curves[i].CalculateLength();
                cachedSegments[i] = Mathf.CeilToInt(l / (distancePerSegment * scaleModifier));
            }
        }

        private void OnEnable() {
            if(Application.isPlaying) {
                if(chainMesh == null) {
                    enabled = false;
                    Debug.LogError($"{name} is missing chain mesh!");
                }
                if(chainMaterial == null) {
                    enabled = false;
                    Debug.LogError($"{name} is missing chain material!");
                }

                if(chainMatricies == null || chainMatricies.Count == 0) {
                    enabled = false;
                    Debug.LogError($"{name} is missing chain matricies or does not contain any chains!");
                }
            }
        }

        private void LateUpdate() {
#if UNITY_EDITOR
            if(chainMesh == null) {
                return;
            }
            if(chainMaterial == null) {
                return;
            }
            if(chainMatricies == null || chainMatricies.Count == 0) {
                return;
            }
#endif
            Graphics.DrawMeshInstanced(chainMesh, 0, chainMaterial, chainMatricies);
        }

        public void UpdateCurve(bool newSegments = false) {
            Bezier[] curves = new Bezier[poses.Count - 1];
            for(int i = 0; i < curves.Length; i++) {
                var p0 = poses[i];
                var p1 = poses[i + 1];
                var str0 = handleStrength[i];
                var str1 = handleStrength[i + 1];
                curves[i] = new Bezier(p0.position, p0.position + p0.rotation * new Vector3(0, 0, str0), p1.position - p1.rotation * new Vector3(0, 0, str1), p1.position);
            }

            chainMatricies.Clear();
            var baseMatrix = transform.localToWorldMatrix;

            int chainIndex = 0;
            for(int i = 0; i < curves.Length; i++) {
                var segments = newSegments ? Mathf.CeilToInt(curves[i].CalculateLength() / (distancePerSegment * scaleModifier)) : cachedSegments[i];
                if(newSegments)
                    cachedSegments[i] = segments;
                var fixedDistanceCurve = curves[i].ArcLengthParameterizedCurve();

                for(int x = 0; x < segments; x++) {
                    var time = fixedDistanceCurve.Evaluate(1f / (segments) * x);
                    var pos = curves[i].Evaluate(time);
                    var rot = Quaternion.LookRotation(curves[i].EvaluateTangent(time)) * Quaternion.Euler(rotationFix);
                    if(chainIndex % 2 == 1)
                        rot *= Quaternion.Euler(0, 90, 0);
                    chainMatricies.Add(baseMatrix * Matrix4x4.TRS(pos, rot, new Vector3(scaleModifier, scaleModifier, scaleModifier)));
                    chainIndex++;
                }
            }
        }

        private void OnDrawGizmosSelected() {
            if(poses == null || poses.Count < 2)
                return;
            using(new GizmosUtility.MatrixScope(transform)) {
                Bezier[] curves = new Bezier[poses.Count - 1];
                for(int i = 0; i < curves.Length; i++) {
                    var p0 = poses[i];
                    var p1 = poses[i + 1];
                    var str0 = handleStrength[i];
                    var str1 = handleStrength[i + 1];
                    curves[i] = new Bezier(p0.position, p0.position + p0.rotation * new Vector3(0, 0, str0), p1.position - p1.rotation * new Vector3(0, 0, str1), p1.position);
                }
                for(int i = 0; i < curves.Length; i++) {
                    GizmosUtility.DrawBezier(curves[i]);
                }
            }
        }
    }
}
