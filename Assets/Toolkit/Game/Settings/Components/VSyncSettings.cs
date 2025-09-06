using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Graphics/VSync", typeof(ISettings))]
    public class VSyncSettings : ScriptableObject, ISettings {

        public enum VSyncMode {
            Disabled = 0,
            Enabled = 1,
            Half = 2,
        }

        [SerializeField] private VSyncMode defaultMode = VSyncMode.Enabled;

        public void Initialize(string group) {
            InGameSettings.RegisterDropdown(group, "vsync", defaultMode, OnChanged);
        }

        private void OnChanged(VSyncMode value) {
            QualitySettings.vSyncCount = value.ToInt();
        }
    }
}
