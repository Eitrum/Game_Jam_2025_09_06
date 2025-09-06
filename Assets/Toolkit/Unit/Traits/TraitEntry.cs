using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Toolkit.Unit {
    [System.Serializable]
    public struct TraitEntry {
        #region Variables

        private const float MIN = TraitsUtility.MIN;
        private const float MAX = TraitsUtility.MAX;

        [SerializeField] private TraitType type;
        [SerializeField, Range(MIN, MAX)] private float value;

        #endregion

        #region Properties

        public TraitType Type {
            get => type;
            set => type = value;
        }

        public float Value {
            get => this.value;
            set => this.value = Mathf.Clamp(value, MIN, MAX);
        }

        #endregion

        #region Constructor

        public TraitEntry(TraitType type) {
            this.type = type;
            this.value = 0;
        }

        public TraitEntry(TraitType type, float value) {
            this.type = type;
            this.value = Mathf.Clamp(value, MIN, MAX);

        }

        #endregion

        #region Overrides

        public override string ToString() {
            return $"({type}:{value})";
        }

        public override int GetHashCode() {
            return (type.ToInt() << 8) ^ value.GetHashCode();
        }

        #endregion

        #region Opertors

        public static implicit operator TraitType(TraitEntry entry) => entry.type;
        public static implicit operator float(TraitEntry entry) => entry.value;
        public static implicit operator int(TraitEntry entry) => Mathf.RoundToInt(entry.value);

        #endregion
    }
}
