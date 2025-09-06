using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Utility/Light Flicker")]
    public class LightFlicker : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool useCurve = false;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField, Range(0f, 10f)] private float perlinStrenght = 0.25f;
        [SerializeField] private bool useRange = true;
        [SerializeField] private MinMax frequencyRange = new MinMax(0.9f, 1.1f);

        private float frequencyValue = 0f;
        private float time = 0f;
        private float perlinY = 0f;

        private float range;
        private float intensity;
        private Light lightSource;

        #endregion

        #region Unity Methods

        void Awake() {
            lightSource = GetComponent<Light>();
            frequencyValue = frequencyRange.Random;
            range = lightSource.range;
            intensity = lightSource.intensity;
        }

        void OnEnable() {

        }

        void OnDisable() {
            lightSource.range = range;
            lightSource.intensity = intensity;
        }

        void LateUpdate() {
            var dt = Time.deltaTime;
            time += dt * frequencyValue;
            if(time > 1f) {
                time -= 1f;
                frequencyValue = frequencyRange.Random;
                perlinY += 0.1f;
            }

            if(useRange)
                lightSource.range = range * Evaluate(time);
            else
                lightSource.intensity = intensity * Evaluate(time);
        }

        private float Evaluate(float t) {
            return useCurve ? curve.Evaluate(t) : (Mathf.PerlinNoise(perlinY, t) * perlinStrenght + 1f);
        }

        #endregion
    }
}
