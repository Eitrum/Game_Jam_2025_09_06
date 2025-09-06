using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolkit.SceneManagement {
    [System.Serializable]
    public struct SceneObject : IBufferSerializable {
        #region Variables

        [SerializeField] private string name;
        [SerializeField] private string path;

        #endregion

        #region Properties

        public int BuildIndex {
            get {
                for(int i = 0, length = SceneManager.sceneCountInBuildSettings; i < length; i++) {
                    if(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i) == path) {
                        return i;
                    }
                }
                return -1;
            }
        }
        public string Name => (!string.IsNullOrEmpty(name)) ? name : throw new System.Exception("Scene object name is null or empty");
        public string Path => path;

        public Scene Scene {
            get => SceneManager.GetSceneByBuildIndex(BuildIndex);
            set => name = value.name;
        }

        #endregion

        #region Constructor

        public SceneObject(string name) {
            this.name = name;
            this.path = SceneManager.GetSceneByName(name).path;
        }

        public SceneObject(Scene scene) {
            this.name = scene.name;
            this.path = scene.path;
        }

        public SceneObject(TMLNode node) {
            name = node.GetString("name");
            path = node.GetString("path");
        }

        #endregion

        #region Conversion

        public static implicit operator Scene(SceneObject obj) {
            return obj.Scene;
        }

        #endregion

        #region Serialize

        public TMLNode CreateTMLNode() {
            TMLNode node = new TMLNode("sceneObject");
            node.AddProperty("name", name);
            node.AddProperty("path", path);
            return node;
        }

        public void Serialize(TMLNode node) {
            node.AddProperty("name", name);
            node.AddProperty("path", path);
        }

        public void Deserialize(TMLNode node) {
            name = node.GetString("name");
            path = node.GetString("path");
        }

        public void Serialize(IBuffer buffer) {
            buffer.Write(name);
            buffer.Write(path);
        }

        public void Deserialize(IBuffer buffer) {
            name = buffer.ReadString();
            path = buffer.ReadString();
        }

        #endregion
    }
}
