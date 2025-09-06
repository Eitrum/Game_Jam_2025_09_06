using System.Collections;
using System.Collections.Generic;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit.Toxel {
    public class StampMesh : MonoBehaviour, IStamp {

        #region Variables

        [SerializeField] private Mesh mesh;
        [SerializeField, Range(0, 127)] private int material;
        [SerializeField] private float falloffDistance = 1f;
        [SerializeField] private AnimationCurve sdfCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private MeshDataCache meshData;

        #endregion

        #region Properties

        public MeshDataCache MeshData {
            get {
                if(meshData == null || meshData.Mesh != mesh) {
                    meshData = new MeshDataCache(mesh);
                }
                return meshData;
            }
        }
        public int Material => material;
        public float FalloffDistance => falloffDistance;
        public AnimationCurve SDFCurve => sdfCurve;

        public Bounds Bounds {
            get {
                var rot = transform.rotation;
                Bounds bounds = MeshData.Bounds;
                var size = bounds.size;
                size.Scale(transform.localScale);
                bounds.size = size;
                bounds = bounds.Rotate(rot);
                bounds.center = MeshDataCache.MeshDataPointToWorldPoint(transform, bounds.center);
                bounds.Expand(falloffDistance);
                return bounds;
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected() {
            if(!mesh)
                return;

            var pos = transform.position;
            var rot = transform.rotation;
            Bounds bounds = MeshData.Bounds;
            var size = bounds.size;
            size.Scale(transform.localScale);
            bounds.size = size;
            bounds = bounds.Rotate(rot);
            bounds.center = MeshDataCache.MeshDataPointToWorldPoint(transform, bounds.center);

            Gizmos.DrawWireCube(bounds.center, bounds.size);

            using(new GizmosUtility.ColorScope(Color.gray))
                Gizmos.DrawWireCube(bounds.center, bounds.size + new Vector3(falloffDistance * 2, falloffDistance * 2, falloffDistance * 2));

            using(new GizmosUtility.ColorScope(Color.cyan.MultiplyAlpha(0.02f)))
                ToxelGizmos.DrawMarchingCubeBounds(bounds.center, bounds.size.Highest());

            using(new GizmosUtility.ColorScope(Color.blue))
                ToxelGizmos.DrawChunkBounds(ToxelUtility.WorldPositionToChunk(pos));

            using(new GizmosUtility.MatrixScope(transform)) {
                using(new GizmosUtility.ColorScope(new Color(0.35f, 0.35f, 0.35f, 0.35f)))
                    Gizmos.DrawMesh(mesh, 0);
                using(new GizmosUtility.ColorScope(Color.gray))
                    Gizmos.DrawWireMesh(mesh);
            }
        }

        #endregion
    }
}
