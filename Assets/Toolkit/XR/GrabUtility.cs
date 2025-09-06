using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.XR
{
    public static class GrabUtility
    {
        #region Inverse Grab

        public static Pose InverseGrabPose(Pose grabLocation, Transform target)
            => InverseGrabPose(grabLocation, target.GetPose(Space.Self));

        public static Pose InverseGrabPose(Pose grabLocation, Transform target, Space space)
            => InverseGrabPose(grabLocation, target.GetPose(space));

        public static Pose InverseGrabPose(Pose grabLocation, Pose target) {
            var inverse = Quaternion.Inverse(grabLocation.rotation);
            return new Pose(
                inverse * (target.position - grabLocation.position),
                inverse * target.rotation);
        }

        public static Pose UpdateGrabPose(Pose grabLocation, Pose inversedGrabPose)
            => new Pose(
                grabLocation.position + grabLocation.rotation * inversedGrabPose.position,
                grabLocation.rotation * inversedGrabPose.rotation);

        #endregion
    }
}
