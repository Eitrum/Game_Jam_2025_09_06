using System;
using System.Collections.Generic;

namespace Toolkit.CodeGenerator
{
    public class CodeVariable : IVariable
    {

        #region Variables

        private AccessModifier accessModifier = AccessModifier.Private;
        private string type = "";
        private string name = "";
        private string defaultValue = "";
        private List<IAttribute> attributes = new List<IAttribute>();

        #endregion

        #region Properties

        public AccessModifier AccessModifier => accessModifier;
        public string Type => type;
        public string Name => name;
        public string DefaultValue => defaultValue;
        public IReadOnlyList<IAttribute> Attributes => attributes;

        #endregion

        #region Constructor

        public CodeVariable() { }

        public CodeVariable(Type type, string name) : this(AccessModifier.Private, type.FullName.Replace("+", "."), name, "") { }

        public CodeVariable(string type, string name) : this(AccessModifier.Private, type, name, "") { }

        public CodeVariable(Type type, string name, string defaultValue) : this(AccessModifier.Private, type.FullName.Replace("+", "."), name, defaultValue) { }

        public CodeVariable(string type, string name, string defaultValue) : this(AccessModifier.Private, type, name, defaultValue) { }

        public CodeVariable(AccessModifier accessModifier, Type type, string name) : this(accessModifier, type.FullName.Replace("+", "."), name, "") { }

        public CodeVariable(AccessModifier accessModifier, string type, string name) : this(accessModifier, type, name, "") { }

        public CodeVariable(AccessModifier accessModifier, Type type, string name, string defaultValue) : this(accessModifier, type.FullName.Replace("+", "."), name, defaultValue) { }

        public CodeVariable(AccessModifier accessModifier, string type, string name, string defaultValue) {
            this.accessModifier = accessModifier;
            if(type == "System.Void")
                type = "void";
            if(type.EndsWith("&")) {
                type = "out " + type.TrimEnd('&');
            }
            this.type = type;
            this.name = name;
            this.defaultValue = defaultValue;
        }

        #endregion

        #region Attribute

        public CodeAttribute AddAttribute(string type) {
            var codeAttribute = new CodeAttribute(type);
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public CodeAttribute AddAttribute(Type type) {
            var codeAttribute = new CodeAttribute(type);
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public CodeAttribute AddAttribute(Type type, string value) {
            var codeAttribute = new CodeAttribute(type, value);
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public CodeAttribute AddAttribute(string type, string value) {
            var codeAttribute = new CodeAttribute(type, value);
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public CodeAttribute AddAttribute(CodeAttribute.Attribute[] attributes) {
            var codeAttribute = new CodeAttribute(attributes);
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public CodeAttribute AddAttribute(CodeAttribute codeAttribute) {
            this.attributes.Add(codeAttribute);
            return codeAttribute;
        }

        public IAttribute AddAttribute(IAttribute iAttribute) {
            this.attributes.Add(iAttribute);
            return iAttribute;
        }

        #endregion

        #region Static Creation

        public static CodeVariable[] CreateArray(AccessModifier accessModifier, string type, IReadOnlyList<string> names) {
            var vars = new CodeVariable[names.Count];
            for(int i = 0, length = names.Count; i < length; i++) {
                vars[i] = new CodeVariable(accessModifier, type, names[i]);
            }
            return vars;
        }

        public static CodeVariable[] CreateArray(AccessModifier accessModifier, string type, IReadOnlyList<string> names, IReadOnlyList<string> values) {
            var vars = new CodeVariable[names.Count];
            for(int i = 0, length = names.Count; i < length; i++) {
                vars[i] = new CodeVariable(accessModifier, type, names[i], values[i]);
            }
            return vars;
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            string attRes = "";
            for(int i = 0; i < attributes.Count; i++) {
                var att = attributes[i];
                attRes += $"{att.GetCode(i == 0 ? indent : 0)}";
            }
            if(string.IsNullOrEmpty(attRes)) {
                attRes = GenUtility.Indent(indent);
            }
            if(string.IsNullOrEmpty(defaultValue))
                return $"{attRes}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)};";
            else
                return $"{attRes}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)} = {defaultValue};";
        }

        #endregion
    }
}
