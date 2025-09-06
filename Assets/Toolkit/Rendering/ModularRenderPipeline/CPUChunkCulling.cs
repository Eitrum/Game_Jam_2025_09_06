using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering {
    public class CPUChunkCulling {

        #region Variables

        public const float MAXIMUM_RENDER_DISTANCE = 1500;

        private readonly float chunkSize;
        private readonly float invChunkSize;
        private readonly float chunkOverlap;
        private readonly float negativeRadius;

        // Calculation Cost
        private TimeSpan calculationCost;
        private long timeStamp;

        // Caches
        private Plane[] frustumPlanes =  new Plane[6];
        private List<ChunkCullingResult> worldPositionDistance = new List<ChunkCullingResult>();

        public bool SortByDistance { get; set; } = false;

        #endregion

        #region Properties

        public GridIterator ChunkArea { get; private set; }
        public int ChunkCount { get; private set; }
        public IReadOnlyList<ChunkCullingResult> CullingResults => worldPositionDistance;

        public TimeSpan CalculationCost => calculationCost;

        #endregion

        #region Constructor

        public CPUChunkCulling(float chunkSize, float chunkOverlap) : this(chunkSize, chunkOverlap, false) { }

        public CPUChunkCulling(float chunkSize, float chunkOverlap, bool sortByDistance) {
            this.chunkSize = chunkSize;
            this.invChunkSize = 1f / chunkSize;
            this.chunkOverlap = chunkOverlap;
            this.negativeRadius = -((this.chunkSize + this.chunkOverlap) * 0.866f);
            this.SortByDistance = sortByDistance;
        }

        #endregion

        #region Run

        public void Dispatch(Camera cam) {
            timeStamp = PerformanceUtility.Timestamp;
            using CameraUtility.FarClipPlaneScope cameraScope = new CameraUtility.FarClipPlaneScope(cam, Mathf.Min(cam.farClipPlane, MAXIMUM_RENDER_DISTANCE));

            GeometryUtility.CalculateFrustumPlanes(cam, frustumPlanes);
            var chunk = ChunkArea = new GridIterator(cam, chunkSize);
            worldPositionDistance.Clear();

            var cPos = cam.transform.position;
            var maxDistance = cam.farClipPlane;

            // Prepare values
            var norm0 = frustumPlanes[0].normal;
            var dist0 = frustumPlanes[0].distance;

            var norm1 = frustumPlanes[1].normal;
            var dist1 = frustumPlanes[1].distance;

            var norm2 = frustumPlanes[2].normal;
            var dist2 = frustumPlanes[2].distance;

            var norm3 = frustumPlanes[3].normal;
            var dist3 = frustumPlanes[3].distance;

            // We can skip front and back plane by distance check.
            //var norm4 = frustumPlanes[4].normal;
            //var dist4 = frustumPlanes[4].distance;

            //var norm5 = frustumPlanes[5].normal;
            //var dist5 = frustumPlanes[5].distance;

            for(int x = 0; x < chunk.width; x++) {
                for(int y = 0; y < chunk.height; y++) {
                    for(int z = 0; z < chunk.depth; z++) {
                        var chunkPos = chunk.GetOffsetAsVector3(x, y, z) * chunkSize;
                        var dist = Vector3.Distance(cPos, chunkPos);
                        if(dist > maxDistance)
                            continue;

                        if((Vector3.Dot(norm0, chunkPos) + dist0) < negativeRadius)
                            continue;

                        if((Vector3.Dot(norm1, chunkPos) + dist1) < negativeRadius)
                            continue;

                        if((Vector3.Dot(norm2, chunkPos) + dist2) < negativeRadius)
                            continue;

                        if((Vector3.Dot(norm3, chunkPos) + dist3) < negativeRadius)
                            continue;

                        //if((Vector3.Dot(norm4, chunkPos) + dist4) < negativeRadius)
                        //    continue;

                        //if((Vector3.Dot(norm5, chunkPos) + dist5) < negativeRadius)
                        //    continue;

                        worldPositionDistance.Add(new ChunkCullingResult(chunk.x + x, chunk.y + y, chunk.z + z, dist));
                    }
                }
            }

            ChunkCount = worldPositionDistance.Count;

            if(SortByDistance)
                Sort.Merge(worldPositionDistance, SortByW);
            calculationCost = PerformanceUtility.GetElapsedTimeSpan(timeStamp);
        }

        private static int SortByW(ChunkCullingResult lhs, ChunkCullingResult rhs) => lhs.distance.CompareTo(rhs.distance);

        public int ReadResults() {
            return ChunkCount;
        }

        public int ReadResults(out List<ChunkCullingResult> positions) {
            positions = worldPositionDistance;
            return ChunkCount;
        }

        #endregion
    }
}
