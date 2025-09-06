using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.Components {
    [NSOFile("Graphics/Window Mode", typeof(ISettings))]
    public class WindowModeSettings : ScriptableObject, ISettings {

#if UNITY_STANDALONE_WIN
        public enum FullScreenMode_Windows {
            ExclusiveFullScreen = 0,
            Borderless = 1,
            Windowed = 3,
        }
#elif UNITY_STANDALONE_OSX
        public enum FullScreenMode_Mac {
            Borderless = 1,
            Maximized = 2,
            Windowed = 3,
        }
#else
        public enum FullScreenMode_Other {
            Borderless = 1,
            Windowed = 3,
        }
#endif

        public void Initialize(string groupName) {
#if UNITY_STANDALONE_WIN
            InGameSettings.RegisterDropdown(groupName, "window", FullScreenMode_Windows.Borderless, (FullScreenMode_Windows mode) => Screen.fullScreenMode = (FullScreenMode)mode);
#elif UNITY_STANDALONE_OSX
            InGameSettings.RegisterDropdown(groupName, "window", FullScreenMode_Mac.Borderless, (FullScreenMode_Mac mode) => Screen.fullScreenMode = (FullScreenMode)mode);
#else
            InGameSettings.RegisterDropdown(groupName, "window", FullScreenMode_Other.Borderless, (FullScreenMode_Other mode) => Screen.fullScreenMode = (FullScreenMode)mode);
#endif
        }
    }
}
