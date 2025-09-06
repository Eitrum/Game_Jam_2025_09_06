using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.CodeGenerator
{
    public class CodeClass : IClass
    {

        #region Variables

        private string name = "";
        private AccessModifier accessModifier = AccessModifier.Public;
        private List<string> inherits = new List<string>();

        private List<IAttribute> attributes = new List<IAttribute>();
        private List<ICode> codeNodes = new List<ICode>();

        #endregion

        #region Properties

        public string Name => name;
        public AccessModifier AccessModifier => accessModifier;
        public string InheritanceText {
            get {
                if(inherits.Count == 0)
                    return "";
                string result = " : ";
                for(int i = 0, length = inherits.Count; i < length; i++) {
                    result += $"{inherits[i]}{(i + 1 < length ? ", " : "")}";
                }
                return result;
            }
        }

        public string[] Inherits => inherits.ToArray();

        public IReadOnlyList<IAttribute> Attributes => attributes;
        public IReadOnlyList<ICode> CodeNodes => codeNodes;

        #endregion

        #region Constructor

        public CodeClass() {

        }

        public CodeClass(string name) {
            this.name = name;
        }

        public CodeClass(string name, AccessModifier accessModifier) : this(name) {
            this.accessModifier = accessModifier;
        }

        public CodeClass(string name, AccessModifier accessModifier, string inherits) : this(name, accessModifier) {
            this.inherits.Add(inherits);
        }

        public CodeClass(string name, AccessModifier accessModifier, Type inherits) : this(name, accessModifier) {
            this.inherits.Add(inherits.FullName);
        }

        public CodeClass(string name, AccessModifier accessModifier, IReadOnlyList<string> inherits) : this(name, accessModifier) {
            for(int i = 0, length = inherits.Count; i < length; i++) {
                this.inherits.Add(inherits[i]);
            }
        }

        public CodeClass(string name, AccessModifier accessModifier, IReadOnlyList<Type> inherits) : this(name, accessModifier) {
            for(int i = 0, length = inherits.Count; i < length; i++) {
                this.inherits.Add(inherits[i].FullName);
            }
        }

        public CodeClass(AccessModifier accessModifier, string name) {
            this.accessModifier = accessModifier;
            this.name = name;
        }

        public CodeClass(AccessModifier accessModifier, string name, Type inherits) : this(accessModifier, name, inherits.FullName) { }
        public CodeClass(AccessModifier accessModifier, string name, string inherits) {
            this.accessModifier = accessModifier;
            this.name = name;
            if(!string.IsNullOrEmpty(inherits))
                this.inherits.Add(inherits);
        }

        public CodeClass(AccessModifier accessModifier, string name, IReadOnlyList<Type> inherits) : this(accessModifier, name, inherits.Select(x => x.FullName)) { }

        public CodeClass(AccessModifier accessModifier, string name, IEnumerable<string> inherits) {
            this.accessModifier = accessModifier;
            this.name = name;
            foreach(var inherit in inherits) {
                this.inherits.Add(inherit);
            }
        }

        public CodeClass(AccessModifier accessModifier, string name, IReadOnlyList<string> inherits) {
            this.accessModifier = accessModifier;
            this.name = name;
            if(inherits != null) {
                for(int i = 0, length = inherits.Count; i < length; i++) {
                    this.inherits.Add(inherits[i]);
                }
            }
        }

        #endregion

        #region Add Subtype/Inherits

        public void AddInheritance(string name) {
            inherits.Add(name);
        }

        public void AddInheritance(IClass otherClass) {
            inherits.Add(otherClass.Name);
        }

        public void CloneInheritance(IClass otherClass) {
            inherits.AddRange(otherClass.Inherits);
        }

        #endregion

        #region Add Constructor

        public void AddConstructor() {
            var method = new CodeMethod(AccessModifier.Public, "", name, CodeBlock.Empty());
            codeNodes.Add(method);
        }

        public void AddConstructor(IMethod method) {
            codeNodes.Add(method);
        }

        public CodeMethod AddConstructor(CodeMethod method) {
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddConstructor(IReadOnlyList<IVariable> parameterList, IBlock block) {
            var method = new CodeMethod(AccessModifier.Public, "", name, parameterList, block);
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddConstructor(IBlock block) {
            var method = new CodeMethod(AccessModifier.Public, "", name, block);
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddConstructor(string block) {
            var method = new CodeMethod(AccessModifier.Public, "", name, new CodeBlock(block));
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddConstructor(AccessModifier accessModifier, IBlock block) {
            var method = new CodeMethod(accessModifier, "", name, block);
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddConstructor(AccessModifier accessModifier, string block) {
            var method = new CodeMethod(accessModifier, "", name, new CodeBlock(block));
            codeNodes.Add(method);
            return method;
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

        #region Custom Code

        public void AddCustom(ICode code) {
            codeNodes.Add(code);
        }

        #endregion

        #region Add Variables

        public T AddVariable<T>(T variable) where T : IVariable {
            codeNodes.Add(variable);
            return variable;
        }

        public CodeVariable AddVariable(AccessModifier accessModifier, Type type, string variableName) {
            var variable = new CodeVariable(accessModifier, type, variableName);
            codeNodes.Add(variable);
            return variable;
        }

        public CodeVariable AddVariable(AccessModifier accessModifier, Type type, string variableName, string defaultValue) {
            var variable = new CodeVariable(accessModifier, type, variableName, defaultValue);
            codeNodes.Add(variable);
            return variable;
        }

        #endregion

        #region Properties

        public void AddProperty(IProperty property) {
            codeNodes.Add(property);
        }

        public CodeProperty AddProperty(CodeProperty property) {
            codeNodes.Add(property);
            return property;
        }

        public CodeProperty AddProperty(AccessModifier accessModifier, Type type, string propertyName, IBlock get, IBlock set, AccessModifier setAccessModifier = AccessModifier.None) {
            var prop = new CodeProperty(accessModifier, type, propertyName, get, set, setAccessModifier);
            codeNodes.Add(prop);
            return prop;
        }

        public CodeProperty AddProperty(AccessModifier accessModifier, Type type, string propertyName, IBlock get) {
            var prop = new CodeProperty(accessModifier, type, propertyName, get);
            codeNodes.Add(prop);
            return prop;
        }

        public CodeProperty AddProperty(AccessModifier accessModifier, Type type, string propertyName, string get, string set, AccessModifier setAccessModifier = AccessModifier.None) {
            var prop = new CodeProperty(accessModifier, type, propertyName, get, set, setAccessModifier);
            codeNodes.Add(prop);
            return prop;
        }

        public CodeProperty AddProperty(AccessModifier accessModifier, Type type, string propertyName, string get) {
            var prop = new CodeProperty(accessModifier, type, propertyName, get);
            codeNodes.Add(prop);
            return prop;
        }

        #endregion

        #region Add Class

        public void AddClass(IClass @class) {
            codeNodes.Add(@class);
        }

        public CodeClass AddClass(CodeClass codeClass) {
            codeNodes.Add(codeClass);
            return codeClass;
        }

        public CodeClass AddClass(string name) {
            var @class = new CodeClass(name);
            codeNodes.Add(@class);
            return @class;
        }

        public CodeClass AddClass(AccessModifier accessModifier, string name) {
            var @class = new CodeClass(name, accessModifier);
            codeNodes.Add(@class);
            return @class;
        }

        #endregion

        #region AddEnum

        public void AddEnum(IEnum @enum) {
            codeNodes.Add(@enum);
        }

        public CodeEnum AddEnum(CodeEnum @enum) {
            codeNodes.Add(@enum);
            return @enum;
        }

        public CodeEnum AddEnum(string name, IReadOnlyList<string> variableNames) {
            var codeEnum = new CodeEnum(name, variableNames);
            codeNodes.Add(codeEnum);
            return codeEnum;
        }

        public CodeEnum AddEnum(string name, IReadOnlyList<string> variableNames, IReadOnlyList<int> values) {
            var codeEnum = new CodeEnum(name, variableNames, values);
            codeNodes.Add(codeEnum);
            return codeEnum;
        }

        #endregion

        #region Add Methods

        public void AddMethod(IMethod method) {
            codeNodes.Add(method);
        }

        public CodeMethod AddMethod(CodeMethod method) {
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddMethod(string name, IBlock block) {
            var method = new CodeMethod(name, block);
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddMethod(AccessModifier accessModifier, string name, IBlock block) {
            var method = new CodeMethod(accessModifier, name, block);
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddMethod(string name, string block) {
            var method = new CodeMethod(name, new CodeBlock(block));
            codeNodes.Add(method);
            return method;
        }

        public CodeMethod AddMethod(AccessModifier accessModifier, string name, string block) {
            var method = new CodeMethod(accessModifier, name, new CodeBlock(block));
            codeNodes.Add(method);
            return method;
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            string attRes = "";
            foreach(var att in attributes) {
                attRes += $"{att.GetCode(indent)}{GenUtility.NEWLINE}";
            }
            return attRes + $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}class {GenUtility.VerifyName(name)}{InheritanceText} {{{GenUtility.NEWLINE}{GenUtility.Process(codeNodes, indent + 1)}{GenUtility.Indent(indent)}}}{GenUtility.NEWLINE}";
        }

        #endregion
    }
}
