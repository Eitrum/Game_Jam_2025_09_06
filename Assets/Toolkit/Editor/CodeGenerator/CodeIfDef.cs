using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.CodeGenerator
{
    public class CodeIfDef : IContainer
    {
        #region Private class (Code Directive)

        private class CodeDirective : ICode
        {
            private string directive;
            private string type;

            public string Directive => directive;

            public CodeDirective(string type, string directive) {
                this.type = type;
                this.directive = directive;
            }

            public string GetCode(int indent) {
                return $"{GenUtility.Indent(indent)}#{type} {directive}";
            }
        }

        #endregion

        #region Variables

        private List<ICode> codeNodes = new List<ICode>();

        #endregion

        #region Constructor

        public CodeIfDef(string directive) {
            codeNodes.Add(new CodeDirective("if", directive));
        }

        #endregion

        #region Add

        public T Add<T>(T code) where T : ICode {
            codeNodes.Add(code);
            return code;
        }

        public T Insert<T>(T code, string directive) where T : ICode {
            bool foundDirective = false;
            for(int i = 0, length = codeNodes.Count; i < length; i++) {
                var node = codeNodes[i];
                if(foundDirective && node is CodeDirective) {
                    codeNodes.Insert(i - 1, code);
                    break;
                }
                else if(node is CodeDirective dir && dir.Directive == directive) {
                    foundDirective = true;
                }
            }
            return code;
        }

        #endregion

        #region Add Directives

        public void AddElseIf(string directive) {
            codeNodes.Add(new CodeDirective("elif", directive));
        }

        public void AddElse() {
            codeNodes.Add(new CodeDirective("else", ""));
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            return $"{GenUtility.Process(codeNodes, indent)}{GenUtility.NEWLINE}{GenUtility.Indent(indent)}#endif{GenUtility.NEWLINE}";
        }

        #endregion
    }
}
