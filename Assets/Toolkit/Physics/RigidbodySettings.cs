using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [System.Serializable]
    public class RigidbodySettings : IRigidbodySettings
    {
        #region Variables

        [SerializeField, Min(0.0000001f)] private float mass = 1f;

        [SerializeField, Min(0f)] private float drag = 0f;
        [SerializeField, Min(0f)] private float angularDrag = 0.05f;

        [SerializeField] private bool useGravity = true;
        [SerializeField] private bool isKinematic = false;
        [SerializeField] private bool detectCollisions = true;

        [SerializeField] private RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
        [SerializeField] private CollisionDetectionMode collisionDetection = CollisionDetectionMode.Discrete;
        [SerializeField] private RigidbodyConstraints constraints = RigidbodyConstraints.None;

        #endregion

        #region Properties

        public float Mass { get => mass; set => mass = value; }
        public float Drag { get => drag; set => drag = value; }
        public float AngularDrag { get => angularDrag; set => angularDrag = value; }

        public bool UseGravity { get => useGravity; set => useGravity = value; }
        public bool IsKinematic { get => isKinematic; set => isKinematic = value; }
        public bool DetectCollisions { get => detectCollisions; set => detectCollisions = value; }

        public RigidbodyInterpolation Interpolation { get => interpolation; set => interpolation = value; }
        public CollisionDetectionMode CollisionDetection { get => collisionDetection; set => collisionDetection = value; }
        public RigidbodyConstraints Constraints { get => constraints; set => constraints = value; }

        #endregion

        #region Methods

        public void Copy(Rigidbody body) {
            mass = body.mass;
            drag = body.linearDamping;
            angularDrag = body.angularDamping;
            useGravity = body.useGravity;
            isKinematic = body.isKinematic;
            interpolation = body.interpolation;
            collisionDetection = body.collisionDetectionMode;
            constraints = body.constraints;
            detectCollisions = body.detectCollisions;
        }

        public void ApplyTo(Rigidbody body) {
            body.mass = mass;
            body.linearDamping = drag;
            body.angularDamping = angularDrag;
            body.useGravity = useGravity;
            body.isKinematic = isKinematic;
            body.interpolation = interpolation;
            body.collisionDetectionMode = collisionDetection;
            body.constraints = constraints;
            body.detectCollisions = detectCollisions;
        }

        #endregion
    }
}
