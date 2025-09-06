using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings {
    [CreateAssetMenu(menuName = "Toolkit/Game/Settings List")]
    public class SettingsList : ScriptableObject {

        [SerializeField] private string groupName = "group";
        [SerializeField] private NSOReferenceArray<ISettings> settings = new NSOReferenceArray<ISettings>();

        public void Initialize() {
            foreach(var setting in settings)
                setting?.Initialize(groupName);
        }
    }

    public interface ISettings {
        void Initialize(string groupName);
    }
}
