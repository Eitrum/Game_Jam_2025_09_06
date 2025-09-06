using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Perks/Wrapper Perk")]
    public class PerkWrapper : ScriptableObject, IPerkMetadata, IPerkBuilder, IPerkBuilderWithData {
        #region Variables

        [SerializeField] private string perkIdOverride = string.Empty;
        [Header("Display")]
        [SerializeField] private string displayName = string.Empty;
        [SerializeField, TextArea(2, 8)] private string description = string.Empty;
        [SerializeField] private Texture2D icon = null;
        [Header("Config")]
        [SerializeField] private PerkType type = PerkType.None;
        [SerializeField] private PerkCategory category = PerkCategory.Uncategorized;

        [Header("Perks")]
        [SerializeField] private IObjRef<IPerkBuilder>[] builders = new IObjRef<IPerkBuilder>[0];

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

        IPerk IPerkBuilder.Create(IUnit owner) => new Instance(this, owner);
        IPerk IPerkBuilderWithData.Create(IUnit owner, IReadOnlyDictionary<PerkBuilderDataType, object> data) => new Instance(this, owner, data);

        #endregion

        private class Instance : IPerk, IPerkEffect {
            #region Variables

            public IUnit Owner { get; private set; }
            public IPerkMetadata Metadata { get; private set; }
            IPerkEffect IPerk.Effect => this;
            List<IPerk> childPerks = new List<IPerk>();

            #endregion

            #region Constructor

            public Instance(PerkWrapper wrapper, IUnit owner) {
                this.Metadata = wrapper;
                this.Owner = owner;
                foreach(var b in wrapper.builders) {
                    var cperk = b?.Reference?.Create(owner);
                    if(cperk != null)
                        childPerks.Add(cperk);
                }
            }

            public Instance(PerkWrapper wrapper, IUnit owner, IReadOnlyDictionary<PerkBuilderDataType, object> data) {
                this.Metadata = wrapper;
                this.Owner = owner;
                foreach(var b in wrapper.builders) {
                    var r = b.Reference;
                    IPerk cperk;
                    if(r is IPerkBuilderWithData pbwd)
                        cperk = pbwd.Create(owner, data);
                    else
                        cperk = r?.Create(owner);
                    if(cperk != null)
                        childPerks.Add(cperk);
                }
            }

            #endregion

            #region Effect Impl

            void IPerkEffect.OnActivate(IUnit owner) {
                foreach(IPerk p in childPerks)
                    p.Effect?.OnActivate(owner);
            }

            void IPerkEffect.OnDeactivate(IUnit owner) {
                foreach(IPerk p in childPerks)
                    p.Effect?.OnDeactivate(owner);
            }

            #endregion
        }
    }
}
