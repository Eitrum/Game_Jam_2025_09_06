using System;

namespace Toolkit {
    [Flags]
    public enum UpdateModeMask {
        None = 0,
        EarlyUpdate = 1,
        Update = 2,
        LateUpdate = 4,
        PostUpdate = 8,
        FixedUpdate = 16,
        OnBeforeRender = 32,
    }

    public enum UpdateMode
    {
        None = 0,
        EarlyUpdate = 1,
        Update = 2,
        LateUpdate = 4,
        PostUpdate = 8,
        FixedUpdate = 16,
        OnBeforeRender = 32,
    }
}
