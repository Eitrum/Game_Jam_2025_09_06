using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct TransformData
    {
        #region Variables

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        #endregion

        #region Properties

        public static TransformData Identity => new TransformData(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);
        public Matrix4x4 Matrix => Matrix4x4.TRS(Position, Rotation, Scale);
        public Pose Pose {
            get => new Pose(Position, Rotation);
            set {
                Position = value.position;
                Rotation = value.rotation;
            }
        }

        public Vector3 this[int index] {
            get {
                switch(index) {
                    case 0: return Position;
                    case 1: return Rotation.eulerAngles;
                    case 2: return Scale;
                }
                throw new IndexOutOfRangeException();
            }
        }

        #endregion

        #region Constructor

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale) {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        public TransformData(Transform transform) : this(transform, Space.World) { }

        public TransformData(Transform transform, Space space) {
            if(space == Space.Self) {
                Position = transform.localPosition;
                Rotation = transform.localRotation;
                Scale = transform.localScale;
            }
            else {
                Position = transform.position;
                Rotation = transform.rotation;
                Scale = transform.lossyScale;
            }
        }

        public TransformData(Matrix4x4 matrix) {
            this.Position = matrix.GetColumn(3);
            this.Rotation = matrix.rotation;
            this.Scale = matrix.lossyScale;
        }

        #endregion

        #region Apply

        public void ApplyTo(Transform transform)
            => ApplyTo(transform, Space.World);

        public void ApplyTo(Transform transform, Space space) {
            if(space == Space.Self) {
                transform.localPosition = Position;
                transform.localRotation = Rotation;
                transform.localScale = Scale;
            }
            else {
                transform.position = Position;
                transform.rotation = Rotation;
                transform.SetLossyScale(Scale);
            }
        }

        #endregion

        #region Copy

        public void CopyFrom(Transform transform)
            => CopyFrom(transform, Space.World);

        public void CopyFrom(Transform transform, Space space) {
            if(space == Space.Self) {
                Position = transform.localPosition;
                Rotation = transform.localRotation;
                Scale = transform.localScale;
            }
            else {
                Position = transform.position;
                Rotation = transform.rotation;
                Scale = transform.lossyScale;
            }
        }

        #endregion

        #region Translate

        public void Translate(Vector3 offset) {
            Position += offset;
        }

        public void Translate(Vector3 offset, Space space) {
            if(space == Space.Self)
                Position += Rotation * offset;
            else
                Position += offset;
        }

        #endregion

        #region Rotate

        public void Rotate(Quaternion rotation) {
            Rotation = rotation * Rotation;
        }

        public void RotateAround(Vector3 point, Quaternion rotation) {
            Position = point + rotation * (Position - point);
            Rotation = rotation * Rotation;
        }

        #endregion

        #region Scale

        public void MultiplyScale(float value) {
            Scale *= value;
        }

        public void MultiplyScaleRelativeToPoint(Vector3 point, float value) {
            Position = point + (point - Position) * value;
            Scale *= value;
        }

        #endregion

        #region TransformedBy

        public TransformData InverseTransformBy(Transform transform) {
            return new TransformData(
                transform.InverseTransformPoint(Position),
                Quaternion.Inverse(transform.rotation) * Rotation,
                Scale.Divide(transform.lossyScale)
                );
        }

        public TransformData InverseTransformBy(Pose pose) {
            var r = Quaternion.Inverse(pose.rotation);
            return new TransformData(
                (r * Position) - pose.position,
                r * Rotation,
                Scale
                );
        }

        public TransformData TransformBy(Transform transform) {
            return new TransformData(
                transform.TransformPoint(Position),
                transform.rotation * Rotation,
                Scale.Multiply(transform.lossyScale)
                );
        }

        public TransformData TransformBy(Pose pose) {
            return new TransformData(
                pose.position + pose.rotation * Position,
                pose.rotation * Rotation,
                Scale
                );
        }

        #endregion

        #region Delta

        public TransformData GetDelta(Transform other) {
            return new TransformData(
                Position - other.position,
                Rotation * Quaternion.Inverse(other.rotation),
                Scale - other.lossyScale
                );
        }

        public TransformData GetDeltaWithoutScale(Transform other) {
            return new TransformData(
                Position - other.position,
                Rotation * Quaternion.Inverse(other.rotation),
                new Vector3(1, 1, 1)
                );
        }

        public TransformData GetDelta(TransformData other) {
            return new TransformData(
                Position - other.Position,
                Rotation * Quaternion.Inverse(other.Rotation),
                Scale - other.Scale
                );
        }

        public TransformData GetDeltaWithoutScale(TransformData other) {
            return new TransformData(
                Position - other.Position,
                Rotation * Quaternion.Inverse(other.Rotation),
                new Vector3(1, 1, 1)
                );
        }

        #endregion

        #region Lerp

        public static TransformData Lerp(TransformData lhs, TransformData rhs, float t)
            => LerpUnclamped(lhs, rhs, Mathf.Clamp01(t));

        public static TransformData LerpUnclamped(TransformData lhs, TransformData rhs, float t) {
            return new TransformData(
                    Vector3.LerpUnclamped(lhs.Position, rhs.Position, t),
                    Quaternion.SlerpUnclamped(lhs.Rotation, rhs.Rotation, t),
                    Vector3.LerpUnclamped(lhs.Scale, rhs.Scale, t)
                );
        }

        public static Pose LerpPose(TransformData lhs, TransformData rhs, float t)
            => LerpPoseUnclamped(lhs, rhs, Mathf.Clamp01(t));

        public static Pose LerpPoseUnclamped(TransformData lhs, TransformData rhs, float t) {
            return new Pose(
                    Vector3.LerpUnclamped(lhs.Position, rhs.Position, t),
                    Quaternion.SlerpUnclamped(lhs.Rotation, rhs.Rotation, t)
              );
        }

        #endregion

        #region Overrides

        public override string ToString() {
            return $"(p[{Position}] r[{Rotation}] s[{Scale}])";
        }

        #endregion

        #region Conversions

        public static implicit operator Pose(TransformData td) => new Pose(td.Position, td.Rotation);
        public static implicit operator TransformData(Pose pose) => new TransformData(pose.position, pose.rotation, Vector3.one);

        #endregion
    }
}
