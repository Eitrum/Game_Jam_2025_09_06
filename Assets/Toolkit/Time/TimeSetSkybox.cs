using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [AddComponentMenu("Toolkit/Day Cycle/Time Set Skybox")]
    public class TimeSetSkybox : MonoBehaviour
    {
        [SerializeField] private Material skyboxMaterial;

        public Material Skybox {
            get => skyboxMaterial;
            set {
                skyboxMaterial = value;
                RenderSettings.skybox = skyboxMaterial;
            }
        }

        private void OnEnable() {
            Timer.NextFrame(() => RenderSettings.skybox = skyboxMaterial);
        }
    }
}
