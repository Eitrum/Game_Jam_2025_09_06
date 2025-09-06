using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Threading
{
    public interface IMainThreadCallback
    {
        void Handle();
    }
}
