using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.CodeGenerator
{
    public class CodeNamespace : INamespace
    {

        #region Variables

        private string @namespace = "";
        private List<IContainer> containers = new List<IContainer>();

        #endregion

        #region Properties

        public string Namespace => @namespace;
        public IContainer[] Containers => containers.ToArray();

        #endregion

        #region Constructor

        public CodeNamespace() { }

        public CodeNamespace(Type type) : this(type.Namespace) { }

        public CodeNamespace(string @namespace) {
            this.@namespace = @namespace;
        }

        #endregion

        #region Add

        public T Add<T>(T code) where T : IContainer {
            containers.Add(code);
            return code;
        }

        #endregion

        #region Container // Make this implementation better and more accessible

        public IContainer AddContainer(IContainer c) {
            containers.Add(c);
            return c;
        }

        public bool HasContainer(IContainer c) {
            return containers.Contains(c);
        }

        public bool RemoveContainer(IContainer c) {
            return containers.Remove(c);
        }

        public T[] GetContainers<T>() where T : class, IContainer {
            return containers.Where(x => x is T).Select(x => x as T).ToArray();
        }

        #endregion

        #region Add Class

        public void AddClass(IClass @class) {
            containers.Add(@class);
        }

        public CodeClass AddClass(CodeClass codeClass) {
            containers.Add(codeClass);
            return codeClass;
        }

        public CodeClass AddClass(string name) {
            var @class = new CodeClass(name);
            containers.Add(@class);
            return @class;
        }

        public CodeClass AddClass(AccessModifier accessModifier, string name) {
            var @class = new CodeClass(name, accessModifier);
            containers.Add(@class);
            return @class;
        }

        #endregion

        #region Add Struct

        public void AddStruct(IStruct @struct) {
            containers.Add(@struct);
        }

        public CodeStruct AddStruct(CodeStruct codeStruct) {
            containers.Add(codeStruct);
            return codeStruct;
        }

        public CodeStruct AddStruct(string name) {
            var @struct = new CodeStruct(name);
            containers.Add(@struct);
            return @struct;
        }

        public CodeStruct AddStruct(AccessModifier accessModifier, string name) {
            var @struct = new CodeStruct(name, accessModifier);
            containers.Add(@struct);
            return @struct;
        }

        #endregion

        #region AddEnum

        public void AddEnum(IEnum @enum) {
            containers.Add(@enum);
        }

        public CodeEnum AddEnum(CodeEnum @enum) {
            containers.Add(@enum);
            return @enum;
        }

        public CodeEnum AddEnum(string name, IReadOnlyList<string> variableNames) {
            var codeEnum = new CodeEnum(name, variableNames);
            containers.Add(codeEnum);
            return codeEnum;
        }

        public CodeEnum AddEnum(string name, IReadOnlyList<string> variableNames, IReadOnlyList<int> values) {
            var codeEnum = new CodeEnum(name, variableNames, values);
            containers.Add(codeEnum);
            return codeEnum;
        }

        #endregion

        #region ICodeGen Impl

        public string GetCode(int indent) {
            if(string.IsNullOrEmpty(@namespace)) {
                return $"{GenUtility.Process(containers, indent)}";
            }
            return $"{GenUtility.Indent(indent)}namespace {GenUtility.VerifyName(@namespace)} {{{GenUtility.NEWLINE}{GenUtility.Process(containers, indent + 1)}{GenUtility.Indent(indent)}}}";
        }

        #endregion
    }
}
