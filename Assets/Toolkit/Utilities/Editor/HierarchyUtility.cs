using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Toolkit.Utility
{
    public static class HierarchyUtility
    {
        #region Game Object Region Creation
        
        [MenuItem("GameObject/Region", priority = -100)]
        public static void CreateRegionObject() {
            CreateRegionObject(Selection.activeTransform);
        }

        [MenuItem("GameObject/Mini Region", priority = -99)]
        public static void CreateMiniRegionObject() {
            CreateMiniRegionObject(Selection.activeTransform);
        }

        public static void CreateRegionObject(Transform parent) {
            GameObject go = new GameObject("----------------[ Region ]----------------");
            go.transform.SetParent(parent, false);
            Selection.activeGameObject = go;
        }

        public static void CreateMiniRegionObject(Transform parent) {
            GameObject go = new GameObject("---[ MiniRegion ]---");
            go.transform.SetParent(parent, false);
            Selection.activeGameObject = go;
        }

        #endregion
    }
}
