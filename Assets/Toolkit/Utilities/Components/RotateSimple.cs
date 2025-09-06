using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Rotate (Simple)")]
    public class RotateSimple : MonoBehaviour, IUpdate
    {
        [SerializeField, Direction(relativeToObject: true)] private Vector3 axis = Vector3.up;
        [SerializeField] private float degreesPerSecond = 180f;

        private TLinkedListNode<IUpdate> node;
        bool INullable.IsNull => this == null;

        #region Properties

        public Vector3 Axis {
            get => axis;
            set => axis = value.normalized;
        }

        public Vector3 AngularVelocityInDegrees {
            get => axis * degreesPerSecond;
            set {
                degreesPerSecond = value.magnitude;
                if(Mathf.Abs(degreesPerSecond) > Mathf.Epsilon) {
                    axis = value.normalized;
                }
            }
        }

        public Vector3 AngularVelocity {
            get => axis * (degreesPerSecond * Mathf.Deg2Rad);
            set {
                degreesPerSecond = (value.magnitude * Mathf.Deg2Rad);
                if(Mathf.Abs(degreesPerSecond) > Mathf.Epsilon) {
                    axis = value.normalized;
                }
            }
        }

        public float DegreesPerSecond {
            get => degreesPerSecond;
            set => degreesPerSecond = value;
        }

        public float RadiansPerSecond {
            get => degreesPerSecond * Mathf.Deg2Rad;
            set => degreesPerSecond = value * Mathf.Rad2Deg;
        }

        #endregion

        #region Init

        void OnEnable() {
            node = UpdateSystem.Subscribe(this as IUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(node);
        }

        #endregion

        #region Update

        void IUpdate.Update(float dt) => Rotate(dt);

        public void Rotate(float dt) {
            this.transform.localRotation *= Quaternion.AngleAxis(degreesPerSecond * dt, axis);
        }

        #endregion

        #region Utility

        [ContextMenu("Randomize Axis")]
        public void RandomizeAxis() {
            axis = Toolkit.Mathematics.Random.OnUnitSphere;
        }

        [ContextMenu("Invert/Speed")]
        public void Invert() {
            degreesPerSecond = -degreesPerSecond;
        }

        [ContextMenu("Invert/Axis")]
        public void InvertAxis() {
            axis = -axis;
        }

        #endregion

        #region Editor

        void OnDrawGizmosSelected() {
            if(transform.parent) {
                using(new GizmosUtility.MatrixScope(transform.parent)) {
                    var pos = transform.localPosition;
                    Gizmos.DrawLine(pos, pos + transform.localRotation * axis);
                    GizmosUtility.DrawCircle(pos, Quaternion.FromToRotation(Vector3.up, axis), 0.5f);
                }
            }
            else {
                var pos = transform.position;
                Gizmos.DrawLine(pos, pos + axis);
                GizmosUtility.DrawCircle(pos, Quaternion.FromToRotation(Vector3.up, axis), 0.5f); ;
            }
        }

        #endregion
    }
}
