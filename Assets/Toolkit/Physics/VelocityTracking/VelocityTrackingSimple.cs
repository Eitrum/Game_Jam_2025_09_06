using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [AddComponentMenu("Toolkit/Physics/Velocity Tracking/Velocity Tracking (Simple)")]
    public class VelocityTrackingSimple : MonoBehaviour, IFixedUpdate, IVelocityTracking
    {
        #region Variables

        private TLinkedListNode<IFixedUpdate> updateNode;

        private Vector3 velocity;
        private Vector3 angularVelocity;

        private Vector3 oldPosition;
        private Quaternion oldRotation;

        #endregion

        #region Properties

        public bool IsNull => this == null;

        public Vector3 Velocity => velocity;
        public Vector3 AngularVelocity => angularVelocity;

        #endregion

        #region Init

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
            velocity = (pos - oldPosition) / dt;
            angularVelocity = rot.GetDelta(oldRotation).ToAngularVelocity() / dt;

            // Store values for next frame
            oldPosition = pos;
            oldRotation = rot;
        }

        #endregion

        #region IVelMultiTracking Impl

        /// <summary>
        /// Simple Velocity Tracking do not have any 3d point for tracking
        /// </summary>
        public Vector3 GetVelocityAt(Vector3 point)
            => VelocityTrackingUtility.GetVelocityRelativeToPoint(transform.position, velocity, angularVelocity, point);

        public Vector3 GetAngularVelocityAt(Vector3 point) => angularVelocity;

        #endregion
    }
}
