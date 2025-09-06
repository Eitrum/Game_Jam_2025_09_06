using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public class PrefabData
    {
        #region Variables

        private GameObject prefab;
        private List<Instance> instances = new List<Instance>();

        #endregion

        #region Properties

        public GameObject Prefab => prefab;
        public IReadOnlyList<Instance> Instances => instances;

        #endregion

        #region Constructor

        public PrefabData(GameObject prefab) {
            this.prefab = prefab;
        }

        #endregion

        #region Add

        public void Add(Vector3 point, Quaternion rot, Vector3 scale) {
            instances.Add(new Instance() {
                point = point,
                rotation = rot,
                scale = scale,
            });
        }

        #endregion

        #region Remove

        public void Remove(int instanceIndex) {
            instances.RemoveAt(instanceIndex);
        }

        #endregion

        public struct Instance
        {
            public Vector3 point;
            public Quaternion rotation;
            public Vector3 scale;

            public Color32 color;
        }
    }
}
