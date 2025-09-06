using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit
{
    public enum PoseFlag
    {
        None = 0,
        Position = 1,
        Rotation = 2,

        PositionRotation = Position | Rotation,
    }

    public static class PoseExtensions
    {
        public static Pose Lerp(this Pose p, Pose other, float t) {
            return LerpUnclamped(p, other, Mathf.Clamp01(t));
        }

        public static Pose LerpUnclamped(this Pose p, Pose other, float t) {
            return new Pose(
                Vector3.LerpUnclamped(p.position, other.position, t),
                Quaternion.SlerpUnclamped(p.rotation, other.rotation, t));
        }
    }
}
