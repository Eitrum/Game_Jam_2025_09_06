using System;
using UnityEngine;

namespace Toolkit.XR
{
    public interface IGrabable : INullable
    {
        // Unity References
        Transform Transform { get; }
        Rigidbody Body { get; }

        // Properties
        bool IsHeld { get; }
        IHand Holder { get; }

        event OnGrabDelegate OnGrab;
        event OnReleaseDelegate OnRelease;
        event OnUpdateDelegate OnUpdate;

        // Methods
        bool CanGrab(IHand hand);
        bool Grab(IHand hand);
        bool Release(IHand hand);
        void UpdateGrabable(IHand hand, float dt);
    }

    public delegate void OnGrabDelegate(IHand hand);
    public delegate void OnReleaseDelegate(IHand hand);
    public delegate void OnUpdateDelegate(IHand hand, float dt);
}
