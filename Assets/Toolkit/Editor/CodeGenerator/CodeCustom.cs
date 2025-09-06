using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.CodeGenerator {
    public class CodeCustom : ICode, IContainer {

        private string code = "";

        public CodeCustom() { }

        public CodeCustom(string code) {
            this.code = code;
        }

        public string GetCode(int indent) {
            if(code.Contains(GenUtility.NEWLINE)) {
                var split = code.Split(GenUtility.NEWLINE[0]);
                for(int i = 1; i < split.Length; i++) {
                    split[i] = GenUtility.Indent(indent) + split[i];
                }
                return $"{GenUtility.Indent(indent)}{string.Join(GenUtility.NEWLINE, split)}{GenUtility.NEWLINE}";
            }
            else {
                return $"{GenUtility.Indent(indent)}{code}{GenUtility.NEWLINE}";
            }
        }
    }
}
