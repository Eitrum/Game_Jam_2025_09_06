using System;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.IO.TML;
using Toolkit.IO.TML.Properties;
using System.Linq;

namespace Toolkit.IO {
    /// <summary>
    /// 
    /// Note: Disposing of TMLNode causes recursive dispose for all children and pools it for future uses.
    /// </summary>
    public sealed class TMLNode : ITMLNode, IDisposable {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLNode]</color> - ";

        private string name;

        private List<ITMLProperty> properties;
        private List<TMLNode> children;

        #endregion

        #region Properties

        public string Name => name;

        public uint Name_32 => TMLUtility.GetHash32(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public byte Name_8 => TMLUtility.GetHash8(name);

        public IReadOnlyList<ITMLProperty> Properties => properties;
        public IReadOnlyList<TMLNode> Children => children;
        public IReadOnlyList<TMLNode> Nodes => children;

        // Child Nodes
        public bool HasChildren => children.Count > 0;
        public bool HasNodes => children.Count > 0;

        // Properties
        public bool HasProperties => properties.Count > 0;

        #endregion

        #region Constructor

        public TMLNode() {
            name = string.Empty;
            properties = new List<ITMLProperty>();
            children = new List<TMLNode>();
        }

        public TMLNode(string name) {
            this.name = name;
            this.properties = new List<ITMLProperty>();
            this.children = new List<TMLNode>();
        }

        public TMLNode(string name, int properties, int children) {
            this.name = name;
            this.properties = new List<ITMLProperty>(properties);
            this.children = new List<TMLNode>(children);
        }

        public TMLNode(string name, IEnumerable<ITMLProperty> properties, IEnumerable<TMLNode> nodes) {
            this.name = name;
            this.properties = new List<ITMLProperty>(properties);
            this.children = new List<TMLNode>(nodes);
        }

        #endregion

        #region Pooling / Disposable

        public static TMLNode CreateNode() {
            return FastPool<TMLNode>.Global.Pop();
        }

        public static TMLNode CreateNode(string name) {
            var n = FastPool<TMLNode>.Global.Pop();
            n.name = name;
            return n;
        }

        public static TMLNode CreateNode(string name, int properties, int children) {
            if(FastPool<TMLNode>.Global.HasPooledObjects) {
                var n = FastPool<TMLNode>.Global.Pop();
                n.name = name;
                if(properties > n.properties.Capacity)
                    n.properties.Capacity = properties;
                if(children > n.children.Capacity)
                    n.children.Capacity = children;
                return n;
            }
            else {
                return new TMLNode(name, properties, children);
            }
        }

        public static TMLNode CreateNode(string name, IEnumerable<ITMLProperty> properties, IEnumerable<TMLNode> children) {
            var n = FastPool<TMLNode>.Global.Pop();
            n.name = name;
            n.properties.AddRange(properties);
            n.children.AddRange(children);
            return n;
        }

        public void Dispose() {
            name = null;
            foreach(var c in children)
                c?.Dispose();
            properties.Clear();
            children.Clear();
            FastPool<TMLNode>.Global.Push(this);
        }

        #endregion

        #region Methods

        public void SetName(string newName) {
            name = newName;
        }

        #endregion

        #region Add Children

        public TMLNode AddNode(string name) {
            var n = CreateNode(name);
            children.Add(n);
            return n;
        }

        public TMLNode AddChild(string name) {
            var n = CreateNode(name);
            children.Add(n);
            return n;
        }

        public void AddNode(TMLNode node) {
            children.Add(node);
        }

        public TMLNode AddNode(string name, int properties, int children) {
            var n = CreateNode(name, properties, children);
            this.children.Add(n);
            return n;
        }

        public void AddNode(string name, out TMLNode newNode) {
            newNode = CreateNode(name);
            children.Add(newNode);
        }

        public void AddNode(string name, out TMLNode newNode, int properties, int children) {
            newNode = CreateNode(name, properties, children);
            this.children.Add(newNode);
        }

        public bool RemoveNode(string name) {
            for(int i = children.Count - 1; i >= 0; i--) {
                if(children[i].IsName(name)) {
                    children.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Remove

        public void RemoveChild(string name) {
            for(int i = children.Count - 1; i >= 0; i--) {
                if(children[i].name.Equals(name))
                    children.RemoveAt(i);
            }
        }

        public void RemoveChild(TMLNode node) {
            for(int i = children.Count - 1; i >= 0; i--) {
                if(children[i] == node)
                    children.RemoveAt(i);
            }
        }

        #endregion

        #region Find Children

        public TMLNode GetNode(string name) {
            for(int i = children.Count - 1; i >= 0; i--) {
                if(children[i].IsName(name))
                    return children[i];
            }
            return null;
        }

        public bool TryGetNode(string name, out TMLNode node) {
            for(int i = children.Count - 1; i >= 0; i--) {
                if(children[i].IsName(name)) {
                    node = children[i];
                    return true;
                }
            }
            node = null;
            return false;
        }

        #endregion

        #region Generic Property

        public ITMLProperty AddProperty(ITMLProperty property) => AddProperty_Internal(property);

        public ITMLProperty GetProperty(string name) => TryGetProperty(name, out ITMLProperty prop) ? prop : null;

        public bool TryGetProperty(string name, out ITMLProperty property) {
            for(int i = properties.Count - 1; i >= 0; i--) {
                if(properties[i].IsName(name)) {
                    property = properties[i];
                    return true;
                }
            }

            property = default;
            return false;
        }

        public T GetProperty<T>(string name) where T : ITMLProperty => TryGetProperty<T>(name, out T property) ? property : default;

        public bool TryGetProperty<T>(string name, out T property) where T : ITMLProperty {
            for(int i = properties.Count - 1; i >= 0; i--) {
                if(properties[i].IsName(name) && properties[i] is T tval) {
                    property = tval;
                    return true;
                }
            }

            property = default;
            return false;
        }

        public bool RemoveProperty(string name) {
            for(int i = properties.Count - 1; i >= 0; i--) {
                if(properties[i].IsName(name)) {
                    properties.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveProperty(ITMLProperty property) {
            for(int i = properties.Count - 1; i >= 0; i--) {
                if(properties[i] == property) {
                    properties.RemoveAt(i); return true;
                }

            }
            return false;
        }

        #endregion

        #region Enum

        public ITMLProperty_Int AddProperty<T>(string name, T value) where T : System.Enum => AddProperty_Internal(new TMLProperty_Int(name, value.ToInt()));
        public TMLProperty_Int AddProperty<T>(string name, T value, T defaultValue) where T : System.Enum => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Int_Array AddProperty<T>(string name, IReadOnlyList<T> values) where T : System.Enum => AddProperty_Internal(new TMLProperty_Int_Array(name, values.Select(x => x.ToInt())));
        public TMLProperty_Int_Array AddProperty<T>(string name, IEnumerable<T> values) where T : System.Enum => AddProperty_Internal(new TMLProperty_Int_Array(name, values.Select(x => x.ToInt())));

        public T GetEnum<T>(string name) where T : System.Enum => TryGetProperty(name, out ITMLProperty_Int prop) ? prop.Int.ToEnum<T>() : default;
        public T GetEnum<T>(string name, T defaultValue) where T : System.Enum => TryGetProperty(name, out ITMLProperty_Int prop) ? prop.Int.ToEnum<T>() : defaultValue;
        public bool TryGetEnum<T>(string name, out T value) where T : System.Enum {
            if(!TryGetProperty(name, out ITMLProperty_Int prop)) {
                value = default;
                return false;
            }
            value = prop.Int.ToEnum<T>();
            return true;
        }

        public IReadOnlyList<T> GetEnums<T>(string name) where T : System.Enum => TryGetProperty(name, out ITMLProperty_Int_Array prop) ? prop.Ints.Select(x => x.ToEnum<T>()).ToArray() : null;
        public bool TryGetEnums<T>(string name, out IReadOnlyList<T> value) where T : System.Enum {
            if(!TryGetProperty(name, out ITMLProperty_Int_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Ints.Select(x => x.ToEnum<T>()).ToArray();
            return true;
        }
        #endregion

        #region Built-in

        // Wrapper to enable 1-liner Add methods.
        private T AddProperty_Internal<T>(T prop) where T : ITMLProperty {
            this.properties.Add(prop);
            return prop;
        }

        // Boolean
        public ITMLProperty_Boolean AddProperty(string name, bool value) => AddProperty_Internal(TMLProperty_Boolean.Create(name, value));
        public TMLProperty_Boolean AddProperty(string name, bool value, bool defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Boolean_Array AddProperty(string name, IReadOnlyList<bool> values) => AddProperty_Internal(new TMLProperty_Boolean_Array(name, values));
        public TMLProperty_Boolean_Array AddProperty(string name, IEnumerable<bool> values) => AddProperty_Internal(new TMLProperty_Boolean_Array(name, values));

        public bool GetBoolean(string name) => TryGetProperty(name, out ITMLProperty_Boolean prop) ? prop.Boolean : false;
        public bool GetBoolean(string name, bool defaultValue) => TryGetProperty(name, out ITMLProperty_Boolean prop) ? prop.Boolean : defaultValue;
        public bool TryGetBoolean(string name, out bool value) {
            if(!TryGetProperty(name, out ITMLProperty_Boolean prop)) {
                value = default;
                return false;
            }
            value = prop.Boolean;
            return true;
        }

        public IReadOnlyList<bool> GetBooleans(string name) => TryGetProperty(name, out ITMLProperty_Boolean_Array prop) ? prop.Booleans : null;
        public bool TryGetBooleans(string name, out IReadOnlyList<bool> value) {
            if(!TryGetProperty(name, out ITMLProperty_Boolean_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Booleans;
            return true;
        }

        // Char
        public TMLProperty_Char AddProperty(string name, char value) => AddProperty_Internal(new TMLProperty_Char(name, value));
        public TMLProperty_Char AddProperty(string name, char value, char defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Char_Array AddProperty(string name, IReadOnlyList<char> values) => AddProperty_Internal(new TMLProperty_Char_Array(name, values));
        public TMLProperty_Char_Array AddProperty(string name, IEnumerable<char> values) => AddProperty_Internal(new TMLProperty_Char_Array(name, values));

        public char GetChar(string name) => TryGetProperty(name, out ITMLProperty_Char prop) ? prop.Char : default;
        public char GetChar(string name, char defaultValue) => TryGetProperty(name, out ITMLProperty_Char prop) ? prop.Char : defaultValue;
        public bool TryGetChar(string name, out char value) {
            if(!TryGetProperty(name, out ITMLProperty_Char prop)) {
                value = default;
                return false;
            }
            value = prop.Char;
            return true;
        }

        public IReadOnlyList<char> GetChars(string name) => TryGetProperty(name, out ITMLProperty_Char_Array prop) ? prop.Chars : null;
        public bool TryGetChars(string name, out IReadOnlyList<char> value) {
            if(!TryGetProperty(name, out ITMLProperty_Char_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Chars;
            return true;
        }

        // sbyte
        public TMLProperty_SByte AddProperty(string name, sbyte value) => AddProperty_Internal(new TMLProperty_SByte(name, value));
        public TMLProperty_SByte AddProperty(string name, sbyte value, sbyte defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_SByte_Array AddProperty(string name, IReadOnlyList<sbyte> values) => AddProperty_Internal(new TMLProperty_SByte_Array(name, values));
        public TMLProperty_SByte_Array AddProperty(string name, IEnumerable<sbyte> values) => AddProperty_Internal(new TMLProperty_SByte_Array(name, values));

        public sbyte GetSByte(string name) => TryGetProperty(name, out ITMLProperty_SByte prop) ? prop.SByte : default;
        public sbyte GetSByte(string name, sbyte defaultValue) => TryGetProperty(name, out ITMLProperty_SByte prop) ? prop.SByte : defaultValue;
        public bool TryGetSByte(string name, out sbyte value) {
            if(!TryGetProperty(name, out ITMLProperty_SByte prop)) {
                value = default;
                return false;
            }
            value = prop.SByte;
            return true;
        }

        public IReadOnlyList<sbyte> GetSBytes(string name) => TryGetProperty(name, out ITMLProperty_SByte_Array prop) ? prop.SBytes : null;
        public bool TryGetSBytes(string name, out IReadOnlyList<sbyte> value) {
            if(!TryGetProperty(name, out ITMLProperty_SByte_Array prop)) {
                value = default;
                return false;
            }
            value = prop.SBytes;
            return true;
        }

        // byte
        public TMLProperty_Byte AddProperty(string name, byte value) => AddProperty_Internal(new TMLProperty_Byte(name, value));
        public TMLProperty_Byte AddProperty(string name, byte value, byte defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Byte_Array AddProperty(string name, IReadOnlyList<byte> values) => AddProperty_Internal(new TMLProperty_Byte_Array(name, values));
        public TMLProperty_Byte_Array AddProperty(string name, IEnumerable<byte> values) => AddProperty_Internal(new TMLProperty_Byte_Array(name, values));

        public byte GetByte(string name) => TryGetProperty(name, out ITMLProperty_Byte prop) ? prop.Byte : default;
        public byte GetByte(string name, byte defaultValue) => TryGetProperty(name, out ITMLProperty_Byte prop) ? prop.Byte : defaultValue;
        public bool TryGetByte(string name, out byte value) {
            if(!TryGetProperty(name, out ITMLProperty_Byte prop)) {
                value = default;
                return false;
            }
            value = prop.Byte;
            return true;
        }

        public IReadOnlyList<byte> GetBytes(string name) => TryGetProperty(name, out ITMLProperty_Byte_Array prop) ? prop.Bytes : null;
        public bool TryGetBytes(string name, out IReadOnlyList<byte> value) {
            if(!TryGetProperty(name, out ITMLProperty_Byte_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Bytes;
            return true;
        }

        // short
        public TMLProperty_Short AddProperty(string name, short value) => AddProperty_Internal(new TMLProperty_Short(name, value));
        public TMLProperty_Short AddProperty(string name, short value, short defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Short_Array AddProperty(string name, IReadOnlyList<short> values) => AddProperty_Internal(new TMLProperty_Short_Array(name, values));
        public TMLProperty_Short_Array AddProperty(string name, IEnumerable<short> values) => AddProperty_Internal(new TMLProperty_Short_Array(name, values));

        public short GetShort(string name) => TryGetProperty(name, out ITMLProperty_Short prop) ? prop.Short : default;
        public short GetShort(string name, short defaultValue) => TryGetProperty(name, out ITMLProperty_Short prop) ? prop.Short : defaultValue;
        public bool TryGetShort(string name, out short value) {
            if(!TryGetProperty(name, out ITMLProperty_Short prop)) {
                value = default;
                return false;
            }
            value = prop.Short;
            return true;
        }

        public IReadOnlyList<short> GetShorts(string name) => TryGetProperty(name, out ITMLProperty_Short_Array prop) ? prop.Shorts : null;
        public bool TryGetShorts(string name, out IReadOnlyList<short> value) {
            if(!TryGetProperty(name, out ITMLProperty_Short_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Shorts;
            return true;
        }

        // ushort
        public TMLProperty_UShort AddProperty(string name, ushort value) => AddProperty_Internal(new TMLProperty_UShort(name, value));
        public TMLProperty_UShort AddProperty(string name, ushort value, ushort defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_UShort_Array AddProperty(string name, IReadOnlyList<ushort> values) => AddProperty_Internal(new TMLProperty_UShort_Array(name, values));
        public TMLProperty_UShort_Array AddProperty(string name, IEnumerable<ushort> values) => AddProperty_Internal(new TMLProperty_UShort_Array(name, values));

        public ushort GetUShort(string name) => TryGetProperty(name, out ITMLProperty_UShort prop) ? prop.UShort : default;
        public ushort GetUShort(string name, ushort defaultValue) => TryGetProperty(name, out ITMLProperty_UShort prop) ? prop.UShort : defaultValue;
        public bool TryGetUShort(string name, out ushort value) {
            if(!TryGetProperty(name, out ITMLProperty_UShort prop)) {
                value = default;
                return false;
            }
            value = prop.UShort;
            return true;
        }

        public IReadOnlyList<ushort> GetUShorts(string name) => TryGetProperty(name, out ITMLProperty_UShort_Array prop) ? prop.UShorts : null;
        public bool TryGetUShorts(string name, out IReadOnlyList<ushort> value) {
            if(!TryGetProperty(name, out ITMLProperty_UShort_Array prop)) {
                value = default;
                return false;
            }
            value = prop.UShorts;
            return true;
        }

        // int
        public TMLProperty_Int AddProperty(string name, int value) => AddProperty_Internal(new TMLProperty_Int(name, value));
        public TMLProperty_Int AddProperty(string name, int value, int defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Int_Array AddProperty(string name, IReadOnlyList<int> values) => AddProperty_Internal(new TMLProperty_Int_Array(name, values));
        public TMLProperty_Int_Array AddProperty(string name, IEnumerable<int> values) => AddProperty_Internal(new TMLProperty_Int_Array(name, values));

        public int GetInt(string name) => TryGetProperty(name, out ITMLProperty_Int prop) ? prop.Int : default;
        public int GetInt(string name, int defaultValue) => TryGetProperty(name, out ITMLProperty_Int prop) ? prop.Int : defaultValue;
        public bool TryGetInt(string name, out int value) {
            if(!TryGetProperty(name, out ITMLProperty_Int prop)) {
                value = default;
                return false;
            }
            value = prop.Int;
            return true;
        }

        public IReadOnlyList<int> GetInts(string name) => TryGetProperty(name, out ITMLProperty_Int_Array prop) ? prop.Ints : null;
        public bool TryGetInts(string name, out IReadOnlyList<int> value) {
            if(!TryGetProperty(name, out ITMLProperty_Int_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Ints;
            return true;
        }

        // uint
        public TMLProperty_UInt AddProperty(string name, uint value) => AddProperty_Internal(new TMLProperty_UInt(name, value));
        public TMLProperty_UInt AddProperty(string name, uint value, uint defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_UInt_Array AddProperty(string name, IReadOnlyList<uint> values) => AddProperty_Internal(new TMLProperty_UInt_Array(name, values));
        public TMLProperty_UInt_Array AddProperty(string name, IEnumerable<uint> values) => AddProperty_Internal(new TMLProperty_UInt_Array(name, values));

        public uint GetUInt(string name) => TryGetProperty(name, out ITMLProperty_UInt prop) ? prop.UInt : default;
        public uint GetUInt(string name, uint defaultValue) => TryGetProperty(name, out ITMLProperty_UInt prop) ? prop.UInt : defaultValue;
        public bool TryGetUInt(string name, out uint value) {
            if(!TryGetProperty(name, out ITMLProperty_UInt prop)) {
                value = default;
                return false;
            }
            value = prop.UInt;
            return true;
        }

        public IReadOnlyList<uint> GetUInts(string name) => TryGetProperty(name, out ITMLProperty_UInt_Array prop) ? prop.UInts : null;
        public bool TryGetUInts(string name, out IReadOnlyList<uint> value) {
            if(!TryGetProperty(name, out ITMLProperty_UInt_Array prop)) {
                value = default;
                return false;
            }
            value = prop.UInts;
            return true;
        }

        // long
        public TMLProperty_Long AddProperty(string name, long value) => AddProperty_Internal(new TMLProperty_Long(name, value));
        public TMLProperty_Long AddProperty(string name, long value, long defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Long_Array AddProperty(string name, IReadOnlyList<long> values) => AddProperty_Internal(new TMLProperty_Long_Array(name, values));
        public TMLProperty_Long_Array AddProperty(string name, IEnumerable<long> values) => AddProperty_Internal(new TMLProperty_Long_Array(name, values));

        public long GetLong(string name) => TryGetProperty(name, out ITMLProperty_Long prop) ? prop.Long : default;
        public long GetLong(string name, long defaultValue) => TryGetProperty(name, out ITMLProperty_Long prop) ? prop.Long : defaultValue;
        public bool TryGetLong(string name, out long value) {
            if(!TryGetProperty(name, out ITMLProperty_Long prop)) {
                value = default;
                return false;
            }
            value = prop.Long;
            return true;
        }

        public IReadOnlyList<long> GetLongs(string name) => TryGetProperty(name, out ITMLProperty_Long_Array prop) ? prop.Longs : null;
        public bool TryGetLongs(string name, out IReadOnlyList<long> value) {
            if(!TryGetProperty(name, out ITMLProperty_Long_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Longs;
            return true;
        }

        // ulong
        public TMLProperty_ULong AddProperty(string name, ulong value) => AddProperty_Internal(new TMLProperty_ULong(name, value));
        public TMLProperty_ULong AddProperty(string name, ulong value, ulong defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_ULong_Array AddProperty(string name, IReadOnlyList<ulong> values) => AddProperty_Internal(new TMLProperty_ULong_Array(name, values));
        public TMLProperty_ULong_Array AddProperty(string name, IEnumerable<ulong> values) => AddProperty_Internal(new TMLProperty_ULong_Array(name, values));

        public ulong GetULong(string name) => TryGetProperty(name, out ITMLProperty_ULong prop) ? prop.ULong : default;
        public ulong GetULong(string name, ulong defaultValue) => TryGetProperty(name, out ITMLProperty_ULong prop) ? prop.ULong : defaultValue;
        public bool TryGetULong(string name, out ulong value) {
            if(!TryGetProperty(name, out ITMLProperty_ULong prop)) {
                value = default;
                return false;
            }
            value = prop.ULong;
            return true;
        }

        public IReadOnlyList<ulong> GetULongs(string name) => TryGetProperty(name, out ITMLProperty_ULong_Array prop) ? prop.ULongs : null;
        public bool TryGetULongs(string name, out IReadOnlyList<ulong> value) {
            if(!TryGetProperty(name, out ITMLProperty_ULong_Array prop)) {
                value = default;
                return false;
            }
            value = prop.ULongs;
            return true;
        }

        // float
        public TMLProperty_Float AddProperty(string name, float value) => AddProperty_Internal(new TMLProperty_Float(name, value));
        public TMLProperty_Float AddProperty(string name, float value, float defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Float_Array AddProperty(string name, IReadOnlyList<float> values) => AddProperty_Internal(new TMLProperty_Float_Array(name, values));
        public TMLProperty_Float_Array AddProperty(string name, IEnumerable<float> values) => AddProperty_Internal(new TMLProperty_Float_Array(name, values));

        public float GetFloat(string name) => TryGetProperty(name, out ITMLProperty_Float prop) ? prop.Float : default;
        public float GetFloat(string name, float defaultValue) => TryGetProperty(name, out ITMLProperty_Float prop) ? prop.Float : defaultValue;
        public bool TryGetFloat(string name, out float value) {
            if(!TryGetProperty(name, out ITMLProperty_Float prop)) {
                value = default;
                return false;
            }
            value = prop.Float;
            return true;
        }

        public IReadOnlyList<float> GetFloats(string name) => TryGetProperty(name, out ITMLProperty_Float_Array prop) ? prop.Floats : null;
        public bool TryGetFloats(string name, out IReadOnlyList<float> value) {
            if(!TryGetProperty(name, out ITMLProperty_Float_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Floats;
            return true;
        }

        // double
        public TMLProperty_Double AddProperty(string name, double value) => AddProperty_Internal(new TMLProperty_Double(name, value));
        public TMLProperty_Double AddProperty(string name, double value, double defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Double_Array AddProperty(string name, IReadOnlyList<double> values) => AddProperty_Internal(new TMLProperty_Double_Array(name, values));
        public TMLProperty_Double_Array AddProperty(string name, IEnumerable<double> values) => AddProperty_Internal(new TMLProperty_Double_Array(name, values));

        public double GetDouble(string name) => TryGetProperty(name, out ITMLProperty_Double prop) ? prop.Double : default;
        public double GetDouble(string name, double defaultValue) => TryGetProperty(name, out ITMLProperty_Double prop) ? prop.Double : defaultValue;
        public bool TryGetDouble(string name, out double value) {
            if(!TryGetProperty(name, out ITMLProperty_Double prop)) {
                value = default;
                return false;
            }
            value = prop.Double;
            return true;
        }

        public IReadOnlyList<double> GetDoubles(string name) => TryGetProperty(name, out ITMLProperty_Double_Array prop) ? prop.Doubles : null;
        public bool TryGetDoubles(string name, out IReadOnlyList<double> value) {
            if(!TryGetProperty(name, out ITMLProperty_Double_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Doubles;
            return true;
        }

        // decimal
        public TMLProperty_Decimal AddProperty(string name, decimal value) => AddProperty_Internal(new TMLProperty_Decimal(name, value));
        public TMLProperty_Decimal AddProperty(string name, decimal value, decimal defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Decimal_Array AddProperty(string name, IReadOnlyList<decimal> values) => AddProperty_Internal(new TMLProperty_Decimal_Array(name, values));
        public TMLProperty_Decimal_Array AddProperty(string name, IEnumerable<decimal> values) => AddProperty_Internal(new TMLProperty_Decimal_Array(name, values));

        public decimal GetDecimal(string name) => TryGetProperty(name, out ITMLProperty_Decimal prop) ? prop.Decimal : default;
        public decimal GetDecimal(string name, decimal defaultValue) => TryGetProperty(name, out ITMLProperty_Decimal prop) ? prop.Decimal : defaultValue;
        public bool TryGetDecimal(string name, out decimal value) {
            if(!TryGetProperty(name, out ITMLProperty_Decimal prop)) {
                value = default;
                return false;
            }
            value = prop.Decimal;
            return true;
        }

        public IReadOnlyList<decimal> GetDecimals(string name) => TryGetProperty(name, out ITMLProperty_Decimal_Array prop) ? prop.Decimals : null;
        public bool TryGetDecimals(string name, out IReadOnlyList<decimal> value) {
            if(!TryGetProperty(name, out ITMLProperty_Decimal_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Decimals;
            return true;
        }

        // DateTime
        public TMLProperty_DateTime AddProperty(string name, TKDateTime value) => AddProperty_Internal(new TMLProperty_DateTime(name, value));
        public TMLProperty_DateTime AddProperty(string name, TKDateTime value, TKDateTime defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_DateTime_Array AddProperty(string name, IReadOnlyList<TKDateTime> values) => AddProperty_Internal(new TMLProperty_DateTime_Array(name, values));
        public TMLProperty_DateTime_Array AddProperty(string name, IEnumerable<TKDateTime> values) => AddProperty_Internal(new TMLProperty_DateTime_Array(name, values));

        public TMLProperty_DateTime AddProperty(string name, DateTime value) => AddProperty_Internal(new TMLProperty_DateTime(name, value));
        public TMLProperty_DateTime AddProperty(string name, DateTime value, DateTime defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_DateTime_Array AddProperty(string name, IReadOnlyList<DateTime> values) => AddProperty_Internal(new TMLProperty_DateTime_Array(name, values));
        public TMLProperty_DateTime_Array AddProperty(string name, IEnumerable<DateTime> values) => AddProperty_Internal(new TMLProperty_DateTime_Array(name, values));

        public TKDateTime GetDateTime(string name) => TryGetProperty(name, out ITMLProperty_TKDateTime prop) ? prop.DateTime : default;
        public TKDateTime GetDateTime(string name, TKDateTime defaultValue) => TryGetProperty(name, out ITMLProperty_TKDateTime prop) ? prop.DateTime : defaultValue;
        public bool TryGetDateTime(string name, out TKDateTime value) {
            if(!TryGetProperty(name, out ITMLProperty_TKDateTime prop)) {
                value = default;
                return false;
            }
            value = prop.DateTime;
            return true;
        }

        public IReadOnlyList<TKDateTime> GetDateTimes(string name) => TryGetProperty(name, out ITMLProperty_TKDateTime_Array prop) ? prop.DateTimes : null;
        public bool TryGetDecimals(string name, out IReadOnlyList<TKDateTime> value) {
            if(!TryGetProperty(name, out ITMLProperty_TKDateTime_Array prop)) {
                value = default;
                return false;
            }
            value = prop.DateTimes;
            return true;
        }

        // string
        public TMLProperty_String AddProperty(string name, string value) => AddProperty_Internal(new TMLProperty_String(name, value));
        public TMLProperty_String AddProperty(string name, string value, string defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_String_Array AddProperty(string name, IReadOnlyList<string> values) => AddProperty_Internal(new TMLProperty_String_Array(name, values));
        public TMLProperty_String_Array AddProperty(string name, IEnumerable<string> values) => AddProperty_Internal(new TMLProperty_String_Array(name, values));

        public string GetString(string name) => TryGetProperty(name, out ITMLProperty_String prop) ? prop.String : default;
        public string GetString(string name, string defaultValue) => TryGetProperty(name, out ITMLProperty_String prop) ? prop.String : defaultValue;
        public bool TryGetString(string name, out string value) {
            if(!TryGetProperty(name, out ITMLProperty_String prop)) {
                value = default;
                return false;
            }
            value = prop.String;
            return true;
        }

        public IReadOnlyList<string> GetStrings(string name) => TryGetProperty(name, out ITMLProperty_String_Array prop) ? prop.Strings : null;
        public bool TryGetStrings(string name, out IReadOnlyList<string> value) {
            if(!TryGetProperty(name, out ITMLProperty_String_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Strings;
            return true;
        }

        // Vector2
        public TMLProperty_Vector2 AddProperty(string name, Vector2 value) => AddProperty_Internal(new TMLProperty_Vector2(name, value));
        public TMLProperty_Vector2 AddProperty(string name, Vector2 value, Vector2 defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Vector2_Array AddProperty(string name, IReadOnlyList<Vector2> values) => AddProperty_Internal(new TMLProperty_Vector2_Array(name, values));
        public TMLProperty_Vector2_Array AddProperty(string name, IEnumerable<Vector2> values) => AddProperty_Internal(new TMLProperty_Vector2_Array(name, values));

        public Vector2 GetVector2(string name) => TryGetProperty(name, out ITMLProperty_Vector2 prop) ? prop.Vector2 : default;
        public Vector2 GetVector2(string name, Vector2 defaultValue) => TryGetProperty(name, out ITMLProperty_Vector2 prop) ? prop.Vector2 : defaultValue;
        public bool TryGetVector2(string name, out Vector2 value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector2 prop)) {
                value = default;
                return false;
            }
            value = prop.Vector2;
            return true;
        }

        public IReadOnlyList<Vector2> GetVector2s(string name) => TryGetProperty(name, out ITMLProperty_Vector2_Array prop) ? prop.Vector2s : null;
        public bool TryGetVector2s(string name, out IReadOnlyList<Vector2> value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector2_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Vector2s;
            return true;
        }

        // Vector3
        public TMLProperty_Vector3 AddProperty(string name, Vector3 value) => AddProperty_Internal(new TMLProperty_Vector3(name, value));
        public TMLProperty_Vector3 AddProperty(string name, Vector3 value, Vector3 defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Vector3_Array AddProperty(string name, IReadOnlyList<Vector3> values) => AddProperty_Internal(new TMLProperty_Vector3_Array(name, values));
        public TMLProperty_Vector3_Array AddProperty(string name, IEnumerable<Vector3> values) => AddProperty_Internal(new TMLProperty_Vector3_Array(name, values));

        public Vector3 GetVector3(string name) => TryGetProperty(name, out ITMLProperty_Vector3 prop) ? prop.Vector3 : default;
        public Vector3 GetVector3(string name, Vector3 defaultValue) => TryGetProperty(name, out ITMLProperty_Vector3 prop) ? prop.Vector3 : defaultValue;
        public bool TryGetVector3(string name, out Vector3 value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector3 prop)) {
                value = default;
                return false;
            }
            value = prop.Vector3;
            return true;
        }

        public IReadOnlyList<Vector3> GetVector3s(string name) => TryGetProperty(name, out ITMLProperty_Vector3_Array prop) ? prop.Vector3s : null;
        public bool TryGetVector3s(string name, out IReadOnlyList<Vector3> value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector3_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Vector3s;
            return true;
        }

        // Vector4
        public TMLProperty_Vector4 AddProperty(string name, Vector4 value) => AddProperty_Internal(new TMLProperty_Vector4(name, value));
        public TMLProperty_Vector4 AddProperty(string name, Vector4 value, Vector4 defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Vector4_Array AddProperty(string name, IReadOnlyList<Vector4> values) => AddProperty_Internal(new TMLProperty_Vector4_Array(name, values));
        public TMLProperty_Vector4_Array AddProperty(string name, IEnumerable<Vector4> values) => AddProperty_Internal(new TMLProperty_Vector4_Array(name, values));

        public Vector4 GetVector4(string name) => TryGetProperty(name, out ITMLProperty_Vector4 prop) ? prop.Vector4 : default;
        public Vector4 GetVector4(string name, Vector4 defaultValue) => TryGetProperty(name, out ITMLProperty_Vector4 prop) ? prop.Vector4 : defaultValue;
        public bool TryGetVector4(string name, out Vector4 value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector4 prop)) {
                value = default;
                return false;
            }
            value = prop.Vector4;
            return true;
        }

        public IReadOnlyList<Vector4> GetVector4s(string name) => TryGetProperty(name, out ITMLProperty_Vector4_Array prop) ? prop.Vector4s : null;
        public bool TryGetVector4s(string name, out IReadOnlyList<Vector4> value) {
            if(!TryGetProperty(name, out ITMLProperty_Vector4_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Vector4s;
            return true;
        }

        // Quaternions
        public TMLProperty_Quaternion AddProperty(string name, Quaternion value) => AddProperty_Internal(new TMLProperty_Quaternion(name, value));
        public TMLProperty_Quaternion AddProperty(string name, Quaternion value, Quaternion defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Quaternion_Array AddProperty(string name, IReadOnlyList<Quaternion> values) => AddProperty_Internal(new TMLProperty_Quaternion_Array(name, values));
        public TMLProperty_Quaternion_Array AddProperty(string name, IEnumerable<Quaternion> values) => AddProperty_Internal(new TMLProperty_Quaternion_Array(name, values));

        public Quaternion GetQuaternion(string name) => TryGetProperty(name, out ITMLProperty_Quaternion prop) ? prop.Quaternion : default;
        public Quaternion GetQuaternion(string name, Quaternion defaultValue) => TryGetProperty(name, out ITMLProperty_Quaternion prop) ? prop.Quaternion : defaultValue;
        public bool TryGetQuaternion(string name, out Quaternion value) {
            if(!TryGetProperty(name, out ITMLProperty_Quaternion prop)) {
                value = default;
                return false;
            }
            value = prop.Quaternion;
            return true;
        }

        public IReadOnlyList<Quaternion> GetQuaternions(string name) => TryGetProperty(name, out ITMLProperty_Quaternion_Array prop) ? prop.Quaternions : null;
        public bool TryGetQuaternions(string name, out IReadOnlyList<Quaternion> value) {
            if(!TryGetProperty(name, out ITMLProperty_Quaternion_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Quaternions;
            return true;
        }

        // Pose
        public TMLProperty_Pose AddProperty(string name, Pose value) => AddProperty_Internal(new TMLProperty_Pose(name, value));
        public TMLProperty_Pose AddProperty(string name, Pose value, Pose defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Pose_Array AddProperty(string name, IReadOnlyList<Pose> values) => AddProperty_Internal(new TMLProperty_Pose_Array(name, values));
        public TMLProperty_Pose_Array AddProperty(string name, IEnumerable<Pose> values) => AddProperty_Internal(new TMLProperty_Pose_Array(name, values));

        public Pose GetPose(string name) => TryGetProperty(name, out ITMLProperty_Pose prop) ? prop.Pose : default;
        public Pose GetPose(string name, Pose defaultValue) => TryGetProperty(name, out ITMLProperty_Pose prop) ? prop.Pose : defaultValue;
        public bool TryGetPose(string name, out Pose value) {
            if(!TryGetProperty(name, out ITMLProperty_Pose prop)) {
                value = default;
                return false;
            }
            value = prop.Pose;
            return true;
        }

        public IReadOnlyList<Pose> GetPoses(string name) => TryGetProperty(name, out ITMLProperty_Pose_Array prop) ? prop.Poses : null;
        public bool TryGetPoses(string name, out IReadOnlyList<Pose> value) {
            if(!TryGetProperty(name, out ITMLProperty_Pose_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Poses;
            return true;
        }

        // Color
        public TMLProperty_Color AddProperty(string name, Color value) => AddProperty_Internal(new TMLProperty_Color(name, value));
        public TMLProperty_Color AddProperty(string name, Color value, Color defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Color_Array AddProperty(string name, IReadOnlyList<Color> values) => AddProperty_Internal(new TMLProperty_Color_Array(name, values));
        public TMLProperty_Color_Array AddProperty(string name, IEnumerable<Color> values) => AddProperty_Internal(new TMLProperty_Color_Array(name, values));

        public Color GetColor(string name) => TryGetProperty(name, out ITMLProperty_Color prop) ? prop.Color : default;
        public Color GetColor(string name, Color defaultValue) => TryGetProperty(name, out ITMLProperty_Color prop) ? prop.Color : defaultValue;
        public bool TryGetColor(string name, out Color value) {
            if(!TryGetProperty(name, out ITMLProperty_Color prop)) {
                value = default;
                return false;
            }
            value = prop.Color;
            return true;
        }

        public IReadOnlyList<Color> GetColors(string name) => TryGetProperty(name, out ITMLProperty_Color_Array prop) ? prop.Colors : null;
        public bool TryGetColors(string name, out IReadOnlyList<Color> value) {
            if(!TryGetProperty(name, out ITMLProperty_Color_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Colors;
            return true;
        }

        // Rect
        public TMLProperty_Rect AddProperty(string name, Rect value) => AddProperty_Internal(new TMLProperty_Rect(name, value));
        public TMLProperty_Rect AddProperty(string name, Rect value, Rect defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Rect_Array AddProperty(string name, IReadOnlyList<Rect> values) => AddProperty_Internal(new TMLProperty_Rect_Array(name, values));
        public TMLProperty_Rect_Array AddProperty(string name, IEnumerable<Rect> values) => AddProperty_Internal(new TMLProperty_Rect_Array(name, values));

        public Rect GetRect(string name) => TryGetProperty(name, out ITMLProperty_Rect prop) ? prop.Rect : default;
        public Rect GetRect(string name, Rect defaultValue) => TryGetProperty(name, out ITMLProperty_Rect prop) ? prop.Rect : defaultValue;
        public bool TryGetRect(string name, out Rect value) {
            if(!TryGetProperty(name, out ITMLProperty_Rect prop)) {
                value = default;
                return false;
            }
            value = prop.Rect;
            return true;
        }

        public IReadOnlyList<Rect> GetRects(string name) => TryGetProperty(name, out ITMLProperty_Rect_Array prop) ? prop.Rects : null;
        public bool TryGetRects(string name, out IReadOnlyList<Rect> value) {
            if(!TryGetProperty(name, out ITMLProperty_Rect_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Rects;
            return true;
        }

        // Bounds
        public TMLProperty_Bounds AddProperty(string name, Bounds value) => AddProperty_Internal(new TMLProperty_Bounds(name, value));
        public TMLProperty_Bounds AddProperty(string name, Bounds value, Bounds defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Bounds_Array AddProperty(string name, IReadOnlyList<Bounds> values) => AddProperty_Internal(new TMLProperty_Bounds_Array(name, values));
        public TMLProperty_Bounds_Array AddProperty(string name, IEnumerable<Bounds> values) => AddProperty_Internal(new TMLProperty_Bounds_Array(name, values));

        public Bounds GetBound(string name) => TryGetProperty(name, out ITMLProperty_Bounds prop) ? prop.Bound : default;
        public Bounds GetBound(string name, Bounds defaultValue) => TryGetProperty(name, out ITMLProperty_Bounds prop) ? prop.Bound : defaultValue;
        public bool TryGetBound(string name, out Bounds value) {
            if(!TryGetProperty(name, out ITMLProperty_Bounds prop)) {
                value = default;
                return false;
            }
            value = prop.Bound;
            return true;
        }

        public IReadOnlyList<Bounds> GetBounds(string name) => TryGetProperty(name, out ITMLProperty_Bounds_Array prop) ? prop.Bounds : null;
        public bool TryGetBounds(string name, out IReadOnlyList<Bounds> value) {
            if(!TryGetProperty(name, out ITMLProperty_Bounds_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Bounds;
            return true;
        }

        // AnimationCurve
        public TMLProperty_AnimationCurve AddProperty(string name, AnimationCurve value) => AddProperty_Internal(new TMLProperty_AnimationCurve(name, value));
        public TMLProperty_AnimationCurve AddProperty(string name, AnimationCurve value, AnimationCurve defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_AnimationCurve_Array AddProperty(string name, IReadOnlyList<AnimationCurve> values) => AddProperty_Internal(new TMLProperty_AnimationCurve_Array(name, values));
        public TMLProperty_AnimationCurve_Array AddProperty(string name, IEnumerable<AnimationCurve> values) => AddProperty_Internal(new TMLProperty_AnimationCurve_Array(name, values));

        public AnimationCurve GetCurve(string name) => TryGetProperty(name, out ITMLProperty_AnimationCurve prop) ? prop.Curve : default;
        public AnimationCurve GetCurve(string name, AnimationCurve defaultValue) => TryGetProperty(name, out ITMLProperty_AnimationCurve prop) ? prop.Curve : defaultValue;
        public bool TryGetCurve(string name, out AnimationCurve value) {
            if(!TryGetProperty(name, out ITMLProperty_AnimationCurve prop)) {
                value = default;
                return false;
            }
            value = prop.Curve;
            return true;
        }

        public IReadOnlyList<AnimationCurve> GetCurves(string name) => TryGetProperty(name, out ITMLProperty_AnimationCurve_Array prop) ? prop.Curves : null;
        public bool TryGetCurves(string name, out IReadOnlyList<AnimationCurve> value) {
            if(!TryGetProperty(name, out ITMLProperty_AnimationCurve_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Curves;
            return true;
        }

        // Gradient
        public TMLProperty_Gradient AddProperty(string name, Gradient value) => AddProperty_Internal(new TMLProperty_Gradient(name, value));
        public TMLProperty_Gradient AddProperty(string name, Gradient value, Gradient defaultValue) => throw new NotImplementedException(TAG + "Default value implementation not implemented!");
        public TMLProperty_Gradient_Array AddProperty(string name, IReadOnlyList<Gradient> values) => AddProperty_Internal(new TMLProperty_Gradient_Array(name, values));
        public TMLProperty_Gradient_Array AddProperty(string name, IEnumerable<Gradient> values) => AddProperty_Internal(new TMLProperty_Gradient_Array(name, values));

        public Gradient GetGradient(string name) => TryGetProperty(name, out ITMLProperty_Gradient prop) ? prop.Gradient : default;
        public Gradient GetGradient(string name, Gradient defaultValue) => TryGetProperty(name, out ITMLProperty_Gradient prop) ? prop.Gradient : defaultValue;
        public bool TryGetGradient(string name, out Gradient value) {
            if(!TryGetProperty(name, out ITMLProperty_Gradient prop)) {
                value = default;
                return false;
            }
            value = prop.Gradient;
            return true;
        }

        public IReadOnlyList<Gradient> GetGradients(string name) => TryGetProperty(name, out ITMLProperty_Gradient_Array prop) ? prop.Gradients : null;
        public bool TryGetGradients(string name, out IReadOnlyList<Gradient> value) {
            if(!TryGetProperty(name, out ITMLProperty_Gradient_Array prop)) {
                value = default;
                return false;
            }
            value = prop.Gradients;
            return true;
        }

        #endregion

        #region Name Check

        public bool IsName(string name) => this.name.Equals(name);
        public bool IsName(uint hash32) => TMLUtility.GetHash32(name) == hash32;
        public bool IsName(ushort hash16) => TMLUtility.GetHash16(name) == hash16;
        public bool IsName(byte hash8) => TMLUtility.GetHash8(name) == hash8;

        #endregion
    }
}
