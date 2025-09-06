using System;

namespace Toolkit.IO {
    public class InsertScope : IDisposable {

        private IBuffer buffer;
        private byte[] temporaryBytes;

        public InsertScope(IBuffer buffer, int index) {
            if(index > buffer.Index)
                throw new Exception();
            this.buffer = buffer;
            var bytesToCache = buffer.Index - index;
            temporaryBytes = new byte[bytesToCache];
            buffer.CopyTo(temporaryBytes, index, bytesToCache);
            buffer.Index = index;
        }

        public void Dispose() {
            if(buffer.TryReserveAsSpan(temporaryBytes.Length, out var span))
                temporaryBytes.CopyTo(span);
            else
                throw new Exception();
        }
    }
}
