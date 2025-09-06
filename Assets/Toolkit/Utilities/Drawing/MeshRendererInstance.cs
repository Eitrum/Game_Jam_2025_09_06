using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    [System.Serializable]
    public class MeshRendererInstance : IRendererInstance
    {
        #region Variables

        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private Matrix4x4 offset;

        #endregion

        #region Properties

        public Mesh Mesh => mesh;
        public Material Material => material;
        public Matrix4x4 Offset => offset;
        public Bounds Bounds => mesh != null ? mesh.bounds : default;

        #endregion

        #region Constructor

        public MeshRendererInstance(Mesh mesh, Material material) {
            this.mesh = mesh;
            this.material = material;
            this.offset = Matrix4x4.identity;
        }

        public MeshRendererInstance(Mesh mesh, Material material, Matrix4x4 offset) {
            this.mesh = mesh;
            this.material = material;
            this.offset = offset;
        }

        #endregion

        #region Drawing

        public void Draw(Matrix4x4 matrix) {
            Graphics.DrawMesh(mesh, matrix * offset, material, 1);
        }

        public void Draw(Matrix4x4 matrix, int layer) {
            Graphics.DrawMesh(mesh, matrix * offset, material, layer);
        }

        public void Draw(Matrix4x4 matrix, int layer, Camera camera) {
            Graphics.DrawMesh(mesh, matrix * offset, material, layer, camera);
        }


        public void Draw(Material customMaterial, Matrix4x4 matrix) {
            Graphics.DrawMesh(mesh, matrix * offset, customMaterial, 1);
        }

        public void Draw(Material customMaterial, Matrix4x4 matrix, int layer) {
            Graphics.DrawMesh(mesh, matrix * offset, customMaterial, layer);
        }

        public void Draw(Material customMaterial, Matrix4x4 matrix, int layer, Camera camera) {
            Graphics.DrawMesh(mesh, matrix * offset, customMaterial, layer, camera);
        }

        #endregion
    }
}
