using System;
using System.Collections.Generic;

namespace Toolkit.CodeGenerator {
    public class CodeAttribute : IAttribute {

        public class Attribute {
            private string type;
            private List<string> values = new List<string>();

            public string Type => type;
            public IReadOnlyList<string> Values => values;

            public Attribute(string type) {
                this.type = type;
            }

            public Attribute(string type, string value) {
                this.type = type;
                values.Add(value);
            }

            public Attribute(Type type) {
                this.type = type.FullName;
            }

            public Attribute(Type type, string value) {
                this.type = type.FullName;
                values.Add(value);
            }

            public Attribute(Type type, string value0, string value1) {
                this.type = type.FullName;
                values.Add(value0);
                values.Add(value1);
            }

            public Attribute(Type type, string value0, string value1, string value2) {
                this.type = type.FullName;
                values.Add(value0);
                values.Add(value1);
                values.Add(value2);
            }

            public Attribute(string type, IEnumerable<string> enumerator) {
                this.type = type;
                this.values.AddRange(enumerator);
            }

            public Attribute(string type, IReadOnlyList<string> values) {
                this.type = type;
                this.values.AddRange(values);
            }
        }

        #region Variables

        private List<Attribute> attributes = new List<Attribute>();

        #endregion

        #region Properties

        public int Count => attributes.Count;
        public IReadOnlyList<Attribute> Attributes => attributes;

        #endregion

        #region Constructor

        public CodeAttribute(Type type) : this(type.FullName) { }
        public CodeAttribute(string type) {
            attributes.Add(new Attribute(type));
        }

        public CodeAttribute(Type type, string value) : this(type.FullName, value) { }
        public CodeAttribute(string type, string value) {
            attributes.Add(new Attribute(type, value));
        }

        public CodeAttribute(Type type, IEnumerable<string> enumerator) : this(type.FullName, enumerator) { }
        public CodeAttribute(string type, IEnumerable<string> enumerator) {
            attributes.Add(new Attribute(type, enumerator));
        }
        public CodeAttribute(Type type, IReadOnlyList<string> values) : this(type.FullName, values) { }
        public CodeAttribute(string type, IReadOnlyList<string> values) {
            attributes.Add(new Attribute(type, values));
        }

        public CodeAttribute(IReadOnlyList<Attribute> attributes) {
            this.attributes.AddRange(attributes);
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            return $"{GenUtility.Indent(indent)}[{ProcessAttributes(attributes)}]";
        }

        public static string ProcessAttributes(IReadOnlyList<Attribute> attributes) {
            var count = attributes.Count - 1;
            string res = "";
            Attribute att;
            for(int i = 0; i < count; i++) {
                att = attributes[i];
                if(att.Values.Count > 0)
                    res += $"{ProcessAttributeName(att.Type)}({ProcessAttributeValues(att.Values)}),";
                else
                    res += $"{ProcessAttributeName(att.Type)},";
            }
            att = attributes[count];
            if(att.Values.Count > 0)
                res += $"{ProcessAttributeName(att.Type)}({ProcessAttributeValues(att.Values)})";
            else
                res += $"{ProcessAttributeName(att.Type)}";
            return res;
        }

        private static string ProcessAttributeName(string name) {
            if(name.EndsWith("Attribute")) {
                return name.Substring(0, name.Length - 9);
            }
            return name;
        }

        private static string ProcessAttributeValues(IReadOnlyList<string> values) {
            var count = values.Count - 1;
            string res = "";
            for(int i = 0; i < count; i++) {
                res += $"{values[i]},";
            }
            res += $"{values[count]}";
            return res;
        }

        #endregion

        #region Default

        public static CodeAttribute SerializeField => new CodeAttribute(typeof(UnityEngine.SerializeField));

        #endregion
    }
}
