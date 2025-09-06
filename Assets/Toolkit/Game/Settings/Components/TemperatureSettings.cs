using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {

    [NSOFile("Interface/Temperature", typeof(ISettings))]
    public class TemperatureSettings : ScriptableObject, ISettings {
        public void Initialize(string groupName) {
            InGameSettings.RegisterDropdown(groupName, "temperature", Weather.TemperatureType.Celcius);
        }
    }
}
