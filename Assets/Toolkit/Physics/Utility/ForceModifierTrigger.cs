using System;
using Unity.Collections;
using UnityEngine;

namespace Toolkit.PhysicEx.Utility
{
    [AddComponentMenu("Toolkit/Physics/Force Modifier (Trigger)")]
    public class ForceModifierTrigger : MonoBehaviour, IForceModifier, IForceArea
    {
        #region Variables

        [SerializeField, TypeFilter(typeof(IForceModifier))] private UnityEngine.Object forceModifierReference;
        [SerializeField] private float multiplier = 1f;
        [SerializeField] private LayerMask mask = ~0;

        private IForceModifier forceModifier;

        #endregion

        #region Properties

        public float Multiplier {
            get => multiplier;
            set => multiplier = value;
        }

        public Vector3 Center => transform.position;
        public Pose Location => transform.GetPose();

        /*public Vector3 DirectionInWorldSpace {
            get {
                if(space == Space.World)
                    return direction;
                return transform.rotation * direction;
            }
            set {
                if(space == Space.World)
                    direction = value.normalized;
                else
                    direction = transform.InverseTransformDirection(value).normalized;
            }
        }

        public Vector3 DirectionInLocalSpace {
            get {
                if(space == Space.World)
                    return transform.InverseTransformDirection(direction);
                return direction;
            }
            set {
                if(space == Space.World)
                    direction = transform.TransformDirection(value).normalized;
                else
                    direction = value.normalized;
            }
        }*/

        public LayerMask Mask {
            get => mask;
            set => mask = value;
        }

        #endregion

        #region Init

        void Awake() {
            forceModifier = forceModifierReference as IForceModifier;
        }

        #endregion

        #region Trigger 

        public void Modify(Rigidbody body, IForceArea area, float multiplier) {
            forceModifier?.Modify(body, area, multiplier);
        }

        private void OnTriggerStay(Collider other) {
            var rb = other.attachedRigidbody;
            if(rb && mask.value.HasFlag(rb.gameObject.layer)) {
                Modify(rb, this, multiplier);
            }
        }

        #endregion

        #region Editor

        private void OnValidate() {
            if(forceModifierReference == null) {
                var parent = transform.parent;
                if(parent) {
                    forceModifierReference = GetComponent<IForceModifier>() as UnityEngine.Object;
                }
            }
        }

        #endregion
    }

    public enum Shape
    {
        None,
        Box,
        Sphere,
        Capsule,
        Mesh,
    }
}
