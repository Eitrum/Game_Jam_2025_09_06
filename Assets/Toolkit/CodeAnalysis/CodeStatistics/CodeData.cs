using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit.CodeAnalysis
{
    public class CodeData
    {

        #region Const

        private const string TAG = "<color=cyan>[Code Data]</color> - ";
        public const int CODE_DENSITY_WIDTH = 80;

        #endregion

        #region Variables

        private TextAsset script = null;

        private int linesOfCode = 0;
        private int commentLines = 0;
        private int codeLines = 0;
        private int characters = 0;

        #endregion

        #region Properties

        public bool IsValid => script != null;
        public TextAsset Asset => script;
        public MonoScript Script => script as MonoScript;
        public string Code => script.text;
        public string Path => AssetDatabase.GetAssetPath(script);

        public string Name => script.name;
        public int LinesOfCode => linesOfCode;
        public int CommentLines => commentLines;
        public int CodeLines => codeLines;
        public int EmptyLines => linesOfCode - (codeLines + commentLines);
        public int Characters => characters;
        public float CodeDensity => (float)(characters / (double)(linesOfCode * CODE_DENSITY_WIDTH));

        #endregion

        #region Constructor

        public CodeData(TextAsset textAsset) {
            this.script = textAsset;
            Analyze();
        }

        public CodeData(MonoScript monoScript) {
            this.script = monoScript;
            Analyze();
        }

        public CodeData(string path) {
            this.script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            Analyze();
        }

        #endregion

        #region Analyze

        public void Analyze() {
            if(!IsValid) {
                Debug.LogError(TAG + "Trying to analyze an invalid code data file");
            }

            var text = script.text;
            Analyze(text);
        }

        private void Analyze(string text) {
            var lines = text.Split('\n');
            linesOfCode = lines.Length;
            characters = 0;
            commentLines = 0;
            codeLines = 0;
            for(int i = 0; i < linesOfCode; i++) {
                var line = lines[i].Trim();
                if(string.IsNullOrEmpty(line))
                    continue;

                characters += line.Length;

                if(line.StartsWith("#") || line.StartsWith("//"))
                    commentLines++;
                else
                    codeLines++;
            }
        }

        public static CodeData Analyze(MonoScript monoScript) {
            return new CodeData(monoScript);
        }

        #endregion
    }
}
