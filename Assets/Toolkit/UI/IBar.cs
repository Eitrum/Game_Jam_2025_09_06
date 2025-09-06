using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{
    public interface IBar
    {
        MinMax Range { get; set; }
        float Value { get; set; }
        float NormalizedValue { get; set; }
        bool Enabled { get; set; }

        Transform Parent { get; }
        event OnBarChangedCallback OnBarChanged;

        void SetValue(float value);
        void SetValueWithoutNotify(float value);
    }

    public delegate void OnBarChangedCallback(float oldValue, float newValue);
}
