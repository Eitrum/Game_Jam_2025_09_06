using System;
using UnityEngine;

namespace Toolkit
{
    public static class QuaternionExtensions
    {

        #region Conversion

        public static Vector4 ToVector4(this Quaternion quaternion)
            => new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

        public static Matrix4x4 ToMatrix(this Quaternion quaternion) => Matrix4x4.Rotate(quaternion);

        #endregion

        #region Utility

        public static Quaternion GetDelta(this Quaternion rotation, Quaternion oldRotation)
            => rotation * Quaternion.Inverse(oldRotation);

        public static Vector3 ToAngularVelocity(this Quaternion rotation) {
            rotation.ToAngleAxis(out float angle, out Vector3 axis);
            return axis * (angle * Mathf.Deg2Rad);
        }

        public static float GetAngle(this Quaternion rotation, Vector3 axis)
            => Vector3.SignedAngle(new Vector3(0, 0, 1), rotation * new Vector3(0, 0, 1), axis);


        public static Quaternion Snap(this Quaternion rotation, float degrees) {
            return Quaternion.Euler(rotation.eulerAngles.Snap(degrees));
        }

        public static Quaternion Snap(this Quaternion rotation, float degrees, Transform relativeTo) {
            if(relativeTo == null)
                Quaternion.Euler(rotation.eulerAngles.Snap(degrees));
            var rot = relativeTo.rotation;
            var invRot = Quaternion.Inverse(rot) * rotation;

            return rot * Quaternion.Euler(invRot.eulerAngles.Snap(degrees));
        }

        #endregion

        #region To Direction

        public static Vector3 Forward(this Quaternion quaternion) => quaternion * new Vector3(0, 0, 1);
        public static Vector3 Right(this Quaternion quaternion) => quaternion * new Vector3(1, 0, 0);
        public static Vector3 Up(this Quaternion quaternion) => quaternion * new Vector3(0, 1, 0);

        #endregion

        #region Equals

        public static bool Equals(this Quaternion value, Quaternion otherValue, float proximity) {
            return
                value.x + proximity >= otherValue.x && value.x - proximity <= otherValue.x &&
                value.y + proximity >= otherValue.y && value.y - proximity <= otherValue.y &&
                value.z + proximity >= otherValue.z && value.z - proximity <= otherValue.z &&
                value.w + proximity >= otherValue.w && value.w - proximity <= otherValue.w;
        }

        #endregion

        #region Isolate

        public static Quaternion IsolateYAxis(this Quaternion quat) {
            return Quaternion.Euler(0, Isolate(quat.Forward(), Vector3.up), 0);
        }

        // Requires testing
        public static Quaternion IsolateXAxis(this Quaternion quat) {
            return Quaternion.Euler(Isolate(quat.Forward(), Vector3.right), 0, 0);
        }

        // Requires testing
        public static Quaternion IsolateZAxis(this Quaternion quat) {
            return Quaternion.Euler(0, 0, Isolate(quat.Forward(), Vector3.forward));
        }

        // Requires testing
        public static Quaternion Isolate(this Quaternion quat, Vector3 axis) {
            return Quaternion.AngleAxis(Isolate(quat.Forward(), axis), axis);
        }

        private static float Isolate(Vector3 lookDir, Vector3 axis) {
            Vector3 right = Vector3.Cross(axis, Vector3.forward);
            var forward = Vector3.Cross(right, axis);
            return Mathf.Atan2(Vector3.Dot(lookDir, right), Vector3.Dot(lookDir, forward)) * Mathf.Rad2Deg;
        }

        #endregion
    }
}
