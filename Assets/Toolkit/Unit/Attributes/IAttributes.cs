using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IAttributes {
        event OnAttributeChangedDelegate OnAttributeChanged;
        Stat GetAttribute(AttributeType type);
        void SetAttribute(AttributeType type, Stat value);
    }

    public delegate void OnAttributeChangedDelegate(AttributeType type);
}
