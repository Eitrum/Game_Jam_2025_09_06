using System;

namespace Toolkit.IO {
    public class SizeBlockScope : IDisposable {

        private IBuffer buffer;
        private int index = 0;

        public SizeBlockScope(IBuffer buffer) {
            this.buffer = buffer;
            this.index = buffer.Index;
            buffer.TryReserve(4);
        }

        public unsafe void Dispose() {
            var delta = buffer.Index - index;
            if(buffer.TryGetPointerAt(index, out var ptr)) {
                BitConverter.GetBytes(delta, ptr);
            }
            //UnityEngine.Debug.Log($"index:{index} ---- size:{delta}");
        }
    }
}
