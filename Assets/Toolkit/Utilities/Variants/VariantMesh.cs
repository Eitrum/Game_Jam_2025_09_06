using UnityEngine;

namespace Toolkit.Utility
{
    [DefaultExecutionOrder(-2000)]
    [AddComponentMenu("Toolkit/Utility/Variants/Variant Mesh")]
    public class VariantMesh : MonoBehaviour, IVariant
    {
        #region Variables

        [SerializeField] private bool activateAtAwake = true;
        [SerializeField] private WeightedMesh[] meshes = { };
        [SerializeField] private MeshFilter[] meshFilters = { };
        [SerializeField] private bool perObject = false;

        #endregion

        #region Properties

        public int VariantCount => meshes.Length;
        public Mesh Variant => meshes.RandomElement(Weight).Mesh;

        #endregion

        #region Unity Methods

        private void Awake() {
            if(activateAtAwake)
                SetVariant();
        }

        #endregion

        #region Utility

        [ContextMenu("Apply Variants")]
        public void SetVariant() {
            if(!perObject) {
                SetVariant(meshes.RandomIndex(Weight));
                return;
            }
            foreach(var mf in meshFilters)
                mf.sharedMesh = Variant;
        }

        public void SetVariant(int index) {
            var m = meshes[index % meshes.Length].Mesh;
            foreach(var mf in meshFilters)
                mf.sharedMesh = m;
        }

        public Mesh GetVariant(int index) {
            return meshes[index].Mesh;
        }

        public static float Weight(WeightedMesh mat) => mat.Weight;

        #endregion



        [System.Serializable]
        public class WeightedMesh
        {
            #region Variables

            [SerializeField] private float weight = 1f;
            [SerializeField] private Mesh mesh;

            #endregion

            #region Properties

            public float Weight => weight;
            public Mesh Mesh => mesh;
            public bool IsValid => mesh != null && weight > Mathf.Epsilon;

            #endregion

            #region Constructor

            public WeightedMesh() {
                this.weight = 1f;
            }

            public WeightedMesh(float weight) {
                this.weight = weight;
            }

            public WeightedMesh(Mesh mesh) {
                this.weight = 1f;
                this.mesh = mesh;
            }

            public WeightedMesh(float weight, Mesh mesh) {
                this.weight = weight;
                this.mesh = mesh;
            }

            #endregion
        }
    }
}
