using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [DefaultExecutionOrder(-200)]
    [AddComponentMenu("Toolkit/Container (Reference)")]
    public class Container : MonoBehaviour
    {
        #region Variables

        private static Dictionary<int, Container> container = new Dictionary<int, Container>();

        [SerializeField] private string containerName = "";
        [SerializeField] private bool keepAlive = false;

        #endregion

        #region Properties

        public string ContainerName => containerName;
        public int ContainerId => containerName.GetHash32();

        #endregion

        #region Unity Method

        private void Awake() {
            if(keepAlive)
                DontDestroyOnLoad(gameObject);
            if(string.IsNullOrEmpty(containerName))
                containerName = name;
            var id = containerName.GetHash32();
            if(container.ContainsKey(id)) {
                var con = container[id];
                if(con == null) { // Check if container is destroyed or not.
                    container.Remove(id);
                }
                else {
                    Debug.LogError($"Container already exists with name '{containerName}'");
                    return;
                }
            }
            container.Add(id, this);
        }

        private void OnDestroy() {
            if(container.TryGetValue(ContainerId, out Container con) && con == this) {
                container.Remove(ContainerId);
            }
        }

        #endregion

        #region Utility

        public void DestroyAllChildren() {
            transform.DestroyAllChildren();
        }

        public void DetachAllChildren() {
            transform.DetachChildren();
        }

        #endregion

        #region Get Container

        public static Container GetContainer(string name) {
            var id = name.GetHash32();
            if(container.TryGetValue(id, out Container con)) {
                if(con == null)
                    container.Remove(id);
                else
                    return con;
            }
            var go = new GameObject(name);
            return go.AddComponent<Container>();
        }

        public static IEnumerable<Container> GetAllContainers() => container.Values;

        #endregion
    }
}
