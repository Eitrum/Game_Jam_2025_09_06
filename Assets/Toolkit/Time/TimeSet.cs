using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Day Cycle/Time Set")]
    public class TimeSet : MonoBehaviour
    {
        [SerializeField, Time] private float time = 0f;

        private void OnEnable() {
            TimeSystem.Set(time);
        }
    }
}
