using UnityEngine;

namespace Toolkit.PhysicEx
{
    [System.Serializable]
    public class PIDController
    {
        #region Variables

        [SerializeField] private float proportionalGain = 1f;
        [SerializeField] private float integralGain = 1f;
        [SerializeField] private float derivativeGain = 1f;

        [SerializeField] private float saturation = 1f;

        private float integrationStored = 0f;
        private float lastE = 0f;

        #endregion

        #region Properties

        public float Proportional {
            get => proportionalGain;
            set => proportionalGain = value;
        }

        public float Integral {
            get => integralGain;
            set => integralGain = value;
        }

        public float Derivative {
            get => derivativeGain;
            set => derivativeGain = value;
        }

        #endregion

        #region Constructor

        public PIDController() { }

        public PIDController(float p, float i, float d) {
            this.proportionalGain = p;
            this.integralGain = i;
            this.derivativeGain = d;
        }

        #endregion

        #region Update

        public float Update(float dt, float current, float target) {
            var e = target - current;
            return Update_Internal(dt, e);
        }

        public float UpdateAngle(float dt, float current, float target) {
            var e = (target - current + 540) % 360 - 180; // angle difference
            return Update_Internal(dt, e);
        }

        private float Update_Internal(float dt, float e) {
            integrationStored = Mathf.Clamp(integrationStored + (e * dt), -saturation, saturation);
            float eRate = (e - lastE) / dt;
            lastE = e;

            var p = e * proportionalGain;
            var i = integralGain * integrationStored;
            var d = derivativeGain * eRate;

            return p + i + d;
        }

        #endregion

        #region Reset

        public void Reset() {
            integrationStored = 0f;
            lastE = 0f;
        }

        #endregion

        #region Simplified

        /// <param name="currentPosition">Current Position</param>
        /// <param name="targetPosition">Target Position</param>
        /// <param name="velocity">Current Velocity</param>
        /// <param name="dt">Delta Time</param>
        public static Vector3 GetAcceleration(Vector3 currentPosition, Vector3 targetPosition, Vector3 velocity, PIDController controller, float dt) {
            var kp = controller.proportionalGain;
            var kd = controller.derivativeGain;
            var g = 1f / (1f + kd * dt + kp * dt * dt);
            var ksg = kp * g;
            var kdg = (kd + kp * dt) * g;

            return (targetPosition - currentPosition) * ksg + (-velocity) * kdg;
        }

        /// <param name="currentPosition">Current Position</param>
        /// <param name="targetPosition">Target Position</param>
        /// <param name="velocity">Current Velocity</param>
        /// <param name="dt">Delta Time</param>
        public static Vector3 GetAcceleration(Vector3 currentPosition, Vector3 targetPosition, Vector3 velocity, float spring, float damping, float dt) {
            var kp = (6f * spring) * (6f * spring) * 0.25f;
            var kd = 4.5f * spring * damping;
            var g = 1f / (1f + kd * dt + kp * dt * dt);
            var ksg = kp * g;
            var kdg = (kd + kp * dt) * g;

            return (targetPosition - currentPosition) * ksg + (-velocity) * kdg;
        }

        public static Vector3 GetTorque(Quaternion current, Quaternion target, Vector3 angularVelocity, PIDController controller, float dt) {
            var kp = controller.proportionalGain;
            var kd = controller.derivativeGain;
            var g = 1f / (1f + kd * dt + kp * dt * dt);
            var ksg = kp * g;
            var kdg = (kd + kp * dt) * g;

            var q = target * Quaternion.Inverse(current);
            if(q.w < 0f) {
                q.x = -q.x;
                q.y = -q.y;
                q.z = -q.z;
                q.w = -q.w;
            }

            q.ToAngleAxis(out float angle, out Vector3 axis);
            return (axis * (ksg * Mathf.Deg2Rad * angle)) + -angularVelocity * kdg;
        }

        public static Vector3 GetTorque(Quaternion current, Quaternion target, Vector3 angularVelocity, float p, float d, float dt) {
            var kp = (6f * p) * (6f * p) * 0.25f;
            var kd = 4.5f * p * d;
            var g = 1f / (1f + kd * dt + kp * dt * dt);
            var ksg = kp * g;
            var kdg = (kd + kp * dt) * g;

            var q = target * Quaternion.Inverse(current);
            if(q.w < 0f) {
                q.x = -q.x;
                q.y = -q.y;
                q.z = -q.z;
                q.w = -q.w;
            }

            q.ToAngleAxis(out float angle, out Vector3 axis);
            return (axis * (ksg * Mathf.Deg2Rad * angle)) + -angularVelocity * kdg;
        }

        #endregion
    }

    [System.Serializable]
    public abstract class PIDController<T>
    {
        [SerializeField] protected float proportionalGain = 1f;
        [SerializeField] protected float integralGain = 1f;
        [SerializeField] protected float derivativeGain = 1f;

        [SerializeField] protected float saturation = 1f;

        protected T integrationStored;
        protected T lastE;

        public PIDController() { }
        public PIDController(float p, float i, float d) {
            this.proportionalGain = p;
            this.integralGain = i;
            this.derivativeGain = d;
        }

        public abstract T Update(float dt, T current, T target);
    }

    [System.Serializable]
    public sealed class PIDControllerVector3 : PIDController<Vector3>
    {
        public PIDControllerVector3() { }
        public PIDControllerVector3(float p, float i, float d) {
            this.proportionalGain = p;
            this.integralGain = i;
            this.derivativeGain = d;
        }

        public override Vector3 Update(float dt, Vector3 current, Vector3 target) {
            var e = target - current;
            integrationStored = (integrationStored + (e * dt)).Clamp(-saturation, saturation);
            var eRate = (e - lastE) / dt;
            lastE = e;

            var p = e * proportionalGain;
            var i = integralGain * integrationStored;
            var d = derivativeGain * eRate;

            return p + i + d;
        }
    }
}
