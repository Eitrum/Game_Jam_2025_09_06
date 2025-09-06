using System.Collections;
using System.Collections.Generic;
using Toolkit.Weather;
using UnityEngine;

namespace Toolkit.PhysicEx {
    [CreateAssetMenu(menuName = "Toolkit/Physics/Fluid Data")]
    public class FluidData : ScriptableObject, IFluidData, ISerializationCallbackReceiver {
        #region Variables

        [SerializeField] private string fluidName;
        [SerializeField, Tooltip("kg / m^3")] private float densityPerCubicMeter = 1000f;
        [SerializeField, Tooltip("mPa*s")] private float viscosity = 0f;

        private float densityForMagnusEffectCached;

        #endregion

        #region Properties

        public string Name => string.IsNullOrEmpty(fluidName) ? name : fluidName;
        public float DensityPerCubicMeter => densityPerCubicMeter;
        public float MagnusEffectDensity => densityForMagnusEffectCached;
        public float Viscosity => viscosity;


        #endregion

        #region Serialization Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            densityForMagnusEffectCached = Mathf.Pow(densityPerCubicMeter * 1000f, 0.33333333333333f) * 0.001f;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion
    }

    public interface IFluidData {
        string Name { get; }
        float DensityPerCubicMeter { get; }
        float MagnusEffectDensity { get; }
    }
}
