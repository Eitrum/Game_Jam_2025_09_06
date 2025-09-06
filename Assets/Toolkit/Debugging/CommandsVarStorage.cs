using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class CommandsVarStorage {
        #region Classes

        private abstract class Value {
            public abstract string Str { get; }
            public abstract bool IsSerializable { get; }

            public override string ToString() => Str;
        }

        private sealed class RawValue : Value {
            private string str;
            public override string Str => str;
            public override bool IsSerializable => true;
            public RawValue(string str) { this.str = str; }
        }

        private sealed class FuncValue : Value {
            private Func<string> str;
            public override string Str => str();
            public override bool IsSerializable => false;
            public FuncValue(Func<string> str) { this.str = str; }
        }

        private sealed class DynValue : Value {
            private string str;
            public override string Str => str;
            public override bool IsSerializable => false;
            public DynValue(string str) { this.str = str; }
            public void SetValue(string str) { this.str = str; }
        }

        #endregion

        #region Variables

        private static Dictionary<string, Value> variables = new Dictionary<string, Value>();
        private static Dictionary<string, TMLNode> extraVariableEncoding = new Dictionary<string, TMLNode>();

        #endregion

        #region Init

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Init() {
            Commands.Add<string, CommandValue<string>>("var set <path> <value>", SetVar, Privilege.Normal);
            Commands.Add<string>("var get <path>", GetVar, Privilege.Normal);
            Commands.Add("var list", ListVar, Privilege.Normal);
            Commands.Add<string>("var remove <path>", RemoveVar, Privilege.Normal);
            Commands.Add("var clear", ClearAllVariables, Privilege.Normal);
            Commands.Add<string, float>("var increment <path> <value>", Increment, Privilege.Normal);
            Commands.Add<string, float>("var decrement <path> <value>", Decrement, Privilege.Normal);

            LoadVariables();

            // Override
            SetDynVar("path.data", Application.persistentDataPath);
            SetDynVar("platform", Application.platform.ToStringFast());
            SetDynVar("version", Application.version);
            SetDynVar("version.unity", Application.unityVersion);

            SetFuncVar("time.dt", () => Time.deltaTime);
            SetFuncVar("time.fdt", () => Time.fixedDeltaTime);
            SetFuncVar("time.frame", () => Time.frameCount);
        }

        #endregion

        #region Increment / Decrement

        public static void Increment(string path, float value) {
            if(!variables.TryGetValue(path, out Value data)) {
                SetDynVar(path, value.ToString());
                return;
            }

            switch(data) {
                case RawValue rval:
                    if(float.TryParse(rval.Str, out var fres))
                        SetVar(path, (fres + value).ToString());
                    else
                        SetVar(path, value.ToString());
                    break;
                case DynValue dval:
                    if(float.TryParse(dval.Str, out var dfres))
                        dval.SetValue((dfres + value).ToString());
                    else
                        dval.SetValue(value.ToString());
                    break;
                case FuncValue funcval:
                    Commands.PrintToConsole("error: unable to increment a function bound variable");
                    break;
                case null:
                    SetDynVar(path, value.ToString());
                    break;
            }
        }

        public static void Decrement(string path, float value) {
            if(!variables.TryGetValue(path, out Value data)) {
                SetDynVar(path, value.ToString());
                return;
            }

            switch(data) {
                case RawValue rval:
                    if(float.TryParse(rval.Str, out var fres))
                        SetVar(path, (fres - value).ToString());
                    else
                        SetVar(path, (-value).ToString());
                    break;
                case DynValue dval:
                    if(float.TryParse(dval.Str, out var dfres))
                        dval.SetValue((dfres - value).ToString());
                    else
                        dval.SetValue((-value).ToString());
                    break;
                case FuncValue funcval:
                    Commands.PrintToConsole("error: unable to decrement a function bound variable");
                    break;
                case null:
                    SetDynVar(path, (-value).ToString());
                    break;
            }
        }

        #endregion

        #region Remove

        private static void ClearAllVariables() {
            variables.Clear();
            extraVariableEncoding.Clear();
            SaveVariables();
        }

        private static void RemoveVar(string path) {
            variables.Remove(path);
            extraVariableEncoding.Remove(path);
            SaveVariables();
        }

        #endregion

        #region List

        public static void ListVar() {
            Commands.PrintToConsole("variables found: " + variables.Count);
            foreach(var c in variables) {
                if(extraVariableEncoding.TryGetValue(c.Key, out var data))
                    Commands.PrintToConsole($"{c.Key}={c.Value} ({Toolkit.IO.TML.TMLParser.ToString(data, false)})");
                else
                    Commands.PrintToConsole($"{c.Key}={c.Value}");
            }
        }

        #endregion

        #region GetVar

        private static void GetVar(string path) {
            if(!variables.TryGetValue(path, out Value value)) {
                Commands.PrintToConsole($"no variable with the path '{path}'");
                return;
            }
            if(extraVariableEncoding.TryGetValue(path, out var data)) {
                Commands.PrintToConsole($"{path}={value} ({Toolkit.IO.TML.TMLParser.ToString(data, false)})");
                return;
            }
            Commands.PrintToConsole($"{path}={value}");
        }

        #endregion

        #region SetFuncVar

        public static void SetFuncVar<T>(string path, Func<T> func)
            => SetFuncVar(path, () => func().ToString());

        public static void SetFuncVar(string path, Func<string> func) {
            variables[path] = new FuncValue(func);
        }

        #endregion

        #region Set DynVar

        public static void SetDynVar(string path, string value) {
            if(variables.TryGetValue(path, out var data) && data is DynValue dval)
                dval.SetValue(value);
            else
                variables[path] = new DynValue(value);
        }

        public static void SetDynVar(string path, string value, TMLNode data) {
            if(variables.TryGetValue(path, out var d) && d is DynValue dval)
                dval.SetValue(value);
            else
                variables[path] = new DynValue(value);
            extraVariableEncoding[path] = data;
        }

        public static void SetDynVar(string path, CommandValue<string> value) {
            if(variables.TryGetValue(path, out var data) && data is DynValue dval)
                dval.SetValue(value.Value);
            else
                variables[path] = new DynValue(value.Value);
            if(value.Data != null && (value.Data.HasNodes || value.Data.HasProperties))
                extraVariableEncoding[path] = value.Data;
        }

        #endregion

        #region Set Var

        public static void SetVar(string path, string value) {
            variables[path] = new RawValue(value);
            SaveVariables();
        }

        public static void SetVar(string path, string value, TMLNode data) {
            variables[path] = new RawValue(value);
            extraVariableEncoding[path] = data;
            SaveVariables();
        }

        private static void SetVar(string path, CommandValue<string> value) {
            variables[path] = new RawValue(value.Value);
            if(value.Data != null && (value.Data.HasNodes || value.Data.HasProperties))
                extraVariableEncoding[path] = value.Data;

            SaveVariables();
        }

        #endregion

        #region TryGet

        public static bool TryGetValue(string name, out string value) {
            if(!variables.TryGetValue(name, out Value valueResult)) {
                value = null;
                return false;
            }
            value = valueResult.Str;
            return true;
        }

        public static bool TryGetValue(string name, out string value, out TMLNode node) {
            if(!variables.TryGetValue(name, out Value valueResult)) {
                node = null;
                value = null;
                return false;
            }
            value = valueResult.Str;
            extraVariableEncoding.TryGetValue(name, out node);
            return true;
        }

        #endregion

        #region Save / Load

        private static void SaveVariables() {
            try {
                TMLNode root = new TMLNode("variables");
                foreach(var c in variables) {
                    if(!c.Value.IsSerializable)
                        continue;
                    var child = root.AddNode(c.Key);
                    child.AddProperty("value", c.Value.Str);
                    if(extraVariableEncoding.TryGetValue(c.Key, out var value))
                        child.AddNode(value);
                }

                var path = Application.persistentDataPath + "/variables.cfg";
                System.IO.File.WriteAllText(path, Toolkit.IO.TML.TMLParser.ToString(root, true));
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        private static void LoadVariables() {
            try {
                variables.Clear();
                extraVariableEncoding.Clear();

                var path = Application.persistentDataPath + "/variables.cfg";
                if(!System.IO.File.Exists(path))
                    return;
                if(!Toolkit.IO.TML.TMLParser.TryParse(System.IO.File.ReadAllText(path), out TMLNode root)) {
                    Debug.Log("Failed to parse?");
                    return;
                }
                foreach(var c in root.Children) {
                    variables.Add(c.Name, new RawValue(c.GetString("value")));
                    if(c.HasNodes)
                        extraVariableEncoding.Add(c.Name, c.Nodes[0]);
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}
