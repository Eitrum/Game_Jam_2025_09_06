using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [AddComponentMenu("Toolkit/AI/Navigation/Terrain Source")]
    public class NavMeshSourceTerrain : MonoBehaviour, INavMeshSource
    {
        [SerializeField, NavArea] private int area = 0;
        private Terrain terrain;

        public int Area {
            get => area;
            set => area = value;
        }

        public NavMeshBuildSource Source {
            get {
                if(terrain == null)
                    terrain = this.GetComponent<Terrain>();

                return new NavMeshBuildSource() {
                    area = area,
                    component = this,
                    shape = NavMeshBuildSourceShape.Terrain,
                    sourceObject = terrain.terrainData,
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
