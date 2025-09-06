using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public struct Arc
    {
        #region Variables
        // Position, Delta, Gravity
        [SerializeField] private float px, py, pz, dx, dy, dz, gx, gy, gz;
        #endregion

        #region Properties

        public Vector3 Position {
            get => new Vector3(px, py, pz);
            set {
                px = value.x;
                py = value.y;
                pz = value.z;
            }
        }

        public Vector3 Direction {
            get => new Vector3(dx, dy, dz);
            set {
                dx = value.x;
                dy = value.y;
                dz = value.z;
            }
        }

        public Vector3 Gravity {
            get => new Vector3(gx, gy, gz);
            set {
                gx = value.x;
                gy = value.y;
                gz = value.z;
            }
        }

        #endregion

        #region Constructor

        public Arc(Vector3 direction) : this(Vector3.zero, direction, UnityEngine.Physics.gravity) { }
        public Arc(Vector3 direction, float velocity) : this(Vector3.zero, direction * velocity, UnityEngine.Physics.gravity) { }

        public Arc(Vector3 position, Vector3 direction) : this(position, direction, UnityEngine.Physics.gravity) { }
        public Arc(Vector3 position, Vector3 direction, float velocity) : this(position, direction * velocity, UnityEngine.Physics.gravity) { }


        public Arc(Vector3 position, Vector3 direction, float velocity, Vector3 gravity) : this(position, direction * velocity, gravity) { }
        public Arc(Vector3 position, Vector3 direction, Vector3 gravity) {
            px = position.x;
            py = position.y;
            pz = position.z;
            dx = direction.x;
            dy = direction.y;
            dz = direction.z;
            gx = gravity.x;
            gy = gravity.y;
            gz = gravity.z;
        }

        #endregion

        #region Evaluation

        public Vector3 Evaluate(float time) {
            var tt2 = time * time / 2f;
            return new Vector3(px + dx * time + gx * tt2, py + dy * time + gy * tt2, pz + dz * time + gz * tt2);
        }

        public static Vector3 Evaluate(Vector3 position, Vector3 direction, Vector3 gravity, float time) {
            return position + direction * time + gravity * (time * time / 2f);
        }


        public static Vector3 EvaluateTargetToTarget(Vector3 start, Vector3 end, float velocity)
            => EvaluateTargetToTarget(start, end, velocity, Physics.gravity);

        public static Vector3 EvaluateTargetToTarget(Vector3 start, Vector3 end, float velocity, Vector3 gravity) {
            var dir = end - start;
            var dist = dir.magnitude;
            var t = dist / velocity * MathUtility.SQR_2;
            return Evaluate(start, Vector3.ClampMagnitude(1f / t * dir - (0.5f * gravity * t), velocity), gravity, t);
        }

        #endregion

        #region Peak Height

        public Vector3 PointAtPeakHeight() => Evaluate(TimeAtPeakHeight());
        public float TimeAtPeakHeight() => gy == 0f ? Mathf.Infinity : dy / -gy;

        #endregion

        #region Static Creation

        public static Arc CreateTargetToTarget(Vector3 start, Vector3 end, float velocity, Vector3 gravity) {
            var dir = end - start;
            var dist = dir.magnitude;
            var t = dist / velocity * MathUtility.SQR_2;
            return new Arc(start, Vector3.ClampMagnitude(1f / t * dir - (0.5f * gravity * t), velocity), gravity);
        }

        /// DOES NOT SUPPORT GRAVITY YET!!! Need to convert XZ-Y into gravity planar calculations!
        public static Arc CreateTargetToTargetWithAngle(Vector3 start, Vector3 end, float angle, Vector3 gravity) {
            var dir = end - start;
            var dirXZ = dir.To_XZ(out float distY);
            var distX = dirXZ.magnitude;
            dirXZ = dirXZ.normalized;
            var esitmatedVelocity = CalculateVelocity(distX, distY, -gravity.magnitude, angle);
            var velocity = (Quaternion.AngleAxis(angle, Vector3.Cross(gravity, dirXZ)) * dirXZ) * esitmatedVelocity;
            return new Arc(start, velocity, gravity);
        }

        public static bool CalculateTargetToTarget(Vector3 start, Vector3 end, float velocity, Vector3 gravity, out Arc arc) {
            var dir = end - start;
            var dist = dir.magnitude;
            var t = dist / velocity * MathUtility.SQR_2;
            arc = new Arc(start, Vector3.ClampMagnitude(1f / t * dir - (0.5f * gravity * t), velocity), gravity);
            return Vector3.Distance(arc.Evaluate(t), end) < 0.05f;
        }

        #endregion

        #region Utility Math

        private static float CalculateVelocity(float horizontalDistance, float heightDelta, float gravity, float angle) {
            var dx = Mathf.Cos(Mathf.Deg2Rad * angle);
            var dy = Mathf.Sin(Mathf.Deg2Rad * angle);
            var tDivV0 = horizontalDistance / dx;
            var temp = -heightDelta + (dy * tDivV0);
            var temp2 = (gravity * -0.5f) * (tDivV0 * tDivV0);
            return Mathf.Sqrt(temp2 / temp);
        }

        #endregion
    }
}
