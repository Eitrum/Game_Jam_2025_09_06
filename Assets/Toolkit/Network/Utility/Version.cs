using System;
using System.Text.RegularExpressions;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Network {
    public interface IReadOnlyVersion {
        public uint Major { get; }
        public uint Minor { get; }
        public uint Patch { get; }

        public string Text { get; }
    }

    [System.Serializable]
    public class Version : IReadOnlyVersion {
        #region Variables

        private const string TAG = "<color=orange>[Version]</color> - ";

        [SerializeField] private uint major = 0;
        [SerializeField] private uint minor = 0;
        [SerializeField] private uint patch = 1;

        #endregion

        #region Properties

        public uint Major => major;
        public uint Minor => minor;
        public uint Patch => patch;

        public string Text => $"v{major}.{minor}.{patch}";
        public bool IsDate => major > 2020 && minor < 13 && minor > 0;

        #endregion

        #region Constructor

        public Version() { }

        public Version(string version) {
            if(!TryParseReference(version, this)) {
                throw new FormatException(TAG + $"Unable to parse '{version}' in constructor!");
            }
        }

        public Version(byte major, byte minor, byte patch) {
            this.major = (uint)major;
            this.minor = (uint)minor;
            this.patch = (uint)patch;
        }

        public Version(ushort major, ushort minor, ushort patch) {
            this.major = (uint)major;
            this.minor = (uint)minor;
            this.patch = (uint)patch;
        }

        public Version(int major, int minor, int patch) {
            this.major = (uint)major;
            this.minor = (uint)minor;
            this.patch = (uint)patch;
        }

        public Version(uint major, uint minor, uint patch) {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public Version(Toolkit.IO.IBuffer buffer) {
            Deserialize(buffer);
        }

        #endregion

        #region Create

        public static Version CreateFromYear()
            => CreateFromYear(1);
        public static Version CreateFromYear(int patch) {
            var date = TKDateTime.UtcNow;
            return new Version(date.Year, date.Month, patch);
        }

        #endregion

        #region Increment

        public void Increment() => patch++;

        #endregion

        #region Parse

        public static Version Parse(string input) {
            if(TryParse(input, out Version v))
                return v;
            throw new Exception(TAG + $"Unable to parse version from '{input}'");
        }

        public static bool TryParse(string input, out Version version) {
            if(!input.StartsWith("v")) {
                version = null;
                return false;
            }
            Regex reg = new Regex(@"\d+");
            var col = reg.Matches(input);
            if(col.Count != 3) {
                version = null;
                return false;
            }

            if(!uint.TryParse(col[0].Value, out uint major) || !uint.TryParse(col[1].Value, out uint minor) || !uint.TryParse(col[2].Value, out uint patch)) {
                version = null;
                return false;
            }

            version = new Version(major, minor, patch);
            return true;
        }

        public static bool TryParseReference(string input, Version version) {
            if(!input.StartsWith("v")) {
                return false;
            }
            Regex reg = new Regex(@"\d+");
            var col = reg.Matches(input);
            if(col.Count != 3) {
                return false;
            }

            if(!uint.TryParse(col[0].Value, out uint major) || !uint.TryParse(col[1].Value, out uint minor) || !uint.TryParse(col[2].Value, out uint patch)) {
                return false;
            }

            version.major = major;
            version.minor = minor;
            version.patch = patch;
            return true;
        }

        #endregion

        #region Overrides

        public override string ToString() => Text;

        public override int GetHashCode() {
            return (int)((major << 16) ^ (minor << 8) ^ patch);
        }

        public override bool Equals(object obj) {
            if(obj is Version ov) {
                return (ov.patch == patch && major == ov.major && minor == ov.minor);
            }
            else if(obj is string s && TryParse(s, out Version v)) {
                return Equals(v);
            }
            return false;
        }

        #endregion

        #region Serialization

        public void Serialize(IBuffer buffer) {
            var majormode = major > ushort.MaxValue ? B.ByteMask.OOIO_OOOO : (major > byte.MaxValue ? B.ByteMask.OOOI_OOOO : (byte)0);
            var minormode = minor > ushort.MaxValue ? B.ByteMask.OOOO_IOOO : (minor > byte.MaxValue ? B.ByteMask.OOOO_OIOO : (byte)0);
            var patchmode = patch > ushort.MaxValue ? B.ByteMask.OOOO_OOIO : (patch > byte.MaxValue ? B.ByteMask.OOOO_OOOI : (byte)0);

            buffer.Write((byte)(majormode | minormode | patchmode));

            switch(majormode) {
                case 0:
                    buffer.Write((byte)major);
                    break;
                case B.ByteMask.OOIO_OOOO:
                    buffer.Write(major);
                    break;
                case B.ByteMask.OOOI_OOOO:
                    buffer.Write((ushort)major);
                    break;
            }
            switch(minormode) {
                case 0:
                    buffer.Write((byte)minor);
                    break;
                case B.ByteMask.OOOO_IOOO:
                    buffer.Write(minor);
                    break;
                case B.ByteMask.OOOO_OIOO:
                    buffer.Write((ushort)minor);
                    break;
            }
            switch(patchmode) {
                case 0:
                    buffer.Write((byte)patch);
                    break;
                case B.ByteMask.OOOO_OOIO:
                    buffer.Write(patch);
                    break;
                case B.ByteMask.OOOO_OOOI:
                    buffer.Write((ushort)patch);
                    break;
            }
        }

        public void Deserialize(Toolkit.IO.IBuffer buffer) {
            try {
                var mode = buffer.ReadByte();
                var majormode = (byte)(mode & B.OOII.OOOO);
                var minormode = (byte)(mode & B.IIOO);
                var patchmode = (byte)(mode & B.OOII);

                switch(majormode) {
                    case 0:
                        major = buffer.ReadByte();
                        break;
                    case B.ByteMask.OOIO_OOOO:
                        major = buffer.ReadUInt();
                        break;
                    case B.ByteMask.OOOI_OOOO:
                        major = buffer.ReadUShort();
                        break;
                    default:
                        Debug.LogWarning(TAG + "Unable to deserialize major");
                        break;
                }
                switch(minormode) {
                    case 0:
                        minor = buffer.ReadByte();
                        break;
                    case B.ByteMask.OOOO_IOOO:
                        minor = buffer.ReadUInt();
                        break;
                    case B.ByteMask.OOOO_OIOO:
                        minor = buffer.ReadUShort();
                        break;
                    default:
                        Debug.LogWarning(TAG + "Unable to deserialize minor");
                        break;
                }
                switch(patchmode) {
                    case 0:
                        patch = buffer.ReadByte();
                        break;
                    case B.ByteMask.OOOO_OOIO:
                        patch = buffer.ReadUInt();
                        break;
                    case B.ByteMask.OOOO_OOOI:
                        patch = buffer.ReadUShort();
                        break;
                    default:
                        Debug.LogWarning(TAG + "Unable to deserialize patch");
                        break;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Operators

        private static bool IsOneNull(Version lhs, Version rhs) => lhs is null || rhs is null;
        private static bool IsBothNull(Version lhs, Version rhs) => lhs is null && rhs is null;

        public static bool operator ==(Version lhs, Version rhs) {
            if(IsBothNull(lhs, rhs)) return true;
            if(IsOneNull(lhs, rhs)) return false;
            return (lhs.patch == rhs.patch && lhs.minor == rhs.minor && lhs.major == rhs.major);
        }

        public static bool operator !=(Version lhs, Version rhs) {
            if(IsBothNull(lhs, rhs)) return false;
            if(IsOneNull(lhs, rhs)) return true;
            return (lhs.patch != rhs.patch || lhs.minor != rhs.minor || lhs.major != rhs.major);
        }

        public static bool operator ==(Version lhs, string rhs) {
            if(TryParse(rhs, out Version vRhs))
                return lhs == vRhs;
            return false;
        }

        public static bool operator !=(Version lhs, string rhs) {
            if(TryParse(rhs, out Version vRhs))
                return lhs != vRhs;
            return true;
        }


        public static bool operator ==(string lhs, Version rhs) {
            if(TryParse(lhs, out Version vLhs))
                return vLhs == rhs;
            return false;
        }

        public static bool operator !=(string lhs, Version rhs) {
            if(TryParse(lhs, out Version vLhs))
                return vLhs != rhs;
            return true;
        }

        public static implicit operator string(Version v) => v.Text;

        #endregion
    }
}
