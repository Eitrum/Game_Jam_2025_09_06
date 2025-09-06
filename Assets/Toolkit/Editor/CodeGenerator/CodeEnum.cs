using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.CodeGenerator {
    public class CodeEnum : IEnum {

        #region Variables

        private AccessModifier accessModifier = AccessModifier.Public;
        private string overrideType = "";
        private string name = "";
        private List<IVariable> variables = new List<IVariable>();
        private List<IAttribute> attributes = new List<IAttribute>();

        #endregion

        #region Properties

        public string Name => name;
        public string Type => string.IsNullOrEmpty(overrideType) ? "int" : overrideType;
        public AccessModifier AccessModifier => accessModifier;
        public IVariable[] Variables => variables.ToArray();

        #endregion

        #region Constructor

        public CodeEnum() { }

        public CodeEnum(string name, IReadOnlyList<string> variables, string type = null)
            : this(AccessModifier.Public, name, variables, type) { }

        public CodeEnum(AccessModifier accessModifier, string name, IReadOnlyList<string> variables, string type = null) {
            this.accessModifier = accessModifier;
            this.name = name;
            this.overrideType = type;
            for(int i = 0, length = variables.Count; i < length; i++) {
                this.variables.Add(new CodeVariable(AccessModifier.None, "", variables[i], i.ToString()));
            }
        }
        public CodeEnum(string name, IReadOnlyList<string> variables, IReadOnlyList<int> values, string type = null)
            : this(AccessModifier.Public, name, variables, values, type) { }

        public CodeEnum(AccessModifier accessModifier, string name, IReadOnlyList<string> variables, IReadOnlyList<int> values, string type = null) {
            this.accessModifier = accessModifier;
            this.name = name;
            this.overrideType = type;
            for(int i = 0, length = variables.Count; i < length; i++) {
                this.variables.Add(new CodeVariable(AccessModifier.None, "", variables[i], values[i].ToString()));
            }
        }

        public CodeEnum(string name, IReadOnlyList<IVariable> variables, string type = null)
            : this(AccessModifier.Public, name, variables, type) { }

        public CodeEnum(AccessModifier accessModifier, string name, IReadOnlyList<IVariable> variables, string type = null) {
            this.accessModifier = accessModifier;
            this.name = name;
            this.overrideType = type;
            for(int i = 0, length = variables.Count; i < length; i++) {
                this.variables.Add(variables[i]);
            }
        }

        #endregion

        #region Add Variables

        public CodeVariable AddVariable(string name) {
            var variable = new CodeVariable(Type, name);
            variables.Add(variable);
            return variable;
        }

        public CodeVariable AddVariable(string name, int value) {
            var variable = new CodeVariable(Type, name, value.ToString());
            variables.Add(variable);
            return variable;
        }

        public CodeVariable AddVariable(string name, string value) {
            var variable = new CodeVariable(Type, name, value);
            variables.Add(variable);
            return variable;
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

        #region ICode Impl

        public string GetCode(int indent) {
            string attRes = "";
            foreach(var att in attributes) {
                attRes += $"{att.GetCode(indent)}{GenUtility.NEWLINE}";
            }
            return $"{attRes}{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}enum {GenUtility.VerifyName(name)} : {Type} {{{GenUtility.NEWLINE}{CustomProcess(variables, indent + 1)}{GenUtility.Indent(indent)}}}{GenUtility.NEWLINE}";
        }

        private string CustomProcess(IReadOnlyList<IVariable> array, int indent) {
            string res = "";
            for(int i = 0, count = array.Count; i < count; i++) {
                string attRes = "";
                var attArray = array[i].Attributes;
                for(int x = 0; x < attArray.Count; x++) {
                    var att = attArray[x];
                    attRes += $"{att.GetCode(0)} ";
                }

                if(string.IsNullOrEmpty(array[i].DefaultValue)) {
                    res += $"{GenUtility.Indent(indent)}{attRes}{GenUtility.VerifyName(array[i].Name)},{GenUtility.NEWLINE}";
                }
                else {
                    res += $"{GenUtility.Indent(indent)}{attRes}{GenUtility.VerifyName(array[i].Name)} = {array[i].DefaultValue},{GenUtility.NEWLINE}";
                }

            }
            return res;
        }

        #endregion
    }
}
