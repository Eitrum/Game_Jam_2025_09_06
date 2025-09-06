using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Day Cycle/Time Environment Light")]
    public class TimeEnvironmentLight : MonoBehaviour
    {
        #region Variables

        [SerializeField] private EnvironmentLightSource ambientMode = EnvironmentLightSource.Color;
        [SerializeField] private Gradient skyColor = new Gradient();
        [SerializeField] private Gradient equatorColor = new Gradient();
        [SerializeField] private Gradient groundColor = new Gradient();
        [SerializeField] private AnimationCurve skyboxIntensityModifier = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        #endregion

        #region Properties

        public EnvironmentLightSource AmbientMode => ambientMode;
        public Color CurrentSkyColor => skyColor.Evaluate(TimeSystem.NormalizedTime);
        public Color CurrentEquatorColor => equatorColor.Evaluate(TimeSystem.NormalizedTime);
        public Color CurrentGroundColor => groundColor.Evaluate(TimeSystem.NormalizedTime);
        public float CurrentSkyboxIntensityModifier => skyboxIntensityModifier.Evaluate(TimeSystem.NormalizedTime);

        #endregion

        #region Unity Methods

        private void OnEnable() {
            RenderSettings.ambientMode = GetAmbientMode(ambientMode);
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
        }

        #endregion

        #region OnTimeUpdate

        public void UpdateAmbientMode() {
            RenderSettings.ambientMode = GetAmbientMode(ambientMode);
        }

        public void UpdateAmbientMode(EnvironmentLightSource source) {
            RenderSettings.ambientMode = GetAmbientMode(source);
        }

        private void OnTimeUpdate(float dt, float time) {
            switch(ambientMode) {
                case EnvironmentLightSource.Gradient:
                    var nt = TimeSystem.NormalizedTime;
                    RenderSettings.ambientSkyColor = skyColor.Evaluate(nt);
                    RenderSettings.ambientEquatorColor = equatorColor.Evaluate(nt);
                    RenderSettings.ambientGroundColor = groundColor.Evaluate(nt);
                    break;
                case EnvironmentLightSource.Color:
                    RenderSettings.ambientLight = skyColor.Evaluate(TimeSystem.NormalizedTime);
                    break;
                case EnvironmentLightSource.Skybox:
                    RenderSettings.ambientIntensity = skyboxIntensityModifier.Evaluate(TimeSystem.NormalizedTime);
                    break;
            }
        }

        #endregion

        #region Ambient mode conversion

        public static UnityEngine.Rendering.AmbientMode GetAmbientMode(EnvironmentLightSource source) {
            switch(source) {
                case EnvironmentLightSource.Color: return UnityEngine.Rendering.AmbientMode.Flat;
                case EnvironmentLightSource.Gradient: return UnityEngine.Rendering.AmbientMode.Trilight;
            }
            return UnityEngine.Rendering.AmbientMode.Skybox;
        }

        #endregion
    }

    public enum EnvironmentLightSource
    {
        Skybox,
        Color,
        Gradient,
    }
}
