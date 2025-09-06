using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public interface IVelocityTracking
    {
        Vector3 Velocity { get; }
        Vector3 AngularVelocity { get; }

        Vector3 GetVelocityAt(Vector3 point);
        Vector3 GetAngularVelocityAt(Vector3 point);
    }
}
