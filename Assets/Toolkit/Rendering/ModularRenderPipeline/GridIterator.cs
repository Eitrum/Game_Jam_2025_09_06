
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Toolkit.Rendering {
    [System.Serializable]
    public struct GridIterator : IEnumerable<(int x, int y, int z)> {
        #region Variables

        public int x;
        public int y;
        public int z;

        public int width;
        public int height;
        public int depth;

        #endregion

        #region Properties

        public int Area => width * height * depth;

        public Vector3Int Position {
            get => new Vector3Int(x, y, z);
            set {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public Vector3Int Size {
            get => new Vector3Int(width, height, depth);
            set {
                width = value.x;
                height = value.y;
                depth = value.z;
            }
        }

        #endregion

        #region Constructor

        public GridIterator(int x, int y, int z, int size) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = size;
            this.height = size;
            this.depth = size;
        }

        public GridIterator(Bounds bounds, float chunkSize) {
            var min = bounds.min.Snap(chunkSize);
            var max = bounds.max.Snap(chunkSize);
            var invChunkSize = 1f / chunkSize;
            var xyz = (min * invChunkSize).RoundToInt();
            var whd = ((max - min) * invChunkSize).RoundToInt();

            x = xyz.x;
            y = xyz.y;
            z = xyz.z;

            width = whd.x + 1;
            height = whd.y + 1;
            depth = whd.z + 1;
        }

        public GridIterator(Camera cam, float chunkSize) : this(cam.GetAABB(), chunkSize) { }

        #endregion

        #region Get Position By Index

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) GetPositionByIndex(int index, Space space) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            if(space == Space.Self)
                return (x, y, z);
            else
                return (this.x + x, this.y + y, this.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) GetPositionByIndex(uint index, Space space)
            => GetPositionByIndex((int)index, space);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPositionByIndex_Vector(int index, Space space) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            if(space == Space.Self)
                return new(x, y, z);
            else
                return new(this.x + x, this.y + y, this.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPositionByIndex_Vector(uint index, Space space)
            => GetPositionByIndex_Vector((int)index, space);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) GetPositionByIndexLocal(int index) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return (x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPositionByIndexLocal_Vector(int index) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return new(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) GetPositionByIndexWorld(int index) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return (this.x + x, this.y + y, this.z + z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int x, int y, int z) GetPositionByIndexWorld(uint index) 
            => GetPositionByIndexWorld((int)index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int GetPositionByIndexWorld_Vector(int index) {
            int sliceSize = height * depth;
            int x = index / sliceSize;
            int remainder = index % sliceSize;
            int y = remainder / depth;
            int z = remainder % depth;
            return new(this.x + x, this.y + y, this.z + z);
        }

        #endregion

        #region GetIndex By Position

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndex(Vector3Int position, Space space, out int index) {
            index = -1;
            switch(space) {
                case Space.World: {
                        if(position.x < x || position.y < y || position.z < z)
                            return false;
                        position -= this.Position;
                        if(position.x > width || position.y > height || position.z > depth)
                            return false;

                        index = (position.x * height * depth) + (position.y * depth) + (position.z);
                        return true;
                    }
                case Space.Self:
                    if(position.x < x || position.y < y || position.z < z)
                        return false;
                    if(position.x > width || position.y > height || position.z > depth)
                        return false;
                    index = (position.x * height * depth) + (position.y * depth) + (position.z);
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndexUnsafe(Vector3Int position, Space space, out int index) {
            index = -1;
            switch(space) {
                case Space.World: {
                        position -= this.Position;
                        index = (position.x * height * depth) + (position.y * depth) + (position.z);
                        return true;
                    }
                case Space.Self:
                    index = (position.x * height * depth) + (position.y * depth) + (position.z);
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndexLocal(Vector3Int position, out int index) {
            index = -1;
            if(position.x < x || position.y < y || position.z < z)
                return false;
            if(position.x > width || position.y > height || position.z > depth)
                return false;
            index = (position.x * height * depth) + (position.y * depth) + (position.z);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndexLocalUnsafe(Vector3Int position, out int index) {
            index = (position.x * height * depth) + (position.y * depth) + (position.z);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndexWorld(Vector3Int position, out int index) {
            index = -1;
            if(position.x < x || position.y < y || position.z < z)
                return false;
            position -= this.Position;
            if(position.x > width || position.y > height || position.z > depth)
                return false;
            index = (position.x * height * depth) + (position.y * depth) + (position.z);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIndexWorldUnsafe(Vector3Int position, out int index) {
            position -= this.Position;
            index = (position.x * height * depth) + (position.y * depth) + (position.z);
            return true;
        }

        #endregion

        #region IEnumerable Impl

        IEnumerator IEnumerable.GetEnumerator() {
            return new IteratorLocal(this);
        }

        IEnumerator<(int x, int y, int z)> IEnumerable<(int x, int y, int z)>.GetEnumerator() {
            return new IteratorLocal(this);
        }

        #endregion

        #region Offset

        /// <summary>
        /// returns (value + griditerator.xyz)
        /// </summary>
        public Vector3 GetOffset(Vector3 value) => new(value.x + x, value.y + y, value.z + z);
        /// <summary>
        /// returns (xyz + griditerator.xyz)
        /// </summary>
        public Vector3 GetOffset(float x, float y, float z) => new(this.x + x, this.y + y, this.z + z);
        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public (float x, float y, float z) GetOffset((float x, float y, float z) pos) => (this.x + pos.x, this.y + pos.y, this.z + pos.z);

        /// <summary>
        /// returns (value + griditerator.xyz)
        /// </summary>
        public Vector3Int GetOffset(Vector3Int value) => new(value.x + x, value.y + y, value.z + z);
        /// <summary>
        /// returns (xyz + griditerator.xyz)
        /// </summary>
        public Vector3Int GetOffset(int x, int y, int z) => new(this.x + x, this.y + y, this.z + z);
        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public (int x, int y, int z) GetOffset((int x, int y, int z) pos) => (this.x + pos.x, this.y + pos.y, this.z + pos.z);

        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public Vector3 GetOffsetAsVector3((int x, int y, int z) pos) => new(this.x + pos.x, this.y + pos.y, this.z + pos.z);
        /// <summary>
        /// returns (xyz + griditerator.xyz)
        /// </summary>
        public Vector3 GetOffsetAsVector3(int x, int y, int z) => new(this.x + x, this.y + y, this.z + z);
        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public Vector3 GetOffsetAsVector3((float x, float y, float z) pos) => new(this.x + pos.x, this.y + pos.y, this.z + pos.z);

        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public Vector3Int GetOffsetAsVector3Int((int x, int y, int z) pos) => new(this.x + pos.x, this.y + pos.y, this.z + pos.z);
        /// <summary>
        /// returns (pos.xyz + griditerator.xyz)
        /// </summary>
        public Vector3Int GetOffsetAsVector3Int((float x, float y, float z) pos) => new(this.x + Mathf.RoundToInt(pos.x), this.y + Mathf.RoundToInt(pos.y), this.z + Mathf.RoundToInt(pos.z));

        #endregion

        #region Iterator

        public struct IteratorLocal : IEnumerator<Vector3Int>, IEnumerator<(int x, int y, int z)> {

            #region Variables

            private GridIterator grid;
            private int index;
            private int length;

            #endregion

            #region Constructor

            public IteratorLocal(GridIterator grid) {
                this.grid = grid;
                index = -1;
                length = grid.Area;
            }

            #endregion

            #region IEnumerator Impl

            object IEnumerator.Current => grid.GetPositionByIndexLocal_Vector(index);
            Vector3Int IEnumerator<Vector3Int>.Current => grid.GetPositionByIndexLocal_Vector(index);
            (int x, int y, int z) IEnumerator<(int x, int y, int z)>.Current => grid.GetPositionByIndexLocal(index);

            public bool MoveNext() {
                index++;
                return (index < length);
            }

            public void Reset() {
                index = -1;
            }

            public void Dispose() { }

            #endregion
        }

        public struct IteratorWorld : IEnumerator<Vector3Int>, IEnumerator<(int x, int y, int z)> {

            #region Variables

            private GridIterator grid;
            private int index;
            private int length;

            #endregion

            #region Constructor

            public IteratorWorld(GridIterator grid) {
                this.grid = grid;
                index = -1;
                length = grid.Area;
            }

            #endregion

            #region IEnumerator Impl

            object IEnumerator.Current => grid.GetPositionByIndexWorld_Vector(index);
            Vector3Int IEnumerator<Vector3Int>.Current => grid.GetPositionByIndexWorld_Vector(index);
            (int x, int y, int z) IEnumerator<(int x, int y, int z)>.Current => grid.GetPositionByIndexWorld(index);

            public bool MoveNext() {
                index++;
                return (index < length);
            }

            public void Reset() {
                index = -1;
            }

            public void Dispose() { }

            #endregion
        }

        #endregion

        #region Editor

        public void DrawGizmos(float size, GizmosUtility.DrawData gridPoints = default) {
            float cx = x + (width - 1) * 0.5f;
            float cy = y + (height - 1) * 0.5f;
            float cz = z + (depth - 1) * 0.5f;

            Gizmos.DrawWireCube(new Vector3(cx * size, cy * size, cz * size), new Vector3((width) * size, (height) * size, (depth) * size));

            var representation = gridPoints.Representation;
            var representationSize = gridPoints.Size;

            if(representation == GizmosUtility.Representation.None || representationSize < Mathf.Epsilon)
                return;

            using(new GizmosUtility.ColorScope(gridPoints.Color)) {
                switch(representation) {
                    case GizmosUtility.Representation.Sphere: {
                            foreach(var pos in this) {
                                var world = GetOffsetAsVector3(pos);
                                Gizmos.DrawSphere(world * size, representationSize);
                            }
                        }
                        break;
                    case GizmosUtility.Representation.WireSphere: {
                            foreach(var pos in this) {
                                var world = GetOffsetAsVector3(pos);
                                Gizmos.DrawWireSphere(world * size, representationSize);
                            }
                        }
                        break;
                    case GizmosUtility.Representation.WireCube: {
                            var cubeSize = new Vector3(representationSize, representationSize, representationSize);
                            foreach(var pos in this) {
                                var world = GetOffsetAsVector3(pos);
                                Gizmos.DrawWireCube(world * size, cubeSize);
                            }
                        }
                        break;
                    case GizmosUtility.Representation.Cube: {
                            var cubeSize = new Vector3(representationSize, representationSize, representationSize);
                            foreach(var pos in this) {
                                var world = GetOffsetAsVector3(pos);
                                Gizmos.DrawCube(world * size, cubeSize);
                            }
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
