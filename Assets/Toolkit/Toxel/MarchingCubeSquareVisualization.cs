using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Toxel.Debugging {
    public class MarchingCubeSquareVisualization : MonoBehaviour {

        #region Variables

        [SerializeField] private Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);

        [Header("Values")]
        [SerializeField, Range(0f, 1f)] private float c000;
        [SerializeField, Range(0f, 1f)] private float c001;
        [SerializeField, Range(0f, 1f)] private float c100;
        [SerializeField, Range(0f, 1f)] private float c101;

        [Space]
        [SerializeField, Range(0f, 1f)] private float c010;
        [SerializeField, Range(0f, 1f)] private float c011;
        [SerializeField, Range(0f, 1f)] private float c110;
        [SerializeField, Range(0f, 1f)] private float c111;

        [Header("Generation Config")]
        [SerializeField] private MarchingCubeUtility.Mode mode = MarchingCubeUtility.Mode.Normal;
        [SerializeField, Range(0f, 1f)] private float threshold = 0.5f;
        [SerializeField] private bool _3x3 = false;
        [SerializeField] private bool generateMesh = false;

        [Header("Visualization")]
        [SerializeField] private Color filledValues = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        [SerializeField] private Color wireframeValues = new Color(0.3f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color marchingCubeTrianglesColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private Color marchingCubeNormalsColor = new Color(0f, 1f, 0f, 1f);
        [SerializeField] private Color marchingCubeMeshColor = new Color(0f, 0f, 0f, 0.4f);
        [SerializeField] private Color debugEdges = Color.clear;
        [SerializeField] private Color edgeLabels = Color.clear;
        [SerializeField] private Color cornerLabels = Color.clear;

        [Header("Debug Values")]
        [SerializeField, Range(0, 255)] private int mask;
        [SerializeField] private bool generateFromEnvironment = false;
        [SerializeField] private bool generateValuesFromMask = false;
        [SerializeField] private bool autoIterate;
        [SerializeField] private string mask_edge;
        [SerializeField] private string[] edge_corner_binding;

        [Header("Table Update")]
        [SerializeField] private int[] edgeTable;

        private List<Vector3> verts = new List<Vector3>();
        private List<int> tris = new List<int>();
        private List<Vector3> norms = new List<Vector3>();
        private List<Triangle> triangles = new List<Triangle>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Color> colors = new List<Color>();
        private GUIStyle edgeLabelStyle;
        private GUIStyle cornerLabelStyle;
        [SerializeField, Readonly] private Mesh m;
        [System.NonSerialized] private double lastIteration;

        #endregion

        #region Properties

        public CornerMask CornerMask {
            get => new CornerMask(MarchingCubeUtility.GetMask(c000, c001, c100, c101, c010, c011, c110, c111));
            set => MarchingCubeUtility.GetValues(value.Mask, out c000, out c001, out c100, out c101, out c010, out c011, out c110, out c111);
        }

        #endregion

        #region Draw

        private void OnDrawGizmosSelected() {
            if(generateValuesFromMask) {
                MarchingCubeUtility.GetValues(mask, out c000, out c001, out c100, out c101, out c010, out c011, out c110, out c111);
            }
            RecalculateMarchingCube();
            if(generateMesh)
                GenerateMesh();

            using(new GizmosUtility.MatrixScope(transform.localToWorldMatrix * Matrix4x4.Translate(center))) {
                DrawFilledValues();
                DrawWireframeValues();
                DrawMarchingCubesTriangles();

                // Debug
                CalculateDebugData();
                DrawDebugEdges();
                DrawDebugCorners();

                if(generateMesh)
                    using(new GizmosUtility.ColorScope(marchingCubeMeshColor))
                        if(!(mask == 0 || mask == 255))
                            Gizmos.DrawMesh(m);
            }

            if(generateValuesFromMask && autoIterate) {
#if UNITY_EDITOR
                if(lastIteration <= double.Epsilon)
                    lastIteration = UnityEditor.EditorApplication.timeSinceStartup;

                if(lastIteration + 1d < UnityEditor.EditorApplication.timeSinceStartup) {
                    lastIteration = UnityEditor.EditorApplication.timeSinceStartup;
                    mask++;
                    if(mask > 255)
                        mask = 0;
                }
#endif
            }
        }

        private void DrawDebugEdges() {
#if UNITY_EDITOR
            var ogmatrix = UnityEditor.Handles.matrix;
            if(edgeLabelStyle == null)
                edgeLabelStyle = new GUIStyle(GUI.skin.label);
            edgeLabelStyle.normal.textColor = edgeLabels;
            UnityEditor.Handles.matrix = Gizmos.matrix;
            using(new GizmosUtility.ColorScope(debugEdges)) {
                var edges = MarchingCubeTables.EDGE_POSITION;
                for(int i = 0, len = edges.Length; i < len; i++) {
                    var c0 = MarchingCubeTables.EDGE_TO_CORNER_A[i];
                    var c1 = MarchingCubeTables.EDGE_TO_CORNER_B[i];
                    Gizmos.DrawLine(
                        MarchingCubeTables.GetVector3Corner(c0, 0.5f),
                        MarchingCubeTables.GetVector3Corner(c1, 0.5f));
                    UnityEditor.Handles.Label(edges[i], $"({i})", edgeLabelStyle);
                }
            }
            UnityEditor.Handles.matrix = ogmatrix;
#endif
        }

        private void DrawDebugCorners() {
#if UNITY_EDITOR
            var ogmatrix = UnityEditor.Handles.matrix;
            if(edgeLabelStyle == null)
                edgeLabelStyle = new GUIStyle(GUI.skin.label);
            edgeLabelStyle.normal.textColor = edgeLabels;
            UnityEditor.Handles.matrix = Gizmos.matrix;
            if(cornerLabelStyle == null)
                cornerLabelStyle = new GUIStyle(GUI.skin.label);
            cornerLabelStyle.normal.textColor = cornerLabels;
            for(int i = 0; i < 8; i++) {
                UnityEditor.Handles.Label(MarchingCubeTables.CORNER_INDEX_TO_POSITION[i], MarchingCubeTables.CORNER_NAME[i], cornerLabelStyle);
            }
            UnityEditor.Handles.matrix = ogmatrix;
#endif
        }

        private void CalculateDebugData() {
            var newMask = MarchingCubeUtility.GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);
            if(mask != newMask) {
                mask = newMask;
                var mtt = MarchingCubeTables.MASK_TO_TRIS[mask];
                mask_edge = string.Join(", ", mtt);
                edge_corner_binding = new string[mtt.Length];
                for(int i = 0, length = mtt.Length; i < length; i++) {
                    var e = mtt[i];
                    var c0 = MarchingCubeTables.EDGE_TO_CORNER_A[e];
                    var c1 = MarchingCubeTables.EDGE_TO_CORNER_B[e];
                    edge_corner_binding[i] = $"{MarchingCubeTables.CORNER_NAME[c0]} <-> {MarchingCubeTables.CORNER_NAME[c1]}";
                }
            }
        }

        private void ClearCache() {
            verts.Clear();
            tris.Clear();
            norms.Clear();
            triangles.Clear();
            uvs.Clear();
            colors.Clear();
        }

        private void AddCube(float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position) {
            MarchingCubeUtility.AddCube(mode, c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verts, tris, norms, uvs, colors);
        }

        private void RecalculateMarchingCube() {
            ClearCache();

            if(_3x3) {
                CubicArray<float> floats = new CubicArray<float>(4);
                floats[1, 1, 1] = c000;
                floats[1, 1, 2] = c001;
                floats[2, 1, 1] = c100;
                floats[2, 1, 2] = c101;

                floats[1, 2, 1] = c010;
                floats[1, 2, 2] = c011;
                floats[2, 2, 1] = c110;
                floats[2, 2, 2] = c111;

                for(int x = 0; x < 3; x++) {
                    for(int y = 0; y < 3; y++) {
                        for(int z = 0; z < 3; z++) {
                            AddCube(
                                floats[x + 0, y + 0, z + 0],
                                floats[x + 0, y + 0, z + 1],
                                floats[x + 1, y + 0, z + 0],
                                floats[x + 1, y + 0, z + 1],

                                floats[x + 0, y + 1, z + 0],
                                floats[x + 0, y + 1, z + 1],
                                floats[x + 1, y + 1, z + 0],
                                floats[x + 1, y + 1, z + 1], threshold, new Vector3(x - 1, y - 1, z - 1));
                        }
                    }
                }
            }
            else {
                AddCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, Vector3.zero);
            }

            for(int i = 0, len = tris.Count; i < len; i += 3) {
                var t = new Triangle(
                    verts[tris[i+0]],
                    verts[tris[i+1]],
                    verts[tris[i+2]]);
                triangles.Add(t);
            }

            if(norms.Count == 0) {
                foreach(var t in triangles) {
                    var n = t.Normal;
                    norms.Add(n);
                    norms.Add(n);
                    norms.Add(n);
                }
            }

            MarchingCubeUtility.SmoothNormals(verts, norms);
        }

        private void GenerateMesh() {
            if(m == null) {
                m = new Mesh();
                m.name = "Marching Cube Debug Mesh";
            }

            m.Clear();
            m.SetVertices(verts, 0, verts.Count,
                UnityEngine.Rendering.MeshUpdateFlags.DontNotifyMeshUsers |
                UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds |
                UnityEngine.Rendering.MeshUpdateFlags.DontResetBoneBounds |
                UnityEngine.Rendering.MeshUpdateFlags.DontValidateIndices);

            m.SetTriangles(tris, 0, false);
            m.SetNormals(norms, 0, norms.Count);
            m.RecalculateBounds();
        }

        private void DrawFilledValues() {
            using(new GizmosUtility.ColorScope(filledValues)) {
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[0], 0.25f * c000);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[1], 0.25f * c001);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[2], 0.25f * c100);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[3], 0.25f * c101);

                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[4], 0.25f * c010);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[5], 0.25f * c011);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[6], 0.25f * c110);
                Gizmos.DrawSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[7], 0.25f * c111);
            }
        }

        private void DrawWireframeValues() {
            using(new GizmosUtility.ColorScope(wireframeValues)) {
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[0], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[1], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[2], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[3], 0.25f);

                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[4], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[5], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[6], 0.25f);
                Gizmos.DrawWireSphere(MarchingCubeTables.CORNER_INDEX_TO_POSITION[7], 0.25f);
            }
        }

        private void DrawMarchingCubesTriangles() {
            using(new GizmosUtility.ColorScope(marchingCubeTrianglesColor)) {
                foreach(var t in triangles) {
                    GizmosUtility.DrawTriangle(t);
                }
            }
            using(new GizmosUtility.ColorScope(marchingCubeNormalsColor)) {
                foreach(var t in triangles) {
                    var c = t.Center;
                    var n = t.Normal;
                    Gizmos.DrawLine(c, c + n * 0.3f);
                }
            }
        }

        #endregion

        #region Util Methods

        [ContextMenu("Rotate / Y Clockwise (Yaw)")]
        private void RotateYClockwise() => CornerMask = CornerMask.RotateYClockwise();

        [ContextMenu("Rotate / Y Counter-Clockwise (Yaw)")]
        private void RotateYCounterClockwise() => CornerMask = CornerMask.RotateYCounterClockwise();


        [ContextMenu("Rotate / X Clockwise (Pitch)")]
        private void RotateXClockwise() => CornerMask = CornerMask.RotateXClockwise();

        [ContextMenu("Rotate / X Counter-Clockwise (Pitch)")]
        private void RotateXCounterClockwise() => CornerMask = CornerMask.RotateXCounterClockwise();


        [ContextMenu("Rotate / Z Clockwise (Roll)")]
        private void RotateZClockwise() => CornerMask = CornerMask.RotateZClockwise();

        [ContextMenu("Rotate / Z Counter-Clockwise (Roll)")]
        private void RotateZCounterClockwise() => CornerMask = CornerMask.RotateZCounterClockwise();

        #endregion

        #region Modification

        [ContextMenu("Load Edge Table")]
        private void LoadEdgeTable() {
            switch(mode) {
                case MarchingCubeUtility.Mode.Normal:
                case MarchingCubeUtility.Mode.Smooth:
                    edgeTable = MarchingCubeTables.MASK_TO_TRIS[mask];
                    break;
                case MarchingCubeUtility.Mode.Toxel:
                case MarchingCubeUtility.Mode.ToxelSmooth:
                    edgeTable = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
                    break;
            }
        }

        [ContextMenu("Update Table")]
        private void UpdateTable() {
            switch(mode) {
                case MarchingCubeUtility.Mode.Normal:
                case MarchingCubeUtility.Mode.Smooth:
                    Debug.LogWarning("Can't update normal/smooth table");
                    break;
                case MarchingCubeUtility.Mode.Toxel:
                case MarchingCubeUtility.Mode.ToxelSmooth:
                    MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask] = edgeTable;
                    MarchingCubeTables.RecalculateNormalsToxel();
                    break;
            }
        }



        [ContextMenu("Update Table (All Rotations)")]
        private void UpdateTableForAllRotations() {
            var og = new EdgeMask(edgeTable);
            switch(mode) {
                case MarchingCubeUtility.Mode.Normal:
                case MarchingCubeUtility.Mode.Smooth:
                    Debug.LogWarning("Can't update normal/smooth table");
                    break;
                case MarchingCubeUtility.Mode.Toxel:
                case MarchingCubeUtility.Mode.ToxelSmooth:
                    // Update default
                    MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask] = edgeTable;
                    var maskthingy = new CornerMask(mask);
                    var iterator = og;

                    // Update Y rotation
                    for(int i = 0; i < 3; i++) {
                        iterator = iterator.RotateYClockwise();
                        maskthingy = maskthingy.RotateYClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }
                    // Update upside down
                    maskthingy = new CornerMask(mask).RotateXClockwise().RotateXClockwise();
                    iterator = og.RotateXClockwise().RotateXClockwise();
                    for(int i = 0; i < 4; i++) {
                        iterator = iterator.RotateYClockwise();
                        maskthingy = maskthingy.RotateYClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }
                    // Update sides
                    // Right
                    maskthingy = new CornerMask(mask).RotateZClockwise();
                    iterator = og.RotateZClockwise();
                    for(int i = 0; i < 4; i++) {
                        maskthingy = maskthingy.RotateXClockwise();
                        iterator = iterator.RotateXClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }
                    // Left
                    maskthingy = new CornerMask(mask).RotateZCounterClockwise();
                    iterator = og.RotateZCounterClockwise();
                    for(int i = 0; i < 4; i++) {
                        maskthingy = maskthingy.RotateXClockwise();
                        iterator = iterator.RotateXClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }
                    // Forward
                    maskthingy = new CornerMask(mask).RotateXClockwise();
                    iterator = og.RotateXClockwise();
                    for(int i = 0; i < 4; i++) {
                        maskthingy = maskthingy.RotateZClockwise();
                        iterator = iterator.RotateZClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }
                    // Back
                    maskthingy = new CornerMask(mask).RotateXCounterClockwise();
                    iterator = og.RotateXCounterClockwise();
                    for(int i = 0; i < 4; i++) {
                        maskthingy = maskthingy.RotateZClockwise();
                        iterator = iterator.RotateZClockwise();
                        MarchingCubeTables.MASK_TO_TRIS_TOXEL[maskthingy.Mask] = iterator.Edges;
                    }

                    MarchingCubeTables.RecalculateNormalsToxel();
                    break;
            }
        }

        [ContextMenu("Print Updated Toxel Array")]
        private void PrintToxelArray() {
            StringBuilder sb = new StringBuilder();
            var arr = MarchingCubeTables.MASK_TO_TRIS_TOXEL;
            for(int i = 0; i < 256; i++) {
                sb.AppendLine($"new int[]{{{string.Join(", ", arr[i])}}},");
            }
            System.IO.File.WriteAllText("toxelEdge.txt", sb.ToString());
        }

        #endregion
    }
}
