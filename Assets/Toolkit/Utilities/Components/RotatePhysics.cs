using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Rotate (Physics)")]
    public class RotatePhysics : MonoBehaviour, IFixedUpdate
    {
        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private float degreesPerSecond = 180f;

        private TLinkedListNode<IFixedUpdate> node = null;
        private Rigidbody body = null;

        bool INullable.IsNull => this == null;

        void Awake() {
            body = GetComponent<Rigidbody>();
        }

        void OnEnable() {
            node = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(node);
        }

        void IFixedUpdate.FixedUpdate(float dt) => Rotate(dt);

        public void Rotate(float dt) {

            body.rotation *= Quaternion.AngleAxis(degreesPerSecond * dt, axis);
        }

        void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * axis);
        }
    }
}
