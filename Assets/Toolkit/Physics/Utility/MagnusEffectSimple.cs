using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Utility/Magnus Effect (Simple)")]
    public class MagnusEffectSimple : NullableBehaviour, IFixedUpdate
    {
        #region Variables

        internal const float AIR_DENSITY = 0.0107f; // Calculated with = Mathf.Pow(1.225 * 1000f, 1f / 3f) * 0.001f;

        [SerializeField] private float radius = 0.5f;
        [SerializeField] private bool ignoreBodyMass = false;
        [SerializeField] private float strength = 1f;

        private Rigidbody body = null;
        private TLinkedListNode<IFixedUpdate> fixedNode = null;
        private Vector3 magnusEffect;

        #endregion

        #region Properties

        public float Radius {
            get => radius;
            set => radius = Mathf.Max(0.001f, value);
        }

        public bool IgnoreBodyMass {
            get => ignoreBodyMass;
            set => ignoreBodyMass = value;
        }

        public float Strength {
            get => strength;
            set => strength = value;
        }

        public Vector3 MagnusEffectForce => magnusEffect;

        #endregion

        #region Init

        void Awake() {
            body = GetComponent<Rigidbody>();
        }

        private void OnEnable() {
            fixedNode = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        private void OnDisable() {
            UpdateSystem.Unsubscribe(fixedNode);
        }

        #endregion

        #region Update

        void IFixedUpdate.FixedUpdate(float dt) {
            var direction = Vector3.Cross(body.angularVelocity, body.linearVelocity);
            var magnitude = 1.33333333333333f * Mathf.PI * AIR_DENSITY * radius * radius * radius * strength;
            magnusEffect = direction * magnitude;
            body.AddForce(magnusEffect, ignoreBodyMass ? ForceMode.Acceleration : ForceMode.Force);
        }

        #endregion

        #region Editor

        void OnDrawGizmosSelected() {
            var pos = transform.position;
            Gizmos.DrawWireSphere(pos, radius);
            using(new GizmosUtility.ColorScope(Color.cyan))
                Gizmos.DrawLine(pos, pos + magnusEffect);
        }

        #endregion
    }
}
