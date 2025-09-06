using System;
using UnityEngine;

namespace Toolkit
{
    public static class Matrix4x4Extensions
    {

        #region Scale

        public static Matrix4x4 SetScale(this Matrix4x4 matrix, float scale) {
            return Matrix4x4.TRS(matrix.GetColumn(3), matrix.rotation, new Vector3(scale, scale, scale));
        }

        public static Matrix4x4 SetScale(this Matrix4x4 matrix, Vector3 scale) {
            return Matrix4x4.TRS(matrix.GetColumn(3), matrix.rotation, scale);
        }

        public static Matrix4x4 Scale(this Matrix4x4 matrix, float value)
            => SetScale(matrix, matrix.GetScale() * value);

        public static Matrix4x4 Scale(this Matrix4x4 matrix, Vector3 value)
            => SetScale(matrix, matrix.GetScale().Multiply(value));

        #endregion

        #region Getters

        public static Vector3 GetPosition(this Matrix4x4 matrix)
            => matrix.GetColumn(3);

        public static Vector3 GetScale(this Matrix4x4 matrix)
            => matrix.lossyScale;

        public static Quaternion GetRotation(this Matrix4x4 matrix)
            => matrix.rotation;

        #endregion

        #region Lerp

        public static Matrix4x4 Lerp(this Matrix4x4 matrix, Matrix4x4 other, float t)
            => LerpUnclamped(matrix, other, Mathf.Clamp01(t));

        public static Matrix4x4 LerpUnclamped(this Matrix4x4 matrix, Matrix4x4 other, float t) {
            var res = new Matrix4x4();
            for(int i = 0; i < 16; i++)
                res[i] = Mathf.LerpUnclamped(matrix[i], other[i], t);
            return res;
        }

        #endregion

        #region From Pose

        public static Matrix4x4 ToMatrix(this Pose pose) {
            return Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
        }

        #endregion
    }
}
