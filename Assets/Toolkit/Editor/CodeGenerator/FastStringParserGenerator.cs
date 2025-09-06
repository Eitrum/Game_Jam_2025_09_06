using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.CodeGenerator {
    public static class FastStringParserGenerator {
        public static string BuildFromEnum<T>() where T : System.Enum {
            TMLNode node = new TMLNode("root");
            var ctt = FastEnum.GetNames<T>();
            for(int i = 0; i < 32; i++) {
                var list = ctt.Where(x => x.Length == i).ToList();
                if(list.Count == 0)
                    continue;
                var np = node.AddNode($"{i}");
                FillNodesFromEnum<T>(np, list, 0);
            }

            {
                var list = ctt.Where(x => x.Length >=32).ToList();
                if(list.Count > 0) {
                    var np = node.AddNode($"32+");
                    FillNodesFromEnum<T>(np, list, 0);
                }
            }
            Debug.Log(Toolkit.IO.TML.TMLParser.ToString(node, true));
            StringBuilder sb = new StringBuilder();
            GenerateCodeBlock(node, sb);
            var codeblock = sb.ToString();

            CodeFile cf = new CodeFile($"FastStringParse_{typeof(T).Name}");
            cf.TagFileAsGenerated = true;
            var ns = cf.AddNamespace(typeof(T).Namespace);
            var cc = ns.AddClass(new CodeClass(AccessModifier.PublicStatic,$"FastStringParse_{typeof(T).Name}"));
            cc.AddMethod(new CodeMethod(AccessModifier.PublicStatic, typeof(T).FullName, $"Get{typeof(T).Name}", new CodeVariable("System.ReadOnlySpan<char>", "input"), new CodeBlock(codeblock.Split('\n'))));
            var res = cf.CreateFile("Assets");
            return res;
        }

        private static void FillNodesFromEnum<T>(TMLNode parent, List<string> names, int index) {
            if(names.Count > 0) {
                if(names[0].Length <= index) {
                    parent.AddProperty("value", $"{typeof(T).FullName}.{names[0]}");
                    return;
                }
                for(int i = (int)'a', len = (int)'z'; i <= len; i++) {
                    var list = names.Where(x => char.ToLower(x[index]) == (char)i).ToList();
                    if(list.Count == 0)
                        continue;
                    var np = parent.AddNode($"_{(char)i}_");
                    FillNodesFromEnum<T>(np, list, index + 1);
                }
            }
        }

        private static void GenerateCodeBlock(TMLNode root, StringBuilder sb) {
            sb.AppendLine("switch(input.Length){");
            var nodes = root.Nodes;
            foreach(var n in nodes) {
                if(int.TryParse(n.Name, out int caseNumber)) {
                    sb.AppendLine($"case {caseNumber}:");
                }
                else { // Should be 32+ 
                    sb.AppendLine("default:");
                }
                sb.AppendLine("{");
                GenerateSubSwitch(n, sb, 0);
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
            sb.AppendLine("return default;");
        }

        private static void GenerateSubSwitch(TMLNode n, StringBuilder sb, int depth) {
            sb.AppendLine($"switch(input[{depth}]){{");

            foreach(var c in n.Nodes) {
                var caseName = c.Name[1];
                sb.AppendLine($"case '{caseName}':");
                sb.AppendLine($"case '{char.ToUpper(caseName)}':{{");

                if(c.HasNodes)
                    GenerateSubSwitch(c, sb, depth + 1);
                else if(c.HasProperties)
                    sb.AppendLine($"return {c.GetString("value")};");
                else
                    sb.AppendLine("return default;"); // SHOULD NOT HAPPEN BUT STILL DOES REEE

                sb.AppendLine($"}}");
            }

            sb.AppendLine("}");
            sb.AppendLine("return default;");
        }
    }
}
