using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public class PerkMetadata : IPerkMetadata {

        #region Variables

        [SerializeField] private string perkId = string.Empty;
        [Header("Display")]
        [SerializeField] private string displayName = string.Empty;
        [SerializeField, TextArea(2, 8)] private string description = string.Empty;
        [SerializeField] private Texture2D icon = null;
        [Header("Config")]
        [SerializeField] private PerkType type = PerkType.None;
        [SerializeField] private PerkCategory category = PerkCategory.Uncategorized;

        #endregion

        #region Properties

        public int PerkId => PerkUtility.GetPerkId(perkId);
        public string Name => displayName;
        public string Description => description;
        public PerkType Type => type;
        public PerkCategory Category => category;
        public Texture2D Icon => icon;

        #endregion
    }
}
