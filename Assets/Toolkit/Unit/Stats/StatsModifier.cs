using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [Serializable]
    public struct StatsModifier {
        #region Variables

        [SerializeField] private StatType statType;
        [SerializeField] private Stat.ValueType valueType;
        [SerializeField] private float value;

        #endregion

        #region Properties

        public StatType StatType => statType;
        public Stat.ValueType ValueType => valueType;
        public float Value => value;

        #endregion

        #region Constructor

        public StatsModifier(StatType statType, float value) {
            this.statType = statType;
            this.valueType = Stat.ValueType.Base;
            this.value = value;
        }

        public StatsModifier(StatType statType, Stat.ValueType valueType, float value) {
            this.statType = statType;
            this.valueType = valueType;
            this.value = value;
        }

        #endregion
    }
}
