using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public class TreeData
    {
        #region Variables

        private Utility.TerrainTreeCollection.TreePrefab treePrefab;
        private List<Instance> instances = new List<Instance>();

        #endregion

        #region Properties

        public Utility.TerrainTreeCollection.TreePrefab Collection_Prefab => treePrefab;
        public GameObject TreePrefab => treePrefab.Prefab;
        public float BendFactor => treePrefab.BendFactor;
        public TreePrototype TreePrototype => new TreePrototype() { prefab = TreePrefab, bendFactor = BendFactor };
        public IReadOnlyList<Instance> Instances => instances;

        #endregion

        #region Constructor

        public TreeData(Utility.TerrainTreeCollection.TreePrefab treePrefab) {
            this.treePrefab = treePrefab;
        }

        #endregion

        #region Add

        public void AddTree(Instance instance) {
            instances.Add(instance);
        }

        public void AddTree(Vector3 position, float rotation, float size) {
            instances.Add(new Instance(position, rotation, size));
        }

        #endregion

        #region Remove

        public void RemoveTree(int instanceIndex) {
            instances.RemoveAt(instanceIndex);
        }

        #endregion

        public struct Instance
        {
            #region Variables

            public Vector3 point;
            public Vector3 normal;
            public float rotation;
            public float height;
            public float width;
            public Color32 instanceColor;

            #endregion

            #region Conversion

            public TreeInstance ToTreeInstance(int prototypeIndex) =>
                new TreeInstance() {
                    position = point,
                    rotation = rotation,
                    widthScale = width,
                    heightScale = height,
                    prototypeIndex = prototypeIndex,
                    color = instanceColor,
                };

            public TreeInstance ToTreeInstance(int prototypeIndex, float terrainHeight) =>
                 new TreeInstance() {
                     position = point.Multiply(1f, 1f / terrainHeight, 1f),
                     rotation = rotation,
                     widthScale = width,
                     heightScale = height,
                     prototypeIndex = prototypeIndex,
                     color = instanceColor,
                 };

            public TreeInstance ToTreeInstance(int prototypeIndex, Vector3 scale) =>
                 new TreeInstance() {
                     position = new Vector3(point.x / scale.x, point.y / scale.y, point.z / scale.x),
                     rotation = rotation,
                     widthScale = width,
                     heightScale = height,
                     prototypeIndex = prototypeIndex,
                     color = instanceColor,
                 };

            #endregion

            #region Constructor

            public Instance(Vector3 position, float rotation, float size) {
                this.point = position;
                this.normal = Vector3.up;
                this.rotation = rotation;
                this.width = size;
                this.height = size;
                this.instanceColor = new Color32(255, 255, 255, 255);
            }

            public Instance(Vector3 position, MinMax sizeRange) {
                this.point = position;
                this.normal = Vector3.up;
                this.rotation = UnityEngine.Random.value * Mathf.PI * 2f;
                var size = sizeRange.Random;
                this.width = size;
                this.height = size;
                this.instanceColor = new Color32(255, 255, 255, 255);
            }

            public Instance(Vector3 position, MinMax sizeRange, System.Random random) {
                this.point = position;
                this.normal = Vector3.up;
                this.rotation = random.NextFloat() * Mathf.PI * 2f;
                var size = sizeRange.Evaluate(random.NextFloat());
                this.width = size;
                this.height = size;
                this.instanceColor = new Color32(255, 255, 255, 255);
            }

            public Instance(Vector3 position, Vector3 normal, float rotation, float height, float width, Color32 color) {
                this.point = position;
                this.normal = normal;
                this.rotation = rotation;
                this.height = height;
                this.width = width;
                this.instanceColor = color;
            }

            #endregion
        }
    }
}
