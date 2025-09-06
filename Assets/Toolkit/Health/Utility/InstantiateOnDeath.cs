using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health.Utility
{
    [AddComponentMenu("Toolkit/Health/Utility/Instantiate On Death (Simple)")]
    public class InstantiateOnDeath : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject prefab;
        [SerializeField] private bool copyRotation = true;
        private IHealth health;

        #endregion

        #region Properties

        public Pose SpawnLocation {
            get {
                if(!transform)
                    return Pose.identity;
                return copyRotation ? new Pose(transform.position, transform.rotation) : new Pose(transform.position, Quaternion.identity);
            }
        }

        #endregion

        #region Init

        private void Awake() {
            health = GetComponentInParent<IHealth>();
        }

        private void OnEnable() {
            if(health != null)
                health.OnDeath += OnDeath;
        }

        private void OnDisable() {
            if(health != null)
                health.OnDeath -= OnDeath;
        }

        #endregion

        #region Death Callback

        [ContextMenu("OnDeath Callback")]
        public void ForceCallback()
            => OnDeath();

        private void OnDeath() {
            var p = SpawnLocation;
            Instantiate(prefab, p.position, p.rotation);
        }

        #endregion
    }
}
