using UnityEngine;

namespace Toolkit.Toxel {
    [System.Serializable]
    public struct ToxelMaterialData {
        #region Variables

        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float amount0;
        [SerializeField] private byte materialId0;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float amount1;
        [SerializeField] private byte materialId1;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float amount2;
        [SerializeField] private byte materialId2;

        #endregion

        #region Properties

        public float Amount => Mathf.Clamp01(amount0 + amount1 + amount2);

        public byte DominantMaterialId {
            get {
                if(amount1 > amount0 & amount1 > amount2) return materialId1;
                if(amount2 > amount0 & amount2 > amount1) return materialId2;
                return materialId0;
            }
        }

        public ToxelMaterialData Normalized {
            get {
                var a = amount0 + amount1 + amount2;
                if(a <= Mathf.Epsilon)
                    return default;


                return default;
            }
        }

        public byte SecondMaterialId {
            get {
                if(amount1 > amount0 & amount1 <= amount2) return materialId1;
                if(amount2 > amount0 & amount2 <= amount1) return materialId2;
                return materialId0;
            }
        }

        public byte ThirdMaterialId {
            get {
                if(amount1 <= amount0 & amount1 <= amount2) return materialId1;
                if(amount2 <= amount0 & amount2 <= amount1) return materialId2;
                return materialId0;
            }
        }

        #endregion

        #region Constructor

        public ToxelMaterialData(float amount) {
            this.amount0 = amount;
            this.amount1 = 0f;
            this.amount2 = 0f;
            this.materialId0 = 0;
            this.materialId1 = 0;
            this.materialId2 = 0;
        }

        public ToxelMaterialData(float amount, byte materialid) {
            this.amount0 = amount;
            this.amount1 = 0f;
            this.amount2 = 0f;
            this.materialId0 = materialid;
            this.materialId1 = 0;
            this.materialId2 = 0;
        }

        public ToxelMaterialData(float amount0, byte materialId0, float amount1, byte materialId1, float amount2, byte materialId2) {
            this.amount0 = amount0;
            this.amount1 = amount1;
            this.amount2 = amount2;
            this.materialId0 = materialId0;
            this.materialId1 = materialId1;
            this.materialId2 = materialId2;
        }

        #endregion

        #region Add

        public void Add(float amount, byte materialid) {
            if(materialid == materialId0)
                amount0 += amount;
            else if(materialid == materialId1)
                amount1 += amount;
            else if(materialid == materialId2)
                amount2 += amount;
            else if(amount0 < Mathf.Epsilon) {
                materialId0 = materialid;
                amount0 = amount;
            }
            else if(amount1 < Mathf.Epsilon) {
                materialId1 = materialid;
                amount1 = amount;
            }
            else if(amount2 < Mathf.Epsilon) {
                materialId2 = materialid;
                amount2 = amount;
            }
            else {

            }
        }

        public void Add(ToxelMaterialData materialData) {
            Add(materialData.amount0, materialData.materialId0);
            Add(materialData.amount1, materialData.materialId1);
            Add(materialData.amount2, materialData.materialId2);
        }

        #endregion

        #region Operators

        public static implicit operator float(ToxelMaterialData voxelMaterialData) => voxelMaterialData.Amount;

        #endregion
    }
}
