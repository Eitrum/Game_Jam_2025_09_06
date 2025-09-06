using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [StructLayout(LayoutKind.Explicit, Size = INFO_SIZE * INFO_LENGTH + 1)]
    public unsafe struct SplatData
    {
        #region Variables

        public const float MINIMUM_PAINT = 0.001f;
        public const int INFO_SIZE = 5;
        public const int INFO_LENGTH = 8;

        [FieldOffset(0)]
        private byte index;

        [FieldOffset(1)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = INFO_LENGTH)]
        private fixed byte id[INFO_LENGTH];

        [FieldOffset(1 + INFO_LENGTH)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * INFO_LENGTH)]
        private fixed float weight[INFO_LENGTH];

        #endregion

        #region Properties

        public bool HasPaint => index > 0 || weight[0] > 0;
        public SplatData Normalized {
            get {
                if(index == 0)
                    return new SplatData();

                float totalWeight = 0f;
                for(int i = 0; i < index; i++) {
                    totalWeight += weight[i];
                }
                var temp = new SplatData();
                temp.Add(this, 1f / totalWeight);
                return temp;
            }
        }

        public float TotalWeight {
            get {
                float totalWeight = 0f;
                for(int i = 0; i < index; i++) {
                    totalWeight += weight[i];
                }
                return totalWeight;
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Returns a normalized splat values based on the weights. This is designed for UnityEngine.Terrain.
        /// </summary>
        /// <returns></returns>
        public float[] GetSplatArray() {
            int length = 0;
            float totalWeight = 0f;
            for(int i = 0; i < index; i++) {
                length = Math.Max(length, id[i]);
                totalWeight += weight[i];
            }
            float[] result = new float[length];
            for(int i = 0; i < index; i++) {
                result[id[i] - 1] = weight[i] / totalWeight;
            }
            return result;
        }

        #endregion

        #region Add

        public bool Add(int textureIndex, float weight) {
            if(weight < MINIMUM_PAINT)
                return false;
            for(int i = 0; i < index; i++) {
                if(this.id[i] - 1 == textureIndex) {
                    this.weight[i] += weight;
                    return true;
                }
            }

            if(index < INFO_LENGTH) {
                this.id[index] = (byte)(textureIndex + 1);
                this.weight[index] = weight;
                index++;
                return true;
            }
            return false;
        }

        public int Add(SplatData otherData) {
            var count = 0;
            for(int i = 0; i < otherData.index; i++) {
                if(Add(otherData.id[i] - 1, otherData.weight[i])) {
                    count++;
                }
            }
            return count;
        }

        public int Add(SplatData otherData, float strength) {
            var count = 0;
            for(int i = 0; i < otherData.index; i++) {
                if(Add(otherData.id[i] - 1, otherData.weight[i] * strength)) {
                    count++;
                }
            }
            return count;
        }

        #endregion

        #region Checks

        public bool HasTextureIndex(int textureIndex) {
            for(int i = 0; i < index; i++) {
                if(id[i] - 1 == textureIndex)
                    return true;
            }
            return false;
        }

        public float GetTextureWeight(int textureIndex) {
            for(int i = 0; i < index; i++) {
                if(id[i] - 1 == textureIndex)
                    return weight[i];
            }
            return 0f;
        }

        public float GetTexturePercentage(int textureIndex) {
            for(int i = 0; i < index; i++) {
                if(id[i] - 1 == textureIndex) {
                    return weight[i] / TotalWeight;
                }
            }
            return 0f;
        }

        #endregion

        #region Clear

        public void ClearAll() {
            for(int i = 0; i < INFO_LENGTH; i++) {
                id[i] = 0;
                weight[i] = 0f;
            }
            index = 0;
        }

        #endregion

        #region Merge

        public static SplatData Lerp(SplatData lhs, SplatData rhs, float t) {
            var data = new SplatData();
            for(int i = 0; i < lhs.index; i++) {
                data.Add(lhs.id[i], Mathf.Lerp(lhs.weight[i], 0f, t));
            }
            for(int i = 0; i < rhs.index; i++) {
                data.Add(rhs.id[i], Mathf.Lerp(0f, rhs.weight[i], t));
            }

            return data;
        }

        #endregion
    }
}
