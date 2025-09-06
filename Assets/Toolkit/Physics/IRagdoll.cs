using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public interface IRagdoll
    {
        bool IsInitialized { get; }
        Rigidbody Root { get; }
        Rigidbody Initialize();
        Rigidbody Initialize(Vector3 direction, float impulseForce);
        void Restore(bool enableAnimation = true);
        void Restore(AnimationClip clip, float duration);
    }
}
