using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IPerk {
        IPerkMetadata Metadata { get; }
        IPerkEffect Effect { get; }
    }
}
