using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [CreateAssetMenu(fileName = "Terrain Tree Collection", menuName = "Toolkit/Terrain/Terrain Tree Collection")]
    public class TerrainTreeCollection : ScriptableObject
    {
        [SerializeField] private TreePrefab[] trees = null;

        public int TreeCount => trees.Length;
        public TreePrefab GetTree(int index) => trees[index];
        public TreePrefab[] GetTrees() => trees;
        public TreePrototype[] GetTreePrototypes() {
            TreePrototype[] prototypes = new TreePrototype[TreeCount];
            for(int i = 0, length = TreeCount; i < length; i++) {
                prototypes[i] = new TreePrototype() {
                    prefab = trees[i].Prefab,
                    bendFactor = trees[i].BendFactor,
                };
            }
            return prototypes;
        }

        [System.Serializable]
        public class TreePrefab
        {
            [SerializeField] private GameObject prefab = null;
            [SerializeField] private float bend = 0f;

            public GameObject Prefab => prefab;
            public float BendFactor => bend;
        }
    }
}
