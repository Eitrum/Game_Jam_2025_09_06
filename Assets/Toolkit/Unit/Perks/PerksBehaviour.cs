using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [AddComponentMenu("Toolkit/Unit/Perks")]
    public class PerksBehaviour : MonoBehaviour, IPerks {
        #region Variables

        private const string TAG = "[Toolkit.PerksBehaviour] - ";
        private Dictionary<int, IPerk> active = new Dictionary<int, IPerk>();
        private List<IPerk> inactive = new List<IPerk>();
        [SerializeField] private List<IObjRef<IPerkBuilder>> initalPerks = new List<IObjRef<IPerkBuilder>>();
        private IUnit owner;

        #endregion

        #region Properties

        public IUnit Owner {
            get {
                if(owner == null)
                    owner = GetComponent<IUnit>();
                return owner;
            }
        }
        public IReadOnlyDictionary<int, IPerk> Active => active;
        public IList<IPerk> Inactive => inactive;

        public int ActiveCount => active.Count;
        public int InactiveCount => inactive.Count;

        public IPerk this[int id]
            => active.TryGetValue(id, out var p) ? p : null;

        #endregion

        #region Init

        void Awake() {
            owner = GetComponent<IUnit>();
            if(owner == null) {
                Debug.LogError(TAG + $"Could not find any component with 'IUnit' on object '{name}'");
            }
        }

        void Start() {
            if(Owner == null)
                return;
            foreach(var p in initalPerks) {
                AddPerk(p?.Reference?.Create(owner));
            }
        }

        void OnDestroy() {
            if(owner == null)
                return;
            foreach(var p in active) {
                p.Value.Effect?.OnDeactivate(owner);
            }
            active.Clear();
            inactive.Clear();
        }

        #endregion

        #region IPerks Impl

        public void AddPerk(IPerk perk) {
            if(perk == null) {
                Debug.LogWarning(TAG + $"Attemping to add null perk to '{owner?.Name ?? "null owner"}'");
                return;
            }
            var id = perk.Metadata.PerkId;
            if(active.ContainsKey(id)) {
                inactive.Add(perk);
                return;
            }
            active.Add(id, perk);
            perk.Effect?.OnActivate(owner);
        }

        public void Remove(IPerk perk) {
            if(perk == null) {
                Debug.LogWarning(TAG + $"Attemping to remove null perk to '{owner?.Name ?? "null owner"}'");
                return;
            }
            var id = perk.Metadata.PerkId;
            if(active.TryGetValue(id, out IPerk activePerk) && activePerk == perk) {
                active.Remove(id);
                perk?.Effect?.OnDeactivate(owner);
                for(int i = inactive.Count - 1; i >= 0; i--) {
                    var inac = inactive[i];
                    if(inac.Metadata.PerkId == id) {
                        inac.Effect?.OnActivate(owner);
                        active.Add(id, inac);
                        return;
                    }
                }
                return;
            }
            inactive.Remove(perk);
        }

        #endregion
    }
}
