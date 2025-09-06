using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility {
    public class GizmosDrawMeshFilterBounds : MonoBehaviour {
        [SerializeField] private Color color = Color.gray;

        private void OnDrawGizmosSelected() {
            if(!enabled) return;
            var mf = GetComponent<MeshFilter>();
            if(!mf || !mf.sharedMesh)
                return;
            using(new GizmosUtility.MatrixScope(transform)) {
                using(new GizmosUtility.ColorScope(color))
                    Gizmos.DrawWireCube(mf.sharedMesh.bounds.center, mf.sharedMesh.bounds.size);
            }
        }
    }
}
