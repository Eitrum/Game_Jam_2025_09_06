using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering {

    public class GPUChunkCulling : IDisposable {

        [StructLayout(LayoutKind.Explicit)]
        private struct Chunk {
            [FieldOffset(0)] public uint id;
            [FieldOffset(4)] public float distance;
        }

        #region Variables

        public const float MAXIMUM_RENDER_DISTANCE = 1500;
        public const int MAXIMUM_CHUNKS = ushort.MaxValue;
        public const int DEFAULT_CHUNK_COUNT = 8_192;

        private readonly float chunkSize;
        private readonly float invChunkSize;
        private readonly float chunkOverlap;
        private readonly float negativeRadius;
        private int currentMaximumChunkCount;

        // Calculation Cost
        private TimeSpan calculationCost;
        private long timeStamp;

        // Compute Shader + Buffers
        public ComputeShader shader;

        public CommandBuffer buffer = new CommandBuffer() { name = "Chunk Compute" };
        private ComputeBuffer frustumBuffer;
        private ComputeBuffer visibleChunksBuffer;
        private bool isDisposed;

        // Caches
        private Plane[] frustumPlanes = new Plane[6];
        private Chunk[] visibleChunksResults;

        // Double buffering
        public bool UseDoubleBuffering { get; set; } = false;
        private GridIterator doubleBufferedChunkArea;
        private bool doubleBufferedRequestOnGoing;
        private int missedFrames = 0;

        // Results
        private List<ChunkCullingResult> cullingResults = new List<ChunkCullingResult>();
        private List<ChunkCullingResult> cullingResultsDoubleBuffer = new List<ChunkCullingResult>();

        public bool SortByDistance { get; set; } = false;
        private Awaitable<AsyncGPUReadbackRequest> awaitableAsyncRequest;

        #endregion

        #region Properties

        public GridIterator ChunkArea { get; private set; }
        public int ChunkCount { get; private set; }
        public List<ChunkCullingResult> CullingResults => cullingResults;

        public TimeSpan CalculationCost => calculationCost;
        public int MissedFrames => missedFrames;
        public bool IsDisposed => isDisposed;

        #endregion

        #region Constructor

        public GPUChunkCulling(float chunkSize, float chunkOverlap) : this(Resources.Load<ComputeShader>("ChunkCulling"), chunkSize, chunkOverlap, false, false) { }

        public GPUChunkCulling(float chunkSize, float chunkOverlap, bool doubleBuffer) : this(Resources.Load<ComputeShader>("ChunkCulling"), chunkSize, chunkOverlap, doubleBuffer, false) { }

        public GPUChunkCulling(float chunkSize, float chunkOverlap, bool doubleBuffer, bool sortByDistance) : this(Resources.Load<ComputeShader>("ChunkCulling"), chunkSize, chunkOverlap, doubleBuffer, sortByDistance) { }

        public GPUChunkCulling(ComputeShader shader, float chunkSize, float chunkOverlap, bool doubleBuffer, bool sortByDistance, int maxChunks = DEFAULT_CHUNK_COUNT) {
            this.shader = ComputeShader.Instantiate(shader);
            this.chunkSize = chunkSize;
            this.invChunkSize = 1f / chunkSize;
            this.chunkOverlap = chunkOverlap;

            this.UseDoubleBuffering = doubleBuffer;
            this.SortByDistance = sortByDistance;

            frustumBuffer = new ComputeBuffer(6, sizeof(float) * 4);
            visibleChunksBuffer = new ComputeBuffer(maxChunks, 8, ComputeBufferType.Append);

            Lifecycle.Track(buffer);
            Lifecycle.Track(frustumBuffer);
            Lifecycle.Track(visibleChunksBuffer);
            Lifecycle.Track(this.shader);

            this.currentMaximumChunkCount = maxChunks;
            if(!doubleBuffer)
                visibleChunksResults = new Chunk[maxChunks];

            this.shader.SetFloat("_ChunkSize", this.chunkSize);
            this.shader.SetFloat("_ChunkOverlap", this.chunkOverlap);
            this.shader.SetFloat("_NegativeRadius", -((this.chunkSize + this.chunkOverlap) * 0.866f));
            this.shader.SetFloat("_MaximumDistance", MAXIMUM_RENDER_DISTANCE);

            this.shader.SetBuffer(0, "visibleChunks", visibleChunksBuffer);
        }

        #endregion

        #region Finalize

#if UNITY_EDITOR
        ~GPUChunkCulling() {
            if(!isDisposed) {
                Dispose();
                Debug.LogError(this.FormatLog("GPU Chunk Culling is not properly disposed of!"));
            }
        }
#endif

        public void Dispose() {
            if(isDisposed)
                return;
            Lifecycle.Destroy(shader);
            Lifecycle.Destroy(buffer);
            Lifecycle.Destroy(frustumBuffer);
            Lifecycle.Destroy(visibleChunksBuffer);
            try {
                awaitableAsyncRequest?.Cancel();
            }
            catch { }
            isDisposed = true;
        }

        #endregion

        #region Run

        public void Dispatch(Camera cam) {
            if(UseDoubleBuffering && doubleBufferedRequestOnGoing) {
                missedFrames++;
                return;
            }
            missedFrames = 0;
            using CameraUtility.FarClipPlaneScope cameraScope = new CameraUtility.FarClipPlaneScope(cam, Mathf.Min(cam.farClipPlane, MAXIMUM_RENDER_DISTANCE));
            // Setup Frustum planes
            GeometryUtility.CalculateFrustumPlanes(cam, frustumPlanes);
            frustumBuffer.SetData(frustumPlanes);
            shader.SetBuffer(0, "_FrustumPlanes", frustumBuffer);

            // Calculate Chunk Area
            var chunkarea = new GridIterator(cam, chunkSize);
            ChunkArea = chunkarea;
            shader.SetInt("_Total", chunkarea.Area - 1);
            shader.SetInts("_Position", chunkarea.x, chunkarea.y, chunkarea.z);
            shader.SetInts("_Size", chunkarea.width, chunkarea.height, chunkarea.depth);
            shader.SetVector("_CameraLocation", cam.transform.position);
            shader.SetFloat("_MaximumDistance", cam.farClipPlane);

            // Dispatch
            visibleChunksBuffer.SetCounterValue(0);

            var count = chunkarea.Area;

            // Handle Buffer size increase
            if((count * 0.65f) > currentMaximumChunkCount && currentMaximumChunkCount != MAXIMUM_CHUNKS) {
                currentMaximumChunkCount *= 2;
                if(currentMaximumChunkCount > MAXIMUM_CHUNKS)
                    currentMaximumChunkCount = MAXIMUM_CHUNKS;
                visibleChunksBuffer.Release();
                visibleChunksBuffer.Dispose();
                visibleChunksBuffer = new ComputeBuffer(currentMaximumChunkCount, 8, ComputeBufferType.Append);
                this.shader.SetBuffer(0, "visibleChunks", visibleChunksBuffer);
            }

            var threadGroups = Mathf.CeilToInt(count / 64.0f);
            shader.Dispatch(0, threadGroups, 1, 1);

            if(UseDoubleBuffering)
                ReadResultsDoubleBuffered();
        }

        public int ReadResults() {
            if(UseDoubleBuffering)
                return ChunkCount;

            timeStamp = PerformanceUtility.Timestamp;

            // Copy from GPU
            ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            ComputeBuffer.CopyCount(visibleChunksBuffer, countBuffer, 0);

            int[] countArray = { 0 };
            countBuffer.GetData(countArray);
            int visibleCount = countArray[0];
            countBuffer.Release();

            ChunkCount = visibleCount;
            if(visibleChunksResults == null || visibleChunksResults.Length < currentMaximumChunkCount)
                visibleChunksResults = new Chunk[currentMaximumChunkCount];
            visibleChunksBuffer.GetData(visibleChunksResults, 0, 0, visibleCount);

            cullingResults.Clear();

            var area = ChunkArea;
            for(int i = 0; i < visibleCount; i++) {
                var c = visibleChunksResults[i];
                var result = new ChunkCullingResult(area.GetPositionByIndexWorld(c.id), c.distance);
                cullingResults.Add(result);
            }

            if(SortByDistance)
                Sort.Merge(cullingResults, SortByW);

            calculationCost = PerformanceUtility.GetElapsedTimeSpan(timeStamp);
            return visibleCount;
        }

        private static int SortByW(ChunkCullingResult lhs, ChunkCullingResult rhs) => lhs.distance.CompareTo(rhs.distance);

        private void ReadResultsDoubleBuffered() {
            doubleBufferedRequestOnGoing = true;
            timeStamp = PerformanceUtility.Timestamp;

            // Swap lists out
            var tlist = cullingResults;
            cullingResults = cullingResultsDoubleBuffer;
            cullingResultsDoubleBuffer = tlist;
            tlist.Clear();

            ChunkCount = cullingResults.Count;

            // Swap chunk areas
            var t = ChunkArea;
            ChunkArea = doubleBufferedChunkArea;
            doubleBufferedChunkArea = t;

            ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            ComputeBuffer.CopyCount(visibleChunksBuffer, countBuffer, 0);

            awaitableAsyncRequest = AsyncGPUReadback.RequestAsync(countBuffer);
            awaitableAsyncRequest.GetAwaiter().OnCompleted(async () => {
                if(isDisposed) {
                    countBuffer.Release();
                    return;
                }
                var req = await awaitableAsyncRequest;
                if(req.hasError) {
                    Debug.LogError(this.FormatLog("Failed to request chunk culling count"));
                    countBuffer.Release();
                    FinishDoubleBuffer();
                    return;
                }

                int visibleCount = req.GetData<int>()[0];
                countBuffer.Release();

                awaitableAsyncRequest = AsyncGPUReadback.RequestAsync(visibleChunksBuffer);
                awaitableAsyncRequest.GetAwaiter().OnCompleted(async () => {
                    var dataReq = await awaitableAsyncRequest;
                    if(isDisposed)
                        return;
                    if(dataReq.hasError) {
                        Debug.LogError(this.FormatLog("Failed to request chunk culling data"));
                        FinishDoubleBuffer();
                    }
                    else {
                        Unity.Collections.NativeArray<Chunk> resultData = dataReq.GetData<Chunk>();
                        for(int i = 0; i < visibleCount; i++) {
                            var c = resultData[i];
                            var result = new ChunkCullingResult(doubleBufferedChunkArea.GetPositionByIndexWorld(c.id), c.distance);
                            cullingResultsDoubleBuffer.Add(result);
                        }

                        resultData.Dispose();

                        if(SortByDistance) {
                            Toolkit.Threading.Job.Run(() => {
                                if(isDisposed)
                                    return;
                                cullingResultsDoubleBuffer.Sort(SortByW);
                                FinishDoubleBuffer();
                            });
                        }
                        else
                            FinishDoubleBuffer();
                    }
                });
            });
        }

        private void FinishDoubleBuffer() {
            if(isDisposed)
                return;
            doubleBufferedRequestOnGoing = false;
            calculationCost = PerformanceUtility.GetElapsedTimeSpan(timeStamp);
        }

        #endregion
    }
}
