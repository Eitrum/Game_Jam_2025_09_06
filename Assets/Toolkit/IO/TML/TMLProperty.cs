using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.IO.TML.Properties.Legacy {
    public class TMLProperty {
        private string name;
        private string rawValue;

        public string Name => name;
        public string FormatProperty => $"{name}=\"{rawValue}\"";
        public string RawValue {
            get => rawValue;
            set => rawValue = value;
        }

        public override string ToString() {
            return FormatProperty;
        }

        #region Constructor

        public TMLProperty() { }
        public TMLProperty(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw new System.Exception("An XML property name can not be empty or null");
            }
            if (name.Contains(" ")) {
                throw new System.Exception($"An XML property name can not contain any spaces: " + name);
            }
            this.name = name;
        }
        public TMLProperty(string name, string value) : this(name) => this.rawValue = value;
        public TMLProperty(string name, byte value) : this(name) => Byte = value;
        public TMLProperty(string name, sbyte value) : this(name) => SByte = value;
        public TMLProperty(string name, short value) : this(name) => Short = value;
        public TMLProperty(string name, ushort value) : this(name) => UShort = value;
        public TMLProperty(string name, int value) : this(name) => Int = value;
        public TMLProperty(string name, uint value) : this(name) => UInt = value;
        public TMLProperty(string name, long value) : this(name) => Long = value;
        public TMLProperty(string name, ulong value) : this(name) => ULong = value;
        public TMLProperty(string name, float value) : this(name) => Float = value;
        public TMLProperty(string name, double value) : this(name) => Double = value;
        public TMLProperty(string name, decimal value) : this(name) => Decimal = value;
        public TMLProperty(string name, bool value) : this(name) => Bool = value;
        public TMLProperty(string name, TKDateTime value) : this(name) => DateTime = value;
        public TMLProperty(string name, Vector2 value) : this(name) => Vector2 = value;
        public TMLProperty(string name, Vector3 value) : this(name) => Vector3 = value;
        public TMLProperty(string name, Vector4 value) : this(name) => Vector4 = value;
        public TMLProperty(string name, Quaternion value) : this(name) => Quaternion = value;
        public TMLProperty(string name, Color value) : this(name) => Color = value;
        public TMLProperty(string name, Color32 value) : this(name) => Color32 = value;
        public TMLProperty(string name, System.Enum enu) : this(name) => SetEnum(enu);
        public TMLProperty(string name, Stat stat) : this(name) => Stat = stat;

        #endregion

        #region Conversion

        public byte Byte {
            get {
                if (byte.TryParse(rawValue, out byte res))
                    return res;
                Debug.LogError($"Could not parse byte value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public sbyte SByte {
            get {
                if (sbyte.TryParse(rawValue, out sbyte res))
                    return res;
                Debug.LogError($"Could not parse byte value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public short Short {
            get {
                if (short.TryParse(rawValue, out short res))
                    return res;
                Debug.LogError($"Could not parse Short value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public ushort UShort {
            get {
                if (ushort.TryParse(rawValue, out ushort res))
                    return res;
                Debug.LogError($"Could not parse ushort value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public float Float {
            get {
                if (float.TryParse(rawValue, out float res))
                    return res;
                Debug.LogError($"Could not parse float value with '{rawValue}'");
                return 0f;
            }
            set => rawValue = value.ToString();
        }

        public int Int {
            get {
                if (int.TryParse(rawValue, out int res))
                    return res;
                Debug.LogError($"Could not parse int value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public uint UInt {
            get {
                if (uint.TryParse(rawValue, out uint res))
                    return res;
                Debug.LogError($"Could not parse uint value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public long Long {
            get {
                if (long.TryParse(rawValue, out long res))
                    return res;
                Debug.LogError($"Could not parse long value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public ulong ULong {
            get {
                if (ulong.TryParse(rawValue, out ulong res))
                    return res;
                Debug.LogError($"Could not parse ulong value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public double Double {
            get {
                if (double.TryParse(rawValue, out double res))
                    return res;
                Debug.LogError($"Could not parse double value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public decimal Decimal {
            get {
                if (decimal.TryParse(rawValue, out decimal res))
                    return res;
                Debug.LogError($"Could not parse double value with '{rawValue}'");
                return 0;
            }
            set => rawValue = value.ToString();
        }

        public bool Bool {
            get {
                if (bool.TryParse(rawValue, out bool res))
                    return res;
                Debug.LogError($"Could not parse bool value with '{rawValue}'");
                return false;
            }
            set => rawValue = value.ToString();
        }

        public TKDateTime DateTime {
            get {
                if (long.TryParse(rawValue, out long res))
                    return new TKDateTime(res);
                Debug.LogError($"Could not parse DateTime value with '{rawValue}'");
                return new TKDateTime(long.MinValue);
            }
            set => rawValue = value.ToString();
        }

        public Vector2 Vector2 {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 2 &&
                    float.TryParse(split[0].Trim(), out float x) &&
                    float.TryParse(split[1].Trim(), out float y))
                    return new Vector2(x, y);
                Debug.LogError($"Could not parse Vector2 value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.x};{value.y}";
        }

        public Vector3 Vector3 {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 3 &&
                    float.TryParse(split[0].Trim(), out float x) &&
                    float.TryParse(split[1].Trim(), out float y) &&
                    float.TryParse(split[2].Trim(), out float z))
                    return new Vector3(x, y, z);
                Debug.LogError($"Could not parse Vector3 value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.x};{value.y};{value.z}";
        }

        public Vector4 Vector4 {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 4 &&
                    float.TryParse(split[0].Trim(), out float x) &&
                    float.TryParse(split[1].Trim(), out float y) &&
                    float.TryParse(split[2].Trim(), out float z) &&
                    float.TryParse(split[2].Trim(), out float w))
                    return new Vector4(x, y, z, w);
                Debug.LogError($"Could not parse Vector4 value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.x};{value.y};{value.z};{value.w}";
        }

        public Quaternion Quaternion {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 4 &&
                    float.TryParse(split[0].Trim(), out float x) &&
                    float.TryParse(split[1].Trim(), out float y) &&
                    float.TryParse(split[2].Trim(), out float z) &&
                    float.TryParse(split[2].Trim(), out float w))
                    return new Quaternion(x, y, z, w);
                Debug.LogError($"Could not parse Quaternion value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.x};{value.y};{value.z};{value.w}";
        }

        public Color Color {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 4 &&
                    float.TryParse(split[0].Trim(), out float r) &&
                    float.TryParse(split[1].Trim(), out float g) &&
                    float.TryParse(split[2].Trim(), out float b) &&
                    float.TryParse(split[2].Trim(), out float a))
                    return new Color(r, g, b, a);
                Debug.LogError($"Could not parse Color value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.r};{value.g};{value.b};{value.a}";
        }

        public Color32 Color32 {
            get {
                var split = rawValue.Split(';');
                if (split.Length == 4 &&
                    byte.TryParse(split[0].Trim(), out byte r) &&
                    byte.TryParse(split[1].Trim(), out byte g) &&
                    byte.TryParse(split[2].Trim(), out byte b) &&
                    byte.TryParse(split[2].Trim(), out byte a))
                    return new Color32(r, g, b, a);
                Debug.LogError($"Could not parse Color32 value with '{rawValue}'");
                return default;
            }
            set => rawValue = $"{value.r};{value.g};{value.b};{value.a}";
        }

        public T GetEnum<T>(T defaultValue = default) where T : struct, System.Enum => rawValue.ToEnum<T>(out T res) ? res : defaultValue;
        public T GetEnum<T>() where T : struct, System.Enum => rawValue.ToEnum<T>();
        public void SetEnum<T>(T enu) where T : System.Enum => rawValue = enu.ToString();

        public Stat Stat {
            get => Stat.Parse(rawValue);
            set => rawValue = value.ToString();
        }

        #endregion
    }
}
