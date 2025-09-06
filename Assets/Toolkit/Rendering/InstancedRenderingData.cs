using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    [System.Serializable]
    public class InstancedRenderingData
    {
        #region Variables

        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private Material[] materials;
        [SerializeField] private Matrix4x4[] locations;

        private int length = -1;
        private float percentage = 1f;

        public int Length {
            get {
                if(length == -1)
                    return locations.Length;
                return length;
            }
            set {
                if(value < -1)
                    length = -1;
                else
                    length = Mathf.Min(value, locations.Length);
            }
        }

        public float DrawPercentage {
            get => percentage;
            set {
                percentage = Mathf.Clamp01(value);
            }
        }

        #endregion

        #region Properties

        public Mesh Mesh => mesh;
        public Material Material => material;
        public Material[] Materials => materials;
        public IReadOnlyList<Matrix4x4> Locations => locations;
        public Matrix4x4[] Raw => locations;

        #endregion

        #region Constructor

        public InstancedRenderingData() { }

        public InstancedRenderingData(Mesh mesh, Material material, Matrix4x4[] locations) {
            this.mesh = mesh;
            this.material = material;
            this.materials = new Material[1] { material };
            this.locations = locations;
        }

        public InstancedRenderingData(Mesh mesh, Material[] materials, Matrix4x4[] locations) {
            this.mesh = mesh;
            this.material = materials[0];
            this.materials = materials;
            this.locations = locations;
        }

        public InstancedRenderingData(Mesh mesh, Material[] materials, List<Matrix4x4> locations) {
            this.mesh = mesh;
            this.material = materials[0];
            this.materials = materials;
            this.locations = locations.ToArray();
        }

        #endregion

        #region Render

        public void Render() {
            var l = Mathf.RoundToInt(Length * percentage);
            if(materials != null && materials.Length > 0)
                for(int m = 0, length = materials.Length; m < length; m++)
                    Graphics.DrawMeshInstanced(mesh, m, materials[m], locations, l, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0);
            else
                for(int i = 0, count = mesh.subMeshCount; i < count; i++)
                    Graphics.DrawMeshInstanced(mesh, i, material, locations, l, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0);
        }

        public void Render(Material material) {
            var l = Mathf.RoundToInt(Length * percentage);
            for(int i = 0, count = mesh.subMeshCount; i < count; i++)
                Graphics.DrawMeshInstanced(mesh, i, material, locations, l, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0);
        }

        public void Render(UnityEngine.Rendering.ShadowCastingMode shadowCasting, bool receiveShadows, int layer) {
            var l = Mathf.RoundToInt(Length * percentage);
            if(materials != null && materials.Length > 0)
                for(int m = 0, length = materials.Length; m < length; m++)
                    Graphics.DrawMeshInstanced(mesh, m, materials[m], locations, l, null, shadowCasting, receiveShadows, layer);
            else
                for(int i = 0, count = mesh.subMeshCount; i < count; i++)
                    Graphics.DrawMeshInstanced(mesh, i, material, locations, l, null, shadowCasting, receiveShadows, layer);
        }

        public void Render(Material material, UnityEngine.Rendering.ShadowCastingMode shadowCasting, bool receiveShadows, int layer) {
            var l = Mathf.RoundToInt(Length * percentage);
            for(int i = 0, count = mesh.subMeshCount; i < count; i++)
                Graphics.DrawMeshInstanced(mesh, 0, material, locations, l, null, shadowCasting, receiveShadows, layer);
        }

        #endregion

        #region Optimization

        public void Optimize(Vector3 position, float drawDistance) {
            Toolkit.Sort.Merge(locations, (a, b) => Vector3.Distance(a.GetPosition(), position).CompareTo(Vector3.Distance(b.GetPosition(), position)), locations.Length);
            for(int i = locations.Length - 1; i >= 0; i--) {
                var dist = Vector3.Distance(locations[i].GetPosition(), position);
                if(dist < drawDistance) {
                    Length = i;
                    return;
                }
            }
            Length = 0;
        }

        #endregion

        #region Removal

        public void RemoveAt(Vector3 position, float radius) {
            int toRemove = 0;
            int lastIndex = locations.Length - 1;
            for(int i = lastIndex; i >= 0; i--) {
                if(Vector3.Distance(locations[i].GetPosition(), position) < radius) {
                    Replace(i, lastIndex);
                    lastIndex--;
                    toRemove++;
                }
            }
            if(toRemove > 0)
                Array.Resize(ref locations, lastIndex + 1);
        }

        private void Replace(int index, int secondIndex) {
            var t = locations[index];
            locations[index] = locations[secondIndex];
            locations[secondIndex] = t;
        }

        #endregion
    }
}
