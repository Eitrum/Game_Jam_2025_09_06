using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public static class VelocityTrackingUtility
    {
        public static Vector3 GetVelocityRelativeToCenter(Vector3 velocity, Vector3 angularVelocity, Vector3 pointToGetVelocityFrom) {
            var dir = pointToGetVelocityFrom;
            return ((Quaternion.Euler(angularVelocity * Mathf.Rad2Deg) * dir) - dir) + velocity;
        }

        public static Vector3 GetVelocityRelativeToPoint(Vector3 point, Vector3 velocity, Vector3 angularVelocity, Vector3 pointToGetVelocityFrom) {
            var dir = pointToGetVelocityFrom - point;
            return ((Quaternion.Euler(angularVelocity * Mathf.Rad2Deg) * dir) - dir) + velocity;
        }
    }
}
