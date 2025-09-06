using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [AddComponentMenu("Toolkit/Day Cycle/Time Update")]
    public class TimeUpdate : MonoBehaviour
    {
        #region Variables

        [SerializeField, Time] private float timePerSecondRealtime = 24f;

        #endregion

        #region Properties

        public float TimePerInGameDay => TimeSystem.FULL_DAY / timePerSecondRealtime;

        #endregion

        #region Methods

        void Update() {
            if(timePerSecondRealtime > Mathf.Epsilon)
                TimeSystem.Add(timePerSecondRealtime * Time.deltaTime);
        }

        #endregion
    }
}
