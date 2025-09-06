using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Rotate Once (On Enable)")]
    public class RotateOnceEnable : MonoBehaviour
    {
        [SerializeField] private Space space = Space.Self;
        [SerializeField] private bool additive = false;

        [SerializeField] private bool xAxis = false;
        [SerializeField] private MinMax xRange = MinMax.EulerRotation;

        [SerializeField] private bool yAxis = false;
        [SerializeField] private MinMax yRange = MinMax.EulerRotation;

        [SerializeField] private bool zAxis = false;
        [SerializeField] private MinMax zRange = MinMax.EulerRotation;

        private void OnEnable() {
            Vector3 rotation = new Vector3(
                xAxis ? xRange.Random : 0f,
                yAxis ? yRange.Random : 0f,
                zAxis ? zRange.Random : 0f);
            if(space == Space.Self)
                transform.localEulerAngles = additive ? (transform.localEulerAngles + rotation) : rotation;
            else
                transform.eulerAngles = additive ? (transform.eulerAngles + rotation) : rotation;
        }
    }
}
