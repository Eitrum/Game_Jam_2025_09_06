using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.CodeGenerator {
    public class CodeFile : IFile, ICode, IContainer {

        #region Variables

        private string fileName = "";
        private string path = "";
        private string creatorTag = "";
        private bool tagFileAsGenerated = true;
        private bool useCleanProcess = false;
        private List<IUsingDirective> usings = new List<IUsingDirective>();
        private List<IContainer> containers = new List<IContainer>();

        #endregion

        #region Properties

        public string FileName => fileName;
        public string Path => path;
        public string Creator { get => creatorTag; set => creatorTag = value; }
        public bool UseCleanProcess {
            get => useCleanProcess;
            set => useCleanProcess = value;
        }
        public bool TagFileAsGenerated {
            get => tagFileAsGenerated;
            set => tagFileAsGenerated = value;
        }
        public IUsingDirective[] UsingDirectives => usings.ToArray();
        public IContainer[] Containers => containers.ToArray();

        #endregion

        #region Constructor

        public CodeFile() { }

        public CodeFile(string fileName) {
            this.fileName = fileName;
        }

        public CodeFile(string fileName, string path) {
            this.fileName = fileName;
            this.path = path;
        }

        #endregion

        #region Creator tag

        public void SetCreatorTag(string str) {
            creatorTag = str;
        }

        public void SetCreatorTag(Type type) {
            creatorTag = type.FullName;
        }

        public void SetCreatorTag<T>() {
            creatorTag = typeof(T).FullName;
        }

        public void SetCreatorTag<T>(T c) {
            creatorTag = typeof(T).FullName;
        }

        #endregion

        #region Using Methods

        public IUsingDirective AddUsing(Type typeToSupport)
            => AddUsing(typeToSupport.Namespace, false);

        public IUsingDirective AddUsing(Type typeToSupport, bool isStatic)
            => AddUsing(typeToSupport.FullName, isStatic);

        public IUsingDirective AddUsing(string @namespace)
            => AddUsing(@namespace, false);

        public IUsingDirective AddUsing(string @namespace, bool isStatic) {
            var t = GetUsing(@namespace);
            if(t != null) {
                // Handle IsStatic Conflicts ??? 
                // Log error
                return t;
            }
            var @using = new CodeUsingDirective(@namespace, isStatic);
            usings.Add(@using);
            return @using;
        }

        public bool AddUsing(IUsingDirective usingDirective) {
            if(HasUsing(usingDirective))
                return false;
            usings.Add(usingDirective);
            return true;
        }

        public bool HasUsing(string @namespace)
            => GetUsing(@namespace) != null;

        public bool HasUsing(IUsingDirective usingDirective)
            => GetUsing(usingDirective.Namespace) != null;

        public bool HasUsing(Type type)
            => GetUsing(type.FullName) != null; // Should handle IsStatic check???

        public IUsingDirective GetUsing(string @namespace) {
            for(int i = 0, length = usings.Count; i < length; i++) {
                if(usings[i].Namespace == @namespace) {
                    return usings[i];
                }
            }
            return default;
        }

        public bool RemoveUsing(string @namespace) {
            for(int i = 0, length = usings.Count; i < length; i++) {
                if(usings[i].Namespace == @namespace) {
                    usings.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveUsing(IUsingDirective usingDirective)
            => usings.Remove(usingDirective);

        public bool RemoveUsing(Type type)
            => usings.Remove(GetUsing(type.FullName));

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

        #region Add namespace

        public void AddNamespace(INamespace @namespace) {
            containers.Add(@namespace);
        }

        public CodeNamespace AddNamespace(CodeNamespace codeNamespace) {
            containers.Add(codeNamespace);
            return codeNamespace;
        }

        public CodeNamespace AddNamespace(string name) {
            var @namespace = new CodeNamespace(name);
            containers.Add(@namespace);
            return @namespace;
        }

        public CodeNamespace AddNamespace(Type type) {
            var @namespace = new CodeNamespace(type);
            containers.Add(@namespace);
            return @namespace;
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


        #region File Creation

        public string CreateFile() {
            string fullPath = $"{System.IO.Path.Combine(path, FileName)}.cs";
            var code = GetCode();
            System.IO.File.WriteAllText(fullPath, code);
            return fullPath;
        }

        public string CreateFile(string path) {
            string fullPath = $"{System.IO.Path.Combine(path, FileName)}.cs";
            var code = GetCode();
            System.IO.File.WriteAllText(fullPath, code);
            return fullPath;
        }

        #endregion

        #region ICodeGen Impl

        public string GetCode() => GetCode(0);

        public string GetCode(int indent) {
            if(usings.Count == 0 && containers.Count == 0) {
                return "";
            }
            if(usings.Count == 0) {
                return $"{CODE_TAG}{CREATOR}{GenUtility.Process(containers, indent)}";
            }
            if(containers.Count == 0) {
                return $"{CODE_TAG}{CREATOR}{GenUtility.Process(usings, indent)}";
            }
            var processedContainers = GenUtility.Process(containers, indent);
            if(useCleanProcess) {
                for(int i = 0; i < usings.Count; i++) {
                    if(!usings[i].IsStatic) {
                        var split = processedContainers.Split(GenUtility.NEWLINE[0]);
                        for(int s = 0; s < split.Length; s++) {
                            if(!split[s].Trim().StartsWith("namespace"))
                                split[s] = split[s].Replace($"{usings[i].Namespace}.", "");
                        }
                        processedContainers = string.Join(GenUtility.NEWLINE, split);
                    }
                }
            }
            return $"{CODE_TAG}{CREATOR}{GenUtility.Process(usings, indent)}\n{processedContainers}";
        }

        private string CREATOR {
            get {
                if(string.IsNullOrEmpty(creatorTag))
                    return "";
                return $"// Created by - {creatorTag}.cs{GenUtility.NEWLINE}{GenUtility.NEWLINE}";
            }
        }

        private string CODE_TAG {
            get {
                if(!tagFileAsGenerated) {
                    return "";
                }
                return
@"///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

";
            }
        }

        #endregion
    }
}
