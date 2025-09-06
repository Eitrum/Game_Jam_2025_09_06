using System;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public class Gravity : MonoBehaviour, IFixedUpdate, IGravity
    {
        #region Variables

        [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0);
        private Rigidbody body;
        private TLinkedListNode<IFixedUpdate> updateNode;

        #endregion

        #region Properties

        public Vector3 Value {
            get => gravity;
            set => gravity = value;
        }

        public Rigidbody Body => body;

        bool INullable.IsNull => this == null;

        #endregion

        #region Init

        void Awake() {
            body = GetComponent<Rigidbody>();
        }

        void OnEnable() {
            updateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(updateNode);
        }

        #endregion

        #region Update

        void IFixedUpdate.FixedUpdate(float dt) {
            body.AddForce(gravity, ForceMode.Acceleration);
        }

        #endregion
    }

    public interface IGravity
    {
        Vector3 Value { get; set; }
        Rigidbody Body { get; }
    }
}
