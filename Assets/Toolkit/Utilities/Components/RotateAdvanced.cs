using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Rotate (Advanced)")]
    public class RotateAdvanced : MonoBehaviour, IUpdate, IPostUpdate, IEarlyUpdate, ILateUpdate, IFixedUpdate, IOnBeforeRender
    {
        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private float degreesPerSecond = 180f;
        [SerializeField] private Space space = Space.Self;

        [Header("Update Settings")]
        [SerializeField] private UpdateModeMask updateMode = UpdateModeMask.Update;

        bool INullable.IsNull => this == null;

        void OnEnable() {
            UpdateSystem.Subscribe(this, updateMode);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(this, updateMode);
        }

        void IUpdate.Update(float dt) => Rotate(dt);
        void IPostUpdate.PostUpdate(float dt) => Rotate(dt);
        void IEarlyUpdate.EarlyUpdate(float dt) => Rotate(dt);
        void ILateUpdate.LateUpdate(float dt) => Rotate(dt);
        void IFixedUpdate.FixedUpdate(float dt) => Rotate(dt);
        void IOnBeforeRender.OnBeforeRender(float dt) => Rotate(dt);

        public void Rotate(float dt) {
            if(space == Space.Self)
                this.transform.localRotation *= Quaternion.AngleAxis(degreesPerSecond * dt, axis);
            else
                this.transform.rotation *= Quaternion.AngleAxis(degreesPerSecond * dt, axis);
        }

        void OnDrawGizmosSelected() {
            if(space == Space.Self)
                Gizmos.DrawLine(transform.position, transform.position + transform.localRotation * axis);
            else
                Gizmos.DrawLine(transform.position, transform.position + transform.rotation * axis);
        }
    }
}
