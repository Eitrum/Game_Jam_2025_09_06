using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [ExecuteAlways]
    public class RiverMesh : BaseComponent
    {
        #region Variables

        [SerializeField, Layer] private int layer;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField, HideInInspector] private bool generatedMesh = false;

        #endregion

        #region Properties

        public Mesh Mesh {
            get {
                if(mesh == null) {
                    mesh = new Mesh();
                    mesh.name = "River Mesh";
                    generatedMesh = true;
                }
                return mesh;
            }
        }

        #endregion

        #region Initialization

        private void Start() {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
            this.enabled = !(mesh == null || material == null);
        }

        private void OnEnable() {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
            SubscribePostUpdate();
        }

        private void OnDisable() {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return;
#endif
            UnsubscribePostUpdate();
        }

        private void OnDestroy() {
            if(generatedMesh && mesh)
                Destroy(mesh);
        }

        #endregion

        #region Update

        protected override void PostUpdateComponent(float dt) => Render();

        public void Render() {
            Graphics.DrawMesh(mesh, Matrix4x4.identity, material, layer);
        }

#if UNITY_EDITOR
        private void LateUpdate() {
            if(!Application.isPlaying && mesh != null && material != null) {
                Render();
            }
        }
#endif

        #endregion
    }
}
