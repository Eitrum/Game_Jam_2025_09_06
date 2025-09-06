#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define FAST_ENUM_LOGGING
#endif

// Comment out to disable raising exceptions
#define FAST_ENUM_DISABLE_EXCEPTIONS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit {
    /*
     * Example of implementation
     * 
     * internal static class FastEnumExamples
     * {
     *     private enum Fruits
     *     {
     *         None,
     *         /// <summary>
     *         /// 
     *         /// </summary>
     *         [FEBind("dir", 0, 0, 1)]
     *         Apple,
     *         [FEBind("sname", "Ban"), FEBind(FEMetadata.Value, 84)]
     *         Banana,
     *     }
     * 
     *     private static void ShowcaseBindingData() {
     *         FastEnum.PrepareAll<Fruits>();
     *         FastEnum.Bind(Fruits.Banana, "sname", "Ban");
     * 
     *         FastEnum.Bind(Fruits.Apple, "sname", "App");
     *         FastEnum.Bind(Fruits.Apple, FEMetadata.Color, UnityEngine.Color.red);
     * 
     *         string value = FastEnum.GetData<Fruits, string>(Fruits.Apple, "sname");
     *         var applyColor = Fruits.Apple.GetData<Fruits, UnityEngine.Color>(FEMetadata.Color);
     *     }
     * }
     */

    public static class FastEnum {
        #region Variables

        private const string TAG = "[FastEnum] - ";
        private static Dictionary<Type, Func<int>> valuesLengthFromType = new Dictionary<Type, Func<int>>();
        private static Dictionary<Type, Func<IReadOnlyList<string>>> getNamesFromType = new Dictionary<Type, Func<IReadOnlyList<string>>>();
        private static Dictionary<Type, Func<IList>> getValuesFromNames = new Dictionary<Type, Func<IList>>();
        private static Dictionary<Type, Func<Type>> getUnderlyingTypeFromType = new Dictionary<Type, Func<Type>>();
        private static Dictionary<Type, Type> enumTypeToFastEnumT = new Dictionary<Type, Type>();

        internal static float TICKS_TO_MILLISECONDS = 1f / (System.Diagnostics.Stopwatch.Frequency / 1000f);
        internal static long totalTimeToCache = 0;
        internal static BoolStack disableLogging = new BoolStack();

        #endregion

        #region Prepare & Add

        public static void PrepareAll<T>(bool threaded = false) where T : Enum => FastEnum<T>.PrepareAll(threaded);

        public static void Add(Type type) {
            if(enumTypeToFastEnumT.ContainsKey(type))
                return;
            try {
                var method = typeof(FastEnum)
                        .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        .First(m => m.IsGenericMethodDefinition && m.Name == nameof(Add) && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 0)
                        .MakeGenericMethod(type);
                method.InvokeStatic();
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        public static void Add<T>() where T : Enum {
            try {
                var type = typeof(T);
                if(enumTypeToFastEnumT.ContainsKey(type))
                    return;
                enumTypeToFastEnumT.Add(type, typeof(FastEnum<T>));
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(FastEnum<T>).TypeHandle);
                valuesLengthFromType.Add(type, GetValueCount<T>);
                getNamesFromType.Add(type, GetNames<T>);
                getValuesFromNames.Add(type, GetValuesAsList<T>);
                getUnderlyingTypeFromType.Add(type, GetUnderlyingType<T>);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        public static bool TryGetFastT(Type enumType, out Type fastEnumT) {
            return enumTypeToFastEnumT.TryGetValue(enumType, out fastEnumT);
        }

        #endregion

        #region Basic Values

        public static int GetValueCount(Type type) => valuesLengthFromType.TryGetValue(type, out Func<int> func) ? func() : System.Enum.GetValues(type).Length;
        public static int GetValueCount<T>() where T : Enum => FastEnum<T>.Entries;

        public static IReadOnlyList<string> GetNames(Type type) => getNamesFromType.TryGetValue(type, out Func<IReadOnlyList<string>> func) ? func() : System.Enum.GetNames(type);
        public static IReadOnlyList<string> GetNames<T>() where T : Enum => FastEnum<T>.Names;

        public static IList GetValues(Type type) => getValuesFromNames.TryGetValue(type, out Func<IList> func) ? func() : System.Enum.GetValues(type);
        public static IReadOnlyList<T> GetValues<T>() where T : Enum => FastEnum<T>.Array;

        public static IList GetValuesAsList<T>() where T : Enum => FastEnum<T>.ArrayAsObject;

        public static Type GetUnderlyingType(Type type) => getUnderlyingTypeFromType.TryGetValue(type, out Func<Type> func) ? func() : System.Enum.GetUnderlyingType(type);
        public static Type GetUnderlyingType<T>() where T : Enum => FastEnum<T>.UnderlyingType;

        #endregion

        #region Binding

        internal static bool TryGetBindT(Type type, out System.Reflection.MethodInfo method) {
            method = null;
            try {
                if(!enumTypeToFastEnumT.TryGetValue(type, out var tValue))
                    return false;
                method = tValue.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .FirstOrDefault(x => x.Name == "Bind" && x.GetParameters().Length == 3);
                return method != null;
            }
            catch(System.Exception e) {
                UnityEngine.Debug.LogException(e);
                return false;
            }
        }
        public static void Bind<T>(this T item, string key, object data) where T : Enum => FastEnum<T>.Bind(item, key, data);

        public static object GetData<T>(this T item, string key) where T : Enum => FastEnum<T>.GetData(item, key);
        public static TOut GetData<T, TOut>(this T item, string key) where T : Enum => FastEnum<T>.GetData<TOut>(item, key);
        public static bool TryGetData<T, TOut>(this T item, string key, out TOut output) where T : Enum => FastEnum<T>.TryGetData(item, key, out output);

        #endregion

        #region Metadata

        public static void Bind<T, TItem>(this T item, FEMetadata metadata, TItem obj) where T : Enum => FastEnum<T>.Bind(item, metadata, obj);
        public static void Bind<T>(this T item, string name, string shortName, UnityEngine.Color color, UnityEngine.Texture icon) where T : Enum {
            Bind(item, FEMetadata.Name, name);
            Bind(item, FEMetadata.ShortName, shortName);
            Bind(item, FEMetadata.Color, color);
            Bind(item, FEMetadata.Icon, icon);
        }

        public static object GetData<T>(this T item, FEMetadata metadata) where T : Enum => FastEnum<T>.GetData(item, metadata);
        public static TOut GetData<T, TOut>(this T item, FEMetadata metadata) where T : Enum => FastEnum<T>.GetData<TOut>(item, metadata);
        public static bool TryGetData<T, TOut>(this T item, FEMetadata metadata, out TOut output) where T : Enum => FastEnum<T>.TryGetData(item, metadata, out output);

        #endregion

        #region Parsing

        public static T Parse<T>(string s) where T : Enum => FastEnum<T>.Parse(s);
        public static bool TryParse<T>(string s, out T res) where T : Enum => FastEnum<T>.TryParse(s, out res);

        public static T Parse<T>(string s, bool ignoreCase) where T : Enum => FastEnum<T>.Parse(s, ignoreCase);
        public static bool TryParse<T>(string s, bool ignoreCase, out T value) where T : Enum => FastEnum<T>.TryParse(s, ignoreCase, out value);

        public static T ParseIgnoreCase<T>(string s) where T : Enum => FastEnum<T>.ParseIgnoreCase(s);
        public static bool TryParseIgnoreCase<T>(string s, out T value) where T : Enum => FastEnum<T>.TryParseIgnoreCase(s, out value);

        #endregion

        #region ToString

        public static string ToStringFast<T>(this T item) where T : Enum => FastEnum<T>.ToString(item);

        #endregion

        #region Scopes

        public class DisableLoggingScope : IDisposable {
            private bool isDisposed = false;

            public DisableLoggingScope() { FastEnum.disableLogging++; }

            ~DisableLoggingScope() => Dispose();

            public void Dispose() {
                if(isDisposed) return;
                isDisposed = true;
                FastEnum.disableLogging--;
            }
        }

        #endregion
    }

    public enum FEMetadata {
        None = 0,
        Name = 1,
        Value = 2,
        ShortName = 4,
        Color = 8,
        Icon = 16,
    }

    [System.AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class FEBindAttribute : Attribute {
        #region Variables

        private const string TAG = "[FEBindAttribute] - ";

        public string Key { get; private set; }
        public object Data { get; private set; }

        public FEMetadata Metadata { get; private set; }

        #endregion

        #region Constructor

        private FEBindAttribute(string key) => this.Key = key;
        private FEBindAttribute(object obj) {
            this.Data = obj;
        }
        private FEBindAttribute(FEMetadata metadata, object obj) : this(obj) {
#if FAST_ENUM_LOGGING
            if(metadata == FEMetadata.None) {
                HandleExceptions(TAG + $"Metadata is not valid 'None'");
                return;
            }
#endif
            this.Metadata = metadata;
        }

        public FEBindAttribute(string key, bool data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, byte data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, char data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, double data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, float data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, int data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, long data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, sbyte data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, short data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, string data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, uint data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, ulong data) : this(key) => this.Data = data;
        public FEBindAttribute(string key, ushort data) : this(key) => this.Data = data;

        public FEBindAttribute(string key, float x, float y, float z) : this(key) => this.Data = new UnityEngine.Vector3(x, y, z);
        public FEBindAttribute(string key, float x, float y) : this(key) => this.Data = new UnityEngine.Vector2(x, y);

        public FEBindAttribute(FEMetadata metadata, string data) {
#if FAST_ENUM_LOGGING
            if(metadata == FEMetadata.None) {
                HandleExceptions(TAG + $"Metadata is not valid 'None'");
                return;
            }
#endif
            this.Metadata = metadata;
            switch(metadata) {
                case FEMetadata.Color: {
                        if(UnityEngine.ColorUtility.TryParseHtmlString(data, out UnityEngine.Color col))
                            this.Data = col;
                        else
                            this.Data = data;
                    }
                    break;
                default:
                    this.Data = data;
                    break;
            }
        }

        public FEBindAttribute(FEMetadata metadata, int data) {
#if FAST_ENUM_LOGGING
            if(metadata == FEMetadata.None) {
                HandleExceptions(TAG + $"Metadata is not valid 'None'");
                return;
            }
#endif
            this.Metadata = metadata;
            switch(metadata) {
                case FEMetadata.Color:
                    this.Data = data.ToColor();
                    break;
                default:
                    this.Data = data;
                    break;
            }
        }

        public FEBindAttribute(FEMetadata metadata, bool data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, byte data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, char data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, double data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, float data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, long data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, sbyte data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, short data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, uint data) {
#if FAST_ENUM_LOGGING
            if(metadata == FEMetadata.None) {
                HandleExceptions(TAG + $"Metadata is not valid 'None'");
                return;
            }
#endif
            this.Metadata = metadata;
            switch(metadata) {
                case FEMetadata.Color:
                    this.Data = data.ToColor();
                    break;
                default:
                    this.Data = data;
                    break;
            }
        }
        public FEBindAttribute(FEMetadata metadata, ulong data) : this(metadata, (object)data) { }
        public FEBindAttribute(FEMetadata metadata, ushort data) : this(metadata, (object)data) { }

        #endregion

        #region Utility

        private static void HandleExceptions(string msg) {
#if FAST_ENUM_DISABLE_EXCEPTIONS
            UnityEngine.Debug.LogError(TAG + $"Metadata is not valid 'None'");
#else
            throw new Exception(TAG +$"Metadata is not valid 'None'");
#endif
        }

        #endregion
    }

    internal static class FastEnum<T> where T : Enum {
        #region Variables

        private static readonly string TAG = $"[FastEnum+{typeof(T)}] - ";

        private static Type type;
        private static Type underlyingType = typeof(T).UnderlyingSystemType;
        private static TypeCode typeCode;
        private static IReadOnlyList<string> names;
        private static IReadOnlyList<T> array;
        private static IList arrayObject;
        private static Dictionary<T, string> enumtoname = new Dictionary<T, string>();
        private static Dictionary<string, T> sttoenum = new Dictionary<string, T>();
        private static Dictionary<string, T> sttoenumigcase = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        private static int entries;
        private static Dictionary<T, Dictionary<string, object>> dataBinding = new Dictionary<T, Dictionary<string, object>>();
        private static bool isPrepared = false;

#if FAST_ENUM_LOGGING
        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif

        #endregion

        #region Properties

        public static Type UnderlyingType => underlyingType;
        public static TypeCode TypeCode => typeCode;

        public static IReadOnlyList<string> Names => names;
        public static int Entries => entries;
        public static IReadOnlyList<T> Array => array;
        public static IList ArrayAsObject => arrayObject;

        #endregion

        #region Constructor

        static FastEnum() {
#if FAST_ENUM_LOGGING
            sw.Start();
#endif
            type = typeof(T);
            underlyingType = System.Enum.GetUnderlyingType(type);
            typeCode = System.Type.GetTypeCode(underlyingType);

            names = System.Enum.GetNames(type);
            var arr = System.Enum.GetValues(type);
            entries = arr.Length;
            var tarr = new T[entries];
            for(int i = 0; i < entries; i++) {
                var t = tarr[i] = (T)arr.GetValue(i);
                sttoenum.Add(names[i], t);
                sttoenumigcase.Add(names[i], t);
                enumtoname.TryAdd(t, names[i]);
            }
            array = tarr;
            arrayObject = tarr;

            for(int i = 0; i < entries; i++)
                dataBinding.TryAdd(array[i], new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

            FastEnum.Add<T>();

#if FAST_ENUM_LOGGING
            sw.Stop();
            var ms = (sw.ElapsedTicks * FastEnum.TICKS_TO_MILLISECONDS);
            FastEnum.totalTimeToCache += sw.ElapsedTicks;
            if(!FastEnum.disableLogging)
                UnityEngine.Debug.Log($"<color=grey>{TAG}Loaded at {ms:0.00}ms [Total: {(FastEnum.totalTimeToCache * FastEnum.TICKS_TO_MILLISECONDS):0.00}ms]</color>");
#endif
        }

        public static void PrepareAll(bool threaded = false) {
            if(isPrepared)
                return;
            isPrepared = true;
            // Threading is unsupported on WebGL
#if !UNITY_WEBGL
            if(threaded)
                System.Threading.Tasks.Task.Run(InternalPrepare);
            else
#endif
                InternalPrepare();
        }

        private static void InternalPrepare() {
#if FAST_ENUM_LOGGING
            sw.Restart();
#endif
            for(int i = 0; i < entries; i++) {
                var t = array[i];
                var attributes = Attribute.GetCustomAttributes(type, typeof(FEBindAttribute));
                foreach(var a in attributes)
                    Bind(t, a);
            }
#if FAST_ENUM_LOGGING
            sw.Stop();
            var ms = (sw.ElapsedTicks * FastEnum.TICKS_TO_MILLISECONDS);
            FastEnum.totalTimeToCache += sw.ElapsedTicks;
            UnityEngine.Debug.Log($"<color=grey>{TAG}Prepared @{ms:0.00}ms</color>");
#endif
        }

        #endregion

        #region Binding

        private static void Bind(T item, System.Attribute attr)
            => Bind(item, attr as FEBindAttribute);

        private static void Bind(T item, FEBindAttribute attr) {
            if(attr == null)
                return;
            if(attr.Metadata != FEMetadata.None)
                Bind(item, attr.Metadata, attr.Data);
            else if(!string.IsNullOrEmpty(attr.Key))
                dataBinding[item].Add(attr.Key, attr.Data);
        }

        public static void Bind(T item, string key, object data) {
            PrepareAll();
            // UnityEngine.Debug.Log($"attempting to bind '{item}' on key '{key}' with '{data}'");
            //if(!dataBinding[item].TryAdd(key, data))
            dataBinding[item][key] = data;
        }

        public static object GetData(T item, string key) {
            if(!dataBinding.TryGetValue(item, out Dictionary<string, object> dict)) {
                HandleException(TAG + $"Unable to find databinding '{ToString(item)}'");
                return null;
            }
            if(!dict.TryGetValue(key, out object obj)) {
                HandleException(TAG + $"Unable to find databinding with key '{key}'");
                return null;
            }
            return obj;
        }
        public static TOut GetData<TOut>(T item, string key) => (TOut)GetData(item, key);

        public static bool TryGetData(T item, string key, out object obj) {
            if(!dataBinding.TryGetValue(item, out Dictionary<string, object> dict)) {
                obj = default;
                return false;
            }
            return dict.TryGetValue(key, out obj);
        }

        public static bool TryGetData<TOut>(T item, string key, out TOut output) {
            if(!dataBinding.TryGetValue(item, out Dictionary<string, object> dict)) {
                output = default;
                return false;
            }
            if(!dict.TryGetValue(key, out object obj)) {
                output = default;
                return false;
            }
            output = (TOut)obj;
            return output != null;
        }

        #endregion

        #region Metadata Bindings

        public static void Bind<TItem>(T item, FEMetadata metadata, TItem obj) {
            if(!FastEnum<FEMetadata>.enumtoname.TryGetValue(metadata, out string val)) {
                HandleException(TAG + $" - Unable to bind metadata '{metadata}'");
                return;
            }

            PrepareAll();
            dataBinding[item].Add(val, obj);
        }

        public static object GetData(T item, FEMetadata metadata) {
            if(!FastEnum<FEMetadata>.enumtoname.TryGetValue(metadata, out string val)) {
                HandleException(TAG + $" - Unable to get metadata '{metadata}'");
                return null;
            }

            return GetData(item, val);
        }

        public static TItem GetData<TItem>(T item, FEMetadata metadata) => (TItem)GetData(item, metadata);

        public static bool TryGetData(T item, FEMetadata metadata, out object output) {
            if(!FastEnum<FEMetadata>.enumtoname.TryGetValue(metadata, out string val)) {
                output = default;
                return false;
            }

            return TryGetData(item, val, out output);
        }

        public static bool TryGetData<TItem>(T item, FEMetadata metadata, out TItem output) {
            if(!FastEnum<FEMetadata>.enumtoname.TryGetValue(metadata, out string val)) {
                output = default;
                return false;
            }
            return TryGetData(item, val, out output);
        }

        #endregion

        #region Parsing

        public static bool TryParse(string s, out T value) => sttoenum.TryGetValue(s, out value);
        public static T Parse(string s) => sttoenum.TryGetValue(s, out T value) ? value : (T)HandleException(TAG + $"Unable to parse '{s}'");

        public static bool TryParse(string s, bool ignoreCase, out T value) => (ignoreCase ? sttoenumigcase : sttoenum).TryGetValue(s, out value);
        public static T Parse(string s, bool ignoreCase) => (ignoreCase ? sttoenumigcase : sttoenum).TryGetValue(s, out T value) ? value : (T)HandleException(TAG + $"Unable to parse '{s}'");

        public static bool TryParseIgnoreCase(string s, out T value) => sttoenumigcase.TryGetValue(s, out value);
        public static T ParseIgnoreCase(string s) => sttoenumigcase.TryGetValue(s, out T value) ? value : (T)HandleException(TAG + $"Unable to parse '{s}'");

        #endregion

        #region ToString

        public static string ToString(T item) => enumtoname.TryGetValue(item, out string val) ? val : "";

        #endregion

        #region Utility

        private static object HandleException(string message) {
#if FAST_ENUM_DISABLE_EXCEPTIONS
#if FAST_ENUM_LOGGING
            UnityEngine.Debug.LogError(message);
#endif
#else
            throw new Exception(message);
#endif
            return null;
        }

        #endregion
    }
}
