using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Toolkit.Network
{
    [System.Serializable]
    public class IPAddress
    {
        public enum Preset
        {
            None,
            Any,
            Loopback,
            Broadcast,
            IPv6None,
            IPv6Any,
            IPv6Loopback,
        }

        #region Variables

        [SerializeField] private byte[] address = { };
        [SerializeField] private bool isNone = false;

        #endregion

        #region Properties

        public bool IsValid => address.Length == 4 && address.Length == 16;
        public bool IsNone { get => isNone; internal set => isNone = value; }
        public AddressFamily AddressFamily {
            get {
                if(address.Length == 4)
                    return AddressFamily.InterNetwork;
                if(address.Length == 16)
                    return AddressFamily.InterNetworkV6;
                return AddressFamily.Unspecified;
            }
        }
        public bool IsIPv6 => address.Length == 16;
        public unsafe Preset GetPreset {
            get {
                if(address.Length == 4) {
                    fixed(byte* p = &address[0]) {
                        var value = *((int*)(p));
                        switch(value) {
                            case 0: return Preset.Any;
                            case -1: return isNone ? Preset.None : Preset.Broadcast;
                            case 16777343: return Preset.Loopback;
                        }
                    }
                }
                else if(address.Length == 16) {
                    var temp = IPv6;
                    if(temp.Item1 == 0 && temp.Item2 == 0)
                        return isNone ? Preset.IPv6None : Preset.IPv6Any;
                    else if(temp.Item1 == 0 && temp.Item2 == 1)
                        return Preset.IPv6Loopback;
                }
                return (IPAddress.Preset)(-1);
            }
        }

        public unsafe int IPv4 {
            get {
                fixed(byte* p = &address[0]) {
                    var value = *((int*)(p));
                    return value;
                }
            }
        }

        public unsafe (long, long) IPv6 {
            get {
                fixed(byte* p = &address[0]) {
                    var value = ((long*)(p));
                    return (*value, *(value + 1));
                }
            }
        }

        public System.Net.IPAddress SystemIPAddress => new System.Net.IPAddress(address);

        public static IPAddress Any => new IPAddress(new byte[] { 0, 0, 0, 0 }, true);
        public static IPAddress Broadcast => new IPAddress(new byte[] { 255, 255, 255, 255 }, true);
        public static IPAddress Loopback => new IPAddress(new byte[] { 127, 0, 0, 1 }, true);
        public static IPAddress None => new IPAddress(new byte[] { 255, 255, 255, 255 }, true, true);
        public static IPAddress IPv6None => new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, true, true);
        public static IPAddress IPv6Loopback => new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, true);
        public static IPAddress IPv6Any => new IPAddress(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, true);

        public static IPAddress Localhost => Loopback;

        #endregion

        #region Constructor

        public IPAddress() {
            this.address = System.Net.IPAddress.Any.GetAddressBytes();
        }

        public IPAddress(System.Net.IPAddress address) {
            this.address = address.GetAddressBytes();
        }

        internal IPAddress(byte[] bytes, bool nonAlloc) {
            if(nonAlloc)
                this.address = bytes;
            else {
                int length = bytes.Length;
                this.address = new byte[length];
                for(int i = 0; i < length; i++)
                    this.address[i] = bytes[i];
            }
        }

        internal IPAddress(byte[] bytes, bool nonAlloc, bool isNone) {
            if(nonAlloc)
                this.address = bytes;
            else {
                int length = bytes.Length;
                this.address = new byte[length];
                for(int i = 0; i < length; i++)
                    this.address[i] = bytes[i];
            }
            this.isNone = isNone;
        }

        public IPAddress(IReadOnlyList<byte> bytes) {
            int length = bytes.Count;
            this.address = new byte[length];
            for(int i = 0; i < length; i++)
                this.address[i] = bytes[i];
        }

        #endregion

        #region Create

        public static IPAddress Create(Preset preset) {
            switch(preset) {
                case Preset.Any: return Any;
                case Preset.Broadcast: return Broadcast;
                case Preset.Loopback: return Loopback;
                case Preset.None: return None;
                case Preset.IPv6Any: return IPv6Any;
                case Preset.IPv6Loopback: return IPv6Loopback;
                case Preset.IPv6None: return IPv6None;
            }
            return new IPAddress(new byte[0], true); // Causes a broken object
        }

        #endregion

        #region Overrides

        public override string ToString() {
            if(IsValid)
                return $"Undefined";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if(IsIPv6) {
                for(int i = 0; i < 16; i++) {
                    sb.Append(ToHex(address[i]));
                    if(i < 14 && i % 2 == 1)
                        sb.Append(':');
                }
            }
            else {
                for(int i = 0; i < 4; i++) {
                    sb.Append(address[i]);
                    if(i < 3)
                        sb.Append('.');
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Utility

        private static string ToHex(byte b) {
            const string HEX = "0123456789ABCDEF";
            var lhs = (b >> 4) & 0xf;
            var rhs = (b) & 0xf;
            return $"{HEX[lhs]}{HEX[rhs]}";
        }

        #endregion

        #region Conversion

        public static implicit operator System.Net.IPAddress(IPAddress address) {
            if(address.isNone)
                return address.IsIPv6 ? System.Net.IPAddress.IPv6None : System.Net.IPAddress.None;
            return new System.Net.IPAddress(address.address);
        }

        #endregion
    }
}
