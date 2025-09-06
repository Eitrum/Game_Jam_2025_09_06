using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public interface IItemData {
        Item Parent { get; set; }
        void Serialize(Toolkit.IO.TMLNode node);
        void Deserialize(Toolkit.IO.TMLNode node);
    }
}
