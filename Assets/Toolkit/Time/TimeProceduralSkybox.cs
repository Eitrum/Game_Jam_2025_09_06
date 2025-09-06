using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.DayCycle
{
    [ExecuteAlways]
    public class TimeProceduralSkybox : MonoBehaviour
    {
        #region Variables

        [SerializeField] private AnimationCurve sunSize = AnimationCurve.Linear(0f, 0.2f, 1f, 0.2f);
        [SerializeField] private AnimationCurve sunSizeConvergence = AnimationCurve.Linear(0f, 2f, 1f, 2f);
        [SerializeField] private AnimationCurve atmosphereThickness = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private Gradient skyTint = new Gradient();
        [SerializeField] private Gradient groundColor = new Gradient();
        [SerializeField] private AnimationCurve exposure = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [NonSerialized] private Material material = null;

        private int _sunSizeId;
        private int _sunSizeConvergenceId;
        private int _atmosphereThicknessId;
        private int _skyTintId;
        private int _groundColorId;
        private int _exposureId;

        #endregion

        #region Properties

        public Material Material => material;

        #endregion

        #region Unity Methods

        private void OnEnable() {
            if(material == null) {
                material = new Material(Shader.Find("Skybox/Procedural"));
                material.name = name;
                _sunSizeId = Shader.PropertyToID("_SunSize");
                _sunSizeConvergenceId = Shader.PropertyToID("_SunSizeConvergence");
                _atmosphereThicknessId = Shader.PropertyToID("_AtmosphereThickness");
                _skyTintId = Shader.PropertyToID("_SkyTint");
                _groundColorId = Shader.PropertyToID("_GroundColor");
                _exposureId = Shader.PropertyToID("_Exposure");
            }
            RenderSettings.skybox = material;
            TimeSystem.OnTimeUpdate += OnTimeUpdate;
        }

        private void OnDisable() {
            TimeSystem.OnTimeUpdate -= OnTimeUpdate;
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                DestroyImmediate(material);
            }
#endif
        }

        private void OnDestroy() {
            if(material != null) {
                if(Application.isPlaying)
                    Destroy(material);
                else
                    DestroyImmediate(material);
            }
        }

#endregion

#region Methods

        private void OnTimeUpdate(float dt, float time) {
            var nt = TimeSystem.NormalizedTime;
            material.SetFloat(_sunSizeId, sunSize.Evaluate(nt));
            material.SetFloat(_sunSizeConvergenceId, sunSizeConvergence.Evaluate(nt));
            material.SetFloat(_atmosphereThicknessId, atmosphereThickness.Evaluate(nt));
            material.SetColor(_skyTintId, skyTint.Evaluate(nt));
            material.SetColor(_groundColorId, groundColor.Evaluate(nt));
            material.SetFloat(_exposureId, exposure.Evaluate(nt));
        }

#endregion
    }
}
