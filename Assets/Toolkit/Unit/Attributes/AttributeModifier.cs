using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [Serializable]
    public struct AttributeModifier {
        #region Variables

        [SerializeField] private AttributeType attrType;
        [SerializeField] private Stat.ValueType valueType;
        [SerializeField] private float value;

        #endregion

        #region Properties

        public AttributeType AttributeType => attrType;
        public Stat.ValueType ValueType => valueType;
        public float Value => value;

        #endregion

        #region Constructor

        public AttributeModifier(AttributeType attrType, float value) {
            this.attrType = attrType;
            this.valueType = Stat.ValueType.Base;
            this.value = value;
        }

        public AttributeModifier(AttributeType attrType, Stat.ValueType valueType, float value) {
            this.attrType = attrType;
            this.valueType = valueType;
            this.value = value;
        }

        #endregion
    }
}
