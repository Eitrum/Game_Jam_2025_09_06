using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    [CreateAssetMenu(fileName = "Nav Mesh Query Filter", menuName = "Toolkit/AI/Navigation/Query Filter")]
    public class NavMeshQueryFilterObject : ScriptableObject, INavMeshQueryFilter
    {
        [SerializeField] private int agentTypeId = 0;
        [SerializeField] private int areaMask = -1;
        [SerializeField] private float[] areaCostMultiplier = null;

        [System.NonSerialized] private bool isInitialized = false;

        private NavMeshQueryFilter queryFilter;
        public NavMeshQueryFilter QueryFilter {
            get {
                if(!isInitialized) {
                    Awake();
                }
                return queryFilter;
            }
        }

        void Awake() {
            if(isInitialized)
                return;
            isInitialized = true;
            queryFilter = new NavMeshQueryFilter() {
                agentTypeID = agentTypeId,
                areaMask = areaMask
            };

            if(areaCostMultiplier == null || areaCostMultiplier.Length != 32) {
                areaCostMultiplier = new float[32];
                for(int i = 0; i < 32; i++) {
                    areaCostMultiplier[i] = 1f;
                }
            }
            else {
                for(int i = 0; i < 32; i++) {
                    if(areaCostMultiplier[i] != 1f)
                        queryFilter.SetAreaCost(i, areaCostMultiplier[i]);
                }
            }
        }

        void OnValidate() {
            if(areaCostMultiplier == null || areaCostMultiplier.Length != 32) {
                areaCostMultiplier = new float[32];
                for(int i = 0; i < 32; i++) {
                    areaCostMultiplier[i] = 1f;
                }
            }
        }
    }
}
