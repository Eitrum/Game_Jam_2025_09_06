using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Rendering
{
    [System.Serializable]
    public class GameObjectRendererInstance : IRendererInstance
    {
        #region Variables

        [SerializeField] private List<MeshRendererInstance> instances = new List<MeshRendererInstance>();
        [System.NonSerialized] private Bounds? bounds;
        private static List<Renderer> renderers = new List<Renderer>();

        #endregion

        #region Properties

        public IReadOnlyList<MeshRendererInstance> Instances => instances;
        public Bounds Bounds {
            get {
                if(!bounds.HasValue) {
                    bounds = BoundsExtension.Combine(instances.Select(x => x.Bounds));
                }
                return bounds.Value;
            }
        }

        #endregion

        #region Constructor

        public GameObjectRendererInstance(MeshRendererInstance instance) {
            instances.Add(instance);
            bounds = null;
        }

        public GameObjectRendererInstance(IReadOnlyList<MeshRendererInstance> instances) {
            this.instances.AddRange(instances);
            bounds = null;
        }

        public GameObjectRendererInstance(IEnumerable<MeshRendererInstance> instances) {
            this.instances.AddRange(instances);
            bounds = null;
        }

        public GameObjectRendererInstance(GameObject gameObject, bool includeChildren = true) {
            Add(gameObject, includeChildren);
        }

        #endregion

        #region Methods

        public void Add(GameObject gameObject, bool includeChildren = true) {
            renderers.Clear();
            if(includeChildren)
                gameObject.GetComponentsInChildren(true, renderers);
            else
                gameObject.GetComponents(renderers);

            foreach(var renderer in renderers) {
                if(renderer is MeshRenderer mr) {
                    var mf = mr.GetComponent<MeshFilter>();
                    if(mf && mr) {
                        instances.Add(new MeshRendererInstance(mf.sharedMesh, mr.sharedMaterial, mr.transform.localToWorldMatrix));
                    }else
                        Debug.Log("Failed to add instance");
                }
                else if(renderer is SkinnedMeshRenderer smr) {
                    instances.Add(new MeshRendererInstance(smr.sharedMesh, smr.sharedMaterial, smr.localToWorldMatrix));
                }
            }
            bounds = null;
        }

        public void Set(GameObject gameObject, bool includeChildren = true) {
            Clear();
            Add(gameObject, includeChildren);
        }

        public void Clear() {
            instances.Clear();
            bounds = null;
        }

        #endregion

        #region Drawing

        public void Draw(Matrix4x4 matrix) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(matrix);
            }
        }

        public void Draw(Matrix4x4 matrix, int layer) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(matrix, layer);
            }
        }

        public void Draw(Matrix4x4 matrix, int layer, Camera camera) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(matrix, layer, camera);
            }
        }


        public void Draw(Material customMaterial, Matrix4x4 matrix) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(customMaterial, matrix);
            }
        }

        public void Draw(Material customMaterial, Matrix4x4 matrix, int layer) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(customMaterial, matrix, layer);
            }
        }

        public void Draw(Material customMaterial, Matrix4x4 matrix, int layer, Camera camera) {
            for(int i = 0, length = instances.Count; i < length; i++) {
                instances[i].Draw(customMaterial, matrix, layer, camera);
            }
        }

        #endregion
    }
}
