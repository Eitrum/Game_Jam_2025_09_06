using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.Debugging;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class PanelManagerDebugCommands {

        public enum SearchMode {
            Storage,
            Asset,
            Resources,
        }

        private const string BASE_COMMAND = "panelsystem ";

        [RuntimeInitializeOnLoadMethod]
        private static void Init() {
            Commands.Add(BASE_COMMAND + "list", ListPanels);
            Commands.Add<int>(BASE_COMMAND + "list <?>", ListPanelsAtIndex);
            Commands.Add(BASE_COMMAND + "list storage", ListStorage);

            // Remove
            Commands.Add<int>(BASE_COMMAND + "close <?>", RemoveByIndex);
            Commands.Add<int, int>(BASE_COMMAND + "close <?> <?>", RemoveByIndex);
            Commands.Add<string>(BASE_COMMAND + "close <?>", RemoveByName);
            Commands.Add<int, string>(BASE_COMMAND + "close <?> <?>", RemoveByName);

            // Add
            Commands.Add<string>(BASE_COMMAND + "add <?>", Add);
            Commands.Add<SearchMode, string>(BASE_COMMAND + "add <?> <?>", Add);
            Commands.Add<int, string>(BASE_COMMAND + "add <?> <?>", Add);
            Commands.Add<int, SearchMode, string>(BASE_COMMAND + "add <?> <?> <?>", Add);

            // GetOrAdd
            Commands.Add<string>(BASE_COMMAND + "getoradd <?>", EnsurePanelExistsByPath);
            Commands.Add<SearchMode, string>(BASE_COMMAND + "getoradd <?> <?>", EnsurePanelExistsByPath);
            Commands.Add<int, string>(BASE_COMMAND + "getoradd <?> <?>", EnsurePanelExistsByPath);
            Commands.Add<int, SearchMode, string>(BASE_COMMAND + "getoradd <?> <?> <?>", EnsurePanelExistsByPath);

            Commands.Add(BASE_COMMAND + "close all", CloseAll);
            Commands.Add<int>(BASE_COMMAND + "close all <?>", CloseAll);
        }

        #region Ensure

        private static void EnsurePanelExistsByPath(string path) {
            EnsurePanelExistsByPath(0, SearchMode.Asset, path);
        }

        private static void EnsurePanelExistsByPath(int index, string path) {
            EnsurePanelExistsByPath(index, SearchMode.Asset, path);
        }

        private static void EnsurePanelExistsByPath(SearchMode mode, string path) {
            EnsurePanelExistsByPath(0, mode, path);
        }

        private static void EnsurePanelExistsByPath(int index, SearchMode mode, string path) {
            GameObject go = null;
            switch(mode) {
                case SearchMode.Asset:
#if UNITY_EDITOR
                    if(!path.EndsWith(".prefab"))
                        path = path + ".prefab";
                    go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else
                    Commands.PrintToConsole("asset mode only works in-editor");
                    return;
#endif
                    break;
                case SearchMode.Resources:
                    go = Resources.Load<GameObject>(path);
                    break;
            }

            if(go == null) {
                Commands.PrintToConsole($"Did not find a panel prefab at path '{path}'");
                return;
            }

            var p = go.GetComponent<Panel>();
            if(p == null) {
                Commands.PrintToConsole($"Prefab is not a panel.");
                return;
            }

            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole($"Manager not found on index '{index}'");
                return;
            }

            manager.GetExistingOrAdd(go);
        }

        #endregion

        #region Close All

        private static void CloseAll() {
            CloseAll(0);
        }

        private static void CloseAll(int index) {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }
            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole("index out of range");
                return;
            }
            manager.CloseAll(true);
        }

        #endregion

        #region List

        private static void ListPanelsAtIndex(int index) {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }
            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole("index out of range");
                return;
            }
            var panels = manager.Panels;
            if(panels.Count == 0)
                Commands.PrintToConsole("no panels found");
            for(int i = 0; i < panels.Count; i++) {
                Commands.PrintToConsole($"{i}. '{panels[i].name}' (id:{panels[i].PanelId}, uid:{panels[i].UniqueId})");
            }
        }

        private static void ListPanels() {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }
            var count = PanelManager.InstanceCount;
            if(count == 1) {
                Commands.PrintToConsole("only 1 panel manager found");
                ListPanelsAtIndex(0);
                return;
            }
            for(int i = 0; i < count; i++) {
                PanelManager.TryGet(i, out var manager);
                Commands.PrintToConsole($"{i}. {manager.name}");
            }
        }

        public static void ListStorage() {
            Commands.PrintToConsole("Panel Storage:");
            int index = 0;
            foreach(var key in PanelStorage.StorageKeys) {
                Commands.PrintToConsole($"{index++}. {key}");
            }
        }

        #endregion

        #region Remove

        private static void RemoveByName(string panelName) {
            RemoveByName(0, panelName);
        }

        private static void RemoveByName(int index, string panelName) {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }
            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole($"Manager not found on index '{index}'");
                return;
            }
            try {
                var p = manager.Panels.FirstOrDefault(x=>x.name == panelName);
                if(p == null) {
                    Commands.PrintToConsole($"panel with name '{panelName}' was not found");
                    return;
                }
                manager.Close(p, true);
            }
            catch(Exception e) {
                Commands.PrintToConsole($"{e.Message}");
            }
        }

        private static void RemoveByIndex(int panelIndex) {
            RemoveByIndex(0, panelIndex);
        }

        private static void RemoveByIndex(int index, int panelIndex) {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }
            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole($"Manager not found on index '{index}'");
                return;
            }
            try {
                var p = manager.Panels[panelIndex];
                manager.Close(p, true);
            }
            catch(Exception e) {
                Commands.PrintToConsole($"{e.Message}");
            }
        }

        #endregion

        #region Add

        private static void Add(string path) {
            Add(0, SearchMode.Storage, path);
        }

        private static void Add(int index, string path) {
            Add(index, SearchMode.Storage, path);
        }

        private static void Add(SearchMode mode, string path) {
            Add(0, mode, path);
        }

        private static void Add(int index, SearchMode mode, string path) {
            if(!PanelManager.Exists) {
                Commands.PrintToConsole("no panel manager exist");
                return;
            }

            if(!PanelManager.TryGet(index, out var manager)) {
                Commands.PrintToConsole($"Manager not found on index '{index}'");
                return;
            }

            GameObject go = null;
            switch(mode) {
                case SearchMode.Storage:
                    manager.Add(path, true);
                    return;
                case SearchMode.Asset:
#if UNITY_EDITOR
                    if(!path.EndsWith(".prefab"))
                        path = path + ".prefab";
                    go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else
                    Commands.PrintToConsole("asset mode only works in-editor");
                    return;
#endif
                    break;
                case SearchMode.Resources:
                    go = Resources.Load<GameObject>(path);
                    break;
            }

            if(go == null) {
                Commands.PrintToConsole($"Did not find a panel prefab at path '{path}'");
                return;
            }

            var p = go.GetComponent<Panel>();
            if(p == null) {
                Commands.PrintToConsole($"Prefab is not a panel.");
                return;
            }

            manager.Add(go);
        }

        #endregion
    }
}
