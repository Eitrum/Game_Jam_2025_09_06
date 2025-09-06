using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Threading
{
    public enum ThreadType
    {
        None = 0,
        Default = 1,
        [InspectorName("")] _Unique = 2,
        Characters,
        Optimization,

    }
}
