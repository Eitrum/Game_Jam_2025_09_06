using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    public interface IVariant
    {
        int VariantCount { get; }
        void SetVariant();
        void SetVariant(int index);
    }
}
