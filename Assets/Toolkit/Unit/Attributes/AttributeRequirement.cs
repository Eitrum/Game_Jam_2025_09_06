using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Serializable]
    public struct AttributeRequirement : IAttributeRequirement {
        #region Variables

        [SerializeField] private AttributeType type;
        [SerializeField, Min(0)] private float amount;

        #endregion

        #region Properties

        public AttributeType Type {
            get => type;
            set => type = value;
        }

        public float Amount {
            get => amount;
            set => amount = Mathf.Max(0, value);
        }

        #endregion

        #region Constructor

        public AttributeRequirement(AttributeType type) {
            this.type = type;
            this.amount = 0;
        }

        public AttributeRequirement(AttributeType type, float amount) {
            this.type = type;
            this.amount = Mathf.Max(0, amount);
        }

        #endregion

        #region Operators

        public static implicit operator AttributeType(AttributeRequirement entry) => entry.type;
        public static implicit operator float(AttributeRequirement entry) => entry.amount;


        #endregion
    }
}
