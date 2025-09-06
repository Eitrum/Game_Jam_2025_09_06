using System;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [AddComponentMenu("Toolkit/AI/Navigation/Mesh Source")]
    public class NavMeshSourceMesh : MonoBehaviour, INavMeshSource
    {
        [SerializeField, NavArea] private int area = 0;
        private Mesh mesh;

        public int Area {
            get => area;
            set => area = value;
        }

        public NavMeshBuildSource Source {
            get {
                if(mesh == null)
                    mesh = this.GetComponent<MeshFilter>().sharedMesh;

                return new NavMeshBuildSource() {
                    area = area,
                    component = this,
                    shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = mesh,
                    transform = transform.localToWorldMatrix
                };
            }
        }

        void OnEnable() {
            if(NavMeshBuildRules.INSTANCED) {
                GetComponentInParent<INavMeshGenerator>()?.Add(this);
            }
            else {

            }
        }

        void OnDisable() {
            if(NavMeshBuildRules.INSTANCED) {
                GetComponentInParent<INavMeshGenerator>()?.Remove(this);
            }
            else {

            }
        }
    }
}
