using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Toxel {
    public static class ToxelUtility {
        #region Consts

        public const string TAG = "[Toolkit.Toxel.ToxelUtility] - ";

        public const int CHUNK_SIZE = 32;
        public const float HALF_CHUNK = CHUNK_SIZE / 2f;
        private const float ONE_PART_CHUNK = 1f / CHUNK_SIZE;

        #endregion

        #region Properties

        public static Vector3 ChunkCenterOffset => new Vector3(HALF_CHUNK, HALF_CHUNK, HALF_CHUNK);
        public static Vector3 ChunkSize => new Vector3(CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE);

        #endregion

        #region Convertions

        public static Vector3Int WorldPositionToChunk(Vector3 position)
            => new Vector3Int(
                Mathf.FloorToInt(position.x * ONE_PART_CHUNK),
                Mathf.FloorToInt(position.y * ONE_PART_CHUNK),
                Mathf.FloorToInt(position.z * ONE_PART_CHUNK));

        public static Vector3 ChunkToWorldPosition(Vector3Int chunk)
            => new Vector3(
                chunk.x * CHUNK_SIZE,
                chunk.y * CHUNK_SIZE,
                chunk.z * CHUNK_SIZE);

        public static Bounds ChunkToWorldBounds(Vector3Int chunk)
            => new Bounds(ChunkToWorldPosition(chunk) + ChunkCenterOffset, ChunkSize);

        public static Bounds ToMarchingCubeBounds(Vector3 position)
            => new Bounds(position.Floor() + new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f));

        #endregion

        #region Naming

        public static string ToChunkName(string fileName, Vector3Int chunk) {
            return $"{fileName}_{chunk.x}_{chunk.y}_{chunk.z}";
        }

        #endregion
    }
}
