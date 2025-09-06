using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Velocity Tracking/Velocity Tracking (Smoothing)")]
    public class VelocityTrackingSmoothing : MonoBehaviour, IFixedUpdate, IVelocityTracking
    {
        #region Variables

        [SerializeField, Range(0f, 1f), Readonly(true)] private float smoothing = 0.1f;
        private TLinkedListNode<IFixedUpdate> updateNode;

        private CircularBuffer<Vector3> velocityBuffer;
        private CircularBuffer<Vector3> angularVelocityBuffer;

        private bool isCached;
        private Vector3 velocity;
        private Vector3 angularVelocity;

        private Vector3 oldPosition;
        private Quaternion oldRotation;

        #endregion

        #region Properties

        public bool IsNull => this == null;

        public float Smoothing {
            get => smoothing;
            set {
                if(smoothing.Equals(value, Mathf.Epsilon)) {
                    this.smoothing = Mathf.Clamp01(value);
                    var frames = Mathf.Clamp((int)(smoothing / Time.fixedDeltaTime), 1, 500);
                    velocityBuffer?.Resize(frames);
                    angularVelocityBuffer?.Resize(frames);
                }
            }
        }

        public int TrackedFrames {
            get {
                var frames = Mathf.Clamp((int)(smoothing / Time.fixedDeltaTime), 1, 500);
                return frames;
            }
        }

        public Vector3 Velocity {
            get {
                Cache();
                return velocity;
            }
        }
        public Vector3 AngularVelocity {
            get {
                Cache();
                return angularVelocity;
            }
        }

        #endregion

        #region Init

        void Awake() {
            var frames = Mathf.Clamp((int)(smoothing / Time.fixedDeltaTime), 1, 500);
            velocityBuffer = new CircularBuffer<Vector3>(frames);
            angularVelocityBuffer = new CircularBuffer<Vector3>(frames);
        }

        void OnEnable() {
            var t = transform;
            oldPosition = t.position;
            oldRotation = t.rotation;
            updateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(updateNode);
        }

        #endregion

        #region Update

        void IFixedUpdate.FixedUpdate(float dt) {
            // Get new values
            var t = transform;
            var pos = t.position;
            var rot = t.rotation;

            // Get Velocity
            velocityBuffer.Write((pos - oldPosition) / dt);
            angularVelocityBuffer.Write(rot.GetDelta(oldRotation).ToAngularVelocity() / dt);

            // Store values for next frame
            oldPosition = pos;
            oldRotation = rot;

            isCached = false;
        }

        private void Cache() {
            if(isCached)
                return;
            isCached = true;

            velocity = velocityBuffer?.Average() ?? default;
            angularVelocity = angularVelocityBuffer?.Average() ?? default;
        }

        #endregion

        #region IVelMultiTracking Impl

        /// <summary>
        /// Simple Velocity Tracking do not have any 3d point for tracking
        /// </summary>
        public Vector3 GetVelocityAt(Vector3 point) {
            Cache();
            return VelocityTrackingUtility.GetVelocityRelativeToPoint(transform.position, velocity, angularVelocity, point);
        }

        public Vector3 GetAngularVelocityAt(Vector3 point) {
            Cache();
            return angularVelocity;
        }

        #endregion
    }
}
