using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Utility
{
    public class PrefabPainterContainerCollection : MonoBehaviour
    {
        [SerializeField] private List<PrefabPainterContainer> containers = new List<PrefabPainterContainer>();

        public PrefabPainterContainer GetContainer(string name) {
            foreach(var con in containers) {
                if(con.ContainerName == name) {
                    return con;
                }
            }
            var go = new GameObject($"Painter ({name})");
            go.transform.SetParent(this.transform);
            var newCon = go.AddComponent<PrefabPainterContainer>();
            newCon.ContainerName = name;
            containers.Add(newCon);
            return newCon;
        }

        [ContextMenu("Clear Empty")]
        public void ClearEmptyContainers() {
            for(int i = containers.Count - 1; i >= 0; i--) {
                if(containers[i] == null) {
                    containers.RemoveAt(i);
                }
                else if(containers[i].ChildCount == 0) {
                    var go = containers[i].gameObject;
                    DestroyImmediate(containers[i]);
                    DestroyImmediate(go);
                    containers.RemoveAt(i);
                }
            }
        }

        [ContextMenu("Move To Own Container")]
        public void MoveAndClear() {
            var go = new GameObject("new container");
            var t = go.transform;
            t.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            foreach(var con in containers) {
                for(int i = con.ChildCount - 1; i >= 0; i--) {
                    con.transform.GetChild(i).SetParent(t);
                }
            }
            ClearEmptyContainers();

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(go.scene);
#endif
        }
    }
}
