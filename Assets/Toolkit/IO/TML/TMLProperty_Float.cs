using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {

    public interface ITMLProperty_Float : ITMLProperty {
        float Float { get; }
    }

    public interface ITMLProperty_Float_Array : ITMLProperty {
        IReadOnlyList<float> Floats { get; }
    }

    public class TMLProperty_Float : TMLProperty_Base<float>, ITMLProperty_Float, ITMLProperty_String {
        #region Variables

        public const byte TYPE_ID = TMLBuiltInTypes.Float;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public float Float => value;
        public IReadOnlyList<float> Floats => new[] { value };
        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Float() : base() { }

        public TMLProperty_Float(string name) : base(name) { }

        public TMLProperty_Float(string name, float value) : base(name, value) { }

        #endregion

        #region Text

        public void WriteTo_Json(StringBuilder builder) {
            builder.Append($"\"{name}\": {value}");
        }

        public void WriteTo_Xml(StringBuilder builder) {
            builder.Append($"{this.name}=\"{value}\"");
        }

        #endregion

        #region Binary

        // Write
        public void WriteTo(IBuffer buffer) {
            buffer.Write(TYPE_ID);
            buffer.Write(name, EncodingType.UTF8);
            buffer.Write(value);
        }

        // Read
        public void ReadFrom(IBuffer buffer) {
            buffer.Index++; // Skip first byte as it contains the type id
            name = buffer.ReadString(EncodingType.UTF8);
            value = buffer.ReadFloat();
        }

        #endregion
    }

    public sealed class TMLProperty_Float_Array : TMLProperty_Base_Array<float>, ITMLProperty_Float_Array, ITMLProperty_Float, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Float_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Float | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public float Float => value.Count > 0 ? value[0] : 0f;
        public IReadOnlyList<float> Floats => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Float_Array() : base() { }

        public TMLProperty_Float_Array(string name) : base(name) { }

        public TMLProperty_Float_Array(string name, float value) : base(name, value) { }

        public TMLProperty_Float_Array(string name, IReadOnlyList<float> floats) : base(name, floats) { }

        public TMLProperty_Float_Array(string name, IEnumerable<float> floats) : base(name, floats) { }

        public TMLProperty_Float_Array(string name, List<float> floats) : base(name, floats) { }

        public TMLProperty_Float_Array(string name, List<float> floats, bool createCopy) : base(name, floats, createCopy) { }

        public TMLProperty_Float_Array(string name, IReadOnlyList<float> floats, bool createCopy) : base(name, floats, createCopy) { }

        #endregion
    }
}
