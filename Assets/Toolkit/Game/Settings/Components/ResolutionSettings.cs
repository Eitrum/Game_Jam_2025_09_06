using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Graphics/Resolution", typeof(ISettings))]
    public class ResolutionSettings : ScriptableObject, ISettings {

        private Vector2Int[] resolutions;
        private string[] resolutioNames;

        public void Initialize(string groupName) {
            resolutions = Screen.resolutions
                .Select(x => new Vector2Int(x.width, x.height))
                .Unique()
                .ToArray();

            resolutioNames = resolutions.Select(x => $"{x.x} x {x.y} ({(x.x / (float)x.y):0.0#})").ToArray();
            var dropdown = InGameSettings.RegisterDropdown(groupName, "resolution", resolutions, new Vector2Int(Screen.width, Screen.height), OnChanged);
            dropdown.SetValueNames(resolutioNames);
        }

        private void OnChanged(Vector2Int resolution) {
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreenMode);
        }
    }
}
