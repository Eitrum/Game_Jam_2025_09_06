using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [AddComponentMenu("Toolkit/AI/Navigation/Mesh Link")]
    public class NavMeshLink : MonoBehaviour, INevMeshLink
    {
        [SerializeField, NavArea] private int area = 0;
        [SerializeField] private bool bidirectional = true;
        [SerializeField] private Vector3 startOffset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 endOffset = new Vector3(0, 0, 1f);
        [SerializeField] private float width = 0f;
        [SerializeField] private float overrideAreaCost = -1f;

        public NavMeshLinkData Link {
            get {
                var pos = transform.position;
                var rot = transform.rotation;

                var startPos = pos + rot * startOffset;
                var endPos = pos + rot * endOffset;

                return new NavMeshLinkData() {
                    area = area,
                    bidirectional = bidirectional,
                    startPosition = startPos,
                    endPosition = endPos,
                    width = width,
                    costModifier = overrideAreaCost
                };
            }
        }

        private void OnEnable() {
            Debug.LogWarning("Toolkit.AI.Navigation.Mesh Link is not yet implemented");
        }

        private void OnDisable() {
            
        }

        void OnDrawGizmosSelected() {
            var pos = transform.position;
            var rot = transform.rotation;

            var startPos = pos + rot * startOffset;
            var endPos = pos + rot * endOffset;

            var l0 = startPos + rot * new Vector3(width / 2f, 0, 0);
            var l1 = endPos + rot * new Vector3(width / 2f, 0, 0);
            var l2 = startPos + rot * new Vector3(-width / 2f, 0, 0);
            var l3 = endPos + rot * new Vector3(-width / 2f, 0, 0);

            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawLine(l0, l1);
            Gizmos.DrawLine(l2, l3);
            Gizmos.DrawLine(l0, l2);
            Gizmos.DrawLine(l1, l3);
        }
    }

    public interface INevMeshLink
    {
        NavMeshLinkData Link { get; }
    }
}
