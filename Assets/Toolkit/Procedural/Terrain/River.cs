using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "River", menuName = "Toolkit/Procedural/Terrain/River")]
    public class River : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField, PowerOfTwo(1, 5)] int downSampling = 2;

        [Header("Size")]
        [SerializeField] private float width = 3f;
        [SerializeField] private AnimationCurve widthCurve = AnimationCurve.Linear(0f, 0.1f, 10f, 1f);
        [SerializeField] private float depth = 1f;
        [SerializeField] private AnimationCurve depthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Texture")]
        [SerializeField] private int riverTextureId = 0;

        [Header("Inertia")]
        [SerializeField] private float inertia = 0.95f;
        [SerializeField] private int maxIterations = 100000;

        public bool Generate(Data data) {
            var w = data.Width;
            var h = data.Height;
            var rw = w / downSampling;
            var hw = h / downSampling;
            var heightmap = data.Heightmap;

            #region Generate River

            float[,] averageHeight = new float[rw, hw];

            for(int x = 0; x < rw; x++) {
                for(int y = 0; y < hw; y++) {
                    averageHeight[x, y] = data.GetHeight(x * downSampling, y * downSampling, downSampling, downSampling);
                }
            }

            Vector2Int startNode = default;
            float highestPoint = 0f;
            for(int x = 0; x < rw; x++) {
                for(int y = 0; y < hw; y++) {
                    var height = averageHeight[x, y];
                    if(height > highestPoint) {
                        startNode = new Vector2Int(x, y);
                        highestPoint = height;
                    }
                }
            }

            Vector2 node = new Vector2(startNode.x * downSampling + data.Random.Next(downSampling), startNode.y * downSampling + data.Random.Next(downSampling));
            List<Vector2> path = new List<Vector2>();
            var direction = (new Vector2(w / 2f, h / 2f) - (startNode * downSampling)).normalized;

            for(int i = 0; i < maxIterations; i++) {
                var rs = data.GetRawSlope(node);
                direction = new Vector2(direction.x * inertia - rs.x * (1f - inertia), direction.y * inertia - rs.y * (1f - inertia));
                if(direction.magnitude <= 0.1f) {
                    var n = data.GetNormal(node).To_Vector2_XZ();
                    if(n.magnitude < Mathf.Epsilon)
                        break;
                    direction = n.normalized;
                }
                node += direction;
                if(node.x < 0 || node.y < 0 || node.y > h || node.x > w)
                    break;
                path.Add(node);
            }

            List<Vector3Int> pathRounded = new List<Vector3Int>();
            Vector2Int temp = path[0].RoundToInt();
            int count = 1;
            for(int i = 1, length = path.Count; i < length; i++) {
                var p = path[i].RoundToInt();
                if(p != temp) {
                    pathRounded.Add(new Vector3Int(temp.x, temp.y, count));
                    temp = p;
                    count = 1;
                }
                else {
                    count++;
                }
            }
            float[,] rivermodification = new float[w, h];

            pathRounded.Add(new Vector3Int(temp.x, temp.y, count));
            for(int i = 0, length = pathRounded.Count; i < length; i++) {
                var p = pathRounded[i];
                var tx = p.x;
                var ty = p.y;
                var modifier = widthCurve.Evaluate(p.z);
                Apply(rivermodification, tx, ty, width * modifier, depth * modifier, depthCurve);
            }

            #endregion

            #region Generate River Mesh
            {
                List<Vector2> meshPath = new List<Vector2>();
                var lastPoint = path[0];
                meshPath.Add(lastPoint);
                int strength = 1;
                List<int> strengthList = new List<int>();
                strengthList.Add(strength);

                for(int i = 1, length = path.Count; i < length; i++) {
                    var dist = (path[i] - lastPoint);
                    if(dist.magnitude >= 1f) {
                        var p = path[i];
                        meshPath.Add(p);
                        lastPoint = p;
                        strengthList.Add(strength);
                        strength = 1;
                    }
                    else {
                        strength++;
                    }
                }
                strengthList.Add(strength);

                float height = data.GetHeight(meshPath[meshPath.Count - 1]);
                MinMax flatRange = new MinMax(height - 1f, height + .5f);
                var mpCountMinusOne = meshPath.Count - 1;
                int startIndexOfClusterFuck = mpCountMinusOne;
                int clusterFuckCheck = 0;
                for(int i = mpCountMinusOne; i >= 0; i--) {
                    if(!flatRange.Contains(data.GetHeight(meshPath[i]))) {
                        if(clusterFuckCheck == 0)
                            startIndexOfClusterFuck = i + 1;
                        clusterFuckCheck++;
                        if(clusterFuckCheck > 50)
                            break;
                    }
                    else {
                        clusterFuckCheck = 0;
                    }
                }
                meshPath.RemoveRange(startIndexOfClusterFuck, mpCountMinusOne - startIndexOfClusterFuck + 1);
                strengthList.RemoveRange(startIndexOfClusterFuck, mpCountMinusOne - startIndexOfClusterFuck + 1);

                Vector3[] worldPoints = new Vector3[meshPath.Count];
                Vector2[] directions = new Vector2[meshPath.Count];
                float[] widthAtPoints = new float[meshPath.Count];
                for(int i = 0, length = meshPath.Count; i < length; i++) {
                    worldPoints[i] = meshPath[i].ToVector3_XZ(data.GetBilinearHeight(meshPath[i]) - (depth / 3f));
                    widthAtPoints[i] = width * widthCurve.Evaluate(strengthList[i]);
                    if(i + 1 < length)
                        directions[i] = meshPath[i] - meshPath[i + 1];
                }
                directions[directions.Length - 1] = directions[directions.Length - 2];

                data.UniqueData.Add(new RiverData(worldPoints, directions, widthAtPoints));
            }
            #endregion

            #region Handle Data

            var splat = data.Splatmap;
            var sw = data.SplatWidth;
            var sh = data.SplatHeight;

            for(int x = 0; x < sw; x++) {
                for(int y = 0; y < sh; y++) {
                    var pos = data.GetHeightmapPositionFromSplatmap(x, y);
                    var d = rivermodification[(int)pos.x, (int)pos.y];
                    if(d > Mathf.Epsilon) {
                        var s = splat[x, y].Normalized;
                        s.Add(riverTextureId, Mathf.Lerp(1f, 20f, d / depth));
                        splat[x, y] = s;
                    }
                }
            }

            var details = data.Details;
            var dw = data.DetailWidth;
            var dh = data.DetailHeight;
            for(int i = 0, length = details.Count; i < length; i++) {
                var amount = details[i].Amount;
                for(int x = 0; x < dw; x++) {
                    for(int y = 0; y < dh; y++) {
                        var pos = data.GetHeightmapPositionFromDetailmap(x, y);
                        var d = rivermodification[(int)pos.x, (int)pos.y];
                        if(d > Mathf.Epsilon)
                            amount[x, y] = 0;
                    }
                }
            }

            var trees = data.Trees;
            for(int i = 0, length = trees.Count; i < length; i++) {
                var instances = trees[i].Instances;
                for(int inc = instances.Count - 1; inc >= 0; inc--) {
                    var t = instances[inc];
                    var x = (int)Mathf.Clamp(t.point.x, 0, w - 1);
                    var y = (int)Mathf.Clamp(t.point.z, 0, h - 1);
                    if(rivermodification[x, y] > Mathf.Epsilon)
                        trees[i].RemoveTree(inc);
                }
            }

            var prefs = data.Prefabs;
            for(int i = 0, length = prefs.Count; i < length; i++) {
                var prefabData = prefs[i];
                var instances = prefabData.Instances;
                for(int pinst = instances.Count - 1; pinst >= 0; pinst--) {
                    var t = instances[pinst];
                    var x = (int)Mathf.Clamp(t.point.x, 0, w - 1);
                    var y = (int)Mathf.Clamp(t.point.z, 0, h - 1);
                    if(rivermodification[x, y] > Mathf.Epsilon)
                        prefabData.Remove(pinst);
                }
            }

            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    heightmap[x, y] -= rivermodification[x, y];
                }
            }

            #endregion

            return true;
        }

        public class RiverData : IUniqueData
        {
            private Vector3[] worldPoints;
            private Vector2[] directions;
            private float[] width;

            public RiverData(Vector3[] worldPoints, Vector2[] directions, float[] width) {
                this.worldPoints = worldPoints;
                this.directions = directions;
                this.width = width;
            }

            public void Build(Transform parent) {
                var rivermesh = parent.GetOrAddComponent<RiverMesh>();
                var mesh = rivermesh.Mesh;

                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();
                List<int> tris = new List<int>();

                for(int i = 0, length = worldPoints.Length - 1; i < length; i++) {
                    var p1 = worldPoints[i + 0];
                    var p2 = worldPoints[i + 1];
                    var dir = directions[i];
                    var width = this.width[i] / 2f;
                    var left = (Vector2.Perpendicular(dir) * width).ToVector3_XZ();
                    var right = -left;

                    // Add Right
                    var trisIndex = i * 8;
                    vertices.Add(p1);
                    vertices.Add(p1 + right);
                    vertices.Add(p2 + right);
                    vertices.Add(p2);

                    uv.Add(new Vector2(0, 0));
                    uv.Add(new Vector2(width, 0));
                    uv.Add(new Vector2(width, 1));
                    uv.Add(new Vector2(0, 1));

                    tris.Add(trisIndex + 0);
                    tris.Add(trisIndex + 1);
                    tris.Add(trisIndex + 2);
                    tris.Add(trisIndex + 3);

                    // Add Left
                    vertices.Add(p1 + left);
                    vertices.Add(p1);
                    vertices.Add(p2);
                    vertices.Add(p2 + left);

                    uv.Add(new Vector2(-width, 0));
                    uv.Add(new Vector2(0, 0));
                    uv.Add(new Vector2(0, 1));
                    uv.Add(new Vector2(-width, 1));

                    tris.Add(trisIndex + 4);
                    tris.Add(trisIndex + 5);
                    tris.Add(trisIndex + 6);
                    tris.Add(trisIndex + 7);
                }

                for(int i = 0, length = worldPoints.Length - 2; i < length; i++) {
                    var vertIndex = i * 8;
                    var nextVertIndex = (i + 1) * 8;

                    // Right side stiching
                    var p1 = vertices[vertIndex + 2];
                    var p2 = vertices[nextVertIndex + 1];
                    var average = (p1 + p2) / 2f;
                    vertices[vertIndex + 2] = average;
                    vertices[nextVertIndex + 1] = average;

                    // Left side stiching
                    var p3 = vertices[vertIndex + 7];
                    var p4 = vertices[nextVertIndex + 4];
                    var averageLeft = (p3 + p4) / 2f;
                    vertices[vertIndex + 7] = averageLeft;
                    vertices[nextVertIndex + 4] = averageLeft;
                }

                mesh.Clear();
                mesh.SetVertices(vertices);
                mesh.SetUVs(0, uv);
                mesh.SetIndices(tris, MeshTopology.Quads, 0, false, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }

        private static void Apply(float[,] array, int x, int y, float width, float depth, AnimationCurve depthCurve) {
            var steps = Mathf.CeilToInt(width / 2);
            var halfDist = width / 2f;
            for(int ix = -steps; ix < steps; ix++) {
                for(int iy = -steps; iy < steps; iy++) {
                    var tx = x + ix;
                    var ty = y + iy;
                    if(tx < 0 || ty < 0 || tx >= array.GetLength(0) || ty >= array.GetLength(1))
                        continue;
                    var dist = new Vector2(ix, iy).magnitude;
                    if(dist > halfDist)
                        continue;
                    var d = depthCurve.Evaluate(dist / halfDist) * depth;
                    if(array[tx, ty] < d)
                        array[tx, ty] = d;
                }
            }
        }
    }
}
