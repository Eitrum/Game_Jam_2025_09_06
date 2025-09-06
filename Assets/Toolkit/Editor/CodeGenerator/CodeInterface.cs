using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.CodeGenerator {
    public class CodeInterface : IContainer {

        #region Variables

        private string name = "";
        private AccessModifier accessModifier = AccessModifier.Public;
        private List<string> inherits = new List<string>();

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

        #endregion

        #region Constructor

        public CodeInterface() {

        }

        public CodeInterface(string name) {
            this.name = name;
        }

        public CodeInterface(string name, AccessModifier accessModifier) : this(name) {
            this.accessModifier = accessModifier;
        }

        public CodeInterface(string name, AccessModifier accessModifier, string inherits) : this(name, accessModifier) {
            this.inherits.Add(inherits);
        }

        public CodeInterface(string name, AccessModifier accessModifier, Type inherits) : this(name, accessModifier) {
            this.inherits.Add(inherits.FullName);
        }

        public CodeInterface(string name, AccessModifier accessModifier, IReadOnlyList<string> inherits) : this(name, accessModifier) {
            for(int i = 0, length = inherits.Count; i < length; i++) {
                if(inherits[i][0] != 'I') {
                    UnityEngine.Debug.LogWarning($"Inheritance '{inherits[i]}' should start with an 'I' to indicate an interface");
                }
                this.inherits.Add(inherits[i]);
            }
        }

        public CodeInterface(string name, AccessModifier accessModifier, IReadOnlyList<Type> inherits) : this(name, accessModifier) {
            for(int i = 0, length = inherits.Count; i < length; i++) {
                if(inherits[i].IsInterface)
                    this.inherits.Add(inherits[i].FullName);
                else
                    UnityEngine.Debug.LogError($"Could not add inheritance '{inherits[i].FullName}' as it is not an interface");
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

        #region Custom Code

        public void AddCustom(ICode code) {
            codeNodes.Add(code);
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
            return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}interface {GenUtility.VerifyName(name)}{InheritanceText} {{{GenUtility.NEWLINE}{ProcessInterface(codeNodes, indent + 1)}{GenUtility.Indent(indent)}}}{GenUtility.NEWLINE}";
        }

        private static string ProcessInterface(IReadOnlyList<ICode> array, int indent) {
            if(array.Count == 0)
                return "";
            string res = "";
            var count = array.Count;
            for(int i = 0; i < count; i++) {
                var code = array[i];
                if(code is CodeProperty prop) {
                    res += $"{GenUtility.Indent(indent)}{prop.Type} {prop.Name} {{ {((!prop.AccessModifier.HasFlag(AccessModifier.Private) && prop.Get != null) ? "get; " : "")}{((!prop.SetAccessModifier.HasFlag(AccessModifier.Private) && prop.Set != null) ? $"set; " : "")}}}{GenUtility.NEWLINE}";
                }
                else if(code is CodeMethod method) {
                    res += $"{GenUtility.Indent(indent)}{method.Type} {GenUtility.VerifyName(method.Name)}({GenUtility.ProcessParameterList(method.Parameters)});{GenUtility.NEWLINE}";
                }
                else {
                    var textCode = code.GetCode(indent);
                    UnityEngine.Debug.LogWarning("Code is not a supported type in CodeInterface.cs : " + textCode);
                    res += textCode;
                }
            }
            return res;
        }

        #endregion
    }
}
