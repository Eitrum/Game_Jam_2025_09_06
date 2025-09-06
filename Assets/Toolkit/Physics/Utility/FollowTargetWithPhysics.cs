using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Follow Target (Physics)")]
    public class FollowTargetWithPhysics : NullableBehaviour, IFixedUpdate
    {
        #region Variables

        [SerializeField] private Transform target;
        [SerializeField] private FollowTargetWithPhysicsSettings settings = null;

        private Rigidbody bodySelf;
        private IVelocityTracking targetVelocityTracking;
        private float perObjectMultiplier = 1f;
        private TLinkedListNode<IFixedUpdate> updateNode = null;
        private float shakiness = 0f;

        #endregion

        #region Properties

        public FollowTargetWithPhysicsSettings Settings {
            get => settings;
            set {
                settings = value;
                if(settings == null)
                    settings = FollowTargetWithPhysicsSettings.Default;
            }
        }
        private float MaxVelocity => settings.MaxVelocity;
        private float MaxTorque => settings.MaxTorque;
        private float Sensitivity => settings.Sensitivity;
        private float Prediction => settings.Prediction;

        public Rigidbody Body => bodySelf;
        public Transform Target {
            get => target;
            set {
                target = value;
                if(target != null) {
                    targetVelocityTracking = target.GetComponent<IVelocityTracking>();
                    if(updateNode == null && enabled)
                        updateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
                }
                else {
                    targetVelocityTracking = null;
                    Unsubscribe();
                }
            }
        }

        public bool HasPrediction => targetVelocityTracking != null && Prediction > Mathf.Epsilon;
        public IVelocityTracking PredictionTarget => targetVelocityTracking;

        public float Multiplier {
            get => perObjectMultiplier;
            set => perObjectMultiplier = Mathf.Clamp01(value);
        }

        #endregion

        #region Init

        void Awake() {
            bodySelf = GetComponent<Rigidbody>();
            bodySelf.maxAngularVelocity = 1000f;
        }

        void OnEnable() {
            if(target) {
                targetVelocityTracking = target.GetComponent<IVelocityTracking>();
                updateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
            }
            if(settings == null)
                settings = FollowTargetWithPhysicsSettings.Default;
        }

        void OnDisable() {
            Unsubscribe();
        }

        private void Unsubscribe() {
            if(updateNode != null) {
                UpdateSystem.Unsubscribe(updateNode);
                updateNode = null;
            }
        }

        #endregion

        #region Update

        void IFixedUpdate.FixedUpdate(float dt) {
            if(!target) {
                Unsubscribe();
                return;
            }

            var sensitivity = (1f / dt) * (Sensitivity * perObjectMultiplier);
            Vector3 destPos = target.position;
            Quaternion destRot = target.rotation;

            // Handle prediction
            if(targetVelocityTracking != null && Prediction > Mathf.Epsilon) {
                var predDt = Prediction * dt;
                var predVel = targetVelocityTracking.Velocity;
                // reduce shakiness for VR (10cm prediction required)

                var v = predVel.magnitude;
                shakiness = Mathf.Clamp01(v / settings.ShakinessSmoothing);
                if(v > 0.001f) {
                    predDt *= shakiness;

                    var predictionAngular = Quaternion.Euler(targetVelocityTracking.AngularVelocity * (predDt) * Mathf.Rad2Deg);
                    var predictionForward = predictionAngular * (predVel * (predDt));

                    destPos += predictionForward;
                    destRot = destRot * predictionAngular;
                }
            }

            var angVel = (destRot.GetDelta(bodySelf.rotation).ToAngularVelocity()) * (sensitivity);
            var velocity = (destPos - bodySelf.position) * sensitivity;

            bodySelf.linearVelocity = velocity.ClampMagnitude(MaxVelocity);
            bodySelf.angularVelocity = angVel.ClampMagnitude(MaxTorque);
        }

        #endregion
    }
}
