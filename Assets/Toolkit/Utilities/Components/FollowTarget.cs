using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Follow Target")]
    public class FollowTarget : BaseComponent, IFollowTarget
    {
        #region Variables

        [SerializeField] private Transform target;

        [SerializeField] private UpdateMode updateMode = UpdateMode.LateUpdate;

        // Location
        [SerializeField] private Space space = Space.World;
        [SerializeField] private TransformMask mask = TransformMask.PositionRotation;

        // Smoothing
        [SerializeField] private SmoothingMode smoothingMode = SmoothingMode.None;
        [SerializeField, Min(0.05f)] private float smoothing = 4f;
        [SerializeField] private float maxDistancePerSecond = 8f;
        [SerializeField] private float maxRotationPerSecond = 90f;
        [SerializeField] private float maxScalePerSecond = 1f;

        #endregion

        #region Properties

        public Transform Target {
            get => target;
            set => target = value;
        }

        public UpdateMode UpdateMode {
            get => updateMode;
            set {
                if(updateMode != value) {
                    Unsubscribe(updateMode);
                    updateMode = value;
                    if(updateMode != UpdateMode.None)
                        Subscribe(updateMode);
                }
            }
        }

        public Space Space {
            get => space;
            set => space = value;
        }

        public TransformMask Mask {
            get => mask;
            set => mask = value;
        }

        public SmoothingMode Mode {
            get => smoothingMode;
            set => smoothingMode = value;
        }

        public float Smoothing {
            get => smoothing;
            set => smoothing = Mathf.Max(0.05f, value);
        }

        public float MaxDistance {
            get => maxDistancePerSecond;
            set => maxDistancePerSecond = Mathf.Max(0f, value);
        }

        public float MaxRotation {
            get => maxRotationPerSecond;
            set => maxRotationPerSecond = Mathf.Max(0f, value);
        }

        public float MaxScale {
            get => maxScalePerSecond;
            set => maxScalePerSecond = Mathf.Max(0f, value);
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

            Vector3 tPosition = space == Space.Self ? transform.localPosition : transform.position;
            Vector3 oPosition = space == Space.Self ? target.localPosition : target.position;
            Vector3 tRotation = space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
            Vector3 oRotation = space == Space.Self ? target.localEulerAngles : target.eulerAngles;
            Vector3 tScale = space == Space.Self ? transform.localScale : transform.lossyScale;
            Vector3 oScale = space == Space.Self ? target.localScale : target.lossyScale;

            if(mask.HasFlag(TransformMask.PositionX))
                tPosition.x = GetPosition(tPosition.x, oPosition.x, dt);
            if(mask.HasFlag(TransformMask.PositionY))
                tPosition.y = GetPosition(tPosition.y, oPosition.y, dt);
            if(mask.HasFlag(TransformMask.PositionZ))
                tPosition.z = GetPosition(tPosition.z, oPosition.z, dt);

            if(mask.HasFlag(TransformMask.RotationX))
                tRotation.x = GetRotation(tRotation.x, oRotation.x, dt);
            if(mask.HasFlag(TransformMask.RotationY))
                tRotation.y = GetRotation(tRotation.y, oRotation.y, dt);
            if(mask.HasFlag(TransformMask.RotationZ))
                tRotation.z = GetRotation(tRotation.z, oRotation.z, dt);

            if(mask.HasFlag(TransformMask.ScaleX))
                tScale.x = GetScale(tScale.x, oScale.x, dt);
            if(mask.HasFlag(TransformMask.ScaleY))
                tScale.y = GetScale(tScale.y, oScale.y, dt);
            if(mask.HasFlag(TransformMask.ScaleZ))
                tScale.z = GetScale(tScale.z, oScale.z, dt);


            if(space == Space.Self) {
                transform.localPosition = tPosition;
                transform.localEulerAngles = tRotation;
                transform.localScale = tScale;
            }
            else {
                transform.position = tPosition;
                transform.eulerAngles = tRotation;
                transform.SetLossyScale(tScale);
            }
        }

        private float GetPosition(float lhs, float rhs, float dt) {
            switch(smoothingMode) {
                case SmoothingMode.Consistent: return Mathf.MoveTowards(lhs, rhs, maxDistancePerSecond * dt);
                case SmoothingMode.Distance: return Mathf.Lerp(lhs, rhs, smoothing * dt);
                case SmoothingMode.Mixed: return CalculateMixedMode(lhs, Mathf.MoveTowards(lhs, rhs, maxDistancePerSecond * dt), Mathf.Lerp(lhs, rhs, smoothing * dt));
            }
            return rhs;
        }

        private float GetRotation(float lhs, float rhs, float dt) {
            switch(smoothingMode) {
                case SmoothingMode.Consistent: return Mathf.MoveTowardsAngle(lhs, rhs, maxRotationPerSecond * dt);
                case SmoothingMode.Distance: return Mathf.LerpAngle(lhs, rhs, smoothing * dt);
                case SmoothingMode.Mixed: return CalculateMixedMode(lhs, Mathf.MoveTowardsAngle(lhs, rhs, maxRotationPerSecond * dt), Mathf.LerpAngle(lhs, rhs, smoothing * dt));
            }
            return rhs;
        }

        private float GetScale(float lhs, float rhs, float dt) {
            switch(smoothingMode) {
                case SmoothingMode.Consistent: return Mathf.MoveTowards(lhs, rhs, maxScalePerSecond * dt);
                case SmoothingMode.Distance: return Mathf.Lerp(lhs, rhs, smoothing * dt);
                case SmoothingMode.Mixed: return CalculateMixedMode(lhs, Mathf.MoveTowards(lhs, rhs, maxScalePerSecond * dt), Mathf.Lerp(lhs, rhs, smoothing * dt));
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

    public interface IFollowTarget
    {
        Transform Target { get; set; }
    }
}
