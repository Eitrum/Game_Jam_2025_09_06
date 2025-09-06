using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [AddComponentMenu("Toolkit/AI/Navigation/Modifier")]
    public class NavMeshSourceModifier : MonoBehaviour, INavMeshSource
    {
        [SerializeField, NavArea] private int area = 0;
        [SerializeField] private Vector3 size = new Vector3(1, 1, 1);

        public int Area {
            get => area;
            set => area = value;
        }

        public NavMeshBuildSource Source {
            get {
                return new NavMeshBuildSource() {
                    area = area,
                    component = this,
                    shape = NavMeshBuildSourceShape.ModifierBox,
                    // Scale the size with the transform scale to make it easier to build.
                    size = size.Multiply(transform.lossyScale),
                    // Set transform scale to 1 to ensure box actually is the correct size.
                    transform = transform.localToWorldMatrix.SetScale(1f)
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

        void OnDrawGizmosSelected() {
            GizmosUtility.DrawWireCube(transform.position, transform.rotation, transform.lossyScale.Multiply(size));
        }
    }
}
