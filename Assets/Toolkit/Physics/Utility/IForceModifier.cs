using System;
using UnityEngine;

namespace Toolkit.PhysicEx.Utility
{
    public interface IForceModifier
    {
        void Modify(Rigidbody body, IForceArea area, float multiplier);
    }
}
