using System;
using UnityEngine;

namespace Toolkit.Toxel {
    public static class ToxelGizmos {
        #region Chunk Draw

        public static void DrawChunkBounds(Vector3Int chunkId) {
            var b = ToxelUtility.ChunkToWorldBounds(chunkId);
            Gizmos.DrawWireCube(b.center, b.size);
        }

        public static void DrawPositionRelativeToBounds(Vector3 position) {
            var p0 = ToxelUtility.ChunkToWorldPosition(ToxelUtility.WorldPositionToChunk(position));
            var p1 = p0 + new Vector3(ToxelUtility.CHUNK_SIZE, 0, 0);
            var p2 = p0 + new Vector3(ToxelUtility.CHUNK_SIZE, 0, ToxelUtility.CHUNK_SIZE);
            var p3 = p0 + new Vector3(0, 0, ToxelUtility.CHUNK_SIZE);

            var p4 = p0 + new Vector3(0, ToxelUtility.CHUNK_SIZE, 0);
            var p5 = p0 + new Vector3(ToxelUtility.CHUNK_SIZE, ToxelUtility.CHUNK_SIZE, 0);
            var p6 = p0 + new Vector3(ToxelUtility.CHUNK_SIZE, ToxelUtility.CHUNK_SIZE, ToxelUtility.CHUNK_SIZE);
            var p7 = p0 + new Vector3(0, ToxelUtility.CHUNK_SIZE, ToxelUtility.CHUNK_SIZE);

            Gizmos.DrawLine(position, p0);
            Gizmos.DrawLine(position, p1);
            Gizmos.DrawLine(position, p2);
            Gizmos.DrawLine(position, p3);

            Gizmos.DrawLine(position, p4);
            Gizmos.DrawLine(position, p5);
            Gizmos.DrawLine(position, p6);
            Gizmos.DrawLine(position, p7);
        }

        #endregion

        #region Marching Cubes

        public static void DrawMarchingCubeBounds(Vector3 position) {
            var pos = position.Floor() + new Vector3(0.5f, 0.5f, 0.5f);
            Gizmos.DrawWireCube(pos, new Vector3(1, 1, 1));
        }

        public static void DrawPositionRelativeToMarchingCubeBounds(Vector3 position) {
            var p0 = position.Floor();
            var p1 = p0 + new Vector3(1, 0, 0);
            var p2 = p0 + new Vector3(1, 0, 1);
            var p3 = p0 + new Vector3(0, 0, 1);

            var p4 = p0 + new Vector3(0, 1, 0);
            var p5 = p0 + new Vector3(1, 1, 0);
            var p6 = p0 + new Vector3(1, 1, 1);
            var p7 = p0 + new Vector3(0, 1, 1);

            Gizmos.DrawLine(position, p0);
            Gizmos.DrawLine(position, p1);
            Gizmos.DrawLine(position, p2);
            Gizmos.DrawLine(position, p3);

            Gizmos.DrawLine(position, p4);
            Gizmos.DrawLine(position, p5);
            Gizmos.DrawLine(position, p6);
            Gizmos.DrawLine(position, p7);
        }

        public static void DrawMarchingCubeBounds(Vector3 position, float range) {
            if(range > 16f)
                range = 16;
            var mainColor = Gizmos.color;
            var c = ToxelUtility.ToMarchingCubeBounds(position).center;
            var irange = Mathf.CeilToInt(range);
            var sqrRange = range * range;
            var size = new Vector3(1, 1, 1);

            for(int x = -irange; x <= irange; x++) {
                for(int y = -irange; y <= irange; y++) {
                    for(int z = -irange; z <= irange; z++) {
                        var offset = new Vector3(x, y, z);
                        if(offset.sqrMagnitude > sqrRange)
                            continue;
                        var a = (sqrRange - offset.sqrMagnitude) / sqrRange;
                        a = a * a;
                        using(new GizmosUtility.ColorScope(mainColor.MultiplyAlpha(a)))
                            Gizmos.DrawWireCube(c + offset, size);
                    }
                }
            }
        }

        #endregion
    }
}
