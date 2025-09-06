using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.TacticalCamera
{
    public class TiltClamp : ScriptableObject, ITacticalCameraModule
    {
        [SerializeField] private MinMax tiltRange = new MinMax(10f, 90f);

        public void UpdateModule(ITacticalCamera tc, float dt) {
            var rot = tc.Pivot.localEulerAngles;
            rot.x = Mathematics.MathUtility.ClampRotation(rot.x, tiltRange);
            tc.Pivot.localEulerAngles = rot;
        }
    }
}
