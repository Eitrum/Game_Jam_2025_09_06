using System;
using UnityEngine;

namespace Toolkit
{
    [System.Flags]
    public enum TransformMask
    {
        None = 0,

        PositionX = 2,
        PositionY = 4,
        PositionZ = 8,

        RotationX = 16,
        RotationY = 32,
        RotationZ = 64,

        ScaleX = 128,
        ScaleY = 256,
        ScaleZ = 512,

        Position = PositionX | PositionY | PositionZ,
        Rotation = RotationX | RotationY | RotationZ,
        Scale = ScaleX | ScaleY | ScaleZ,
        PositionRotation = Position | Rotation,
        All = Position | Rotation | Scale
    }

    public static class TransformMaskUtility
    {
        public static RigidbodyConstraints ToRigidbodyConstraints(this TransformMask mask) {
            return (RigidbodyConstraints)mask;
        }

        public static TransformMask ToTransformMask(this RigidbodyConstraints constraints) {
            return (TransformMask)constraints;
        }
    }
}
