using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.XR
{
    public interface ITracking
    {
        XRHand Hand { get; }
        bool Enabled { get; set; }
        bool Update { get; set; }

        Transform Transform { get; }
        Pose Pose { get; }
        Pose Offset { get; set; }

        // Used for manual updating
        void UpdateTracking();
    }
}
