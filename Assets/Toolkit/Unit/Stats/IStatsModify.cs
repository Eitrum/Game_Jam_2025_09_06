using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IStatsModify {
        void GetStatsModifiersNonAlloc(List<StatsModifier> modifiers);
    }
}
