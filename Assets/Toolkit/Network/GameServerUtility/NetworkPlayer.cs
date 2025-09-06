using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Network {
    [System.Serializable]
    public class NetworkPlayer : IBufferSerializable {
        #region Variables

        private static byte[] cacheCopyBytes = new byte[16];

        public readonly byte[] Address;
        public readonly int Port;
        public readonly int Id;
        // SessionToken?

        public string DisplayName { get; private set; } = string.Empty;
        public readonly HashSet<int> OwnedObjects = new HashSet<int>();


        public readonly static NetworkPlayer Unknown = new NetworkPlayer(0, 0, 0) { DisplayName = "Unknown" };
        public readonly static NetworkPlayer Server = new NetworkPlayer(0, 0, 0) { DisplayName = "Server" };

        #endregion

        #region Properties

        public bool HasAddress => Address != null && Address.Length > 0 && Port != 0;

        #endregion

        #region Constructor

        public NetworkPlayer() { }

        public NetworkPlayer(int id) {
            this.Id = id;
        }

        public NetworkPlayer(int id, string displayName) {
            this.Id = id;
            this.DisplayName = displayName;
        }

        public NetworkPlayer(System.Net.IPEndPoint endPoint, int id) {
            lock(cacheCopyBytes) {
                endPoint.Address.TryWriteBytes(cacheCopyBytes, out int written);
                Address = new byte[written];
                for(int i = 0; i < written; i++)
                    Address[i] = cacheCopyBytes[i];
            }
            Port = endPoint.Port;
            Id = id;
        }

        public NetworkPlayer(uint address, int port, int id) {
            Address = Toolkit.IO.BitConverter.GetBytes(address);
            Port = port;
            Id = id;
        }

        public NetworkPlayer(byte i0, byte i1, byte i2, byte i3, ushort port, int id) {
            Address = new byte[4] { i0, i1, i2, i3 };
            Port = port;
            Id = id;
        }

        public NetworkPlayer(IBuffer buffer) {
            Deserialize(buffer);
        }

        #endregion

        #region Comparison Checks

        public bool IsEndPoint(System.Net.IPEndPoint endPoint) {
            if(endPoint.Port != Port)
                return false;
            lock(cacheCopyBytes) {
                if(!endPoint.Address.TryWriteBytes(cacheCopyBytes, out int written)) {
                    Debug.LogError(this.FormatLog("Failed to read address"));
                    return false;
                }
                if(written != Address.Length)
                    return false;
                // Not 100% safe, but should work in most cases.
                for(int i = 0; i < written; i++)
                    if(cacheCopyBytes[i] != Address[i])
                        return false;
                return true;
            }
        }

        #endregion

        #region Overrides

        public override string ToString() {
            return $"{Address.Join('.')}:{Port} [{Id}] - {DisplayName}";
        }

        #endregion

        #region IBufferSerializable Impl

        public void Serialize(IBuffer buffer)
            => Serialize(buffer, false);

        public void Serialize(IBuffer buffer, bool includeEndPoint) {
            buffer.Write(includeEndPoint);
            if(includeEndPoint) {
                buffer.Write(Address);
                buffer.Write(Port);
            }
            buffer.Write(Id);
            buffer.Write(DisplayName);
        }

        public void Deserialize(IBuffer buffer) {
            var includeEndPoint = buffer.ReadBoolean();
            if(includeEndPoint) {
                DeserializeCache.Address.Set(this, buffer.ReadByteArray());
                DeserializeCache.Port.Set(this, buffer.ReadInt());
            }
            DeserializeCache.Id.Set(this, buffer.ReadInt());
            DisplayName = buffer.ReadString();
        }

        private static class DeserializeCache {
            public static readonly IO.TReflection.TReflectionVariable<byte[]> Address;
            public static readonly IO.TReflection.TReflectionVariable<int> Port;
            public static readonly IO.TReflection.TReflectionVariable<int> Id;
            static DeserializeCache() {
                var serializer = IO.TReflection.ClassSerializer.Get<NetworkPlayer>();
                Address = serializer.Get<byte[]>(nameof(NetworkPlayer.Address));
                Port = serializer.Get<int>(nameof(NetworkPlayer.Port));
                Id = serializer.Get<int>(nameof(NetworkPlayer.Id));
            }
        }

        #endregion
    }
}
