using System;
using UnityEngine;

namespace Toolkit.IO {
    public interface IBufferSerializable {
        void Serialize(IBuffer buffer);
        void Deserialize(IBuffer buffer);
    }
}
