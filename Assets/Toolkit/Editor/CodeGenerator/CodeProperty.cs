using System;
using System.Collections.Generic;

namespace Toolkit.CodeGenerator {
    public class CodeProperty : IProperty {

        #region Variables

        private AccessModifier accessModifier = AccessModifier.Public;
        private AccessModifier setAccessModifier = AccessModifier.Public;
        private string type = "";
        private string name = "";
        private IBlock getBlock = null;
        private IBlock setBlock = null;

        #endregion

        #region Properties

        public AccessModifier AccessModifier => GenUtility.CalculateRestrictivePriority(accessModifier, setAccessModifier);
        public AccessModifier GetAccessModifier => accessModifier;
        public AccessModifier SetAccessModifier => setAccessModifier;
        public string Type => type;
        public string Name => name;
        public IBlock Get => getBlock;
        public IBlock Set => setBlock;

        #endregion

        #region Constructor

        public CodeProperty() { }

        public CodeProperty(Type type, string name, string get)
            : this(AccessModifier.Public, type.FullName, name, get, null) { }

        public CodeProperty(string type, string name, string get)
            : this(AccessModifier.Public, type, name, get, null) { }

        public CodeProperty(Type type, string name, IBlock get)
            : this(AccessModifier.Public, type.FullName, name, get, null) { }

        public CodeProperty(string type, string name, IBlock get)
            : this(AccessModifier.Public, type, name, get, null) { }

        public CodeProperty(AccessModifier accessModifier, Type type, string name, string get)
            : this(accessModifier, type.FullName, name, get, null) { }

        public CodeProperty(AccessModifier accessModifier, string type, string name, string get)
            : this(accessModifier, type, name, get, null) { }

        public CodeProperty(AccessModifier accessModifier, Type type, string name, IBlock get)
            : this(accessModifier, type.FullName, name, get, null) { }

        public CodeProperty(AccessModifier accessModifier, string type, string name, IBlock get)
            : this(accessModifier, type, name, get, null) { }

        public CodeProperty(AccessModifier accessModifier, Type type, string name, string get, string set, AccessModifier setAccessModififer = AccessModifier.None)
            : this(accessModifier, type.FullName, name, (string.IsNullOrEmpty(get) ? null : new CodeBlock(get)), (string.IsNullOrEmpty(set) ? null : new CodeBlock(set)), setAccessModififer) { }

        public CodeProperty(AccessModifier accessModifier, string type, string name, string get, string set, AccessModifier setAccessModififer = AccessModifier.None)
            : this(accessModifier, type, name, (string.IsNullOrEmpty(get) ? null : new CodeBlock(get)), (string.IsNullOrEmpty(set) ? null : new CodeBlock(set)), setAccessModififer) { }

        public CodeProperty(AccessModifier accessModifier, Type type, string name, IBlock get, IBlock set, AccessModifier setAccessModifier = AccessModifier.None)
            : this(accessModifier, type.FullName, name, get, set, setAccessModifier) { }

        public CodeProperty(AccessModifier accessModifier, string type, string name, IBlock get, IBlock set, AccessModifier setAccessModifier = AccessModifier.None) {
            this.accessModifier = accessModifier;
            this.type = type;
            this.name = name;
            this.getBlock = get;
            this.setBlock = set;
            this.setAccessModifier = setAccessModifier;
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            string getPrefix = "get";
            string setPrefix = "set";
            if(accessModifier.HasFlag(AccessModifier.Event)) {
                if(setBlock == null || getBlock == null) {
                    throw new Exception("An event property need both add and remove block. Get == Add, Set == Remove");
                }
                getPrefix = "add";
                setPrefix = "remove";
            }
            else {
                if(setBlock == null && getBlock.IsSingleLine) {
                    return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)} {getBlock.GetCode(0)}";
                }
                if(getBlock == null && setBlock.IsSingleLine) {
                    return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)} {setBlock.GetCode(0)}";
                }
            }

            if(setAccessModifier != AccessModifier.None && accessModifier != setAccessModifier) {
                var priority = GenUtility.CalculateRestrictivePriority(accessModifier, setAccessModifier);
                if(priority == accessModifier)
                    return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)} {{{GenUtility.NEWLINE}{InsertBlock(getPrefix, getBlock, indent)}{InsertBlock(setAccessModifier, setPrefix, setBlock, indent)}{GenUtility.Indent(indent)}}}";
                else
                    return $"{GenUtility.Indent(indent)}{GenUtility.Generate(setAccessModifier)}{type} {GenUtility.VerifyName(name)} {{{GenUtility.NEWLINE}{InsertBlock(accessModifier, getPrefix, getBlock, indent)}{InsertBlock(setPrefix, setBlock, indent)}{GenUtility.Indent(indent)}}}";
            }
            else {
                return $"{GenUtility.Indent(indent)}{GenUtility.Generate(accessModifier)}{type} {GenUtility.VerifyName(name)} {{{GenUtility.NEWLINE}{InsertBlock(getPrefix, getBlock, indent)}{InsertBlock(setPrefix, setBlock, indent)}{GenUtility.Indent(indent)}}}";
            }
        }

        private string InsertBlock(string preGetSet, IBlock block, int indent) {
            if(block == null)
                return "";
            return $"{GenUtility.Indent(indent + 1)}{preGetSet} {block.GetCode(indent + 2)}";
        }

        private string InsertBlock(AccessModifier accessModififer, string preGetSet, IBlock block, int indent) {
            if(block == null)
                return "";
            return $"{GenUtility.Indent(indent + 1)}{GenUtility.Generate(accessModifier)}{preGetSet} {block.GetCode(indent + 2)}";
        }

        #endregion
    }
}
