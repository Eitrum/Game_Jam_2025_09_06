using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolkit.Toxel {
    public static class MarchingCubeTables {
        #region Structs

        public struct V3 {
            public float x, y, z;

            public V3(float x, float y, float z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static implicit operator Vector3(V3 v) => new Vector3(v.x, v.y, v.z);
            public static implicit operator V3(Vector3 v) => new V3(v.x, v.y, v.z);
        }

        #endregion

        #region Variables

        private const string TAG = ColorTable.RichTextTags.GREY + "[Toolkit.Toxel.MarchingCubeTables]</color> - ";
        private static bool LOG_INIT_TIME = true; // Keep static to remove unreachable code warnings

        public static readonly V3[][] MASK_TO_NORMALS = null;
        public static readonly Toolkit.Mathematics.Triangle[][] MASK_TO_TRIANGLES = null;

        public static readonly V3[][] MASK_TO_NORMALS_TOXEL = null;
        public static readonly Toolkit.Mathematics.Triangle[][] MASK_TO_TRIANGLES_TOXEL = null;

        #endregion

        #region Init

        static MarchingCubeTables() {
            Stopwatch sw = Stopwatch.StartNew();
            MASK_TO_NORMALS = new V3[256][];
            MASK_TO_TRIANGLES = new Toolkit.Mathematics.Triangle[256][];
            MASK_TO_NORMALS_TOXEL = new V3[256][];
            MASK_TO_TRIANGLES_TOXEL = new Toolkit.Mathematics.Triangle[256][];
            RecalculateNormals();
            RecalculateNormalsToxel();

            if(LOG_INIT_TIME)
                Debug.Log(TAG + "Total::" + sw.GetMillisecondsFormatted());
        }

        public static void RecalculateNormalsToxel() {
            Stopwatch sw = Stopwatch.StartNew();
            for(int mask = 0; mask < 256; mask++) {
                var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
                int index = 0;
                var arr = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask] = new V3[tris.Length / 3];
                var arr2 = MarchingCubeTables.MASK_TO_TRIANGLES_TOXEL[mask] = new Toolkit.Mathematics.Triangle[tris.Length / 3];

                for(int i = 0, length = tris.Length; i < length; i += 3) {
                    var edge0 = tris[i+0];
                    var edge1 = tris[i+1];
                    var edge2 = tris[i+2];
                    var value0 = MarchingCubeTables.EDGE_POSITION[edge0];
                    var value1 = MarchingCubeTables.EDGE_POSITION[edge1];
                    var value2 = MarchingCubeTables.EDGE_POSITION[edge2];

                    var triangle = new Toolkit.Mathematics.Triangle(value0, value1, value2);
                    var norm = triangle.Normal;
                    arr[index] = norm;
                    arr2[index++] = triangle;
                }
            }
            if(LOG_INIT_TIME)
                Debug.Log(TAG + "PreCacheTrianglesAndNormalsToxel::" + sw.GetMillisecondsFormatted());
        }

        public static void RecalculateNormals() {
            Stopwatch sw = Stopwatch.StartNew();
            for(int mask = 0; mask < 256; mask++) {
                var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
                int index = 0;
                var arr = MarchingCubeTables.MASK_TO_NORMALS[mask] = new V3[tris.Length / 3];
                var arr2 = MarchingCubeTables.MASK_TO_TRIANGLES[mask] = new Toolkit.Mathematics.Triangle[tris.Length / 3];

                for(int i = 0, length = tris.Length; i < length; i += 3) {
                    var edge0 = tris[i+0];
                    var edge1 = tris[i+1];
                    var edge2 = tris[i+2];
                    var value0 = MarchingCubeTables.EDGE_POSITION[edge0];
                    var value1 = MarchingCubeTables.EDGE_POSITION[edge1];
                    var value2 = MarchingCubeTables.EDGE_POSITION[edge2];

                    var triangle = new Toolkit.Mathematics.Triangle(value0, value1, value2);
                    var norm = triangle.Normal;
                    arr[index] = norm;
                    arr2[index++] = triangle;
                }
            }
            if(LOG_INIT_TIME)
                Debug.Log(TAG + "PreCacheNormals::" + sw.GetMillisecondsFormatted());
        }

        #endregion

        #region TABLES

        public static V3[] CORNER_INDEX_TO_POSITION = new V3[] {
            new V3(-0.5f, -0.5f, -0.5f),    // c000
            new V3(-0.5f, -0.5f, 0.5f),     // c001
            new V3(0.5f, -0.5f, -0.5f),     // c100
            new V3(0.5f, -0.5f, 0.5f),      // c101

            new V3(-0.5f, 0.5f, -0.5f) ,    // c010
            new V3(-0.5f, 0.5f, 0.5f),      // c011
            new V3(0.5f, 0.5f,-0.5f),       // c110
            new V3(0.5f, 0.5f, 0.5f)};      // c111

        public static V3[] EDGE_POSITION = new V3[]{
            new V3(-0.5f, -0.5f, 0f), new V3(0f, -0.5f, -0.5f), new V3(0.5f, -0.5f, 0f), new V3(0f, -0.5f, 0.5f),
            new V3(-0.5f, 0.5f, 0f), new V3(0f, 0.5f, -0.5f), new V3(0.5f, 0.5f, 0f), new V3(0f, 0.5f, 0.5f),
            new V3(-0.5f, 0f, -0.5f), new V3(-0.5f, 0f, 0.5f), new V3(0.5f, 0f, -0.5f), new V3(0.5f, 0f, 0.5f),
        };

        public static int[] EDGE_TO_CORNER_A = new int[]{
            0, 0, 2, 1,
            4, 4, 6, 5,
            0, 1, 2, 3
        };

        public static int[] EDGE_TO_CORNER_B = new int[]{
            1, 2, 3, 3,
            5, 6, 7, 7,
            4, 5, 6, 7
        };

        public static string[] CORNER_NAME = new string[]{
            "c000",
            "c001",
            "c100",
            "c101",

            "c010",
            "c011",
            "c110",
            "c111"
        };

        public static int[][] MASK_TO_TRIS = new int[][] {
            new int[]{},
            new int[]{0, 1, 8},
            new int[]{3, 0, 9},
            new int[]{1, 8, 9, 3, 1, 9},
            new int[]{2, 10, 1},
            new int[]{2, 10, 0, 0, 10, 8},
            new int[]{2, 10, 1, 0, 9, 3},
            new int[]{2, 10, 9, 3, 2, 9, 10, 8, 9},
            new int[]{2, 3, 11},
            new int[]{3, 11, 2, 0, 1, 8},
            new int[]{0, 9, 11, 2, 0, 11},
            new int[]{1, 8, 11, 2, 1, 11, 8, 9, 11},
            new int[]{3, 11, 10, 1, 3, 10},
            new int[]{3, 11, 8, 0, 3, 8, 11, 10, 8},
            new int[]{0, 9, 10, 1, 0, 10, 9, 11, 10},
            new int[]{9, 11, 10, 8, 9, 10},
            new int[]{8, 5, 4},
            new int[]{1, 5, 4, 0, 1, 4},
            new int[]{8, 5, 4, 3, 0, 9},
            new int[]{9, 3, 5, 4, 9, 5, 3, 1, 5},
            new int[]{8, 5, 4, 2, 10, 1},
            new int[]{5, 4, 2, 5, 2, 10, 4, 0, 2},
            new int[]{5, 4, 8, 2, 10, 1, 0, 9, 3},
            new int[]{2, 5, 4, 3, 2, 4, 2, 10, 5, 3, 4, 9},
            new int[]{8, 5, 4, 3, 11, 2},
            new int[]{1, 5, 4, 0, 1, 4, 3, 11, 2},
            new int[]{8, 5, 4, 0, 9, 11, 2, 0, 11},
            new int[]{1, 5, 4, 2, 1, 11, 11, 1, 4, 11, 4, 9},
            new int[]{3, 11, 10, 1, 3, 10, 8, 5, 4},
            new int[]{0, 3, 11, 0, 11, 5, 0, 5, 4, 11, 10, 5},
            new int[]{0, 9, 10, 1, 0, 10, 9, 11, 10, 8, 5, 4},
            new int[]{9, 11, 10, 9, 10, 5, 9, 5, 4},
            new int[]{9, 4, 7},
            new int[]{9, 4, 7, 1, 8, 0},
            new int[]{0, 4, 7, 3, 0, 7},
            new int[]{1, 8, 4, 3, 1, 4, 4, 7, 3},
            new int[]{2, 10, 1, 9, 4, 7},
            new int[]{2, 10, 8, 0, 2, 8, 9, 4, 7},
            new int[]{0, 4, 7, 7, 3, 0, 2, 10, 1},
            new int[]{3, 4, 7, 3, 2, 10, 3, 10, 4, 10, 8, 4},
            new int[]{9, 4, 7, 3, 11, 2},
            new int[]{9, 4, 7, 3, 11, 2, 1, 8, 0},
            new int[]{11, 2, 4, 4, 7, 11, 2, 0, 4},
            new int[]{1, 4, 7, 7, 2, 1, 2, 7, 11, 1, 8, 4},
            new int[]{3, 11, 10, 1, 3, 10, 9, 4, 7},
            new int[]{9, 4, 7, 3, 11, 8, 0, 3, 8, 11, 10, 8},
            new int[]{0, 4, 7, 1, 0, 10, 10, 0, 7, 10, 7, 11},
            new int[]{10, 8, 11, 8, 4, 7, 7, 11, 8},
            new int[]{8, 5, 7, 9, 8, 7},
            new int[]{1, 5, 7, 0, 1, 7, 9, 0, 7},
            new int[]{5, 7, 3, 8, 5, 3, 0, 8, 3},
            new int[]{1, 5, 7, 3, 1, 7},
            new int[]{8, 5, 7, 9, 8, 7, 2, 10, 1},
            new int[]{5, 7, 9, 10, 5, 2, 2, 5, 9, 2, 9, 0},
            new int[]{8, 5, 3, 0, 8, 3, 5, 7, 3, 1, 2, 10},
            new int[]{5, 7, 3, 5, 3, 2, 5, 2, 10},
            new int[]{8, 5, 7, 9, 8, 7, 3, 11, 2},
            new int[]{0, 1, 7, 9, 0, 7, 1, 5, 7, 3, 11, 2},
            new int[]{0, 8, 5, 0, 5, 11, 0, 11, 2, 5, 7, 11},
            new int[]{1, 5, 7, 1, 7, 11, 1, 11, 2},
            new int[]{8, 5, 7, 9, 8, 7, 3, 11, 10, 1, 3, 10},
            new int[]{0, 3, 9, 10, 5, 7, 11, 10, 7, 9, 3, 11, 11, 7, 9},
            new int[]{10, 5, 7, 11, 10, 7, 1, 0, 8, 1, 8, 5, 5, 10, 1},
            new int[]{10, 5, 7, 11, 10, 7},
            new int[]{10, 6, 5},
            new int[]{1, 8, 0, 10, 6, 5},
            new int[]{9, 3, 0, 5, 10, 6},
            new int[]{1, 8, 9, 3, 1, 9, 10, 6, 5},
            new int[]{2, 6, 5, 1, 2, 5},
            new int[]{0, 2, 6, 8, 0, 6, 5, 8, 6},
            new int[]{2, 6, 5, 1, 2, 5, 3, 0, 9},
            new int[]{2, 6, 5, 3, 2, 9, 9, 2, 5, 9, 5, 8},
            new int[]{6, 5, 10, 2, 3, 11},
            new int[]{6, 5, 10, 3, 11, 2, 1, 8, 0},
            new int[]{0, 9, 11, 2, 0, 11, 10, 6, 5},
            new int[]{1, 8, 11, 2, 1, 11, 8, 9, 11, 10, 6, 5},
            new int[]{6, 5, 3, 11, 6, 3, 5, 1, 3},
            new int[]{3, 6, 5, 0, 3, 5, 3, 11, 6, 0, 5, 8},
            new int[]{1, 0, 9, 1, 9, 6, 1, 6, 5, 9, 11, 6},
            new int[]{8, 9, 11, 8, 11, 6, 8, 6, 5},
            new int[]{4, 8, 10, 6, 4, 10},
            new int[]{10, 6, 0, 1, 10, 0, 6, 4, 0},
            new int[]{8, 10, 6, 4, 8, 6, 9, 3, 0},
            new int[]{4, 9, 3, 4, 3, 10, 4, 10, 6, 3, 1, 10},
            new int[]{1, 2, 4, 8, 1, 4, 2, 6, 4},
            new int[]{2, 6, 4, 0, 2, 4},
            new int[]{1, 2, 4, 8, 1, 4, 2, 6, 4, 0, 9, 3},
            new int[]{2, 6, 4, 2, 4, 9, 2, 9, 3},
            new int[]{6, 4, 8, 10, 6, 8, 2, 3, 11},
            new int[]{10, 6, 0, 1, 10, 0, 6, 4, 0, 2, 3, 11},
            new int[]{0, 9, 11, 2, 0, 11, 10, 6, 4, 8, 10, 4},
            new int[]{1, 10, 2, 4, 9, 11, 6, 4, 11, 10, 6, 2, 6, 11, 2},
            new int[]{6, 4, 8, 11, 6, 3, 3, 6, 8, 3, 8, 1},
            new int[]{6, 4, 0, 6, 0, 3, 6, 3, 11},
            new int[]{1, 0, 8, 11, 6, 4, 9, 11, 4, 4, 8, 0, 0, 9, 4},
            new int[]{4, 9, 11, 6, 4, 11},
            new int[]{7, 9, 4, 5, 10, 6},
            new int[]{10, 6, 5, 0, 1, 8, 4, 7, 9},
            new int[]{7, 3, 0, 4, 7, 0, 5, 10, 6},
            new int[]{4, 7, 1, 8, 4, 1, 7, 3, 1, 5, 10, 6},
            new int[]{5, 1, 2, 6, 5, 2, 7, 9, 4},
            new int[]{8, 0, 6, 5, 8, 6, 0, 2, 6, 4, 7, 9},
            new int[]{4, 7, 3, 0, 4, 3, 1, 2, 6, 5, 1, 6},
            new int[]{8, 4, 5, 3, 2, 6, 7, 3, 6, 4, 7, 6, 4, 6, 5},
            new int[]{9, 4, 7, 2, 3, 11, 6, 5, 10},
            new int[]{10, 6, 5, 1, 8, 0, 9, 4, 7, 3, 11, 2},
            new int[]{11, 2, 4, 7, 11, 4, 2, 0, 4, 6, 5, 10},
            new int[]{1, 4, 7, 7, 2, 1, 2, 7, 11, 1, 8, 4, 6, 5, 10},
            new int[]{6, 5, 3, 11, 6, 3, 5, 1, 3, 7, 9, 4},
            new int[]{3, 6, 5, 0, 3, 5, 3, 11, 6, 0, 5, 8, 4, 7, 9},
            new int[]{11, 6, 7, 1, 0, 4, 5, 1, 4, 5, 4, 7, 7, 6, 5},
            new int[]{6, 7, 11, 4, 5, 8, 7, 6, 5, 5, 4, 7},
            new int[]{7, 9, 10, 6, 7, 10, 9, 8, 10},
            new int[]{0, 10, 6, 9, 0, 6, 0, 1, 10, 9, 6, 7},
            new int[]{8, 10, 6, 0, 8, 3, 3, 8, 6, 3, 6, 7},
            new int[]{7, 3, 1, 7, 1, 10, 7, 10, 6},
            new int[]{6, 7, 9, 6, 9, 1, 6, 1, 2, 9, 8, 1},
            new int[]{0, 2, 6, 0, 6, 7, 0, 7, 9},
            new int[]{8, 1, 0, 6, 7, 3, 2, 6, 3, 1, 2, 0, 2, 3, 0},
            new int[]{6, 7, 3, 2, 6, 3},
            new int[]{7, 9, 10, 6, 7, 10, 9, 8, 10, 11, 2, 3},
            new int[]{1, 10, 6, 1, 6, 7, 0, 1, 7, 9, 0, 7, 3, 11, 2},
            new int[]{7, 11, 6, 0, 8, 10, 2, 0, 10, 10, 6, 11, 11, 2, 10},
            new int[]{10, 2, 1, 11, 6, 7, 11, 2, 6, 10, 6, 2},
            new int[]{6, 7, 11, 8, 1, 3, 9, 8, 3, 7, 9, 3, 3, 11, 7},
            new int[]{7, 11, 6, 3, 9, 0, 3, 11, 7, 3, 7, 9},
            new int[]{6, 7, 11, 8, 1, 0},
            new int[]{6, 7, 11},
            new int[]{6, 11, 7},
            new int[]{1, 8, 0, 11, 7, 6},
            new int[]{3, 0, 9, 7, 6, 11},
            new int[]{3, 1, 8, 9, 3, 8, 7, 6, 11},
            new int[]{11, 7, 6, 10, 1, 2},
            new int[]{10, 8, 0, 2, 10, 0, 11, 7, 6},
            new int[]{7, 6, 11, 0, 9, 3, 2, 10, 1},
            new int[]{2, 10, 9, 3, 2, 9, 10, 8, 9, 11, 7, 6},
            new int[]{7, 6, 2, 3, 7, 2},
            new int[]{3, 7, 6, 2, 3, 6, 1, 8, 0},
            new int[]{7, 6, 0, 9, 7, 0, 6, 2, 0},
            new int[]{2, 1, 8, 2, 8, 7, 2, 7, 6, 8, 9, 7},
            new int[]{10, 1, 7, 6, 10, 7, 1, 3, 7},
            new int[]{10, 8, 0, 6, 10, 7, 7, 10, 0, 7, 0, 3},
            new int[]{0, 7, 6, 1, 0, 6, 0, 9, 7, 1, 6, 10},
            new int[]{10, 8, 9, 10, 9, 7, 10, 7, 6},
            new int[]{4, 8, 5, 6, 11, 7},
            new int[]{4, 0, 1, 5, 4, 1, 6, 11, 7},
            new int[]{8, 5, 4, 3, 0, 9, 7, 6, 11},
            new int[]{9, 3, 5, 4, 9, 5, 3, 1, 5, 7, 6, 11},
            new int[]{1, 2, 10, 4, 8, 5, 6, 11, 7},
            new int[]{5, 4, 2, 10, 5, 2, 4, 0, 2, 6, 11, 7},
            new int[]{5, 4, 8, 10, 1, 2, 3, 0, 9, 11, 7, 6},
            new int[]{2, 5, 4, 3, 2, 4, 2, 10, 5, 3, 4, 9, 7, 6, 11},
            new int[]{6, 2, 3, 7, 6, 3, 4, 8, 5},
            new int[]{5, 4, 0, 1, 5, 0, 2, 3, 7, 6, 2, 7},
            new int[]{7, 6, 0, 9, 7, 0, 6, 2, 0, 4, 8, 5},
            new int[]{9, 7, 4, 2, 1, 5, 6, 2, 5, 4, 7, 6, 6, 5, 4},
            new int[]{10, 1, 7, 6, 10, 7, 1, 3, 7, 5, 4, 8},
            new int[]{10, 5, 6, 0, 3, 7, 4, 0, 7, 5, 4, 6, 4, 7, 6},
            new int[]{0, 7, 6, 1, 0, 6, 0, 9, 7, 1, 6, 10, 5, 4, 8},
            new int[]{5, 6, 10, 7, 4, 9, 4, 7, 6, 6, 5, 4},
            new int[]{9, 4, 6, 11, 9, 6},
            new int[]{4, 6, 11, 9, 4, 11, 0, 1, 8},
            new int[]{3, 0, 6, 11, 3, 6, 0, 4, 6},
            new int[]{4, 6, 11, 8, 4, 1, 1, 4, 11, 1, 11, 3},
            new int[]{11, 9, 4, 6, 11, 4, 10, 1, 2},
            new int[]{9, 4, 6, 11, 9, 6, 2, 10, 8, 0, 2, 8},
            new int[]{3, 0, 6, 11, 3, 6, 0, 4, 6, 2, 10, 1},
            new int[]{3, 2, 11, 8, 4, 6, 10, 8, 6, 11, 10, 6, 11, 2, 10},
            new int[]{9, 4, 2, 3, 9, 2, 4, 6, 2},
            new int[]{9, 4, 2, 3, 9, 2, 4, 6, 2, 0, 1, 8},
            new int[]{6, 2, 0, 4, 6, 0},
            new int[]{4, 6, 2, 4, 2, 1, 4, 1, 8},
            new int[]{6, 10, 1, 6, 1, 9, 6, 9, 4, 1, 3, 9},
            new int[]{3, 9, 0, 6, 10, 8, 4, 6, 8, 9, 4, 8, 8, 0, 9},
            new int[]{0, 4, 6, 0, 6, 10, 0, 10, 1},
            new int[]{8, 4, 6, 10, 8, 6},
            new int[]{6, 11, 8, 5, 6, 8, 11, 9, 8},
            new int[]{5, 6, 11, 5, 11, 0, 5, 0, 1, 11, 9, 0},
            new int[]{3, 8, 5, 11, 3, 5, 3, 0, 8, 11, 5, 6},
            new int[]{3, 1, 5, 3, 5, 6, 3, 6, 11},
            new int[]{6, 11, 8, 5, 6, 8, 11, 9, 8, 10, 1, 2},
            new int[]{5, 6, 10, 9, 0, 2, 11, 9, 2, 6, 11, 10, 11, 2, 10},
            new int[]{3, 8, 5, 11, 3, 5, 3, 0, 8, 11, 5, 6, 2, 10, 1},
            new int[]{2, 11, 3, 6, 10, 5, 10, 6, 11, 11, 2, 10},
            new int[]{9, 8, 5, 3, 9, 2, 2, 9, 5, 2, 5, 6},
            new int[]{9, 0, 3, 5, 6, 2, 1, 5, 2, 0, 1, 2, 2, 3, 0},
            new int[]{6, 2, 0, 6, 0, 8, 6, 8, 5},
            new int[]{5, 6, 2, 1, 5, 2},
            new int[]{6, 10, 5, 3, 9, 8, 1, 3, 8, 5, 10, 1, 1, 8, 5},
            new int[]{0, 3, 9, 10, 5, 6},
            new int[]{8, 1, 0, 10, 5, 6, 1, 8, 5, 5, 10, 1},
            new int[]{5, 6, 10},
            new int[]{5, 10, 11, 7, 5, 11},
            new int[]{10, 11, 7, 5, 10, 7, 8, 0, 1},
            new int[]{7, 5, 10, 11, 7, 10, 3, 0, 9},
            new int[]{1, 8, 9, 3, 1, 9, 11, 7, 5, 10, 11, 5},
            new int[]{11, 7, 1, 2, 11, 1, 7, 5, 1},
            new int[]{2, 11, 7, 2, 7, 8, 2, 8, 0, 7, 5, 8},
            new int[]{11, 7, 1, 2, 11, 1, 7, 5, 1, 3, 0, 9},
            new int[]{2, 11, 3, 5, 8, 9, 7, 5, 9, 11, 7, 9, 9, 3, 11},
            new int[]{2, 3, 5, 10, 2, 5, 3, 7, 5},
            new int[]{2, 3, 5, 10, 2, 5, 3, 7, 5, 1, 8, 0},
            new int[]{7, 5, 10, 9, 7, 0, 0, 7, 10, 0, 10, 2},
            new int[]{2, 1, 10, 9, 7, 5, 8, 9, 5, 10, 8, 5, 10, 1, 8},
            new int[]{3, 7, 5, 1, 3, 5},
            new int[]{3, 7, 5, 3, 5, 8, 3, 8, 0},
            new int[]{7, 5, 1, 7, 1, 0, 7, 0, 9},
            new int[]{5, 8, 9, 7, 5, 9},
            new int[]{4, 8, 11, 7, 4, 11, 8, 10, 11},
            new int[]{4, 0, 1, 7, 4, 11, 11, 4, 1, 11, 1, 10},
            new int[]{4, 8, 11, 7, 4, 11, 8, 10, 11, 9, 3, 0},
            new int[]{4, 9, 7, 1, 10, 11, 3, 1, 11, 11, 7, 9, 9, 3, 11},
            new int[]{4, 1, 2, 7, 4, 2, 4, 8, 1, 7, 2, 11},
            new int[]{4, 0, 2, 4, 2, 11, 4, 11, 7},
            new int[]{4, 1, 2, 7, 4, 2, 4, 8, 1, 7, 2, 11, 0, 9, 3},
            new int[]{11, 3, 2, 9, 7, 4, 11, 7, 9, 9, 3, 11},
            new int[]{7, 4, 8, 7, 8, 2, 7, 2, 3, 8, 10, 2},
            new int[]{10, 2, 1, 7, 4, 0, 3, 7, 0, 2, 0, 1, 0, 2, 3},
            new int[]{7, 4, 9, 10, 2, 0, 8, 10, 0, 4, 8, 0, 0, 9, 4},
            new int[]{4, 9, 7, 1, 10, 2},
            new int[]{1, 3, 7, 1, 7, 4, 1, 4, 8},
            new int[]{7, 4, 0, 3, 7, 0},
            new int[]{4, 9, 7, 0, 8, 1, 9, 4, 8, 8, 0, 9},
            new int[]{4, 9, 7},
            new int[]{5, 10, 9, 4, 5, 9, 10, 11, 9},
            new int[]{5, 10, 9, 4, 5, 9, 10, 11, 9, 8, 0, 1},
            new int[]{4, 5, 10, 4, 10, 3, 4, 3, 0, 10, 11, 3},
            new int[]{4, 5, 8, 11, 3, 1, 10, 11, 1, 5, 10, 1, 1, 8, 5},
            new int[]{5, 1, 2, 4, 5, 9, 9, 5, 2, 9, 2, 11},
            new int[]{5, 8, 4, 2, 11, 9, 0, 2, 9, 9, 4, 8, 8, 0, 9},
            new int[]{11, 3, 2, 4, 5, 1, 0, 4, 1, 2, 3, 0, 0, 1, 2},
            new int[]{2, 11, 3, 5, 8, 4},
            new int[]{2, 9, 4, 10, 2, 4, 2, 3, 9, 10, 4, 5},
            new int[]{2, 9, 4, 10, 2, 4, 2, 3, 9, 10, 4, 5, 0, 1, 8},
            new int[]{2, 0, 4, 2, 4, 5, 2, 5, 10},
            new int[]{8, 4, 5, 1, 8, 5, 1, 5, 10, 1, 10, 2},
            new int[]{5, 1, 3, 5, 3, 9, 5, 9, 4},
            new int[]{9, 0, 3, 8, 4, 5, 4, 8, 0, 0, 9, 4},
            new int[]{4, 5, 1, 0, 4, 1},
            new int[]{4, 5, 8},
            new int[]{11, 9, 8, 10, 11, 8},
            new int[]{10, 11, 9, 10, 9, 0, 10, 0, 1},
            new int[]{8, 10, 11, 8, 11, 3, 8, 3, 0},
            new int[]{1, 10, 11, 3, 1, 11},
            new int[]{11, 9, 8, 11, 8, 1, 11, 1, 2},
            new int[]{9, 0, 2, 11, 9, 2},
            new int[]{3, 2, 11, 1, 0, 8, 2, 3, 0, 0, 1, 2},
            new int[]{2, 11, 3},
            new int[]{9, 8, 10, 9, 10, 2, 9, 2, 3},
            new int[]{0, 3, 9, 2, 1, 10, 0, 1, 2, 2, 3, 0},
            new int[]{0, 8, 10, 2, 0, 10},
            new int[]{1, 10, 2},
            new int[]{8, 1, 3, 9, 8, 3},
            new int[]{0, 3, 9},
            new int[]{0, 8, 1},
            new int[]{}
        };

        public static int[][] MASK_TO_TRIS_TOXEL = new int[][] {
new int[]{},
new int[]{0, 1, 8},
new int[]{3, 0, 9},
new int[]{1, 8, 9, 3, 1, 9},
new int[]{2, 10, 1},
new int[]{2, 10, 0, 0, 10, 8},
new int[]{10, 9, 3, 3, 2, 10, 10, 1, 0, 0, 9, 10},
new int[]{2, 10, 9, 3, 2, 9, 10, 8, 9},
new int[]{2, 3, 11},
new int[]{11, 8, 0, 0, 3, 11, 11, 2, 1, 1, 8, 11},
new int[]{0, 9, 11, 2, 0, 11},
new int[]{1, 8, 11, 2, 1, 11, 8, 9, 11},
new int[]{3, 11, 10, 1, 3, 10},
new int[]{3, 11, 8, 0, 3, 8, 11, 10, 8},
new int[]{0, 9, 10, 1, 0, 10, 9, 11, 10},
new int[]{9, 11, 10, 8, 9, 10},
new int[]{8, 5, 4},
new int[]{1, 5, 4, 0, 1, 4},
new int[]{3, 5, 4, 4, 9, 3, 3, 0, 8, 8, 5, 3},
new int[]{9, 3, 5, 4, 9, 5, 3, 1, 5},
new int[]{2, 4, 8, 8, 1, 2, 2, 10, 5, 5, 4, 2},
new int[]{5, 4, 2, 5, 2, 10, 4, 0, 2},
new int[]{0, 8, 1, 3, 2, 10, 10, 9, 3, 9, 10, 5, 5, 4, 9},
new int[]{2, 5, 4, 3, 2, 4, 2, 10, 5, 3, 4, 9},
new int[]{3, 11, 4, 4, 8, 3, 11, 5, 4, 5, 2, 8, 5, 11, 2, 2, 3, 8},
new int[]{11, 5, 4, 0, 3, 11, 0, 11, 4, 1, 5, 11, 11, 2, 1},
new int[]{5, 11, 2, 0, 8, 5, 0, 5, 2, 9, 11, 5, 5, 4, 9},
new int[]{11, 5, 4, 2, 1, 11, 11, 1, 5, 11, 4, 9},
new int[]{4, 3, 11, 10, 5, 4, 10, 4, 11, 1, 3, 4, 4, 8, 1},
new int[]{0, 3, 11, 0, 11, 5, 0, 5, 4, 11, 10, 5},
new int[]{8, 1, 0, 4, 9, 10, 5, 4, 10, 9, 11, 10},
new int[]{9, 11, 10, 9, 10, 5, 9, 5, 4},
new int[]{9, 4, 7},
new int[]{7, 1, 8, 8, 4, 7, 7, 9, 0, 0, 1, 7},
new int[]{0, 4, 7, 3, 0, 7},
new int[]{1, 8, 4, 3, 1, 4, 4, 7, 3},
new int[]{7, 9, 2, 2, 10, 7, 9, 1, 2, 1, 4, 10, 1, 9, 4, 4, 7, 10},
new int[]{7, 2, 10, 8, 4, 7, 8, 7, 10, 0, 2, 7, 7, 9, 0},
new int[]{10, 4, 7, 3, 2, 10, 3, 10, 7, 0, 4, 10, 10, 1, 0},
new int[]{3, 4, 7, 3, 2, 10, 3, 10, 4, 10, 8, 4},
new int[]{4, 2, 3, 3, 9, 4, 4, 7, 11, 11, 2, 4},
new int[]{0, 3, 9, 8, 4, 7, 7, 1, 8, 1, 7, 11, 11, 2, 1},
new int[]{11, 2, 4, 4, 7, 11, 2, 0, 4},
new int[]{1, 4, 7, 7, 2, 1, 2, 7, 11, 1, 8, 4},
new int[]{4, 10, 1, 3, 9, 4, 3, 4, 1, 11, 10, 4, 4, 7, 11},
new int[]{9, 0, 3, 7, 11, 8, 4, 7, 8, 11, 10, 8},
new int[]{10, 4, 7, 1, 0, 10, 10, 0, 4, 10, 7, 11},
new int[]{10, 8, 11, 8, 4, 7, 7, 11, 8},
new int[]{8, 5, 7, 9, 8, 7},
new int[]{1, 5, 7, 0, 1, 7, 9, 0, 7},
new int[]{5, 7, 3, 8, 5, 3, 0, 8, 3},
new int[]{1, 5, 7, 3, 1, 7},
new int[]{2, 7, 9, 8, 1, 2, 8, 2, 9, 5, 7, 2, 2, 10, 5},
new int[]{7, 2, 10, 9, 0, 7, 7, 0, 2, 7, 10, 5},
new int[]{1, 0, 8, 10, 5, 3, 2, 10, 3, 5, 7, 3},
new int[]{5, 7, 3, 5, 3, 2, 5, 2, 10},
new int[]{2, 8, 5, 7, 11, 2, 7, 2, 5, 9, 8, 2, 2, 3, 9},
new int[]{3, 9, 0, 2, 1, 7, 11, 2, 7, 1, 5, 7},
new int[]{0, 8, 5, 0, 5, 11, 0, 11, 2, 5, 7, 11},
new int[]{1, 5, 7, 1, 7, 11, 1, 11, 2},
new int[]{7, 11, 10, 5, 7, 10, 8, 1, 3, 9, 8, 3},
new int[]{0, 3, 9, 10, 5, 7, 11, 10, 7},
new int[]{0, 8, 1, 7, 11, 10, 5, 7, 10},
new int[]{10, 5, 7, 11, 10, 7},
new int[]{10, 6, 5},
new int[]{0, 6, 5, 5, 8, 0, 0, 1, 10, 10, 6, 0},
new int[]{9, 3, 6, 6, 5, 9, 3, 10, 6, 10, 0, 5, 10, 3, 0, 0, 9, 5},
new int[]{6, 9, 3, 1, 10, 6, 1, 6, 3, 8, 9, 6, 6, 5, 8},
new int[]{2, 6, 5, 1, 2, 5},
new int[]{0, 2, 6, 8, 0, 6, 5, 8, 6},
new int[]{9, 6, 5, 1, 0, 9, 1, 9, 5, 2, 6, 9, 9, 3, 2},
new int[]{9, 6, 5, 3, 2, 9, 9, 2, 6, 9, 5, 8},
new int[]{3, 5, 10, 10, 2, 3, 3, 11, 6, 6, 5, 3},
new int[]{2, 1, 10, 11, 6, 5, 5, 3, 11, 3, 5, 8, 8, 0, 3},
new int[]{5, 0, 9, 11, 6, 5, 11, 5, 9, 2, 0, 5, 5, 10, 2},
new int[]{10, 2, 1, 5, 8, 11, 6, 5, 11, 8, 9, 11},
new int[]{6, 5, 3, 11, 6, 3, 5, 1, 3},
new int[]{3, 6, 5, 0, 3, 5, 3, 11, 6, 0, 5, 8},
new int[]{1, 0, 9, 1, 9, 6, 1, 6, 5, 9, 11, 6},
new int[]{8, 9, 11, 8, 11, 6, 8, 6, 5},
new int[]{4, 8, 10, 6, 4, 10},
new int[]{10, 6, 0, 1, 10, 0, 6, 4, 0},
new int[]{3, 10, 6, 4, 9, 3, 4, 3, 6, 8, 10, 3, 3, 0, 8},
new int[]{4, 9, 3, 4, 3, 10, 4, 10, 6, 3, 1, 10},
new int[]{1, 2, 4, 8, 1, 4, 2, 6, 4},
new int[]{2, 6, 4, 0, 2, 4},
new int[]{0, 8, 1, 3, 2, 4, 9, 3, 4, 2, 6, 4},
new int[]{2, 6, 4, 2, 4, 9, 2, 9, 3},
new int[]{3, 4, 8, 10, 2, 3, 10, 3, 8, 6, 4, 3, 3, 11, 6},
new int[]{2, 1, 10, 11, 6, 0, 3, 11, 0, 6, 4, 0},
new int[]{0, 8, 10, 2, 0, 10, 11, 6, 4, 9, 11, 4},
new int[]{1, 10, 2, 4, 9, 11, 6, 4, 11},
new int[]{3, 4, 8, 11, 6, 3, 3, 6, 4, 3, 8, 1},
new int[]{6, 4, 0, 6, 0, 3, 6, 3, 11},
new int[]{1, 0, 8, 11, 6, 4, 9, 11, 4},
new int[]{4, 9, 11, 6, 4, 11},
new int[]{9, 10, 6, 6, 7, 9, 9, 4, 5, 5, 10, 9},
new int[]{4, 5, 8, 9, 0, 1, 1, 7, 9, 7, 1, 10, 10, 6, 7},
new int[]{10, 3, 0, 4, 5, 10, 4, 10, 0, 7, 3, 10, 10, 6, 7},
new int[]{5, 8, 4, 6, 7, 1, 10, 6, 1, 7, 3, 1},
new int[]{9, 1, 2, 6, 7, 9, 6, 9, 2, 5, 1, 9, 9, 4, 5},
new int[]{4, 5, 8, 9, 0, 6, 7, 9, 6, 0, 2, 6},
new int[]{4, 5, 1, 0, 4, 1, 3, 2, 6, 7, 3, 6},
new int[]{8, 4, 5, 3, 2, 6, 7, 3, 6},
new int[]{6, 7, 11, 10, 2, 3, 3, 5, 10, 5, 3, 9, 9, 4, 5},
new int[]{6, 7, 11, 8, 4, 5, 0, 3, 9, 10, 2, 1},
new int[]{6, 7, 11, 10, 2, 4, 5, 10, 4, 2, 0, 4},
new int[]{6, 7, 11, 5, 8, 4, 10, 2, 1},
new int[]{7, 11, 6, 4, 5, 3, 9, 4, 3, 5, 1, 3},
new int[]{4, 5, 8, 7, 11, 6, 9, 0, 3},
new int[]{11, 6, 7, 1, 0, 4, 5, 1, 4},
new int[]{4, 5, 8, 6, 7, 11},
new int[]{7, 9, 10, 6, 7, 10, 9, 8, 10},
new int[]{0, 10, 6, 9, 0, 6, 0, 1, 10, 9, 6, 7},
new int[]{10, 3, 0, 6, 7, 10, 10, 7, 3, 10, 0, 8},
new int[]{7, 3, 1, 7, 1, 10, 7, 10, 6},
new int[]{6, 7, 9, 6, 9, 1, 6, 1, 2, 9, 8, 1},
new int[]{0, 2, 6, 0, 6, 7, 0, 7, 9},
new int[]{8, 1, 0, 6, 7, 3, 2, 6, 3},
new int[]{6, 7, 3, 2, 6, 3},
new int[]{11, 6, 7, 3, 9, 10, 2, 3, 10, 9, 8, 10},
new int[]{2, 1, 10, 3, 9, 0, 11, 6, 7},
new int[]{7, 11, 6, 0, 8, 10, 2, 0, 10},
new int[]{10, 2, 1, 11, 6, 7},
new int[]{6, 7, 11, 8, 1, 3, 9, 8, 3},
new int[]{7, 11, 6, 3, 9, 0},
new int[]{6, 7, 11, 8, 1, 0},
new int[]{6, 7, 11},
new int[]{6, 11, 7},
new int[]{11, 7, 0, 0, 1, 11, 7, 8, 0, 8, 6, 1, 8, 7, 6, 6, 11, 1},
new int[]{0, 6, 11, 11, 3, 0, 0, 9, 7, 7, 6, 0},
new int[]{6, 1, 8, 9, 7, 6, 9, 6, 8, 3, 1, 6, 6, 11, 3},
new int[]{1, 7, 6, 6, 10, 1, 1, 2, 11, 11, 7, 1},
new int[]{7, 8, 0, 2, 11, 7, 2, 7, 0, 10, 8, 7, 7, 6, 10},
new int[]{2, 11, 3, 1, 0, 9, 9, 10, 1, 10, 9, 7, 7, 6, 10},
new int[]{11, 3, 2, 6, 10, 9, 7, 6, 9, 10, 8, 9},
new int[]{7, 6, 2, 3, 7, 2},
new int[]{8, 7, 6, 2, 1, 8, 2, 8, 6, 3, 7, 8, 8, 0, 3},
new int[]{7, 6, 0, 9, 7, 0, 6, 2, 0},
new int[]{2, 1, 8, 2, 8, 7, 2, 7, 6, 8, 9, 7},
new int[]{10, 1, 7, 6, 10, 7, 1, 3, 7},
new int[]{8, 7, 6, 0, 3, 8, 8, 3, 7, 8, 6, 10},
new int[]{0, 7, 6, 1, 0, 6, 0, 9, 7, 1, 6, 10},
new int[]{10, 8, 9, 10, 9, 7, 10, 7, 6},
new int[]{8, 11, 7, 7, 4, 8, 8, 5, 6, 6, 11, 8},
new int[]{11, 0, 1, 5, 6, 11, 5, 11, 1, 4, 0, 11, 11, 7, 4},
new int[]{4, 9, 7, 5, 6, 11, 11, 8, 5, 8, 11, 3, 3, 0, 8},
new int[]{7, 4, 9, 11, 3, 5, 6, 11, 5, 3, 1, 5},
new int[]{6, 10, 5, 7, 4, 8, 8, 11, 7, 11, 8, 1, 1, 2, 11},
new int[]{6, 10, 5, 7, 4, 2, 11, 7, 2, 4, 0, 2},
new int[]{2, 11, 3, 5, 6, 10, 4, 9, 7, 1, 0, 8},
new int[]{6, 10, 5, 11, 3, 2, 7, 4, 9},
new int[]{8, 2, 3, 7, 4, 8, 7, 8, 3, 6, 2, 8, 8, 5, 6},
new int[]{2, 1, 5, 6, 2, 5, 7, 4, 0, 3, 7, 0},
new int[]{4, 9, 7, 5, 6, 0, 8, 5, 0, 6, 2, 0},
new int[]{9, 7, 4, 2, 1, 5, 6, 2, 5},
new int[]{5, 6, 10, 8, 1, 7, 4, 8, 7, 1, 3, 7},
new int[]{10, 5, 6, 0, 3, 7, 4, 0, 7},
new int[]{4, 9, 7, 8, 1, 0, 5, 6, 10},
new int[]{7, 4, 9, 5, 6, 10},
new int[]{9, 4, 6, 11, 9, 6},
new int[]{1, 6, 11, 9, 0, 1, 9, 1, 11, 4, 6, 1, 1, 8, 4},
new int[]{3, 0, 6, 11, 3, 6, 0, 4, 6},
new int[]{1, 6, 11, 8, 4, 1, 1, 4, 6, 1, 11, 3},
new int[]{1, 9, 4, 6, 10, 1, 6, 1, 4, 11, 9, 1, 1, 2, 11},
new int[]{6, 10, 8, 4, 6, 8, 9, 0, 2, 11, 9, 2},
new int[]{2, 11, 3, 1, 0, 6, 10, 1, 6, 0, 4, 6},
new int[]{3, 2, 11, 8, 4, 6, 10, 8, 6},
new int[]{9, 4, 2, 3, 9, 2, 4, 6, 2},
new int[]{0, 3, 9, 8, 4, 2, 1, 8, 2, 4, 6, 2},
new int[]{6, 2, 0, 4, 6, 0},
new int[]{4, 6, 2, 4, 2, 1, 4, 1, 8},
new int[]{6, 10, 1, 6, 1, 9, 6, 9, 4, 1, 3, 9},
new int[]{3, 9, 0, 6, 10, 8, 4, 6, 8},
new int[]{0, 4, 6, 0, 6, 10, 0, 10, 1},
new int[]{8, 4, 6, 10, 8, 6},
new int[]{6, 11, 8, 5, 6, 8, 11, 9, 8},
new int[]{5, 6, 11, 5, 11, 0, 5, 0, 1, 11, 9, 0},
new int[]{3, 8, 5, 11, 3, 5, 3, 0, 8, 11, 5, 6},
new int[]{3, 1, 5, 3, 5, 6, 3, 6, 11},
new int[]{10, 5, 6, 2, 11, 8, 1, 2, 8, 11, 9, 8},
new int[]{5, 6, 10, 9, 0, 2, 11, 9, 2},
new int[]{2, 11, 3, 10, 5, 6, 1, 0, 8},
new int[]{6, 10, 5, 2, 11, 3},
new int[]{8, 2, 3, 5, 6, 8, 8, 6, 2, 8, 3, 9},
new int[]{9, 0, 3, 5, 6, 2, 1, 5, 2},
new int[]{6, 2, 0, 6, 0, 8, 6, 8, 5},
new int[]{5, 6, 2, 1, 5, 2},
new int[]{6, 10, 5, 3, 9, 8, 1, 3, 8},
new int[]{0, 3, 9, 10, 5, 6},
new int[]{10, 5, 6, 8, 1, 0},
new int[]{5, 6, 10},
new int[]{5, 10, 11, 7, 5, 11},
new int[]{0, 11, 7, 5, 8, 0, 5, 0, 7, 10, 11, 0, 0, 1, 10},
new int[]{0, 5, 10, 11, 3, 0, 11, 0, 10, 7, 5, 0, 0, 9, 7},
new int[]{1, 10, 11, 3, 1, 11, 9, 7, 5, 8, 9, 5},
new int[]{11, 7, 1, 2, 11, 1, 7, 5, 1},
new int[]{2, 11, 7, 2, 7, 8, 2, 8, 0, 7, 5, 8},
new int[]{3, 2, 11, 9, 7, 1, 0, 9, 1, 7, 5, 1},
new int[]{2, 11, 3, 5, 8, 9, 7, 5, 9},
new int[]{2, 3, 5, 10, 2, 5, 3, 7, 5},
new int[]{1, 10, 2, 0, 3, 5, 8, 0, 5, 3, 7, 5},
new int[]{5, 0, 9, 10, 2, 5, 5, 2, 0, 5, 9, 7},
new int[]{2, 1, 10, 9, 7, 5, 8, 9, 5},
new int[]{3, 7, 5, 1, 3, 5},
new int[]{3, 7, 5, 3, 5, 8, 3, 8, 0},
new int[]{7, 5, 1, 7, 1, 0, 7, 0, 9},
new int[]{5, 8, 9, 7, 5, 9},
new int[]{4, 8, 11, 7, 4, 11, 8, 10, 11},
new int[]{11, 0, 1, 7, 4, 11, 11, 4, 0, 11, 1, 10},
new int[]{9, 7, 4, 0, 8, 11, 3, 0, 11, 8, 10, 11},
new int[]{4, 9, 7, 1, 10, 11, 3, 1, 11},
new int[]{4, 1, 2, 7, 4, 2, 4, 8, 1, 7, 2, 11},
new int[]{4, 0, 2, 4, 2, 11, 4, 11, 7},
new int[]{0, 8, 1, 9, 7, 4, 3, 2, 11},
new int[]{9, 7, 4, 11, 3, 2},
new int[]{7, 4, 8, 7, 8, 2, 7, 2, 3, 8, 10, 2},
new int[]{10, 2, 1, 7, 4, 0, 3, 7, 0},
new int[]{7, 4, 9, 10, 2, 0, 8, 10, 0},
new int[]{4, 9, 7, 1, 10, 2},
new int[]{1, 3, 7, 1, 7, 4, 1, 4, 8},
new int[]{7, 4, 0, 3, 7, 0},
new int[]{4, 9, 7, 0, 8, 1},
new int[]{4, 9, 7},
new int[]{5, 10, 9, 4, 5, 9, 10, 11, 9},
new int[]{8, 4, 5, 1, 10, 9, 0, 1, 9, 10, 11, 9},
new int[]{4, 5, 10, 4, 10, 3, 4, 3, 0, 10, 11, 3},
new int[]{4, 5, 8, 11, 3, 1, 10, 11, 1},
new int[]{9, 1, 2, 4, 5, 9, 9, 5, 1, 9, 2, 11},
new int[]{5, 8, 4, 2, 11, 9, 0, 2, 9},
new int[]{11, 3, 2, 4, 5, 1, 0, 4, 1},
new int[]{2, 11, 3, 5, 8, 4},
new int[]{2, 9, 4, 10, 2, 4, 2, 3, 9, 10, 4, 5},
new int[]{0, 3, 9, 1, 10, 2, 8, 4, 5},
new int[]{2, 0, 4, 2, 4, 5, 2, 5, 10},
new int[]{1, 10, 2, 5, 8, 4},
new int[]{5, 1, 3, 5, 3, 9, 5, 9, 4},
new int[]{8, 4, 5, 9, 0, 3},
new int[]{4, 5, 1, 0, 4, 1},
new int[]{4, 5, 8},
new int[]{11, 9, 8, 10, 11, 8},
new int[]{10, 11, 9, 10, 9, 0, 10, 0, 1},
new int[]{8, 10, 11, 8, 11, 3, 8, 3, 0},
new int[]{1, 10, 11, 3, 1, 11},
new int[]{11, 9, 8, 11, 8, 1, 11, 1, 2},
new int[]{9, 0, 2, 11, 9, 2},
new int[]{3, 2, 11, 1, 0, 8},
new int[]{2, 11, 3},
new int[]{9, 8, 10, 9, 10, 2, 9, 2, 3},
new int[]{2, 1, 10, 0, 3, 9},
new int[]{0, 8, 10, 2, 0, 10},
new int[]{1, 10, 2},
new int[]{8, 1, 3, 9, 8, 3},
new int[]{0, 3, 9},
new int[]{0, 8, 1},
new int[]{},
        };

        #endregion

        #region Corner Methods

        public static Vector3 GetVector3Corner(int corner, float distance) {
            switch(corner) {
                case 0: return new Vector3(-distance, -distance, -distance);
                case 1: return new Vector3(-distance, -distance, distance);
                case 2: return new Vector3(distance, -distance, -distance);
                case 3: return new Vector3(distance, -distance, distance);

                case 4: return new Vector3(-distance, distance, -distance);
                case 5: return new Vector3(-distance, distance, distance);
                case 6: return new Vector3(distance, distance, -distance);
                case 7: return new Vector3(distance, distance, distance);
            }
            return default;
        }

        public static V3 GetV3FromCorner(int corner, float distance) {
            switch(corner) {
                case 0: return new V3(-distance, -distance, -distance);
                case 1: return new V3(-distance, -distance, distance);
                case 2: return new V3(distance, -distance, -distance);
                case 3: return new V3(distance, -distance, distance);

                case 4: return new V3(-distance, distance, -distance);
                case 5: return new V3(-distance, distance, distance);
                case 6: return new V3(distance, distance, -distance);
                case 7: return new V3(distance, distance, distance);
            }
            return default;
        }

        #endregion
    }
}
