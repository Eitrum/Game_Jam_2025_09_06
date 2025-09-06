using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Follow Target (Rotation Only)")]
    public class FollowRotation : BaseComponent, IFollowTarget
    {
        #region Variables

        [SerializeField] private Transform target;
        [SerializeField] private bool x = true, y = true, z = true;
        [SerializeField] private Space space = Space.World;

        [SerializeField] private UpdateMode updateMode = UpdateMode.LateUpdate;

        [SerializeField] private SmoothingMode smoothingMode = SmoothingMode.None;
        [SerializeField, Min(0.05f)] private float smoothing = 4f;
        [SerializeField] private float maxRotationPerSecond = 90f;

        #endregion

        #region Properties

        public Transform Target {
            get => target;
            set => target = value;
        }

        public bool X {
            get => x;
            set => x = value;
        }

        public bool Y {
            get => y;
            set => y = value;
        }

        public bool Z {
            get => z;
            set => z = value;
        }

        #endregion

        #region Init

        private void OnEnable() {
            Subscribe(updateMode);
        }

        private void OnDisable() {
            Unsubscribe(updateMode);
        }

        #endregion

        #region Update

        protected override void EarlyUpdateComponent(float dt)
            => UpdateLocation(dt);

        protected override void UpdateComponent(float dt)
            => UpdateLocation(dt);

        protected override void LateUpdateComponent(float dt)
            => UpdateLocation(dt);

        protected override void PostUpdateComponent(float dt)
            => UpdateLocation(dt);

        protected override void OnBeforeRenderComponent(float dt)
            => UpdateLocation(dt);

        public void UpdateLocation(float dt) {
            if(!target)
                return;

            Vector3 tRotation = space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
            Vector3 oRotation = space == Space.Self ? target.localEulerAngles : target.eulerAngles;

            if(x)
                tRotation.x = GetRotation(tRotation.x, oRotation.x, dt);
            if(y)
                tRotation.y = GetRotation(tRotation.y, oRotation.y, dt);
            if(z)
                tRotation.z = GetRotation(tRotation.z, oRotation.z, dt);

            if(space == Space.Self) {
                transform.localEulerAngles = tRotation;
            }
            else {
                transform.eulerAngles = tRotation;
            }
        }

        #endregion

        #region Utility

        private float GetRotation(float lhs, float rhs, float dt) {
            switch(smoothingMode) {
                case SmoothingMode.Consistent: return Mathf.MoveTowardsAngle(lhs, rhs, maxRotationPerSecond * dt);
                case SmoothingMode.Distance: return Mathf.LerpAngle(lhs, rhs, smoothing * dt);
                case SmoothingMode.Mixed: return CalculateMixedMode(lhs, Mathf.MoveTowardsAngle(lhs, rhs, maxRotationPerSecond * dt), Mathf.LerpAngle(lhs, rhs, smoothing * dt));
            }
            return rhs;
        }

        private static float CalculateMixedMode(float lhs, float con, float smooth) {
            var conr = Mathf.Abs(con - lhs);
            var smoothr = Mathf.Abs(smooth - lhs);
            return conr > smoothr ? smooth : con;
        }

        #endregion

        #region Editor

        private void OnDrawGizmos() {
            if(target) {
                Gizmos.DrawLine(transform.position, target.position);
            }
        }

        #endregion

        public enum SmoothingMode
        {
            None,
            Consistent,
            Distance,
            Mixed,
        }
    }
}
