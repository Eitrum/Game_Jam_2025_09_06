using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    [CreateAssetMenu(menuName = "Toolkit/UI/HUD Preset")]
    public class HUDPreset : ScriptableObject {
        #region Variables

        [SerializeField] private HUDModule[] prefabs = { };

        #endregion

        #region Properties

        public IReadOnlyList<HUDModule> Prefabs => prefabs;

        #endregion
    }
}
