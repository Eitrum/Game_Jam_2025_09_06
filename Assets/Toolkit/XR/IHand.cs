using System;

namespace Toolkit.XR
{
    public interface IHand
    {
        XRHand Hand { get; }
        float GripStrength { get; }
        ITracking Tracking { get; }

        void Grab();
        void Release();
    }
}
