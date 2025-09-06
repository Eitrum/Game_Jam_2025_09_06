using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.CodeGenerator {
    public class CodeBlock : IBlock {

        #region Variables

        private List<ICode> codeNodes = new List<ICode>();

        #endregion

        #region Properties

        public IReadOnlyList<ICode> Nodes => codeNodes;
        public bool IsSingleLine {
            get {
                if(codeNodes.Count == 1) {
                    var codeSplit = codeNodes[0].GetCode(0).Split('\n');
                    if(codeSplit.Length == 2 && string.IsNullOrEmpty(codeSplit[1])) {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region Constructor

        public CodeBlock() { }

        public CodeBlock(string singleLineCode) {
            codeNodes.Add(new CodeCustom(singleLineCode));
        }

        public CodeBlock(IReadOnlyList<string> linesOfCode) {
            codeNodes.Add(new CodeCustom(string.Join(GenUtility.NEWLINE, linesOfCode)));
        }

        public CodeBlock(ICode code) {
            codeNodes.Add(code);
        }

        public CodeBlock(IReadOnlyList<ICode> codes) {
            for(int i = 0, length = codes.Count; i < length; i++) {
                codeNodes.Add(codes[i]);
            }
        }

        #endregion

        #region Add Code (Custom)

        public void AddCode(ICode code) {
            codeNodes.Add(code);
        }

        public CodeCustom AddCode(string code) {
            var customCode = new CodeCustom(code);
            codeNodes.Add(customCode);
            return customCode;
        }

        public CodeCustom[] AddCode(IReadOnlyList<string> linesOfCode) {
            var code = new CodeCustom[linesOfCode.Count];
            for(int i = 0, length = linesOfCode.Count; i < length; i++) {
                var cc = new CodeCustom(linesOfCode[i]);
                codeNodes.Add(cc);
                code[i] = cc;
            }

            return code;
        }

        #endregion

        #region ICode Impl

        public string GetCode(int indent) {
            if(codeNodes.Count == 0) {
                return $"{{ }}{GenUtility.NEWLINE}";
            }
            if(codeNodes.Count == 1) {
                var code = codeNodes[0].GetCode(0);
                var split = code.Split('\n');
                if(split.Length == 1) {
                    return $"=> {code.Replace("return ", "")}{GenUtility.NEWLINE}";
                }
                else if(split.Length == 2 && string.IsNullOrEmpty(split[1])) {
                    return $"=> {code.Replace("return ", "")}";
                }
            }

            return $"{{{GenUtility.NEWLINE}{GenUtility.Process(codeNodes, indent)}{GenUtility.NEWLINE}{GenUtility.Indent(indent - 1)}}}{GenUtility.NEWLINE}";
        }

        #endregion

        #region Switch Block

        public static CodeBlock CreateReturnSwitchBlock(string inputName, IReadOnlyList<string> cases, IReadOnlyList<string> returnValues, string endReturn) {
            int length = Math.Min(cases.Count, returnValues.Count);
            if(length == 0)
                return CodeBlock.Empty();
            List<string> switchString = new List<string>();
            switchString.Add($"switch({inputName}) {{");
            for(int i = 0; i < length; i++) {
                switchString.Add($"{GenUtility.INDENT}case {cases[i]}: return {returnValues[i]};");
            }
            switchString.Add("}");
            if(!string.IsNullOrEmpty(endReturn))
                switchString.Add(endReturn);
            return new CodeBlock(switchString);
        }

        public static CodeBlock CreateAssignSwitchBlock(string inputName, string valueName, IReadOnlyList<string> cases, IReadOnlyList<string> variables) {
            int length = Math.Min(cases.Count, variables.Count);
            if(length == 0)
                return CodeBlock.Empty();
            List<string> switchString = new List<string>();
            switchString.Add($"switch({inputName}) {{");
            for(int i = 0; i < length; i++) {
                switchString.Add($"{GenUtility.INDENT}case {cases[i]}: {variables[i]} = {valueName};");
                switchString.Add($"{GenUtility.INDENT}break;");
            }
            switchString.Add("}");
            return new CodeBlock(switchString);
        }

        public static CodeBlock CreateAssignAndReturnSwitchBlock(string inputName, string valueName, IReadOnlyList<string> cases, IReadOnlyList<string> variables, IReadOnlyList<string> returnValues, string endReturn) {
            int length = Math.Min(cases.Count, variables.Count);
            if(length == 0)
                return CodeBlock.Empty();
            List<string> switchString = new List<string>();
            switchString.Add($"switch({inputName}) {{");
            for(int i = 0; i < length; i++) {
                switchString.Add($"{GenUtility.INDENT}case {cases[i]}: {variables[i]} = {valueName};");
                switchString.Add($"{GenUtility.INDENT}return {returnValues[i]};");
            }
            switchString.Add("}");
            if(!string.IsNullOrEmpty(endReturn))
                switchString.Add(endReturn);
            return new CodeBlock(switchString);
        }

        #endregion

        #region Empty

        public static CodeBlock Empty() {
            return new CodeBlock();
        }

        #endregion
    }
}
