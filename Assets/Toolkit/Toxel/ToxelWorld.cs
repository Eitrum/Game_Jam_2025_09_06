using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Toxel {
    public class ToxelWorld : MonoBehaviour {
        #region Variables

        [SerializeField, CreateNew] private ToxelWorldData data;

        private Dictionary<Vector3Int, ToxelChunk> chunkLookup = new Dictionary<Vector3Int, ToxelChunk>();

        #endregion

        #region Properties

        public Material TerrainMaterial => data.TerrainMaterial;

        #endregion

        #region Add

        public void Add(Vector3Int chunkId) {

        }

        #endregion

        #region Request Chunks



        #endregion
    }
}
