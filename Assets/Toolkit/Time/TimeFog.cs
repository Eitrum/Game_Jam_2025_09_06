using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    [AddComponentMenu("Toolkit/Day Cycle/Time Fog")]
    public class TimeFog : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Gradient fogColor = new Gradient();
        [SerializeField] private FogMode mode = FogMode.ExponentialSquared;
        [SerializeField] private AnimationCurve fogStartDistance = AnimationCurve.Linear(0f, 50f, 1f, 50f);
        [SerializeField] private AnimationCurve fogEndDistance = AnimationCurve.Linear(0f, 300f, 1f, 300f);
        [SerializeField] private AnimationCurve fogDensity = AnimationCurve.Linear(0f, 0.01f, 1f, 0.01f);

        #endregion

        #region Properties

        public Color CurrentFogColor => fogColor.Evaluate(TimeSystem.NormalizedTime);
        public FogMode Mode => mode;
        public MinMax CurrentFogRange => new MinMax(fogStartDistance.Evaluate(TimeSystem.NormalizedTime), fogEndDistance.Evaluate(TimeSystem.NormalizedTime));
        public float CurrentFogDensity => fogDensity.Evaluate(TimeSystem.NormalizedTime);

        #endregion

        #region Unity Methods

        private void OnEnable() {
            RenderSettings.fog = true;
            UpdateFogMode();
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1) {
            if(enabled) {
                RenderSettings.fog = true;
                UpdateFogMode();
                OnTimeUpdate(0, 0);
            }
        }

        private void OnDisable() {
            RenderSettings.fog = false;
            UpdateFogMode();
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        #endregion

        #region Fog Time Update

        public void UpdateFogMode() {
            RenderSettings.fogMode = mode;
        }

        public void UpdateFogMode(FogMode mode) {
            RenderSettings.fogMode = mode;
        }

        private void OnTimeUpdate(float dt, float time) {
            RenderSettings.fogColor = fogColor.Evaluate(TimeSystem.NormalizedTime);
            switch(mode) {
                case FogMode.Exponential:
                case FogMode.ExponentialSquared:
                    RenderSettings.fogDensity = fogDensity.Evaluate(TimeSystem.NormalizedTime);
                    break;
                case FogMode.Linear:
                    RenderSettings.fogStartDistance = fogStartDistance.Evaluate(TimeSystem.NormalizedTime);
                    RenderSettings.fogEndDistance = fogEndDistance.Evaluate(TimeSystem.NormalizedTime);
                    break;
            }
        }

        #endregion
    }
}
