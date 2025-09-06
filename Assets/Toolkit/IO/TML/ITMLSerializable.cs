using UnityEngine;

namespace Toolkit.IO {
    public interface ITMLSerializable {
        void Serialize(TMLNode node);
        void Deserialize(TMLNode node);
    }
}
