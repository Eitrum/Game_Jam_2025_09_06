using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Game/Run In Background", typeof(ISettings))]
    public class RunInBackgroundSettings : ScriptableObject, ISettings {

        [SerializeField] private bool defaultValue = true;

        public void Initialize(string groupName) {
            InGameSettings.RegisterToggle(groupName, "runinbackground", defaultValue, OnChanged);
        }

        private static void OnChanged(bool value) {
            Application.runInBackground = value;
        }
    }
}
