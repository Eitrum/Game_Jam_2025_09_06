using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Toxel {
    public static class MarchingCubeUtility {

        public enum Mode {
            Normal,
            Smooth,
            SmoothPlus,
            Toxel,
            ToxelSmooth,
            ToxelSmoothPlus,
        }

        public const float DEFAULT_TRESHOLD = 0.5f;

        #region Get Mask

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMask(
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111) {
            int mask = 0;
            if(c000) mask |= 1;
            if(c001) mask |= 2;
            if(c100) mask |= 4;
            if(c101) mask |= 8;

            if(c010) mask |= 16;
            if(c011) mask |= 32;
            if(c110) mask |= 64;
            if(c111) mask |= 128;
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMask(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold = DEFAULT_TRESHOLD) {
            int mask = 0;
            if(c000 >= threshold) mask |= 1;
            if(c001 >= threshold) mask |= 2;
            if(c100 >= threshold) mask |= 4;
            if(c101 >= threshold) mask |= 8;

            if(c010 >= threshold) mask |= 16;
            if(c011 >= threshold) mask |= 32;
            if(c110 >= threshold) mask |= 64;
            if(c111 >= threshold) mask |= 128;
            return mask;
        }

        public static void GetValues(int mask, out float c000, out float c001, out float c100, out float c101, out float c010, out float c011, out float c110, out float c111) {
            c000 = mask.HasFlag(1) ? 1 : 0;
            c001 = mask.HasFlag(2) ? 1 : 0;
            c100 = mask.HasFlag(4) ? 1 : 0;
            c101 = mask.HasFlag(8) ? 1 : 0;

            c010 = mask.HasFlag(16) ? 1 : 0;
            c011 = mask.HasFlag(32) ? 1 : 0;
            c110 = mask.HasFlag(64) ? 1 : 0;
            c111 = mask.HasFlag(128) ? 1 : 0;
        }

        #endregion

        #region Position To Hash

        private static readonly int HASHMASK = B.OOII.IIII.IIII;

        public static int PositionToHash(Vector3 vec) {
            int result = 0;
            var x = (int)((vec.x + 5) * 12) & HASHMASK;
            var y = (int)((vec.y + 5) * 12) & HASHMASK;
            var z = (int)((vec.z + 5) * 12) & HASHMASK;
            result = x << 24 | y << 12 | z;
            return result;
        }

        #endregion

        #region Normal Smoothing

        public class NormalSmoothData {
            public Vector3 Normal;
            public List<Vector3> Normals;

            public NormalSmoothData() {
                Normals = FastPool<List<Vector3>>.Global.Pop();
            }

            public NormalSmoothData(Vector3 normal) {
                Normals = FastPool<List<Vector3>>.Global.Pop();
                Normals.Add(normal);
            }

            public void Calculate() {
                if(Normals.Count > 1) {
                    float x = 0, y = 0, z = 0;
                    for(int i = 0, len = Normals.Count; i < len; i++) {
                        var t = Normals[i];
                        x += t.x;
                        y += t.y;
                        z += t.z;
                    }
                    var m = 1f / Normals.Count;
                    Normal = new Vector3(x * m, y * m, z * m);
                }
                else {
                    Normal = Normals[0];
                }
            }
        }

        private static Dictionary<int, NormalSmoothData> normalsPreCache = new Dictionary<int, NormalSmoothData>();

        public static void SmoothNormals(IReadOnlyList<Vector3> verticies, IList<Vector3> normals) {
            var len = verticies.Count;
            normalsPreCache.Clear();
            for(int i = 0; i < len; i++) {
                var hash = PositionToHash(verticies[i]);
                if(!normalsPreCache.TryGetValue(hash, out NormalSmoothData data))
                    normalsPreCache.Add(hash, data = new NormalSmoothData());
                data.Normals.Add(normals[i]);
            }

            foreach(var v in normalsPreCache.Values)
                v.Calculate();

            for(int i = 0; i < len; i++) {
                var hash = PositionToHash(verticies[i]);
                if(normalsPreCache.TryGetValue(hash, out var data))
                    normals[i] = data.Normal;
            }
        }

        #endregion

        #region Vertex Smoothing

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ValueFromIndex(int index, float c000, float c001, float c100, float c101, float c010, float c011, float c110, float c111) {
            switch(index) {
                case 0: return c000;
                case 1: return c001;
                case 2: return c100;
                case 3: return c101;
                case 4: return c010;
                case 5: return c011;
                case 6: return c110;
                case 7: return c111;
            }
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ToxelMaterialData ValueFromIndex(int index, ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101, ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111) {
            switch(index) {
                case 0: return c000;
                case 1: return c001;
                case 2: return c100;
                case 3: return c101;
                case 4: return c010;
                case 5: return c011;
                case 6: return c110;
                case 7: return c111;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MarchingCubeTables.V3 SmoothVertex(float threshold, MarchingCubeTables.V3 p0, MarchingCubeTables.V3 p1, float value0, float value1) {
            if(value0 > value1)
                return SmoothVertex(threshold, p1, p0, value1, value0);

            if(value0.Equals(value1, Mathf.Epsilon))
                return new MarchingCubeTables.V3(
                    Mathf.Lerp(p0.x, p1.x, 0.5f),
                    Mathf.Lerp(p0.y, p1.y, 0.5f),
                    Mathf.Lerp(p0.z, p1.z, 0.5f));

            var value = (threshold - value0) / (value1 - value0);
            return new MarchingCubeTables.V3(
                    Mathf.LerpUnclamped(p0.x, p1.x, value),
                    Mathf.LerpUnclamped(p0.y, p1.y, value),
                    Mathf.LerpUnclamped(p0.z, p1.z, value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SmoothVertexT(float threshold, float value0, float value1) {
            if(value0.Equals(value1, Mathf.Epsilon))
                return 0;
            return (threshold - value0) / (value1 - value0);
        }

        #endregion

        #region Generic Add Cube (Testing)

        public static unsafe void AddCube(
            Mode mode,
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {
            switch(mode) {
                case Mode.Normal:
                case Mode.Smooth:
                case Mode.SmoothPlus:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, position, verticies, triangles);
                    break;
                case Mode.Toxel:
                case Mode.ToxelSmooth:
                case Mode.ToxelSmoothPlus:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, position, verticies, triangles);
                    break;
            }
        }

        public static unsafe void AddCube(
            Mode mode,
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {
            switch(mode) {
                case Mode.Normal:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles);
                    break;
                case Mode.Smooth:
                case Mode.SmoothPlus:
                    AddNormalCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles);
                    break;
                case Mode.Toxel:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles);
                    break;
                case Mode.ToxelSmooth:
                case Mode.ToxelSmoothPlus:
                    AddToxelCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles);
                    break;
            }
        }

        public static unsafe void AddCube(
            Mode mode,
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {
            switch(mode) {
                case Mode.Normal:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.Smooth:
                    AddNormalCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.SmoothPlus:
                    AddNormalCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.Toxel:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.ToxelSmooth:
                    AddToxelCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.ToxelSmoothPlus:
                    AddToxelCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
            }
        }

        public static unsafe void AddCube(
            Mode mode,
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {
            switch(mode) {
                case Mode.Normal:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
                case Mode.Smooth:
                    AddNormalCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
                case Mode.SmoothPlus:
                    AddNormalCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
                case Mode.Toxel:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
                case Mode.ToxelSmooth:
                    AddToxelCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
                case Mode.ToxelSmoothPlus:
                    AddToxelCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors, materialId);
                    break;
            }
        }

        public static unsafe void AddCube(
            Mode mode,
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {
            switch(mode) {
                case Mode.Normal:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.Smooth:
                    AddNormalCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.SmoothPlus:
                    AddNormalCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.Toxel:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.ToxelSmooth:
                    AddToxelCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
                case Mode.ToxelSmoothPlus:
                    AddToxelCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals);
                    break;
            }
        }

        public static unsafe void AddCube(
            Mode mode,
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {
            switch(mode) {
                case Mode.Normal:
                    AddNormalCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
                case Mode.Smooth:
                    AddNormalCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
                case Mode.SmoothPlus:
                    AddNormalCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
                case Mode.Toxel:
                    AddToxelCube(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
                case Mode.ToxelSmooth:
                    AddToxelCubeSmooth(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
                case Mode.ToxelSmoothPlus:
                    AddToxelCubeSmoothPlus(c000, c001, c100, c101, c010, c011, c110, c111, threshold, position, verticies, triangles, normals, uv, colors);
                    break;
            }
        }

        #endregion

        #region Add Normal Cube (Bool variant)

        public static unsafe void AddNormalCube(
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddNormalCube(
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
        }

        public static unsafe void AddNormalCube(
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
            
            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        #endregion

        #region Add Normal Cube

        public static unsafe void AddNormalCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddNormalCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
        }

        public static unsafe void AddNormalCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddNormalCube(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Add Normal Cube Smoothed

        public static unsafe void AddNormalCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(DEFAULT_TRESHOLD, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddNormalCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddNormalCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
        }

        public static unsafe void AddNormalCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddNormalCubeSmooth(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Add Normal Cube Smoothed Plus

        public static unsafe void AddNormalCubeSmoothPlus(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }
        }

        public static unsafe void AddNormalCubeSmoothPlus(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddNormalCubeSmoothPlus(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }

            AddColorNormal(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Add Toxel Cube

        public static unsafe void AddToxelCube(
            bool c000, bool c001, bool c100, bool c101,
            bool c010, bool c011, bool c110, bool c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddToxelCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddToxelCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }
            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
        }

        public static unsafe void AddToxelCube(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            var length = tris.Length;
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddToxelCube(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            var length = tris.Length;
            float x = position.x, y = position.y, z = position.z;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var value = MarchingCubeTables.EDGE_POSITION[edge];

                verticies.Add(new Vector3(x + value.x, y + value.y, z + value.z));
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Add Toxel Cube Smoothed

        public static unsafe void AddToxelCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(DEFAULT_TRESHOLD, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddToxelCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];

            for(int i = 0, length = tris.Length; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }
        }

        public static unsafe void AddToxelCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }
        }

        public static unsafe void AddToxelCubeSmooth(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddToxelCubeSmooth(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            var norm = MarchingCubeTables.MASK_TO_NORMALS_TOXEL[mask];
            for(int i = 0, len = norm.Length; i < len; i++) {
                var vec = (Vector3)norm[i];
                normals.Add(vec);
                normals.Add(vec);
                normals.Add(vec);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Add Toxel Cube Smoothed Plus

        public static unsafe void AddToxelCubeSmoothPlus(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }
        }

        public static unsafe void AddToxelCubeSmoothPlus(
            float c000, float c001, float c100, float c101,
            float c010, float c011, float c110, float c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors,
            int materialId = 0
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors, materialId);
        }

        public static unsafe void AddToxelCubeSmoothPlus(
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            float threshold,
            Vector3 position,
            List<Vector3> verticies,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uv,
            List<Color> colors
            ) {

            int mask = GetMask(c000, c001, c100, c101, c010, c011, c110, c111, threshold);

            if(mask == 0 || mask == 255)
                return;

            var index = verticies.Count;
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            int length = tris.Length;
            for(int i = 0; i < length; i++) {
                var edge = tris[i];
                var pointA = MarchingCubeTables.EDGE_TO_CORNER_A[edge];
                var pointB = MarchingCubeTables.EDGE_TO_CORNER_B[edge];
                var valueA = ValueFromIndex(pointA, c000, c001, c100, c101, c010, c011, c110, c111);
                var valueB = ValueFromIndex(pointB, c000, c001, c100, c101, c010, c011, c110, c111);
                var value = SmoothVertex(threshold, MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointA], MarchingCubeTables.CORNER_INDEX_TO_POSITION[pointB], valueA, valueB);

                verticies.Add(position + value);
                triangles.Add(index + i);
            }

            for(int i = 0; i < length; i += 3) {
                var u = verticies[index+i+1] - verticies[index+i+0];
                var v = verticies[index+i+2] - verticies[index+i+0];

                var norm = new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;

                normals.Add(norm);
                normals.Add(norm);
                normals.Add(norm);
            }

            AddColorToxel(mask, c000, c001, c100, c101, c010, c011, c110, c111, uv, colors);
        }

        #endregion

        #region Color / UV ToxelData

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddColorNormal(
           int mask,
           bool c000, bool c001, bool c100, bool c101,
           bool c010, bool c011, bool c110, bool c111,
           List<Vector2> uv, List<Color> colors,
           int materialId = 0) {
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;

            for(int i = 0; i < length; i += 3) {
                var uvvalue = new Vector2(materialId / 256f + (materialId * 1.001f), materialId / 256f);

                uv.Add(uvvalue);
                uv.Add(uvvalue);
                uv.Add(uvvalue);

                colors.Add(new Color(1, 0, 0));
                colors.Add(new Color(0, 1, 0));
                colors.Add(new Color(0, 0, 1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddColorNormal(
           int mask,
           float c000, float c001, float c100, float c101,
           float c010, float c011, float c110, float c111,
           List<Vector2> uv, List<Color> colors,
           int materialId = 0) {
            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;

            for(int i = 0; i < length; i += 3) {
                var uvvalue = new Vector2(materialId / 256f + (materialId * 1.001f), materialId / 256f);

                uv.Add(uvvalue);
                uv.Add(uvvalue);
                uv.Add(uvvalue);

                colors.Add(new Color(1, 0, 0));
                colors.Add(new Color(0, 1, 0));
                colors.Add(new Color(0, 0, 1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddColorNormal(
            int mask,
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            List<Vector2> uv, List<Color> colors) {
            Dictionary<byte, float> materials = new Dictionary<byte, float>();
            for(int i = 0; i < 8; i++) {
                var t = ValueFromIndex(i, c000, c001, c100, c101, c010, c011, c110, c111);
                if(!materials.TryGetValue(t.DominantMaterialId, out float v))
                    materials.Add(t.DominantMaterialId, t.Amount);
                else
                    materials[t.DominantMaterialId] += t.Amount;
            }

            ToxelMaterialData highest = default;
            foreach(var m in materials)
                if(m.Value > highest.Amount)
                    highest = new ToxelMaterialData(m.Value, m.Key);

            var tris = MarchingCubeTables.MASK_TO_TRIS[mask];
            var length = tris.Length;

            for(int i = 0; i < length; i += 3) {
                var edge0 = tris[i + 0];
                var edge1 = tris[i + 1];
                var edge2 = tris[i + 2];

                var p0a = MarchingCubeTables.EDGE_TO_CORNER_A[edge0];
                var p0b = MarchingCubeTables.EDGE_TO_CORNER_B[edge0];
                var p1a = MarchingCubeTables.EDGE_TO_CORNER_A[edge1];
                var p1b = MarchingCubeTables.EDGE_TO_CORNER_B[edge1];
                var p2a = MarchingCubeTables.EDGE_TO_CORNER_A[edge2];
                var p2b = MarchingCubeTables.EDGE_TO_CORNER_B[edge2];

                var v0 = ValueFromIndex(p0a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v1 = ValueFromIndex(p0b, c000, c001, c100, c101, c010, c011, c110, c111);
                var v2 = ValueFromIndex(p1a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v3 = ValueFromIndex(p1b, c000, c001, c100, c101, c010, c011, c110, c111);
                var v4 = ValueFromIndex(p2a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v5 = ValueFromIndex(p2b, c000, c001, c100, c101, c010, c011, c110, c111);

                var node0 = v0.Amount > v1.Amount  ? v0 : v1;
                var node0o = v0.Amount > v1.Amount  ? v1 : v0;
                var node1 = v2.Amount > v3.Amount  ? v2 : v3;
                var node1o = v2.Amount > v3.Amount  ? v3 : v2;
                var node2 = v4.Amount > v5.Amount  ? v4 : v5;
                var node2o = v4.Amount > v5.Amount  ? v5 : v4;

                var uvvalue = new Vector2(node0.DominantMaterialId / 256f + (node2.DominantMaterialId * 1.001f), node1.DominantMaterialId / 256f);

                uv.Add(uvvalue);
                uv.Add(uvvalue);
                uv.Add(uvvalue);

                //var v0amount = SmoothVertexT(threshold, node0.Amount, node0o.Amount);
                //var v1amount = SmoothVertexT(threshold, node1.Amount, node1o.Amount);
                //var v2amount = SmoothVertexT(threshold, node2.Amount, node2o.Amount);

                colors.Add(new Color(node0.Amount, 0, 0));
                colors.Add(new Color(0, node1.Amount, 0));
                colors.Add(new Color(0, 0, node2.Amount));
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddColorToxel(
           int mask,
           float c000, float c001, float c100, float c101,
           float c010, float c011, float c110, float c111,
           List<Vector2> uv, List<Color> colors,
           int materialId = 0) {
            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            var length = tris.Length;

            for(int i = 0; i < length; i += 3) {
                var uvvalue = new Vector2(materialId / 256f + (materialId * 1.001f), materialId / 256f);

                uv.Add(uvvalue);
                uv.Add(uvvalue);
                uv.Add(uvvalue);

                colors.Add(new Color(1, 0, 0));
                colors.Add(new Color(0, 1, 0));
                colors.Add(new Color(0, 0, 1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddColorToxel(
            int mask,
            ToxelMaterialData c000, ToxelMaterialData c001, ToxelMaterialData c100, ToxelMaterialData c101,
            ToxelMaterialData c010, ToxelMaterialData c011, ToxelMaterialData c110, ToxelMaterialData c111,
            List<Vector2> uv, List<Color> colors) {
            Dictionary<byte, float> materials = new Dictionary<byte, float>();
            for(int i = 0; i < 8; i++) {
                var t = ValueFromIndex(i, c000, c001, c100, c101, c010, c011, c110, c111);
                if(!materials.TryGetValue(t.DominantMaterialId, out float v))
                    materials.Add(t.DominantMaterialId, t.Amount);
                else
                    materials[t.DominantMaterialId] += t.Amount;
            }

            ToxelMaterialData highest = default;
            foreach(var m in materials)
                if(m.Value > highest.Amount)
                    highest = new ToxelMaterialData(m.Value, m.Key);

            var tris = MarchingCubeTables.MASK_TO_TRIS_TOXEL[mask];
            var length = tris.Length;

            for(int i = 0; i < length; i += 3) {
                var edge0 = tris[i + 0];
                var edge1 = tris[i + 1];
                var edge2 = tris[i + 2];

                var p0a = MarchingCubeTables.EDGE_TO_CORNER_A[edge0];
                var p0b = MarchingCubeTables.EDGE_TO_CORNER_B[edge0];
                var p1a = MarchingCubeTables.EDGE_TO_CORNER_A[edge1];
                var p1b = MarchingCubeTables.EDGE_TO_CORNER_B[edge1];
                var p2a = MarchingCubeTables.EDGE_TO_CORNER_A[edge2];
                var p2b = MarchingCubeTables.EDGE_TO_CORNER_B[edge2];

                var v0 = ValueFromIndex(p0a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v1 = ValueFromIndex(p0b, c000, c001, c100, c101, c010, c011, c110, c111);
                var v2 = ValueFromIndex(p1a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v3 = ValueFromIndex(p1b, c000, c001, c100, c101, c010, c011, c110, c111);
                var v4 = ValueFromIndex(p2a, c000, c001, c100, c101, c010, c011, c110, c111);
                var v5 = ValueFromIndex(p2b, c000, c001, c100, c101, c010, c011, c110, c111);

                var node0 = v0.Amount > v1.Amount  ? v0 : v1;
                var node0o = v0.Amount > v1.Amount  ? v1 : v0;
                var node1 = v2.Amount > v3.Amount  ? v2 : v3;
                var node1o = v2.Amount > v3.Amount  ? v3 : v2;
                var node2 = v4.Amount > v5.Amount  ? v4 : v5;
                var node2o = v4.Amount > v5.Amount  ? v5 : v4;

                var uvvalue = new Vector2(node0.DominantMaterialId / 256f + (node2.DominantMaterialId * 1.001f), node1.DominantMaterialId / 256f);

                uv.Add(uvvalue);
                uv.Add(uvvalue);
                uv.Add(uvvalue);

                //var v0amount = SmoothVertexT(threshold, node0.Amount, node0o.Amount);
                //var v1amount = SmoothVertexT(threshold, node1.Amount, node1o.Amount);
                //var v2amount = SmoothVertexT(threshold, node2.Amount, node2o.Amount);

                colors.Add(new Color(node0.Amount, 0, 0));
                colors.Add(new Color(0, node1.Amount, 0));
                colors.Add(new Color(0, 0, node2.Amount));
            }
        }

        #endregion
    }
}
