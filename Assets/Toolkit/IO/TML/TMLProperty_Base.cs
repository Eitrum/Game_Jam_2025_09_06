using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolkit.IO.TML.Properties {
    public abstract class TMLProperty_Base<T> : ITMLProperty, ITMLProperty_Xml, ITMLProperty_Json {
        #region Variables

        protected string name;
        protected T value;

        #endregion

        #region Properties

        public string Name => name;
        public byte Name_8 => TMLUtility.GetHash8(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public uint Name_32 => TMLUtility.GetHash32(name);

        public abstract byte TypeId { get; }

        // Raw fallback
        public T Value => value;

        #endregion

        #region Constructor

        protected TMLProperty_Base() { }


        protected TMLProperty_Base(string name) {
            this.name = name;
            this.value = default;
        }

        protected TMLProperty_Base(string name, T value) {
            this.name = name;
            this.value = value;
        }

        public void Deconstruct(out string name, out T value) {
            name = this.name;
            value = this.value;
        }

        #endregion

        #region Set

        public void SetName(string name) { this.name = name; }

        public void SetValue(T value) { this.value = value; }

        #endregion

        #region Checks

        public bool IsName(string name) => this.name.Equals(name);

        #endregion

        #region XML Impl
        
        public virtual string GetFormattedXml() => $"{name}=\"{value}\"";

        #endregion

        #region JSON Impl
        
        public virtual string GetFormattedJson() => $"{name}: \"{value}\"";
        public virtual void WriteToAsJson(StringBuilder sb) {
            sb.Append(GetFormattedJson());
        }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{value}";

        #endregion
    }

    public abstract class TMLProperty_Base_Array<T> : ITMLProperty, ITMLProperty_Xml {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Base_Array]</color> - ";
        protected string name;
        protected IReadOnlyList<T> value;

        // Used for internal references
        protected List<T> list;

        #endregion

        #region Properties

        public string Name => name;
        public byte Name_8 => TMLUtility.GetHash8(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public uint Name_32 => TMLUtility.GetHash32(name);
        public abstract byte TypeId { get; }

        public IReadOnlyList<T> Value { get; }

        #endregion

        #region Constructor

        protected TMLProperty_Base_Array() {
            name = string.Empty;
            value = list = new List<T>();
        }

        protected TMLProperty_Base_Array(string name) {
            this.name = name;
            this.value = this.list = new List<T>();
        }

        protected TMLProperty_Base_Array(string name, T value) {
            this.name = name;
            this.value = this.list = new List<T>() { value };
        }

        protected TMLProperty_Base_Array(string name, IReadOnlyList<T> values) {
            this.name = name;
            this.value = values;
        }

        protected TMLProperty_Base_Array(string name, IEnumerable<T> values) {
            this.name = name;
            this.value = this.list = new List<T>(values);
        }

        protected TMLProperty_Base_Array(string name, List<T> values) {
            this.name = name;
            this.value = this.list = values;
        }

        protected TMLProperty_Base_Array(string name, List<T> values, bool createCopy) {
            this.name = name;
            if(createCopy)
                this.value = this.list = new List<T>(values);
            else
                this.value = this.list = values;
        }

        protected TMLProperty_Base_Array(string name, IReadOnlyList<T> values, bool createCopy) {
            this.name = name;
            if(createCopy)
                this.value = this.list = new List<T>(values);
            else
                this.value = values;
        }

        public void Deconstructor(out string name, out IReadOnlyList<T> value) {
            name = this.name;
            value = this.value;
        }

        #endregion

        #region Set / Add

        public void SetName(string name) { this.name = name; }

        public void AddValue(T value) {
#if UNITY_EDITOR
            if(this.list == null)
                Debug.LogError(TAG + "Unable to add a value as property doesn't include a list");
#endif

            this.list?.Add(value);
        }

        public void RemoveValueAt(int index) {
#if UNITY_EDITOR
            if(this.list == null)
                Debug.LogError(TAG + "Unable to remove a value as property doesn't include a list");
#endif
            this.list?.RemoveAt(index);
        }

        #endregion

        #region Checks

        public bool IsName(string name) => this.name.Equals(name);

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:[{string.Join('|', value)}]";
        public virtual string GetFormattedXml() => $"{name}=\"[{string.Join('|', value)}]\"";
        public virtual string GetFormattedJson() => $"{name}: [{string.Join(',', value.Select(x => $"\"{x}\""))}]";
        public static string DefaultToString(TMLProperty_Base_Array<T> @base) => $"[{string.Join('|', @base.value)}]";

        #endregion
    }
}
