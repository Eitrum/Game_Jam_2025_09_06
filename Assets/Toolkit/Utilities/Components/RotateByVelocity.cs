using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Rotate By Velocity")]
    public class RotateByVelocity : NullableBehaviour, IFixedUpdate
    {
        #region Variables

        [SerializeField, Min(0.01f)] private float minimumVelocity = 0.01f;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float smoothness = 0f;

        private Rigidbody body;
        private TLinkedListNode<IFixedUpdate> fixedUpdateNode;

        #endregion

        #region Properties

        public float MinimumVelocity {
            get => minimumVelocity;
            set => minimumVelocity = Mathf.Max(0.01f, value);
        }

        public float Smoothness {
            get => smoothness;
            set => smoothness = Mathf.Clamp01(value);
        }

        public Rigidbody Body => body;

        #endregion

        #region Init

        void Awake() {
            body = GetComponentInParent<Rigidbody>();
        }

        void OnEnable() {
            fixedUpdateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(fixedUpdateNode);
        }

        #endregion

        #region Update

        void IFixedUpdate.FixedUpdate(float dt) {
            var vel = body.linearVelocity;
            if(vel.magnitude > minimumVelocity) {
                body.MoveRotation(
                    Quaternion.RotateTowards(
                        body.rotation,
                        Quaternion.LookRotation(vel),
                        Time.fixedDeltaTime * 360f * (1f - smoothness)
                    ));
            }
        }

        #endregion
    }
}
