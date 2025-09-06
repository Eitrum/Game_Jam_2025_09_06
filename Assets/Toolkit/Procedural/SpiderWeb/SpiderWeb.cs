using System.Collections;
using System.Collections.Generic;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Procedural.SpiderWeb
{
    public class SpiderWeb : MonoBehaviour
    {
        [Header("Direction 0")]
        [SerializeField] private Transform dir0;
        [SerializeField] private float width0 = 1f;

        [Header("Direction 1")]
        [SerializeField] private Transform dir1;
        [SerializeField] private float width1 = 1f;

        [Header("Mesh Data")]
        [SerializeField] private Mesh mesh = null;
        [SerializeField] private MeshFilter mf;

        [ContextMenu("Generate")]
        public void Generate() {
            if(mesh == null)
                mesh = new Mesh();
            mesh.name = "spider web mesh";

            if(!dir0.ToRay().Hit(out RaycastHit hit0)) {
                Debug.LogError("Spider Web Generate error, could not hit dir 0");
                return;
            }
            if(!dir1.ToRay().Hit(out RaycastHit hit1)) {
                Debug.LogError("Spider Web Generate error, could not hit dir 1");
                return;
            }

            var bulge = transform.InverseTransformDirection(transform.position - (hit0.point + hit1.point) / 2f);

            var p00 = transform.InverseTransformPoint(hit0.point + Quaternion.LookRotation(hit0.normal, dir0.up) * new Vector3(-width0, 0, 0));
            var p01 = transform.InverseTransformPoint(hit0.point + Quaternion.LookRotation(hit0.normal, dir0.up) * new Vector3(width0, 0, 0));

            var p10 = transform.InverseTransformPoint(hit1.point + Quaternion.LookRotation(hit1.normal, dir1.up) * new Vector3(-width1, 0, 0));
            var p11 = transform.InverseTransformPoint(hit1.point + Quaternion.LookRotation(hit1.normal, dir1.up) * new Vector3(width1, 0, 0));

            var curve0Handle = Vector3.Lerp(p00, p10, 0.5f) + bulge * Vector3.Distance(p00, p10);
            var curve0 = new Bezier(p00, curve0Handle, curve0Handle, p10);

            var curve1Handle = Vector3.Lerp(p01, p11, 0.5f) + bulge * Vector3.Distance(p01, p11);
            var curve1 = new Bezier(p01, curve1Handle, curve1Handle, p11);

            Vector3[] verts = new Vector3[25];
            Vector2[] uvs = new Vector2[25];
            int[] tris = new int[64];
            for(int y = 0; y < 5; y++) {
                var p0 = curve0.Evaluate(y / 4f);
                var p1 = curve1.Evaluate(y / 4f);
                for(int x = 0; x < 5; x++) {
                    verts[y * 5 + x] = Vector3.Lerp(p0, p1, x / 4f);
                    uvs[y * 5 + x] = new Vector2(x / 4f, y / 4f);
                }
            }
            for(int y = 0; y < 4; y++) {
                for(int x = 0; x < 4; x++) {
                    var trisIndex = y * 16 + x * 4;
                    tris[trisIndex + 0] = (y + 0) * 5 + (x + 0);
                    tris[trisIndex + 1] = (y + 1) * 5 + (x + 0);
                    tris[trisIndex + 2] = (y + 1) * 5 + (x + 1);
                    tris[trisIndex + 3] = (y + 0) * 5 + (x + 1);
                }
            }

            mesh.Clear();
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(tris, MeshTopology.Quads, 0, false, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            if(mf)
                mf.sharedMesh = mesh;
        }

        private void OnDisable() {
            if(mesh != null) {
                DestroyImmediate(mesh);
            }
        }

        private void OnDrawGizmos() {
            var pos = transform.position;
            var rot = transform.rotation;

            if(!dir0.ToRay().Hit(out RaycastHit hit0)) {
                return;
            }
            if(!dir1.ToRay().Hit(out RaycastHit hit1)) {
                return;
            }

            var bulge = transform.position - (hit0.point + hit1.point) / 2f;

            var p00 = hit0.point + Quaternion.LookRotation(hit0.normal, dir0.up) * new Vector3(-width0, 0, 0);
            var p01 = hit0.point + Quaternion.LookRotation(hit0.normal, dir0.up) * new Vector3(width0, 0, 0);

            var p10 = hit1.point + Quaternion.LookRotation(hit1.normal, dir1.up) * new Vector3(-width1, 0, 0);
            var p11 = hit1.point + Quaternion.LookRotation(hit1.normal, dir1.up) * new Vector3(width1, 0, 0);

            var curve0Handle = Vector3.Lerp(p00, p10, 0.5f) + bulge * Vector3.Distance(p00, p10);
            var curve0 = new Bezier(p00, curve0Handle, curve0Handle, p10);

            var curve1Handle = Vector3.Lerp(p01, p11, 0.5f) + bulge * Vector3.Distance(p01, p11);
            var curve1 = new Bezier(p01, curve1Handle, curve1Handle, p11);

            Gizmos.DrawLine(pos, hit0.point);
            Gizmos.DrawLine(pos, hit1.point);

            using(new GizmosUtility.ColorScope(Color.blue)) {
                Gizmos.DrawLine(p00, p01);
                Gizmos.DrawLine(p10, p11);
            }

            using(new GizmosUtility.ColorScope(Color.red)) {
                GizmosUtility.DrawBezier(curve0);
                GizmosUtility.DrawBezier(curve1);
            }

            using(new GizmosUtility.ColorScope(Color.green)) {
                for(int y = 0; y < 5; y++) {
                    var p0 = curve0.Evaluate(y / 4f);
                    var p1 = curve1.Evaluate(y / 4f);
                    for(int x = 0; x < 5; x++) {
                        var p = Vector3.Lerp(p0, p1, x / 4f);
                        Gizmos.DrawSphere(p, 0.05f);
                    }
                }
            }
        }
    }
}
