using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Graphics/FPS", typeof(ISettings))]
    public class FPSSettings : ScriptableObject, ISettings {

        [SerializeField] private int defaultValue = 60;
        [SerializeField] private MinMaxInt range = new MinMaxInt(20, 300);

        public void Initialize(string groupName) {
            InGameSettings.RegisterRange(groupName, "fps", defaultValue, range.min, range.max, OnUpdate);
        }

        private void OnUpdate(int value) {
            Application.targetFrameRate = (value >= range.max) ? (-1) : value;
        }
    }
}
