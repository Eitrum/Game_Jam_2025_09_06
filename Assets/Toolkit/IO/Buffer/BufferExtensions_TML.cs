using System;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;
using BitConverter = Toolkit.IO.BitConverter;
using UnityEngine;
using System.Collections.Generic;

namespace Toolkit.IO {
    public static partial class BufferExtensions {

        public enum TMLBufferSerializationMode : byte {
            Binary = 0,
            TML = 1,
            Json = 2,
        }


        public static void Write(this IBuffer buffer, TMLNode node, TMLBufferSerializationMode mode = TMLBufferSerializationMode.Binary) {
            buffer.Write((byte)TMLBufferSerializationMode.TML);
            buffer.Write(TML.TMLParser.ToString(node, false)); // Implement binary and json
        }

        public static TMLNode ReadTMLNode(this IBuffer buffer) {
            var mode = (TMLBufferSerializationMode) buffer.ReadByte();
            var tml = buffer.ReadString();
            return TML.TMLParser.Parse(tml);
        }
    }
}
