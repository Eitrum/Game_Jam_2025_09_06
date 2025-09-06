using System;
using UnityEngine;

namespace Toolkit
{
    public class NullableBehaviour : MonoBehaviour, INullable
    {
        bool INullable.IsNull => this == null;
    }
}
