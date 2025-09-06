using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using Toolkit.IO.TML;
using UnityEngine;

namespace Toolkit.Game {
    public static class InGameSettings {
        public delegate void OnSettingChangedDelegate<T>(T value);
        public delegate bool OnCanApplySettingsDelegate<T>(T value);

        #region Initialize

        static InGameSettings() {
            Toolkit.Debugging.Commands.Add(BASE_COMMAND + "save", () => Save());
            Toolkit.Debugging.Commands.Add(BASE_COMMAND + "load", () => Load());
            Toolkit.Debugging.Commands.Add(BASE_COMMAND + "reset", () => { ResetAll(); ApplyAllChanges(true); });
            Toolkit.Debugging.Commands.Add(BASE_COMMAND + "filelocation", () => Debug.Log(TAG + FileLocation));
            Toolkit.Debugging.Commands.Add<string>(BASE_COMMAND + "filelocation path", (string path) => FileLocation = path);
        }

        #endregion

        #region Variables

        public const string BASE_COMMAND = "settings ";
        private const string TAG = "[Toolkit.Game.Settings] - ";
        private static Dictionary<string, Group> groups = new Dictionary<string, Group>();
        private static TMLNode lastLoadedNode = null;
        private static string fileLocation;

        #endregion

        #region Properties

        public static string FileLocation {
            get {
                if(string.IsNullOrEmpty(fileLocation))
                    fileLocation = Application.persistentDataPath + "/settings.cfg";
                return fileLocation;
            }
            set {
                if(!System.IO.Path.IsPathFullyQualified(System.IO.Path.GetFullPath(value))) {
                    Debug.LogError(TAG + $"'{value}' is not a valid file path");
                    return;
                }
                Debug.Log(TAG + $"Assigned new path: '{value}'");
                fileLocation = value;
            }
        }

        public static IReadOnlyDictionary<string, Group> Groups => groups;

        #endregion

        #region Serialization

        public static TMLNode Serialize() {
            TMLNode root = new TMLNode("settings");
            foreach(var g in groups) {
                g.Value.Serialize(root);
            }
            return root;
        }

        public static void Deserialize(TMLNode node) {
            lastLoadedNode = node;
            foreach(var c in node.Children) {
                if(!groups.TryGetValue(c.Name, out var group)) {
                    group = new Group(c.Name);
                    groups.Add(c.Name, group);
                }
                group.Deserialize(node);
            }
        }

        #endregion

        #region GetGroup

        private static Group GetGroup(string name) {
            if(!groups.TryGetValue(name, out var group)) {
                group = new Group(name);

                // We deserialize group so we have node reference stored within the group.
                if(lastLoadedNode != null && lastLoadedNode.TryGetNode(name, out var node))
                    group.Deserialize(node);
                groups.Add(name, group);
            }
            return group;
        }

        #endregion

        #region Register

        // ------------- Toggle ---------------------

        public static Toggleable RegisterToggle(string group, string id, bool defaultValue) {
            var entry = new Toggleable(group, id, defaultValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static Toggleable RegisterToggle(string group, string id, bool defaultValue, OnSettingChangedDelegate<bool> callback) {
            var entry = new Toggleable(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += callback;
            GetGroup(group).Add(entry);
            return entry;
        }

        public static Toggleable RegisterToggle(string group, string id, bool defaultValue, OnSettingChangedDelegate<bool> callback, OnCanApplySettingsDelegate<bool> settingsFilter) {
            var entry = new Toggleable(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += callback;
            if(settingsFilter != null)
                entry.CanApplySettings += settingsFilter;
            GetGroup(group).Add(entry);
            return entry;
        }

        // ------------- Value ---------------------

        public static Entry<T> RegisterValue<T>(string group, string id, T defaultValue) {
            var entry = new Entry<T>(group, id, defaultValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static Entry<T> RegisterValue<T>(string group, string id, T defaultValue, OnSettingChangedDelegate<T> callback) {
            var entry = new Entry<T>(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += callback;
            GetGroup(group).Add(entry);
            return entry;
        }

        public static Entry<T> RegisterValue<T>(string group, string id, T defaultValue, OnSettingChangedDelegate<T> callback, OnCanApplySettingsDelegate<T> settingsFilter) {
            var entry = new Entry<T>(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += callback;
            if(settingsFilter != null)
                entry.CanApplySettings += settingsFilter;
            GetGroup(group).Add(entry);
            return entry;
        }

        // ------------- Enum ---------------------

        public static EnumEntry<T> RegisterDropdown<T>(string group, string id, T defaultValue) where T : System.Enum {
            var entry = new EnumEntry<T>(group, id, defaultValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static EnumEntry<T> RegisterDropdown<T>(string group, string id, T defaultValue, OnSettingChangedDelegate<T> callback) where T : System.Enum {
            var entry = new EnumEntry<T>(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingsChangedEnum += callback;
            GetGroup(group).Add(entry);
            return entry;
        }

        public static EnumEntry<T> RegisterDropdown<T>(string group, string id, T defaultValue, OnSettingChangedDelegate<T> callback, OnCanApplySettingsDelegate<T> settingsFilter) where T : System.Enum {
            var entry = new EnumEntry<T>(group, id, defaultValue);
            if(callback != null)
                entry.OnSettingsChangedEnum += callback;
            if(settingsFilter != null)
                entry.OnCanApplySettingsEnum += settingsFilter;
            GetGroup(group).Add(entry);
            return entry;
        }

        // ------------- Array ---------------

        public static ArrayEntry<T> RegisterDropdown<T>(string group, string id, IReadOnlyList<T> elements, T defaultValue) {
            var entry = new ArrayEntry<T>(group, id, elements, defaultValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static ArrayEntry<T> RegisterDropdown<T>(string group, string id, IReadOnlyList<T> elements, T defaultValue, OnSettingChangedDelegate<T> callback) {
            var entry = new ArrayEntry<T>(group, id, elements, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += (int val) => callback?.Invoke(elements[val]);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static ArrayEntry<T> RegisterDropdown<T>(string group, string id, IReadOnlyList<T> elements, T defaultValue, OnSettingChangedDelegate<T> callback, OnCanApplySettingsDelegate<T> settingsFilter) {
            var entry = new ArrayEntry<T>(group, id, elements, defaultValue);
            if(callback != null)
                entry.OnSettingChanged += (int val) => callback?.Invoke(elements[val]);
            if(settingsFilter != null)
                entry.CanApplySettings += (int val) => settingsFilter?.Invoke(elements[val]) ?? true;
            GetGroup(group).Add(entry);
            return entry;
        }

        // ------------- Float Range ---------------

        public static RangeEntry RegisterRange(string group, string id, float defaultValue, float minValue, float maxValue) {
            var entry = new RangeEntry(group, id, defaultValue, minValue, maxValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static RangeEntry RegisterRange(string group, string id, float defaultValue, float minValue, float maxValue, OnSettingChangedDelegate<float> callback) {
            var entry = new RangeEntry(group, id, defaultValue, minValue, maxValue);
            entry.OnSettingChanged += callback;
            GetGroup(group).Add(entry);
            return entry;
        }

        // ------------- Int Range ---------------

        public static IntRangeEntry RegisterRange(string group, string id, int defaultValue, int minValue, int maxValue) {
            var entry = new IntRangeEntry(group, id, defaultValue, minValue, maxValue);
            GetGroup(group).Add(entry);
            return entry;
        }

        public static IntRangeEntry RegisterRange(string group, string id, int defaultValue, int minValue, int maxValue, OnSettingChangedDelegate<int> callback) {
            var entry = new IntRangeEntry(group, id, defaultValue, minValue, maxValue);
            entry.OnSettingChanged += callback;
            GetGroup(group).Add(entry);
            return entry;
        }

        #endregion

        #region Find

        public static bool TryFindGroup(string name, out Group group) {
            return groups.TryGetValue(name, out group);
        }

        public static bool TryFindEntry(string group, string id, out Entry entry) {
            if(!groups.TryGetValue(group, out var g)) {
                entry = null;
                return false;
            }
            return g.Entries.TryGetValue(id, out entry);
        }

        public static bool TryFindEntryAs<T>(string group, string id, out T entry) where T : Entry {
            if(!groups.TryGetValue(group, out var g)) {
                entry = default;
                return false;
            }
            if(!g.Entries.TryGetValue(id, out Entry baseEntry)) {
                entry = default;
                return false;
            }
            entry = baseEntry as T;
            return entry != null;
        }

        #endregion

        #region Modification

        public static void ApplyChanges(string group, bool save = true) {
            if(string.IsNullOrEmpty(group)) {
                ApplyAllChanges();
                return;
            }
            if(groups.TryGetValue(group, out var g))
                g.ApplyAllChanges();
            if(save)
                Save();
        }

        public static void ApplyAllChanges(bool save = true) {
            foreach(var g in groups)
                g.Value.ApplyAllChanges();
            if(save)
                Save();
        }

        public static void DiscardChanges(string group) {
            if(string.IsNullOrEmpty(group)) {
                DiscardAllChanges();
                return;
            }
            if(groups.TryGetValue(group, out var g))
                g.DiscardAllChanges();
        }

        public static void DiscardAllChanges() {
            foreach(var g in groups)
                g.Value.DiscardAllChanges();
        }

        public static void Reset(string group) {
            if(string.IsNullOrEmpty(group)) {
                ResetAll();
                return;
            }
            if(groups.TryGetValue(group, out var g))
                g.ResetAll();
        }

        public static void ResetAll() {
            foreach(var g in groups)
                g.Value.ResetAll();
        }

        public static void ForceUpdate(string group) {
            if(groups.TryGetValue(group, out var g))
                g.ForceUpdateAll();
        }

        public static void ForceUpdateAll() {
            foreach(var g in groups)
                g.Value.ForceUpdateAll();
        }

        #endregion

        #region Save

        public static bool Save() {
            try {
                TMLNode node = Serialize();
                var path = FileLocation;
                var text = Toolkit.IO.TML.TMLParser.ToString(node, true);
                var directoryPath = System.IO.Path.GetDirectoryName(path);
                if(!System.IO.Directory.Exists(directoryPath))
                    System.IO.Directory.CreateDirectory(directoryPath);
                System.IO.File.WriteAllText(FileLocation, text);
                Debug.Log(TAG + "Saved Settings to file");
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        public static bool Load() {
            try {
                var path = FileLocation;
                if(!System.IO.File.Exists(path))
                    return false;
                var text = System.IO.File.ReadAllText(FileLocation);
                if(!Toolkit.IO.TML.TMLParser.TryParse(text, out TMLNode node))
                    return false;
                Deserialize(node);
                ApplyAllChanges(false);
                Debug.Log(TAG + "Loaded settings from file");
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        #endregion

        public interface IIncrementable {
            bool AllowWrap { get; }
            void Decrement();
            void Increment();
        }

        public interface IArrayEntry {
            System.Type ArrayValueType { get; }
            int Options { get; }
            int Selected { get; set; }
            string[] Names { get; }
        }

        public interface IEnumEntry {
            System.Enum GenericEnumValue { get; set; }
            System.Type EnumType { get; }
        }

        public class Group {
            #region Variables

            public string Name { get; private set; }
            private Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
            private TMLNode lastLoadedSettings;

            #endregion

            #region Properties

            public int Count => entries.Count;
            public IReadOnlyDictionary<string, Entry> Entries => entries;
            public bool HasLoadedNode => lastLoadedSettings != null;
            public bool IsDirty {
                get {
                    foreach(var e in entries)
                        if(e.Value.IsDirty)
                            return true;
                    return false;
                }
            }

            #endregion

            #region Constructor

            public Group(string name) {
                this.Name = name;
            }

            #endregion

            #region Add / Remove

            public bool Add(Entry entry) {
                var id = entry.Id;
                if(entries.TryGetValue(id, out Entry existingEntry)) {
                    Debug.LogError(TAG + "Adding an entry that already exists!!");
                    return false;
                }
                entries.Add(id, entry);
                if(lastLoadedSettings != null && lastLoadedSettings.TryGetNode(id, out var node)) {
                    entry.Deserialize(node);
                    entry.ApplyChange();
                }
                entry.RegisterAsCommand();
                return true;
            }

            public bool Remove(Entry entry) {
                return entries.Remove(entry.Id);
            }

            #endregion

            #region Serialization

            public void ClearLoadedNode() {
                lastLoadedSettings = null;
            }

            public void Serialize(TMLNode parent) {
                var node = parent.AddNode(Name);
                foreach(var e in entries) {
                    var entryNode = node.AddNode(e.Key);
                    e.Value.Serialize(entryNode);
                }
            }

            public void Deserialize(TMLNode parent, bool cacheNodeForLateAddedEntries = true) {
                if(!parent.TryGetNode(Name, out var node)) {
                    Debug.LogWarning(TAG + "Failed to deserialize group as group node didn't exist");
                    return;
                }
                if(cacheNodeForLateAddedEntries)
                    lastLoadedSettings = node;
                foreach(var e in entries) {
                    if(node.TryGetNode(e.Key, out var entryNode))
                        e.Value.Deserialize(entryNode);
                }
            }

            #endregion

            #region Operations

            public void ApplyAllChanges() {
                foreach(var e in entries)
                    e.Value.ApplyChange();
            }

            public void DiscardAllChanges() {
                foreach(var e in entries)
                    e.Value.DiscardChanges();
            }

            public void ResetAll() {
                foreach(var e in entries)
                    e.Value.Reset();
            }

            public void ForceUpdateAll() {
                foreach(var e in entries)
                    e.Value.ForceUpdate();
            }

            #endregion
        }

        public abstract class Entry {
            #region Variables

            public string Group { get; protected set; }
            public string Id { get; protected set; }
            public virtual bool IsDirty { get; protected set; } = true;
            protected bool IsFirstTime = true;

            #endregion

            #region Constructor

            public Entry(string group, string id) {
                this.Group = group;
                this.Id = id;
            }

            public abstract void RegisterAsCommand();
            public abstract void UnregisterAsCommand();

            #endregion

            #region Modification Methods

            public abstract void ApplyChange();
            public abstract void DiscardChanges();
            public abstract void Reset();

            public abstract void ForceUpdate();

            #endregion

            #region Serialization

            public abstract void Serialize(TMLNode node);
            public abstract void Deserialize(TMLNode node);

            #endregion
        }

        public class Entry<T> : Entry {
            #region Variables

            protected T savedValue;
            protected T modifiedValue;
            protected T defaultValue;

            protected Toolkit.Debugging.Command cachedCommand;
            public event OnSettingChangedDelegate<T> OnModifiedValueChanged;
            public event OnSettingChangedDelegate<T> OnSettingChanged;
            public event OnCanApplySettingsDelegate<T> CanApplySettings;

            #endregion

            #region Properties

            public virtual T SavedValue => savedValue;
            public virtual T DefaultValue => defaultValue;

            public virtual T ModifiedValue {
                get => modifiedValue;
                set {
                    if(this.modifiedValue.Equals(value))
                        return;

                    bool check = true;
                    try {
                        if(CanApplySettings != null)
                            check = CanApplySettings.Invoke(value);
                    }
                    catch {
                        check = false;
                    }

                    if(!check) {
                        Debug.LogWarning(TAG + $"Attempting to change from ({modifiedValue}) to ({value}). But was not allowed");
                        return;
                    }

                    this.modifiedValue = value;
                    IsDirty = !this.modifiedValue.Equals(savedValue);
                    OnModifiedValueChanged?.Invoke(this.modifiedValue);
                }
            }

            #endregion

            #region Constructor

            public Entry(string group, string id, T value) : base(group, id) {
                this.defaultValue = value;
                this.savedValue = value;
                this.modifiedValue = value;
            }

            #endregion

            #region Override Methods

            public override void Reset() {
                ModifiedValue = defaultValue;
            }

            public override void DiscardChanges() {
                ModifiedValue = savedValue;
                IsDirty = false;
            }

            public override void ApplyChange() {
                if(!IsDirty && !IsFirstTime)
                    return;
                IsFirstTime = false;
                IsDirty = false;
                this.savedValue = this.modifiedValue;
                OnModifiedValueChanged?.Invoke(this.modifiedValue);
                OnSettingChanged?.Invoke(this.modifiedValue);
            }

            public override void ForceUpdate() {
                OnModifiedValueChanged?.Invoke(this.modifiedValue);
                OnSettingChanged?.Invoke(this.modifiedValue);
            }

            #endregion

            #region Commands

            public override void RegisterAsCommand() {
                cachedCommand = Toolkit.Debugging.Commands.Add<T>(BASE_COMMAND + $"{Group} {Id}", (T value) => {
                    Debug.Log(TAG + $"Changing '{Group}/{Id}' from ({modifiedValue}) to ({value})");
                    ModifiedValue = value;
                    ApplyChange();
                },
                Debugging.Privilege.Normal);
            }

            public override void UnregisterAsCommand() {
                Toolkit.Debugging.Commands.Remove(cachedCommand);
            }

            #endregion

            #region Serialization

            public override void Serialize(TMLNode node) {
                node.AddProperty(TMLUtility.CreateProperty("value", ModifiedValue));
            }

            public override void Deserialize(TMLNode node) {
                ModifiedValue = TMLUtility.GetPropertyValue<T>(node, "value");
                IsDirty = true;
            }

            #endregion
        }

        public sealed class Toggleable : Entry<bool> {
            public Toggleable(string group, string id, bool value) : base(group, id, value) { }
            public void Toggle() => ModifiedValue = !ModifiedValue;
        }

        public sealed class ArrayEntry<T> : IntRangeEntry, IArrayEntry {

            #region Variables

            private string[] generatedValueNames;
            public IReadOnlyList<T> Values { get; private set; }

            #endregion

            #region Properties

            public T Value {
                get => GetValue();
                set => SetValue(value);
            }

            public System.Type ArrayValueType => typeof(T);

            public int Options => Values.Count;
            public int Selected { get => modifiedValue; set => ModifiedValue = value; }

            public string[] Names {
                get {
                    if(generatedValueNames == null) {
                        generatedValueNames = new string[Values.Count];
                        for(int i = 0; i < Values.Count; i++) {
                            generatedValueNames[i] = Values[i].ToString();
                        }
                    }
                    return generatedValueNames;
                }
            }

            #endregion

            #region Constructor

            public ArrayEntry(string group, string id, IReadOnlyList<T> values, T defaultValue) : base(group, id, Mathf.Max(0, values.IndexOf(defaultValue)), 0, values.Count) {
                this.Values = values;
            }

            public ArrayEntry(string group, string id, IReadOnlyList<T> values, T defaultValue, bool allowWrap) : base(group, id, Mathf.Max(0, values.IndexOf(defaultValue)), 0, values.Count, allowWrap) {
                this.Values = values;
            }

            public void SetValueNames(string[] names) {
                if(names.Length != this.Values.Count) {
                    Debug.LogError(TAG + "The names array and values list do not match");
                    return;
                }
                this.generatedValueNames = names;
            }

            #endregion

            #region Array Value Access

            public bool SetValue(T value) {
                var index = Values.IndexOf(value);
                if(index < 0)
                    return false;
                ModifiedValue = index;
                return true;
            }

            public T GetValue() {
                try {
                    return Values[modifiedValue];
                }
                catch {
                    return Values[defaultValue];
                }
            }

            #endregion
        }

        public sealed class RangeEntry : Entry<float> {
            #region Variables

            public float Min { get; private set; }
            public float Max { get; private set; }

            #endregion

            #region Properties

            public override float ModifiedValue {
                get => base.ModifiedValue;
                set => base.ModifiedValue = Mathf.Clamp(value, Min, Max);
            }

            public float Normalized {
                get => Mathf.InverseLerp(Min, Max, ModifiedValue);
                set => ModifiedValue = Mathf.Lerp(Min, Max, value);
            }

            #endregion

            #region Constructor

            public RangeEntry(string group, string id, float defaultValue, float min, float max) : base(group, id, defaultValue) {
                this.Min = min;
                this.Max = max;
            }

            #endregion
        }

        public class IntRangeEntry : Entry<int>, IIncrementable {
            #region Variables

            public int Min { get; private set; }
            public int Max { get; private set; }
            public bool AllowWrap { get; set; } = false;

            #endregion

            #region Properties

            public override int ModifiedValue {
                get => base.ModifiedValue;
                set => base.ModifiedValue = Mathf.Clamp(value, Min, Max);
            }

            public float Normalized {
                get => Mathf.InverseLerp(Min, Max, ModifiedValue);
                set => ModifiedValue = Mathf.RoundToInt(Mathf.Lerp(Min, Max, value));
            }

            #endregion

            #region Constructor

            public IntRangeEntry(string group, string id, int value, int min, int max) : base(group, id, value) {
                this.Min = min;
                this.Max = max;
            }

            public IntRangeEntry(string group, string id, int value, int min, int max, bool allowWrap) : base(group, id, value) {
                this.Min = min;
                this.Max = max;
                this.AllowWrap = allowWrap;
            }

            #endregion

            #region IIncremental Impl

            public void Decrement() {
                if(AllowWrap && modifiedValue - 1 <= Min)
                    ModifiedValue = Max;
                else
                    ModifiedValue--;
            }
            public void Increment() {
                if(AllowWrap && modifiedValue + 1 >= Max)
                    ModifiedValue = Min;
                else
                    ModifiedValue++;
            }

            #endregion
        }

        public sealed class EnumEntry<T> : Entry<int>, IEnumEntry where T : System.Enum {

            public event OnSettingChangedDelegate<T> OnSettingsChangedEnum;
            public event OnCanApplySettingsDelegate<T> OnCanApplySettingsEnum;
            private Debugging.Command enumCachedCommand;

            #region Properties

            System.Enum IEnumEntry.GenericEnumValue { get => ModifiedValue.ToEnum<T>(); set => ModifiedValue = value.ToInt(); }
            System.Type IEnumEntry.EnumType => typeof(T);

            #endregion

            #region Constructor

            public EnumEntry(string group, string id, T defaultValue) : base(group, id, defaultValue.ToInt()) { }

            #endregion

            #region Overrides

            public override int ModifiedValue {
                get => base.ModifiedValue;
                set {
                    if(this.ModifiedValue == value)
                        return;
                    bool check = true;
                    try {
                        if(OnCanApplySettingsEnum != null)
                            check = OnCanApplySettingsEnum.Invoke(value.ToEnum<T>());
                    }
                    catch {
                        check = false;
                    }
                    if(check)
                        base.ModifiedValue = value;
                }
            }

            public override void ApplyChange() {
                base.ApplyChange();
                OnSettingsChangedEnum?.Invoke(ModifiedValue.ToEnum<T>());
            }

            public override void RegisterAsCommand() {
                enumCachedCommand = Toolkit.Debugging.Commands.Add<T>(BASE_COMMAND + $"{Group} {Id}", (T value) => {
                    Debug.Log(TAG + $"Changing '{Group}/{Id}' from ({modifiedValue}) to ({value})");
                    ModifiedValue = value.ToInt();
                    ApplyChange();
                },
                Debugging.Privilege.Normal);
                base.RegisterAsCommand();
            }

            public override void UnregisterAsCommand() {
                Debugging.Commands.Remove(enumCachedCommand);
                base.UnregisterAsCommand();
            }

            #endregion

            #region IIncremental Impl

            public void Decrement() {
                var arr = FastEnum<T>.Array;
                int val = arr.IndexOf(modifiedValue.ToEnum<T>());
                val--;
                ModifiedValue = arr[Mathf.Max(val, 0)].ToInt();
            }

            public void Increment() {
                var arr = FastEnum<T>.Array;
                var val = arr.IndexOf(modifiedValue.ToEnum<T>());
                val++;
                ModifiedValue = arr[Mathf.Min(val, arr.Count - 1)].ToInt();
            }

            #endregion
        }
    }
}
