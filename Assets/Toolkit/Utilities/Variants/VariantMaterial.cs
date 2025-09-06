using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [DefaultExecutionOrder(-2000)]
    [AddComponentMenu("Toolkit/Utility/Variants/Variant Material")]
    public class VariantMaterial : MonoBehaviour, IVariant
    {
        #region Variables

        [SerializeField] private bool activateAtAwake = true;
        [SerializeField] private WeightedMaterial[] materials = { };
        [SerializeField] private Renderer[] renderers = { };
        [SerializeField] private int sharedMaterialIndex = 0;

        #endregion

        #region Properties

        public int VariantCount => materials.Length;
        public Material Variant => materials.RandomElement(Weight).Material;

        #endregion

        #region Unity Methods

        private void Awake() {
            if(activateAtAwake)
                SetVariant();
        }

        #endregion

        #region Utility

        [ContextMenu("Apply Random")]
        public void SetVariant() {
            var mat = Variant;
            if(sharedMaterialIndex > 0) {
                foreach(var rend in renderers) {
                    var mats = rend.sharedMaterials;
                    mats[sharedMaterialIndex % mats.Length] = mat;
                    rend.sharedMaterials = mats;
                }
            }
            else {
                foreach(var rend in renderers)
                    rend.sharedMaterial = mat;
            }
        }

        public void SetVariant(int index) {
            var mat = materials[index % materials.Length].Material;
            if(sharedMaterialIndex > 0) {
                foreach(var rend in renderers) {
                    var mats = rend.sharedMaterials;
                    mats[sharedMaterialIndex % mats.Length] = mat;
                    rend.sharedMaterials = mats;
                }
            }
            else {
                foreach(var rend in renderers)
                    rend.sharedMaterial = mat;
            }
        }

        public Material GetVariant(int index) {
            return materials[index].Material;
        }

        public static float Weight(WeightedMaterial mat) => mat.Weight;

        #endregion


        [System.Serializable]
        public class WeightedMaterial
        {
            #region Variables

            [SerializeField] private float weight = 1f;
            [SerializeField] private Material material;

            #endregion

            #region Properties

            public float Weight => weight;
            public Material Material => material;
            public bool IsValid => material != null && weight > Mathf.Epsilon;

            #endregion

            #region Constructor

            public WeightedMaterial() {
                this.weight = 1f;
            }

            public WeightedMaterial(float weight) {
                this.weight = weight;
            }

            public WeightedMaterial(Material material) {
                this.weight = 1f;
                this.material = material;
            }

            public WeightedMaterial(float weight, Material material) {
                this.weight = weight;
                this.material = material;
            }

            #endregion
        }
    }
}
