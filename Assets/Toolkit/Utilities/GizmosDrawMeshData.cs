using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Utility {
    public class GizmosDrawMeshData : MonoBehaviour {

        #region Variables

        [Header("Vert Normals")]
        [SerializeField] private bool normals;
        [SerializeField, RangeEx(-1f, 2f, 0.05f)] private float normalsLength = 0.1f;
        [SerializeField] private Color normalsColor = Color.red;

        [Header("Triangle Normals")]
        [SerializeField] private bool triangleNormals;
        [SerializeField, RangeEx(-1f, 2f, 0.05f)] private float triangleNormalsLength = 0.1f;
        [SerializeField] private Color triangleNormalsColor = Color.green;

        [Header("Voxel")]
        [SerializeField, PowerOfTwo(3, 6)] private int voxelCount = 32;

        [Header("Voxel Based IsInside")]
        [SerializeField] private bool isInside;
        [SerializeField] private Color isInsideColor = Color.blue;
        [System.NonSerialized] private CubicArray<bool> isInsideCache;

        [Header("Voxel Based Distance")]
        [SerializeField] private bool voxelDistance;
        [SerializeField] private Color voxelDistanceColor = Color.blue;
        [System.NonSerialized] private CubicArray<Line> voxelDistanceCache;

        #endregion

        private void OnDrawGizmosSelected() {
            if(!enabled) return;
            var mf = GetComponent<MeshFilter>();
            if(!mf) return;
            if(!mf.sharedMesh) return;
            var mdata = new MeshDataCache(mf.sharedMesh);
            if(mdata == null) return;

            using(new GizmosUtility.MatrixScope(transform)) {
                if(normals) {
                    using(new GizmosUtility.ColorScope(normalsColor)) {
                        var vert = mdata.Verticies;
                        var norm = mdata.Normals;
                        for(int i = 0, len = vert.Count; i < len; i++) {
                            Gizmos.DrawLine(vert[i], vert[i] + norm[i] * normalsLength);
                        }
                    }
                }

                if(triangleNormals) {
                    using(new GizmosUtility.ColorScope(triangleNormalsColor)) {
                        var vert = mdata.Triangles;
                        var norm = mdata.TrianglesNormal;
                        for(int i = 0, len = vert.Count; i < len; i++) {
                            var p = vert[i].Center;
                            Gizmos.DrawLine(p, p + norm[i] * triangleNormalsLength);
                        }
                    }
                }

                if(isInside) {
                    using(new GizmosUtility.ColorScope(isInsideColor)) {
                        if(isInsideCache == null || isInsideCache.Count == 0)
                            BakeIsInside(mdata);

                        var bounds = mdata.Bounds;
                        var w = isInsideCache.Width;
                        var h = isInsideCache.Height;
                        var d = isInsideCache.Depth;

                        var xscale = bounds.size.x / w;
                        var yscale = bounds.size.y / h;
                        var zscale = bounds.size.z / d;
                        var scale = new Vector3(xscale, yscale, zscale);

                        for(int x = 0; x < w; x++) {
                            for(int y = 0; y < h; y++) {
                                for(int z = 0; z < d; z++) {
                                    if(!isInsideCache[x, y, z])
                                        continue;
                                    var p = bounds.GetPointFromLerp(x * xscale, y * yscale, z * zscale);
                                    Gizmos.DrawCube(p, scale);
                                }
                            }
                        }
                    }
                }
                if(voxelDistance) {
                    using(new GizmosUtility.ColorScope(voxelDistanceColor)) {
                        if(voxelDistanceCache == null || voxelDistanceCache.Count == 0)
                            BakeIsInside(mdata);

                        var bounds = mdata.Bounds;
                        var w = voxelDistanceCache.Width;
                        var h = voxelDistanceCache.Height;
                        var d = voxelDistanceCache.Depth;

                        var xscale = bounds.size.x / w;
                        var yscale = bounds.size.y / h;
                        var zscale = bounds.size.z / d;
                        var scale = new Vector3(xscale, yscale, zscale);

                        for(int x = 0; x < w; x++) {
                            for(int y = 0; y < h; y++) {
                                for(int z = 0; z < d; z++) {
                                    if(isInside && !isInsideCache[x, y, z])
                                        continue;
                                    var l = voxelDistanceCache[x, y, z];
                                    GizmosUtility.DrawLine(l);
                                }
                            }
                        }
                    }
                }
            }
        }

        [ContextMenu("Rebake")]
        private void Rebake() {
            var mf = GetComponent<MeshFilter>();
            if(!mf) return;
            if(!mf.sharedMesh) return;
            var mdata = new MeshDataCache(mf.sharedMesh);
            if(mdata == null) return;
            BakeIsInside(mdata);
        }

        private void BakeIsInside(MeshDataCache mdata) {
            var bounds = mdata.Bounds;
            isInsideCache = new CubicArray<bool>(voxelCount);
            voxelDistanceCache = new CubicArray<Line>(voxelCount);
            Debug.Log("TestTest:" + voxelDistanceCache.Count);
            var xscale = bounds.size.x / voxelCount;
            var yscale = bounds.size.y / voxelCount;
            var zscale = bounds.size.z / voxelCount;

            for(int x = 0; x < voxelCount; x++) {
                for(int y = 0; y < voxelCount; y++) {
                    for(int z = 0; z < voxelCount; z++) {
                        var p = bounds.GetPointFromLerp(x * xscale, y * yscale, z * zscale);
                        mdata.CalculateClosestPoint(p, out var output, out bool isinside);
                        isInsideCache[x, y, z] = isinside;
                        voxelDistanceCache[x, y, z] = new Line(p, output);
                    }
                }
            }
        }
    }
}
