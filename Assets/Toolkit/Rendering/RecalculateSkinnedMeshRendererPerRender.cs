using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [ExecuteAlways]
    public class RecalculateSkinnedMeshRendererPerRender : MonoBehaviour
    {
        private SkinnedMeshRenderer smr;

        private void Awake() {
            smr = GetComponent<SkinnedMeshRenderer>();
        }

        private void OnEnable() {
            smr.forceMatrixRecalculationPerRender = true;
        }

        private void OnDisable() {
            smr.forceMatrixRecalculationPerRender = false;
        }
    }
}
