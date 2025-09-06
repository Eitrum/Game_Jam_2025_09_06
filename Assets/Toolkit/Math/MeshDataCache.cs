using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics {
    public class MeshDataCache {
        #region Variables

        private const string TAG = "[Toolkit.MeshDataCache] - ";
        private static Dictionary<Mesh, MeshDataCache> meshToCache = new Dictionary<Mesh, MeshDataCache>();

        private List<Vector3> verticies;
        private List<Vector3> normals;
        private List<int> tris;
        private MeshTopology topology;
        private List<Triangle> triangles;
        private List<Vector3> trianglesNormals;
        private List<Triangle.ClosestPointCache> trianglesClosestPointCache;
        private Bounds? bounds;

        public Mesh Mesh { get; private set; }

        #endregion

        #region Properties

        public MeshTopology Topology => topology;
        public int VerticiesCount => verticies.Count;
        public int TriangleCount => Tris.Count / 3;

        public Bounds Bounds {
            get {
                if(!bounds.HasValue) {
                    bounds = Mesh.bounds;
                }
                return bounds.Value;
            }
        }

        public IReadOnlyList<Vector3> Verticies {
            get {
                if(verticies == null) {
                    verticies = new List<Vector3>();
                    Mesh.GetVertices(verticies);
                }
                return verticies;
            }
        }
        public IReadOnlyList<Vector3> Normals {
            get {
                if(normals == null) {
                    normals = new List<Vector3>();
                    Mesh.GetNormals(normals);
                }
                return verticies;
            }
        }
        public IReadOnlyList<int> Tris {
            get {
                if(tris == null) {
                    tris = new List<int>();
                    Mesh.GetTriangles(tris, 0);
                }
                return tris;
            }
        }

        public IReadOnlyList<Triangle> Triangles {
            get {
                if(triangles == null) {
                    triangles = new List<Triangle>();
                    var trisCache = Tris;
                    var vertCache = Verticies;
                    for(int i = 0; i < trisCache.Count; i += 3) {
                        var index0 = trisCache[i + 0];
                        var index1 = trisCache[i + 1];
                        var index2 = trisCache[i + 2];

                        triangles.Add(new Triangle(
                            vertCache[index0],
                            vertCache[index1],
                            vertCache[index2]));
                    }
                }
                return triangles;
            }
        }
        public IReadOnlyList<Triangle.ClosestPointCache> TrianglesClosestPointCache {
            get {
                if(trianglesClosestPointCache == null) {
                    trianglesClosestPointCache = new List<Triangle.ClosestPointCache>();
                    foreach(var t in Triangles) {
                        t.PrecalculateClosestPointCache(out var cache);
                        trianglesClosestPointCache.Add(cache);
                    }
                }
                return trianglesClosestPointCache;
            }
        }

        public IReadOnlyList<Vector3> TrianglesNormal {
            get {
                if(trianglesNormals == null) {
                    trianglesNormals = new List<Vector3>();
                    var triangles = Triangles;
                    foreach(var tri in triangles)
                        trianglesNormals.Add(tri.Normal);
                }
                return trianglesNormals;
            }
        }

        #endregion

        #region Constructor

        public MeshDataCache(Mesh mesh) {
            if(mesh == null)
                throw new ArgumentException(TAG + "unable to create a cache object from null mesh.");

            Mesh = mesh;
            topology = mesh.GetTopology(0);
            AddOrCopy(this, mesh);
        }

        // Used to copy data from global mesh cache to locally created ones.
        private void CopyFrom(MeshDataCache o) {
            verticies = o.verticies;
            normals = o.normals;
            triangles = o.triangles;
            topology = o.topology;
            triangles = o.triangles;
            trianglesNormals = o.trianglesNormals;
        }

        #endregion

        #region Closest Point To Mesh

        public void CalculateClosestPoint(Vector3 point, out Vector3 pointOnMesh) {
            pointOnMesh = point;

            if(TriangleCount > 1024) {
                var tc = new ThreadedCalculations(point, Triangles, TrianglesClosestPointCache, 512);
                var res =  tc.GetResult();
                pointOnMesh = Triangles[res.Index].ClosestPoint(point);
            }
            else {
                var d = float.MaxValue;
                var tris = Triangles;
                var clpcache = TrianglesClosestPointCache;
                for(int i = 0, len = tris.Count; i < len; i++) {
                    var p = tris[i].ClosestPoint(point, clpcache[i]);
                    var dist = Vector3.Distance(p, point);
                    if(dist < d) {
                        pointOnMesh = p;
                        d = dist;
                    }
                }
            }
        }

        public void CalculateClosestPoint(Vector3 point, out Vector3 pointOnMesh, out bool isInside) {
            pointOnMesh = point;
            isInside = false;
            int index = 0;
            int isinsidecount = 0;
            int edgecount = 0;

            var d = float.MaxValue;
            var tris = Triangles;
            var clpcache = TrianglesClosestPointCache;
            for(int i = 0, len = tris.Count; i < len; i++) {
                var p = tris[i].ClosestPoint(point, clpcache[i]);
                var dist = Vector3.Distance(p, point);

                if(dist.Equals(d, 0.0001f)) {
                    if(edgecount == 0) {
                        var firstTriIsInside = Vector3.Dot(TrianglesNormal[index], (point - pointOnMesh).normalized) < 0f;
                        if(firstTriIsInside)
                            isinsidecount++;
                    }
                    edgecount++;
                    var triIsInside = Vector3.Dot(TrianglesNormal[i], (point - p).normalized) < 0f;
                    if(triIsInside)
                        isinsidecount++;
                }
                else if(dist < d) {
                    index = i;
                    pointOnMesh = p;
                    d = dist;
                    isinsidecount = -1;
                    edgecount = 0;
                }
            }
            if(isinsidecount == -1) {
                var dir = point - pointOnMesh;
                isInside = Vector3.Dot(TrianglesNormal[index], dir.normalized) < 0f;
            }
            else {
                if((isinsidecount) > (edgecount / 2f)) {
                    isInside = true;
                }
            }
        }

        private class ThreadedCalculations {

            public struct Result {
                public int Index;
                public float ResultValue;

                public override string ToString() {
                    return $"{Index} {ResultValue}";
                }
            }

            private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            private int started = 0;
            private List<Result> resultList;

            public ThreadedCalculations(Vector3 point, IReadOnlyList<Triangle> triangles, IReadOnlyList<Triangle.ClosestPointCache> cache, int chunkSize) {
                sw.Start();
                resultList = new List<Result>(triangles.Count / chunkSize + 1);
                for(int i = 0, len = triangles.Count; i < len; i += chunkSize) {
                    started++;
                    int start = i + 0;
                    Toolkit.Threading.Job.Run(() => CalculateSmallest(point, triangles, cache, start, chunkSize), OnComplet);
                }
            }

            private void OnComplet(Result res) {
                lock(resultList)
                    resultList.Add(res);
            }

            public Result GetResult() {
                while(resultList.Count < started && sw.ElapsedMilliseconds < 1000) {

                }

                return resultList.Lowest(x => x.ResultValue);
            }

            private Result CalculateSmallest(Vector3 point, IReadOnlyList<Triangle> triangles, IReadOnlyList<Triangle.ClosestPointCache> cache, int start, int chunkSize) {
                var end = Mathf.Min(triangles.Count, start + chunkSize);
                Result res = new Result() {
                    Index = -1,
                    ResultValue = float.MaxValue,
                };
                for(int i = start; i < end; i++) {
                    var p = triangles[i].ClosestPoint(point, cache[i]);
                    var d = Vector3.Distance(p, point);
                    if(d < res.ResultValue) {
                        res.Index = i;
                        res.ResultValue = d;
                    }
                }
                return res;
            }
        }

        #endregion

        #region Static - Util

        public static Vector3 WorldPointToMeshDataPoint(Transform t, Vector3 worldPoint) => t == null ? worldPoint : t.InverseTransformPoint(worldPoint);
        public static Vector3 MeshDataPointToWorldPoint(Transform t, Vector3 meshDataPoint) => t == null ? meshDataPoint : t.TransformPoint(meshDataPoint);

        #endregion

        #region Static - Cache

        private static void AddOrCopy(MeshDataCache cache, Mesh mesh) {
            if(mesh == null || cache == null)
                return;
            if(!meshToCache.TryGetValue(mesh, out var otherCache)) {
                meshToCache.Add(mesh, cache);
                return;
            }

            cache.CopyFrom(otherCache);
        }

        public static bool TryGet(Mesh mesh, out MeshDataCache cache) {
            try {
                cache = Get(mesh);
                return true;
            }
            catch(Exception e) {
                Debug.LogException(e);
                cache = null;
                return false;
            }
        }

        public static MeshDataCache Get(Mesh mesh) {
            if(mesh == null)
                throw new ArgumentException(TAG + "unable to create a cache object from null mesh.");

            return meshToCache.TryGetValue(mesh, out MeshDataCache cache) ?
                cache :
                new MeshDataCache(mesh);
        }

        #endregion

        #region Static - Clear

        public static bool Clear(MeshDataCache cache) {
            return Clear(cache.Mesh);
        }

        public static bool Clear(Mesh mesh) {
            if(mesh == null) return false;
            return meshToCache.Remove(mesh);
        }

        public static void ClearAll() {
            meshToCache.Clear();
        }

        #endregion
    }
}
