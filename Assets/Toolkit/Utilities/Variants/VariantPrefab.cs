using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Variants/Variant Prefab")]
    public class VariantPrefab : MonoBehaviour, IVariant
    {
        #region Variables

        [SerializeField] private bool spawnAtAwake = true;
        [SerializeField] private bool spawnAsChild = true;
        [SerializeField] private Transform container = null;
        [SerializeField] private WeightedPrefab[] prefabs = new WeightedPrefab[0];

        #endregion

        #region Properties

        public int PrefabCount => prefabs.Length;
        public GameObject Prefab => prefabs.RandomElement(Weight)?.Prefab;
        public bool SpawnAsChild => spawnAsChild;
        public Transform Container => container != null ? container : transform;

        int IVariant.VariantCount => PrefabCount;

        #endregion

        #region Unity Methods

        private void Awake() {
            if(spawnAtAwake) {
                Spawn();
            }
        }

        #endregion

        #region IVariant Impl

        void IVariant.SetVariant() => Spawn();

        void IVariant.SetVariant(int index) => Spawn(index);

        #endregion

        #region Utility Methods

        public GameObject Spawn() {
            if(spawnAsChild)
                return Instantiate(Prefab, Container);
            else
                return Instantiate(Prefab);
        }

        public GameObject Spawn(int index) {
            var pref = prefabs[index % prefabs.Length].Prefab;
            if(spawnAsChild)
                return Instantiate(pref, Container);
            else
                return Instantiate(pref);
        }

        private static float Weight(WeightedPrefab p) => p.Weight;

        #endregion

        #region Editor

        [ContextMenu("Apply Container")]
        private void ApplyContainer() {
            if(container == null)
                container = transform;
        }

        #endregion
    }

    [System.Serializable]
    public class WeightedPrefab
    {
        #region Variables

        [SerializeField] private float weight = 1f;
        [SerializeField] private GameObject prefab = null;

        #endregion

        #region Properties

        public float Weight => weight;
        public GameObject Prefab => prefab;

        #endregion

        #region Constructor

        public WeightedPrefab() {
            weight = 1f;
        }

        public WeightedPrefab(float weight) {
            this.weight = weight;
        }

        public WeightedPrefab(float weight, GameObject prefab) {
            this.weight = weight;
            this.prefab = prefab;
        }

        public WeightedPrefab(GameObject prefab) {
            this.weight = 1f;
            this.prefab = prefab;
        }

        #endregion
    }
}
