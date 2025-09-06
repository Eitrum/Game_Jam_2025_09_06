using System;
using System.Collections.Generic;

namespace Toolkit.CodeGenerator {
    public class CodeUsingDirective : IUsingDirective {

        #region Variables

        private string @namespace = "";
        private bool isStatic = false;

        #endregion

        #region Properties

        public string Namespace => @namespace;
        public bool IsStatic => isStatic;

        #endregion

        #region Constructor

        public CodeUsingDirective() {
            // Handle this case
        }

        public CodeUsingDirective(string @namespace) {
            this.@namespace = @namespace;
        }

        public CodeUsingDirective(string @namespace, bool isStatic) : this(@namespace) {
            this.isStatic = isStatic;
        }

        #endregion

        #region ICodeGen Impl

        public string GetCode(int indent) {
            if(isStatic) {
                var name = @namespace.Split('.');
                return $"{GenUtility.Indent(indent)}using static {name[name.Length - 1]} = {@namespace};";
            }
            return $"{GenUtility.Indent(indent)}using {GenUtility.VerifyName(@namespace)};";
        }

        #endregion
    }
}
