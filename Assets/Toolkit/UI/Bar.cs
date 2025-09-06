using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{
    public abstract class Bar : MonoBehaviour, IBar
    {
        public abstract MinMax Range { get; set; }
        public abstract float Value { get; set; }
        public abstract float NormalizedValue { get; set; }
        public abstract bool Enabled { get; set; }
        public abstract Transform Parent { get; }
        public abstract event OnBarChangedCallback OnBarChanged;
        public abstract void SetValue(float value);
        public abstract void SetValueWithoutNotify(float value);
    }
}
