using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Light Scale")]
    public class LightScale : MonoBehaviour
    {
        private Light lightComponent;
        private float scaled = 1f;
        private float defaultRange;

        private void Awake() {
            lightComponent = GetComponent<Light>();
            defaultRange = lightComponent.range;
        }

        private void OnEnable() {
            scaled = transform.lossyScale.magnitude;
            if(lightComponent)
                lightComponent.range = defaultRange * scaled;
        }

        private void OnDisable() {
            if(lightComponent)
                lightComponent.range = defaultRange;
        }
    }
}
