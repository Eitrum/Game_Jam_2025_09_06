using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering {
    [System.Serializable]
    public struct ChunkCullingResult {

        #region Variables

        public static float DefaultChunkScale = 64f;

        // Chunk IDs
        public int x;
        public int y;
        public int z;

        // Distance
        public float distance;

        #endregion

        #region Constructor

        public ChunkCullingResult(in int x, in int y, in int z) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.distance = 0;
        }

        public ChunkCullingResult(in (int x, int y, int z) pos) {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
            this.distance = 0;
        }

        public ChunkCullingResult(in Vector3Int pos) {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
            this.distance = 0;
        }

        public ChunkCullingResult(in int x, in int y, in int z, in float distance) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.distance = distance;
        }

        public ChunkCullingResult(in (int x, int y, int z) pos, in float distance) {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
            this.distance = distance;
        }

        public ChunkCullingResult(in Vector3Int pos, in float distance) {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
            this.distance = distance;
        }

        #endregion

        #region Recalculate

        public void RecalculateDistance(Camera c) {
            distance = Vector3.Distance(GetWorldLocation(), c.transform.position);
        }

        public void RecalculateDistance(Camera c, float scale) {
            distance = Vector3.Distance(GetWorldLocation(scale), c.transform.position);
        }

        public void RecalculateDistance(Vector3 pos) {
            distance = Vector3.Distance(GetWorldLocation(), pos);
        }

        public void RecalculateDistance(Vector3 pos, float scale) {
            distance = Vector3.Distance(GetWorldLocation(scale), pos);
        }

        #endregion

        #region Conversion

        public Vector3 GetWorldLocation() => new Vector3(x * DefaultChunkScale, y * DefaultChunkScale, z * DefaultChunkScale);
        public Vector3 GetWorldLocation(float scale) => new Vector3(x * scale, y * scale, z * scale);

        public (int x, int y, int z) GetChunk() => (x, y, z);

        #endregion
    }
}
