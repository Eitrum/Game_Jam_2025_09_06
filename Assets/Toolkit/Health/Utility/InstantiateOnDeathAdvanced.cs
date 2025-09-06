using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health.Utility
{
    [AddComponentMenu("Toolkit/Health/Utility/Instantiate On Death (Advanced)")]
    public class InstantiateOnDeathAdvanced : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject prefab;

        [SerializeField] private Space rotation = Space.Self;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;

        private IHealth health;

        #endregion

        #region Properties

        public Pose SpawnLocation {
            get {
                if(!transform)
                    return Pose.identity;
                switch(rotation) {
                    case Space.Self:
                        var rot = transform.rotation;
                        return new Pose(transform.position + rot * positionOffset, rot * Quaternion.Euler(rotationOffset));
                    case Space.World:
                        return new Pose(transform.position + positionOffset, Quaternion.Euler(rotationOffset));
                }
                return Pose.identity;
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

        #region Editor

        private void OnDrawGizmosSelected() {
            var p = transform.position;
            var sp = SpawnLocation;

            Gizmos.DrawLine(p, sp.position);
            Gizmos.DrawWireSphere(sp.position, 0.2f);
        }

        #endregion
    }
}
