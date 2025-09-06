using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Toxel {
    [CreateAssetMenu(menuName = "Toolkit/Toxel/New World")]
    public class ToxelWorldData : ScriptableObject {
        [SerializeField] private string worldName;
        [SerializeField] private Material terrainMaterial = null;

        public string WorldName => worldName;
        public Material TerrainMaterial => terrainMaterial;
    }
}
