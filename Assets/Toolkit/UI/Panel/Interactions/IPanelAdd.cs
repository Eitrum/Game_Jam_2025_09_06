using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem {
    public interface IPanelAdd {
        event System.Action<Panel> OnPanelOpen;
        Promise<Panel> Open();
    }
}
