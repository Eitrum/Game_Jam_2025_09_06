using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [DefaultExecutionOrder(-2000)]
    [AddComponentMenu("Toolkit/Utility/Variants/Variant Child Transforms")]
    public class VariantChildTransform : MonoBehaviour, IVariant
    {
        #region Variables

        [SerializeField] private bool activateAtAwake = true;
        [SerializeField] private WeightedTransform[] transforms = new WeightedTransform[0];

        #endregion

        #region Properties

        public int VariantCount => transforms.Length;
        public Transform Variant => transforms.RandomElement(Weight).Transform;

        #endregion

        #region Unity Method

        private void Awake() {
            if(activateAtAwake)
                SetVariant();
        }

        #endregion

        #region Utility Methods

        public void SetVariant() {
            foreach(var tra in transforms)
                tra.Transform.SetActive(false);

            var t = transforms.RandomElement(Weight);
            if(t != null)
                t.Transform.SetActive(true);
        }

        public void SetVariant(int index) {
            index = index % transforms.Length;
            for(int i = 0, length = transforms.Length; i < length; i++) {
                transforms[i].Transform.SetActive(i == index);
            }
        }

        private static float Weight(WeightedTransform t) => t.Weight;

        #endregion
    }

    [System.Serializable]
    public class WeightedTransform
    {
        #region Variables

        [SerializeField] private float weight = 1f;
        [SerializeField] private Transform transform = null;

        #endregion

        #region Properties

        public float Weight => weight;
        public Transform Transform => transform;
        public bool IsValid => transform != null && weight > 0f;

        #endregion

        #region Constructor

        public WeightedTransform() {
            weight = 1f;
        }

        public WeightedTransform(float weight) {
            this.weight = weight;
        }

        public WeightedTransform(float weight, Transform transform) {
            this.weight = weight;
            this.transform = transform;
        }

        public WeightedTransform(Transform transform) {
            this.weight = 1f;
            this.transform = transform;
        }

        #endregion
    }
}
