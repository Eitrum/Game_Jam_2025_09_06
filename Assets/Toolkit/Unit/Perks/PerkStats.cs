using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Perks/Stats Perk")]
    public class PerkStats : ScriptableObject, IPerk, IPerkMetadata, IPerkEffect, IPerkBuilder, IPerkBuilderWithData {

        #region Variables

        [SerializeField] private string perkIdOverride = string.Empty;
        [Header("Display")]
        [SerializeField] private string displayName = string.Empty;
        [SerializeField, TextArea(2, 8)] private string description = string.Empty;
        [SerializeField] private Texture2D icon = null;
        [Header("Config")]
        [SerializeField] private PerkType type = PerkType.Stats;
        [SerializeField] private PerkCategory category = PerkCategory.Uncategorized;

        [Header("Attribute Changes")]
        [SerializeField] private StatsEntry[] entries;

        #endregion

        #region Properties

        public int PerkId => PerkUtility.GetPerkId(string.IsNullOrEmpty(perkIdOverride) ? name : perkIdOverride);
        public string Name => displayName;
        public string Description => description;
        public PerkType Type => type;
        public PerkCategory Category => category;
        public Texture2D Icon => icon;

        #endregion

        #region IPerk Impl

        public IPerkMetadata Metadata => this;
        IPerkEffect IPerk.Effect => this;

        IPerk IPerkBuilder.Create(IUnit owner) => this;
        IPerk IPerkBuilderWithData.Create(IUnit owner, IReadOnlyDictionary<PerkBuilderDataType, object> data) => this;

        void IPerkEffect.OnActivate(IUnit owner) {
            if(owner == null || owner.Attributes == null)
                return;
            foreach(var e in entries) {
                var stat = owner.Stats.GetStat(e.Type);
                stat += e.Stat;
                owner.Stats.SetStat(e.Type, stat);
            }
        }

        void IPerkEffect.OnDeactivate(IUnit owner) {
            if(owner == null || owner.Attributes == null)
                return;
            foreach(var e in entries) {
                var stat = owner.Stats.GetStat(e.Type);
                stat -= e.Stat;
                owner.Stats.SetStat(e.Type, stat);
            }
        }

        #endregion
    }
}
