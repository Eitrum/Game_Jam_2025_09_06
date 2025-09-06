using UnityEngine;
using UnityEngine.LowLevel;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Toolkit {
    public static class PlayerLoopUtilty {

        public class Node {
            #region Variables

            private string name = string.Empty;
            private bool enabled = true;
            private Type type;
            private Mode mode = Mode.None;
            private Node parent;

            private PlayerLoopSystem cachedSystem;
            private IntPtr nativeFunctionPointer;
            private IntPtr nativeConditionalFunctionPointer;
            private NativeFunction nativeFunction;
            private PlayerLoopSystem.UpdateFunction managedUpdateFunction;

            private List<Node> subSystems = new List<Node>();

            #endregion

            #region Properties

            public string Path {
                get {
                    if(string.IsNullOrEmpty(name))
                        return string.Empty;
                    if(parent != null && !string.IsNullOrEmpty(parent.name))
                        return $"{parent.Path}/{Name}";
                    return Name;
                }
            }
            public string Name => name;
            public bool Enabled {
                get => enabled;
                set {
                    enabled = value;
                }
            }

            public bool EnabledHierarchy {
                get => enabled && (parent?.Enabled ?? true);
            }

            public int Index {
                get {
                    if(parent == null)
                        return 0;
                    return parent.subSystems.IndexOf(this);
                }
            }

            public Type Type => type;
            public Mode Mode { get; internal set; }
            public Node Parent => parent;

            public IReadOnlyList<Node> SubSystems => subSystems;


            /// <summary>
            /// C++ Native Engine Functions
            /// </summary>
            public NativeFunction NativeFunction => nativeFunction;

            /// <summary>
            /// C# Custom update methods
            /// </summary>
            public PlayerLoopSystem.UpdateFunction ManagedFunction => managedUpdateFunction;

            #endregion

            #region Constructor

            private Node() { }
            public Node(PlayerLoopSystem system) : this(null, system) { }
            private Node(Node parent, PlayerLoopSystem system) {
                this.parent = parent;
                this.cachedSystem = system;
                type = system.type;
                name = system.type?.Name ?? string.Empty;
                nativeFunctionPointer = system.updateFunction;
                managedUpdateFunction = system.updateDelegate;
                if(nativeFunctionPointer != IntPtr.Zero) {
                    mode = Mode.Native;
                    if(!TryGetNativeFunction(system, out nativeFunction)) {
                        mode = Mode.NativeUnaccessable;
                        //Debug.LogError(TAG + $"Failed to retrieve native function of {Path}");
                    }
                }
                else if(managedUpdateFunction != null)
                    mode = Mode.Managed;
                else
                    mode = Mode.None;

                if(system.subSystemList != null)
                    foreach(var pls in system.subSystemList)
                        subSystems.Add(new Node(this, pls));
            }

            #endregion

            #region PlayerLoopSystem

            public PlayerLoopSystem GetPlayerLoopSystem() {
                if(!enabled) {
                    return default;
                }

                PlayerLoopSystem pls = cachedSystem;
                pls.subSystemList = subSystems
                    .Where(x => x.enabled)
                    .Select(x => x.GetPlayerLoopSystem())
                    .ToArray();

                return pls;
            }

            #endregion

            #region Find

            public bool TryFindByPath(string path, out Node system) {
                // TODO: Implement faster search
                if(Path.Equals(path, StringComparison.OrdinalIgnoreCase)) {
                    system = this;
                    return true;
                }

                foreach(var child in subSystems) {
                    if(child.TryFindByPath(path, out system)) {
                        return true;
                    }
                }
                system = null;
                return false;
            }

            public bool TryFind(string name, out Node system) {
                if((!string.IsNullOrEmpty(this.name) && !string.IsNullOrEmpty(name)) && this.name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    system = this;
                    return true;
                }
                foreach(var child in subSystems) {
                    if(child.TryFind(name, out system))
                        return true;
                }

                system = null;
                return false;
            }

            public bool TryFind<T>(out Node system)
                => TryFind(typeof(T), out system);
            public bool TryFind(Type type, out Node system) {
                if(this.type == type) {
                    system = this;
                    return true;
                }
                foreach(var child in subSystems) {
                    if(child.TryFind(type, out system))
                        return true;
                }

                system = null;
                return false;
            }

            #endregion

            #region Add / Remove / Insert

            public Node Add(PlayerLoopSystem system) {
                var us = new Node(this, system);
                subSystems.Add(us);
                return us;
            }

            public bool Remove(PlayerLoopSystem system) {
                for(int i = subSystems.Count - 1; i >= 0; i--) {
                    if(subSystems[i].type == system.type) {
                        subSystems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            public bool RemoveThis() {
                return parent?.Remove(this) ?? false;
            }

            public bool Remove(Node updateSystem) {
                return subSystems.Remove(updateSystem);
            }

            public Node Insert(int index, PlayerLoopSystem system) {
                var us = new Node(this, system);
                subSystems.Insert(index, us);
                return us;
            }

            #endregion

            #region Run

            public void Run(InvokeMode invokeMode = InvokeMode.Children) {
                switch(mode) {
                    case Mode.Native: {
                            if(nativeFunction == null)
                                throw new Exception(TAG + $"Unable to update native function '{Path}'");
                            nativeFunction();
                        }
                        break;
                    case Mode.Managed: {
                            if(managedUpdateFunction == null)
                                throw new Exception(TAG + $"Unable to update managed function on '{Path}'");
                            managedUpdateFunction();
                        }
                        break;
                    default: {
                            if(invokeMode == InvokeMode.NoChildren)
                                Debug.LogError(TAG + $"Unable to update '{Path}' as no function found");
                        }
                        break;
                }

                switch(invokeMode) {
                    case InvokeMode.Children: {
                            foreach(var c in subSystems)
                                if(c.enabled)
                                    c.Run(invokeMode);
                        }
                        break;
                    case InvokeMode.IncludeDisabledChildren: {
                            foreach(var c in subSystems)
                                c.Run(invokeMode);
                        }
                        break;
                }
            }

            #endregion

            #region String

            public string SingleEntryString() {
                return $"{name} ({(EnabledHierarchy ? "Enabled" : "Disabled")}) [{mode.ToStringFast()}]";
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                RecursiveToString(sb, 0);
                return sb.ToString();
            }

            private void RecursiveToString(StringBuilder sb, int depth) {
                sb.AppendLine($"{StringUtil.GetRepeatingCharCache('\t').Get(depth)}{SingleEntryString()}");
                foreach(var child in subSystems)
                    child.RecursiveToString(sb, depth + 1);
            }

            #endregion
        }

        public enum InvokeMode {
            /// <summary>
            /// Only run the Update System called
            /// </summary>
            NoChildren,

            /// <summary>
            /// Runs the update system including all children recursively.
            /// </summary>
            Children,

            /// <summary>
            /// Runs the update system including all children and disabled ones.
            /// </summary>
            IncludeDisabledChildren
        }

        public enum Mode {
            /// <summary>
            /// Nothing
            /// </summary>
            None,
            /// <summary>
            /// C# Callbacks
            /// </summary>
            Managed,
            /// <summary>
            /// Unity's Internal Native Functions
            /// </summary>
            Native,
            /// <summary>
            /// Unity's Internal native Functions that failed to be retrieved
            /// </summary>
            NativeUnaccessable,

            /// <summary>
            /// Root node
            /// </summary>
            Root,
        }

        #region Variables

        private const string TAG = "[Toolkit.PlayerLoopUtility] - ";
        public delegate void NativeFunction();

        private static Node updateSystem;

        internal static Node GetCurrentUpdateSystem() => updateSystem;

        #endregion

        #region Init

        static PlayerLoopUtilty() {
            updateSystem = new Node(PlayerLoop.GetCurrentPlayerLoop());
            updateSystem.Mode = Mode.Root;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
#endif
        private static void Initialize() {
            //updateSystem = new UpdateSystem(PlayerLoop.GetCurrentPlayerLoop());
            //Debug.Log(updateSystem.ToString());

            Debugging.Commands.Add("playerloop save", Save);
            Debugging.Commands.Add<string, bool>("playerloop enable <node> <enabled(true|false)>", SetNodeActive);
            Debugging.Commands.Add("playerloop reset", Reset);

            Debugging.Commands.Add("playerloop list", () => {
                Debugging.Commands.PrintToConsole(updateSystem.ToString());
            });

            Debugging.Commands.Add<string>("playerloop list <node>", (s) => {
                if(Find(s, out var system))
                    Debugging.Commands.PrintToConsole(system.ToString());
                else
                    Debugging.Commands.PrintToConsole($"Node ('{s}') not found");
            });
        }

        #endregion

        #region Generic Methods

        public static void Save() {
            PlayerLoop.SetPlayerLoop(updateSystem.GetPlayerLoopSystem());
        }

        public static void Reset() {
            Debug.LogError(TAG + "Reset called! Critical system could be disabled!");
            updateSystem = new Node(PlayerLoop.GetDefaultPlayerLoop());
            updateSystem.Mode = Mode.Root;
            Save();
        }

        public static void SetNodeActive(string node, bool enabled)
            => SetNodeActive(node, enabled, true);
        public static void SetNodeActive(string node, bool enabled, bool save) {
            if(!Find(node, out var system))
                return;
            system.Enabled = enabled;
            if(save)
                Save();
        }

        #endregion

        #region Find

        public static bool Find(string name, out Node system) {
            return updateSystem.TryFind(name, out system);
        }

        public static bool Find<T>(out Node system) {
            return updateSystem.TryFind<T>(out system);
        }

        public static bool Find(Type type, out Node system) {
            return updateSystem.TryFind(type, out system);
        }

        #endregion

        #region GetNative

        public static bool TryGetNativeFunction(PlayerLoopSystem pls, out NativeFunction function) {
            if(pls.updateFunction == IntPtr.Zero) {
                Debug.LogWarning(TAG + "Player loop provided has no update function pointer");
                function = null;
                return false;
            }
            try {
                var ptr = pls.updateFunction;
                unsafe { // NO IDEA WHY!  https://discussions.unity.com/t/prevent-custom-update-loop-running-post-playmode/875683/4
                    ptr = new IntPtr(*((Int64*)ptr.ToPointer()));
                }
                function = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NativeFunction)) as NativeFunction;
                return function != null;
            }
            catch {
                function = null;
                return false;
            }
        }

        #endregion

        #region Create Container

        public static Node AddContainer(Type type) {
            PlayerLoopSystem pls = new PlayerLoopSystem(){
                type = type
            };
            return updateSystem.Add(pls);
        }

        #endregion

        #region Insert

        public static Node AddLast(string parentNode, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFind(parentNode, out var system)) {
                var newSystem = system.Add(new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find node '{parentNode}'");
                return null;
            }
        }

        public static Node AddFirst(string parentNode, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFind(parentNode, out var system)) {
                var newSystem = system.Insert(0, new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find node '{parentNode}'");
                return null;
            }
        }

        public static Node InsertAfter(string node, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFind(node, out var system)) {
                var newSystem = system.Parent.Insert(system.Index + 1, new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find system '{node}'");
                return null;
            }
        }

        public static Node InsertBefore(string node, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFind(node, out var system)) {
                var newSystem = system.Parent.Insert(system.Index, new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find system '{node}'");
                return null;
            }
        }

        public static Node InsertAfterByPath(string path, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFindByPath(path, out var system)) {
                var newSystem = system.Parent.Insert(system.Index + 1, new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find system by path '{path}'");
                return null;
            }
        }

        public static Node InsertBeforeByPath(string path, Type type, PlayerLoopSystem.UpdateFunction updateFunction, bool save = true) {
            if(updateSystem.TryFindByPath(path, out var system)) {
                var newSystem = system.Parent.Insert(system.Index, new PlayerLoopSystem() {
                    type = type,
                    updateDelegate = updateFunction
                });
                if(save)
                    Save();
                return newSystem;
            }
            else {
                Debug.LogError(TAG + $"Unable to find system by path '{path}'");
                return null;
            }
        }

        #endregion

        #region Remove

        public static bool Remove(string path, out PlayerLoopSystem system, bool save = true) {
            if(updateSystem.TryFindByPath(path, out var s)) {
                s.RemoveThis();
                system = s.GetPlayerLoopSystem();
                if(save)
                    Save();
                return true;
            }
            else {
                Debug.LogError(TAG + $"Unable to find system by path '{path}'");
                system = default;
                return false;
            }
        }

        #endregion
    }
}
