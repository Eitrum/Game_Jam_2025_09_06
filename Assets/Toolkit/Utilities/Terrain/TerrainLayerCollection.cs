using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [CreateAssetMenu(fileName = "Terrain Layer Collection", menuName = "Toolkit/Terrain/Terrain Layer Collection")]
    public class TerrainLayerCollection : ScriptableObject
    {
        [SerializeField] private TerrainLayer[] layers = null;

        public int LayerCount => layers.Length;
        public TerrainLayer GetLayer(int index) => layers[index];
        public TerrainLayer[] GetLayers() => layers;
    }
}
