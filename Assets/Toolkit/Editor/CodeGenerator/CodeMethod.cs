using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.CodeGenerator {
    public class CodeMethod : IMethod {

        #region Variables

        private AccessModifier accessModifier = AccessModifier.None;
        private string returnType = "";
        private string name = "";
        private List<IVariable> parameterList = new List<IVariable>();
        private IBlock block = null;

        #endregion

        #region Properties

        public AccessModifier AccessModifier => accessModifier;
        public string Type => returnType;
        public string Name => name;
        public IVariable[] Parameters => parameterList.ToArray();
        public IBlock Block => block;

        public bool IsConstructor => string.IsNullOrEmpty(returnType) &&
            (accessModifier == AccessModifier.Public || accessModifier == (AccessModifier.Private) ||
            accessModifier == (AccessModifier.Protected) || accessModifier == (AccessModifier.Internal) ||
            accessModifier == (AccessModifier.ProtectedInternal) || accessModifier == (AccessModifier.PrivateProtected));
        public bool IsStaticConstructor => string.IsNullOrEmpty(returnType) && accessModifier == AccessModifier.Static;

        #endregion

        #region Constructor

        public CodeMethod() { }

        public CodeMethod(string name, IBlock block) : this(AccessModifier.Private, "void", name, new IVariable[0], block) { }
        public CodeMethod(AccessModifier accessModifier, string name, IBlock block) : this(accessModifier, "void", name, new IVariable[0], block) { }
        public CodeMethod(string returnType, string name, IBlock block) : this(AccessModifier.Private, returnType, name, new IVariable[0], block) { }
        public CodeMethod(AccessModifier accessModifier, string returnType, string name, IBlock block) : this(accessModifier, returnType, name, new IVariable[0], block) { }
        public CodeMethod(Type returnType, string name, IBlock block) : this(AccessModifier.Private, returnType.FullName, name, new IVariable[0], block) { }
        public CodeMethod(AccessModifier accessModifier, Type returnType, string name, IBlock block) : this(accessModifier, returnType.FullName, name, new IVariable[0], block) { }
        public CodeMethod(string name, IReadOnlyList<IVariable> parameters, IBlock block) : this(AccessModifier.Private, "void", name, parameters, block) { }
        public CodeMethod(AccessModifier accessModifier, string name, IReadOnlyList<IVariable> parameters, IBlock block) : this(accessModifier, "void", name, parameters, block) { }
        public CodeMethod(string name, IVariable parameter, IBlock block) : this(AccessModifier.Private, "void", name, new IVariable[] { parameter }, block) { }
        public CodeMethod(AccessModifier accessModifier, string name, IVariable parameter, IBlock block) : this(accessModifier, "void", name, new IVariable[] { parameter }, block) { }
        public CodeMethod(string returnType, string name, IReadOnlyList<IVariable> parameters, IBlock block) : this(AccessModifier.Private, returnType, name, parameters, block) { }
        public CodeMethod(string returnType, string name, IVariable parameter, IBlock block) : this(AccessModifier.Private, returnType, name, new IVariable[] { parameter }, block) { }
        public CodeMethod(Type returnType, string name, IVariable parameter, IBlock block) : this(AccessModifier.Private, returnType.FullName, name, new IVariable[] { parameter }, block) { }
        public CodeMethod(AccessModifier accessModifier, Type returnType, string name, IVariable parameter, IBlock block) : this(accessModifier, returnType.FullName, name, new IVariable[] { parameter }, block) { }
        public CodeMethod(Type returnType, string name, IReadOnlyList<IVariable> parameters, IBlock block) : this(AccessModifier.Private, returnType.FullName, name, parameters, block) { }
        public CodeMethod(AccessModifier accessModifier, Type returnType, string name, IReadOnlyList<IVariable> parameters, IBlock block) : this(accessModifier, returnType.FullName, name, parameters, block) { }
        public CodeMethod(AccessModifier accessModifier, string returnType, string name, IVariable parameter, IBlock block) : this(accessModifier, returnType, name, new IVariable[] { parameter }, block) { }

        public CodeMethod(AccessModifier accessModifier, string returnType, string name, IReadOnlyList<IVariable> variables, IBlock block) {
            this.accessModifier = accessModifier;
            if(returnType == "System.Void")
                returnType = "void";
            this.returnType = returnType;
            this.name = name;
            if(variables != null)
                this.parameterList.AddRange(variables);
            this.block = block;
        }

        #endregion

        #region Add Parameter

        public void AddParameter(IVariable variable) {
            parameterList.Add(variable);
        }

        public CodeVariable AddParameter(CodeVariable variable) {
            parameterList.Add(variable);
            return variable;
        }

        public CodeVariable AddParameter(string type, string name) {
            return AddParameter(new CodeVariable(type, name));
        }

        public CodeVariable AddParameter(string type, string name, string defaultValue) {
            return AddParameter(new CodeVariable(type, name, defaultValue));
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}{returnType} {GenUtility.VerifyName(name)}({GenUtility.ProcessParameterList(parameterList)}) {block.GetCode(indent + 1)}";
        }

        #endregion

        #region Constructor Generation

        public static CodeMethod CreateAssignConstructor(IClass @class, IReadOnlyList<IVariable> parameters, IReadOnlyList<IVariable> variables) {
            var count = Math.Min(parameters.Count, variables.Count);
            List<string> lines = new List<string>();
            for(int i = 0; i < count; i++) {
                lines.Add($"this.{GenUtility.VerifyName(variables[i].Name)} = {GenUtility.VerifyName(parameters[i].Name)};");
            }
            return new CodeMethod(AccessModifier.Public, "", @class.Name, parameters, new CodeBlock(lines));
        }

        public static CodeMethod CreateAssignConstructor(IClass @class, IReadOnlyList<IVariable> parameters, IReadOnlyList<string> variables) {
            var count = Math.Min(parameters.Count, variables.Count);
            List<string> lines = new List<string>();
            for(int i = 0; i < count; i++) {
                lines.Add($"this.{GenUtility.VerifyName(variables[i])} = {GenUtility.VerifyName(parameters[i].Name)};");
            }
            return new CodeMethod(AccessModifier.Public, "", @class.Name, parameters, new CodeBlock(lines));
        }

        #endregion
    }
}
