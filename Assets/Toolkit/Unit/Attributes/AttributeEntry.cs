using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Serializable]
    public struct AttributeEntry {
        #region Variables

        [SerializeField] private AttributeType type;
        [SerializeField] private Stat stat;

        #endregion

        #region Properties

        public AttributeType Type {
            get => type;
            set => type = value;
        }

        public Stat Stat {
            get => stat;
            set => stat = value;
        }

        #endregion

        #region Constructor

        public AttributeEntry(AttributeType type) {
            this.type = type;
            this.stat = new Stat();
        }

        public AttributeEntry(AttributeType type, Stat stat) {
            this.type = type;
            this.stat = stat;
        }

        #endregion

        #region Operators

        public static implicit operator AttributeType(AttributeEntry entry) => entry.type;
        public static implicit operator Stat(AttributeEntry entry) => entry.stat;

        #endregion
    }
}
