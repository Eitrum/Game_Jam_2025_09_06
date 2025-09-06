using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public class PhysicsUpdateSettings : ScriptableSingleton<PhysicsUpdateSettings>
    {
        protected override bool KeepInResources => true;

        #region Variables

        [SerializeField] private PhysicsSettings physicsSettings = null;

        #endregion

        #region Properties

        public static PhysicsSettings PhysicsSettings {
            get => Instance.physicsSettings;
            set => Instance.physicsSettings = value;
        }
        public static bool IsToolkitPhysicsUpdate => Instance.physicsSettings != null ? Instance.physicsSettings.Mode == PhysicsSettings.SimulationMode.Toolkit : false;
        public static bool UpdatePerFrame => Instance.physicsSettings != null ? Instance.physicsSettings.UpdatesPerSecond < Mathf.Epsilon : Time.fixedDeltaTime < Mathf.Epsilon;

        #endregion

        #region Methods

        public static bool TryGetSmoothing(out int smoothing) {
            if(!Instance.physicsSettings) {
                smoothing = 0;
                return false;
            }
            smoothing = Mathf.Clamp(Instance.physicsSettings.FixedTimeSmoothing, 0, 100);
            return smoothing > 0;
        }

        #endregion
    }
}
