using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolkit.CodeAnalysis {
    public class CodeDataHierarchy {

        public enum CodeDataMode {
            Hierarchy,
            Namespace,
            Flat
        }

        private CodeDataContainer root = null;
        private CodeDataMode mode = CodeDataMode.Hierarchy;

        private CodeDataHierarchy(CodeDataContainer root, CodeDataMode mode) {
            this.root = root;
            this.mode = mode;
        }

        public CodeDataContainer Root => root;
        public CodeDataMode Mode {
            get => mode;
        }

        public static CodeDataHierarchy Build(MonoScript[] scripts, CodeDataMode mode) {
            var codeDataArray = scripts.Select(x => new CodeData(x)).ToArray();

            if(mode == CodeDataMode.Flat) {
                CodeDataContainer container = new CodeDataContainer("All");
                for(int i = 0, length = codeDataArray.Length; i < length; i++) {
                    container.Add(codeDataArray[i]);
                }
                container.Analyze();
                return new CodeDataHierarchy(container, mode);
            }
            if(mode == CodeDataMode.Hierarchy) {
                CodeDataContainer container = new CodeDataContainer("Project");
                for(int i = 0, length = codeDataArray.Length; i < length; i++) {
                    var codeData = codeDataArray[i];
                    var path = codeData.Path;
                    var folders = path.Split('/', '\\');
                    var tempContainer = container;
                    for(int f = 0; f < folders.Length - 1; f++) {
                        var tC = tempContainer.GetSubContainer(folders[f]);
                        if(tC == null) {
                            tC = new CodeDataContainer(folders[f]);
                            tempContainer.Add(tC);
                        }
                        tempContainer = tC;
                    }
                    tempContainer.Add(codeData);
                }
                container.Analyze();
                return new CodeDataHierarchy(container, mode);
            }

            return default;
        }
    }

    public class CodeDataContainer {

        #region Variables

        private string containerName = "";
        private int linesOfCode = 0;
        private int commentLines = 0;
        private int codeLines = 0;
        private int characters = 0;
        internal bool foldout = false;

        private List<CodeDataContainer> subContainers = new List<CodeDataContainer>();
        private List<CodeData> codeFiles = new List<CodeData>();

        #endregion

        #region Properties

        public string Name => containerName;
        public int LinesOfCode => linesOfCode;
        public int CommentLines => commentLines;
        public int CodeLines => codeLines;
        public int EmptyLines => linesOfCode - (codeLines + commentLines);
        public int Characters => characters;
        public float CodeDensity => (characters / (linesOfCode * CodeData.CODE_DENSITY_WIDTH));

        public int CodeDataContainerCount => subContainers.Count;
        public int CodeDataCount => codeFiles.Count;

        #endregion

        #region Constructor

        public CodeDataContainer(string name) {
            containerName = name;
        }

        #endregion

        #region Analyze

        public void Analyze() => Analyze(true);

        public void Analyze(bool recursive) {
            if(recursive) {
                for(int i = subContainers.Count - 1; i >= 0; i--) {
                    subContainers[i].Analyze(recursive);
                }
            }
            linesOfCode = subContainers.Sum(x => x.linesOfCode) + codeFiles.Sum(x => x.LinesOfCode);
            commentLines = subContainers.Sum(x => x.commentLines) + codeFiles.Sum(x => x.CommentLines);
            codeLines = subContainers.Sum(x => x.codeLines) + codeFiles.Sum(x => x.CodeLines);
            characters = subContainers.Sum(x => x.characters) + codeFiles.Sum(x => x.Characters);
        }

        #endregion

        #region Add/remove

        public void Add(CodeDataContainer container) {
            subContainers.Add(container);
        }

        public void Remove(CodeDataContainer container) {
            subContainers.Remove(container);
        }

        public void Add(CodeData code) {
            codeFiles.Add(code);
        }

        public void Remove(CodeData code) {
            codeFiles.Remove(code);
        }

        public CodeDataContainer GetSubContainer(string name) {
            return subContainers.FirstOrDefault(x => x.containerName == name);
        }

        public CodeData GetCodeData(string name) {
            return codeFiles.FirstOrDefault(x => x.Name == name);
        }

        public CodeData GetCodeData(string name, bool recursive) {
            if(recursive) {
                var code = codeFiles.FirstOrDefault(x => x.Name == name);
                if(code == null) {
                    code = subContainers.Select(x => x.GetCodeData(name, recursive)).FirstOrDefault(x => x != null);
                }
                return code;
            }
            return codeFiles.FirstOrDefault(x => x.Name == name);
        }

        #endregion

        #region Getters

        public CodeDataContainer GetCodeDataContainer(int index) {
            return subContainers[index];
        }

        public CodeData GetCodeData(int index) {
            return codeFiles[index];
        }

        #endregion
    }
}
