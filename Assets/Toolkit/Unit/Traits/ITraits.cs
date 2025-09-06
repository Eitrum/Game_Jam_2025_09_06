using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {

    public delegate void OnTraitChangedCallback(TraitType type);

    public interface ITrait {
        float this[TraitType type] { get; set; }
        float GetTrait(TraitType type);
        void SetTrait(TraitType type, float value);
        event OnTraitChangedCallback OnTraitChanged;
    }
}
