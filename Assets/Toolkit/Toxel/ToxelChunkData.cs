using System;
using UnityEngine;

namespace Toolkit.Toxel {
    public class ToxelChunkData : ScriptableObject {
        [SerializeField] private CubicArray<byte> data;
    }
}
