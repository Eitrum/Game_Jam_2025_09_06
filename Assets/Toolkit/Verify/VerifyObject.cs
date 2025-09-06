using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Verify
{
    [CreateAssetMenu(fileName = "new verify object", menuName = "Toolkit/Verify/Verify Object")]
    public class VerifyObject : ScriptableObject
    {
        #region Variables

        [SerializeField] private List<string> paths = new List<string>() { "Assets/" };
        [SerializeField] private List<string> blacklist = new List<string>() { "Assets/Toolkit/" };

        #endregion

        #region Properties

        public IReadOnlyList<string> Paths => paths;
        public IReadOnlyList<string> Blacklist => blacklist;

        #endregion

        #region Add / Remove

        public void AddToBlacklist(string path) => blacklist.Add(path);
        public bool RemoveFromBlacklist(string path) => blacklist.Remove(path);

        public bool AddPath(string path) {
            foreach(var p in paths) {
                if(path.StartsWith(p)) {
                    Debug.LogWarning($"Attempting to add path that is a child of other path @ '{p}' -> '{path}'");
                    return false;
                }
            }
            for(int i = paths.Count - 1; i >= 0; i--) {
                if(paths[i].StartsWith(path)) {
                    Debug.LogWarning($"Attempting to add a parent path of an other path @ '{path}' -> '{paths[i]}'. Will remove the child path.");
                    paths.RemoveAt(i);
                }
            }

            paths.Add(path);
            return true;
        }

        public bool RemovePath(string path) => paths.Remove(path);

        #endregion
    }
}
