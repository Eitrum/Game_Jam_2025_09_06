using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML {

    public enum TMLDocumentType {
        Unknown = 0,
        Json = 1,
        Xml = 2,
        Binary = 3,
    }

    public sealed class TMLDocument : IDisposable {

        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG+ "[TMLDocument]</color> - ";
        private string path;
        private string text;
        private byte[] data;
        private TMLDocumentType documentType;
        private TMLNode root;

        #endregion

        #region Properties

        public string Path => path;
        public string Text => text;
        public IReadOnlyList<byte> Data => data;
        public TMLDocumentType DocumentType {
            get => documentType;
            set => documentType = value;
        }

        public TMLNode Root {
            get {
                if(root == null)
                    Load();
                return root;
            }
            set {
                this.root = value;
            }
        }

        #endregion

        #region Constructor

        public TMLDocument() { }
        
        public TMLDocument(string path) {
            this.path = path;
        }
        
        public TMLDocument(string path, TMLDocumentType type) {
            this.path = path;
            this.documentType = type;
        }

        public TMLDocument(string path, string text) {
            this.path = path;
            this.text = text;
        }

        public TMLDocument(string path, byte[] data) {
            this.path = path;
            this.data = data;
        }

        public TMLDocument(string path, TMLNode root) {
            this.path = path;
            this.root = root;
        }

        public TMLDocument(TMLNode root) {
            this.root = root;
        }

        public void Dispose() {
            this.root?.Dispose(); // Causes recursive dispose
            this.text = null;
            this.path = null;
        }

        #endregion

        #region Load Generic

        public void Load() {
            documentType = GetTypeFromPath();
            if(documentType == TMLDocumentType.Unknown) {
                UnityEngine.Debug.LogWarning(TAG + $"Unable to read file type from '{path}'");
                return;
            }
            switch(documentType) {
                case TMLDocumentType.Json:
                    LoadFromXml();
                    break;
                case TMLDocumentType.Xml:
                    LoadFromXml();
                    break;
                case TMLDocumentType.Binary:

                    break;
            }
        }

        private TMLDocumentType GetTypeFromPath() {
            if(string.IsNullOrEmpty(path))
                return TMLDocumentType.Unknown;
            if(!System.IO.File.Exists(path))
                return TMLDocumentType.Unknown;
            try {
                using(var stream = System.IO.File.OpenRead(path))
                using(var reader = new System.IO.StreamReader(stream)) {
                    char[] buffer = new char[1];
                    int n = reader.ReadBlock(buffer, 0, 1);
                    if(n == 0)
                        return TMLDocumentType.Unknown;
                    switch(buffer[0]) {
                        case '<': return TMLDocumentType.Xml;
                        case '{': return TMLDocumentType.Json;
                    }
                    return TMLDocumentType.Binary; // Binary is always fallback if file has data.
                }
            }
            catch(System.Exception e) {
                UnityEngine.Debug.LogException(e);
                return TMLDocumentType.Unknown;
            }
        }

        #endregion

        #region Load (Xml)

        public void LoadFromXml() {
            if(string.IsNullOrEmpty(text)) {
                if(string.IsNullOrEmpty(path)) {
                    UnityEngine.Debug.LogWarning(TAG + $"Unable to load xml data due to no file path.");
                    return;
                }

                try {
                    if(!System.IO.File.Exists(path)) {
                        UnityEngine.Debug.LogWarning(TAG + $"Unable to load xml data from path '{path}'");
                        return;
                    }
                    text = System.IO.File.ReadAllText(path);
                }
                catch(Exception e) {
                    UnityEngine.Debug.LogException(e);
                    return;
                }
            }
            LoadFromXml(text);
        }

        public void LoadFromXml(string xml) {
            root = TML.TMLParser.Parse(xml);
            documentType = TMLDocumentType.Xml;
        }

        #endregion

        #region Load (Json)

        public void LoadFromJson() {
            if(string.IsNullOrEmpty(text)) {
                if(string.IsNullOrEmpty(path)) {
                    UnityEngine.Debug.LogWarning(TAG + $"Unable to load Json data due to no file path.");
                    return;
                }

                try {
                    if(!System.IO.File.Exists(path)) {
                        UnityEngine.Debug.LogWarning(TAG + $"Unable to load Json data from path '{path}'");
                        return;
                    }
                    text = System.IO.File.ReadAllText(path);
                }
                catch(Exception e) {
                    UnityEngine.Debug.LogException(e);
                    return;
                }
            }
            LoadFromJson(text);
        }

        public void LoadFromJson(string json) {
            if(TML.TMLJsonParser.TryParse(json, out root)) {
                documentType = TMLDocumentType.Json;
            }
        }

        #endregion

        #region Load (Binary)

        public void LoadFromBinary() {
            if(string.IsNullOrEmpty(text)) {
                if(string.IsNullOrEmpty(path)) {
                    UnityEngine.Debug.LogWarning(TAG + $"Unable to load Binary data due to no file path.");
                    return;
                }

                try {
                    if(!System.IO.File.Exists(path)) {
                        UnityEngine.Debug.LogWarning(TAG + $"Unable to load Binary data from path '{path}'");
                        return;
                    }
                    data = System.IO.File.ReadAllBytes(path);
                }
                catch(Exception e) {
                    UnityEngine.Debug.LogException(e);
                    return;
                }
            }
            LoadFromBinary(data);
        }

        public void LoadFromBinary(byte[] data) {
            documentType = TMLDocumentType.Binary;
            throw new Exception(TAG + " Binary loading is not implemented yet");
        }

        #endregion

        #region Save

        public void SaveToPath(string path, TMLDocumentType documentType) {
            this.documentType = documentType;
            SaveToPath(path);
        }

        public void SaveToPathAsXml() {
            this.documentType = TMLDocumentType.Xml;
            SaveToPath();
        }

        public void SaveToPathAsXml(string path) {
            this.documentType = TMLDocumentType.Xml;
            SaveToPath(path);
        }

        public void SaveToPathAsBinary() {
            this.documentType = TMLDocumentType.Binary;
            SaveToPath();
        }

        public void SaveToPathAsBinary(string path) {
            this.documentType = TMLDocumentType.Binary;
            SaveToPath(path);
        }

        public void SaveToPathAsJson() {
            this.documentType = TMLDocumentType.Json;
            SaveToPath();
        }

        public void SaveToPathAsJson(string path) {
            this.documentType = TMLDocumentType.Json;
            SaveToPath(path);
        }

        public void SaveToPath(string path) {
            this.path = path;
            SaveToPath(false);
        }

        public void SaveToPath(string path, bool forceRewrite) {
            this.path = path;
            SaveToPath(forceRewrite);
        }

        public void SaveToPath()
            => SaveToPath(false);

        public void SaveToPath(bool forceRewrite) {
            if(string.IsNullOrEmpty(path)) {
                Debug.LogWarning(TAG + "Can't save to path as no path is written");
                return;
            }
            try {
                switch(documentType) {
                    case TMLDocumentType.Xml: {
                            if(forceRewrite || string.IsNullOrEmpty(text))
                                text = TMLParser.ToString(root, true);
                            System.IO.File.WriteAllText(path, text);
                        }
                        break;
                    case TMLDocumentType.Json: {
                            //if(string.IsNullOrEmpty(text))
                            //text = TMLJsonParser.ToString(root, true);
                            //System.IO.File.WriteAllText(path, text);
                            Debug.LogError(TAG + "Json writer not implemented!");
                        }
                        break;
                    case TMLDocumentType.Binary: {
                            Debug.LogError(TAG + "Binary writer not implemented!");
                        }
                        break;
                    case TMLDocumentType.Unknown: {
                            Debug.LogError(TAG + "Unable to save file to path due to missing document type.");
                            return;
                        }
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Write

        public void WriteToXml() {
            if(root == null) {
                Debug.LogWarning(TAG + "No root exist to write");
                return;
            }
            text = TMLParser.ToString(root, true);
        }

        public void WriteToXml(bool beautify) {
            if(root == null) {
                Debug.LogWarning(TAG + "No root exist to write");
                return;
            }
            text = TMLParser.ToString(root, beautify);
        }

        public void WriteToJson() {
            if(root == null) {
                Debug.LogWarning(TAG + "No root exist to write");
                return;
            }
            Debug.LogError(TAG + "Json writer not implemented");
        }

        public void WriteToJson(bool beautify) {
            if(root == null) {
                Debug.LogWarning(TAG + "No root exist to write");
                return;
            }
            Debug.LogError(TAG + "Json writer not implemented");
        }

        #endregion

        #region Utility

        public TMLNode CreateNewRoot()
            => CreateNewRoot(false);

        public TMLNode CreateNewRoot(bool disposeOld) {
            if(disposeOld)
                root?.Dispose();
            root = TMLNode.CreateNode("root");
            return root;
        }

        public ReadOnlySpan<char> GetSpan(int start, int length) => text.AsSpan(start, length);
        public ReadOnlySpan<char> GetSpanStartEnd(int start, int end) => GetSpan(start, end - start + 1);

        #endregion
    }
}
