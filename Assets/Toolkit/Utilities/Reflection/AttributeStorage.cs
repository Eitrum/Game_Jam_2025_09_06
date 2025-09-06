using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit {
    public static class AttributeStorage {

        public class Storage {
            #region Variables

            public Type AttributeType { get; private set; }

            private List<Storage> children = new List<Storage>();
            private List<ClassAttribute> classAttributes = new List<ClassAttribute>();
            private List<FieldAttribute> fieldAttributes = new List<FieldAttribute>();
            private List<PropertyAttribute> propertyAttributes = new List<PropertyAttribute>();
            private List<MethodAttribute> methodAttributes = new List<MethodAttribute>();

            #endregion

            #region Properties

            public IReadOnlyList<ClassAttribute> ClassAttributes => classAttributes;
            public IReadOnlyList<FieldAttribute> FieldAttributes => fieldAttributes;
            public IReadOnlyList<PropertyAttribute> PropertyAttributes => propertyAttributes;
            public IReadOnlyList<MethodAttribute> MethodAttributes => methodAttributes;

            #endregion

            #region Constructor

            public Storage(Type attributeType) {
                this.AttributeType = attributeType;
            }

            #endregion

            #region Add

            public void Add(Storage storage) {
                this.children.Add(storage);
            }

            public void Add(ClassAttribute classAttribute) {
                this.classAttributes.Add(classAttribute);
            }

            public void Add(FieldAttribute fieldAttribute) {
                this.fieldAttributes.Add(fieldAttribute);
            }

            public void Add(PropertyAttribute propertyAttribute) {
                this.propertyAttributes.Add(propertyAttribute);
            }

            public void Add(MethodAttribute methodAttribute) {
                this.methodAttributes.Add(methodAttribute);
            }

            #endregion

            #region GetAll

            public IEnumerable<ClassAttribute> GetAllClasses(bool includeChildren = true) {
                foreach(var c in classAttributes)
                    yield return c;
                foreach(var c in children)
                    foreach(var ca in c.GetAllClasses(includeChildren))
                        yield return ca;
            }

            public IEnumerable<FieldAttribute> GetAllFields(bool includeChildren = true) {
                foreach(var c in fieldAttributes)
                    yield return c;
                foreach(var c in children)
                    foreach(var ca in c.GetAllFields(includeChildren))
                        yield return ca;
            }

            public IEnumerable<PropertyAttribute> GetAllProperties(bool includeChildren = true) {
                foreach(var c in propertyAttributes)
                    yield return c;
                foreach(var c in children)
                    foreach(var ca in c.GetAllProperties(includeChildren))
                        yield return ca;
            }

            public IEnumerable<MethodAttribute> GetAllMethods(bool includeChildren = true) {
                foreach(var c in methodAttributes)
                    yield return c;
                foreach(var c in children)
                    foreach(var ca in c.GetAllMethods(includeChildren))
                        yield return ca;
            }

            #endregion
        }

        public class ClassAttribute {
            #region Variables

            public Assembly Assembly { get; private set; }
            public Type Class { get; private set; }
            public Attribute Attribute { get; private set; }

            #endregion

            #region Constructor

            public ClassAttribute(Assembly assembly, Type c, Attribute attribute) {
                this.Assembly = assembly;
                this.Class = c;
                this.Attribute = attribute;
            }

            #endregion

            #region Overrides

            public override string ToString() {
                return $"{Class.FullName}";
            }

            #endregion
        }

        public class FieldAttribute {
            #region Variables

            public Assembly Assembly { get; private set; }
            public Type Class { get; private set; }
            public FieldInfo FieldInfo { get; private set; }
            public Attribute Attribute { get; private set; }

            #endregion

            #region Constructor

            public FieldAttribute(Assembly assembly, Type c, FieldInfo fieldInfo, Attribute attribute) {
                this.Assembly = assembly;
                this.Class = c;
                this.FieldInfo = fieldInfo;
                this.Attribute = attribute;
            }

            #endregion

            #region Overrides

            public override string ToString() {
                return $"{Class.FullName}.{FieldInfo.Name} (static:{FieldInfo.IsStatic})";
            }

            #endregion
        }

        public class PropertyAttribute {
            #region Variables

            public Assembly Assembly { get; private set; }
            public Type Class { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public Attribute Attribute { get; private set; }

            #endregion

            #region Constructor

            public PropertyAttribute(Assembly assembly, Type c, PropertyInfo propertyInfo, Attribute attribute) {
                this.Assembly = assembly;
                this.Class = c;
                this.PropertyInfo = propertyInfo;
                this.Attribute = attribute;
            }

            #endregion

            #region Overrides

            public override string ToString() {
                return $"{Class.FullName}.{PropertyInfo.Name} (read:{PropertyInfo.CanRead} --- write:{PropertyInfo.CanWrite})";
            }

            #endregion
        }

        public class MethodAttribute {
            #region Variables

            public Assembly Assembly { get; private set; }
            public Type Class { get; private set; }
            public MethodInfo MethodInfo { get; private set; }
            public Attribute Attribute { get; private set; }

            #endregion

            #region Constructor

            public MethodAttribute(Assembly assembly, Type c, MethodInfo methodInfo, Attribute attribute) {
                this.Assembly = assembly;
                this.Class = c;
                this.MethodInfo = methodInfo;
                this.Attribute = attribute;
            }

            #endregion

            #region Overrides

            public override string ToString() {
                return $"{Class.FullName}.{MethodInfo.Name} (static:{MethodInfo.IsStatic})";
            }

            #endregion
        }

        #region Variables

        private const string TAG = "[Toolkit.AttributeStorage] - ";
        private static readonly Type BASE_ATTRIBUTE = typeof(Attribute);
        private static readonly Type UNITY_BASE_ATTRIBUTE = typeof(UnityEngine.PropertyAttribute);
        private static Dictionary<Type, Storage> lookupTable = new Dictionary<Type, Storage>();
        private static bool isInitialized = false;
        private static Promise initializePromise = new Promise(false).AllowMultiThreadResponse();
        private static int waitIndex = 0;

        private static string[] ASSEMBLY_TO_IGNORE = {
            "System",
            "Unity.",
            "UnityEngine",
            "UnityEditor",
            "Mono.",
            "Microsoft.",
            "JetBrains.",
            "mscorlib",
            "netstandard",
            "Bee.",
            "nunit.",
            "unityplastic",
            "PlayerBuildProgramLibrary.",
            "ReportGeneratorMerged",
            "log4net",
            "I18N"
        };

        /// <summary>
        /// Used to overwrite the ignore list
        /// </summary>
        private static string[] ASSEMBLIES_TO_INCLUDE = {

        };

        private static Type[] ATTRIBUTES_TO_REMOVE = {

        };

        private static List<Assembly> IGNORED_ASSEMBLIES = new List<Assembly> ();

        #endregion

        #region Properties

        public static bool IsInitialized => isInitialized;
        public static IEnumerable<Storage> GetAll() => lookupTable.Values;

        #endregion

        #region Init

        public static void Reinitialize() {
            if(!isInitialized)
                return;
            lookupTable.Clear();
            isInitialized = false;
            initializePromise = new Promise().AllowMultiThreadResponse();
#if UNITY_WEBGL
           Debug.LogError(TAG + "Currently NOT SUPPORTED ON WEBGL");
#else
            var job = Threading.Job.Run(LoadAll);
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        private static void Initialize() {
#if UNITY_WEBGL
           Debug.LogError(TAG + "Currently NOT SUPPORTED ON WEBGL");
#else
            var job = Threading.Job.Run(LoadAll);
#endif
        }

        private static void Validate(bool waitUntilInitialized = false) {
            if(!isInitialized) {
                if(!waitUntilInitialized) {
                    Debug.LogWarning(TAG + "Attempting to access attribute storage before fully initialized");
                }
                else {
#if UNITY_WEBGL
                    Debug.LogError(TAG + "Currently NOT SUPPORTED ON WEBGL");
#else
                    while(!isInitialized && waitIndex++ < 100) {
                        System.Threading.Thread.Sleep(1);
                    }
                    if(!isInitialized) {
                        Debug.LogError(TAG + "Slow machine? or failed to properly load assemblies");
                    }
#endif
                }
            }
        }

        public static async Task WaitUntilInitialized() {
            if(isInitialized)
                return;
            await initializePromise;
        }

        #endregion

        #region Load

        private static bool TryGetStorage(Type type, out Storage storage) {
            if(!lookupTable.TryGetValue(type, out storage)) {
                storage = new Storage(type);
                lookupTable.Add(type, storage);

                if(type.BaseType == BASE_ATTRIBUTE || type.BaseType == UNITY_BASE_ATTRIBUTE) {
                    // Do nothing
                }
                else {
                    if(TryGetStorage(type.BaseType, out var parent)) {
                        parent.Add(storage);
                    }
                }
            }

            return true;
        }

        private static bool ShouldIgnore(Assembly assembly) {
            var fullName = assembly.FullName;
            bool shouldBeIgnored = false;
            for(int i = ASSEMBLY_TO_IGNORE.Length - 1; i >= 0; i--) {
                if(fullName.StartsWith(ASSEMBLY_TO_IGNORE[i])) {
                    shouldBeIgnored = true;
                    break;
                }
            }
            if(shouldBeIgnored) {
                for(int i = ASSEMBLIES_TO_INCLUDE.Length - 1; i >= 0; i--) {
                    if(fullName.StartsWith(ASSEMBLY_TO_IGNORE[i])) {
                        shouldBeIgnored = false;
                        break;
                    }
                }
            }
            return shouldBeIgnored;
        }

        private static void LoadAll() {
            try {
                using(PerformanceUtility.CreateStopwatchScope(typeof(AttributeStorage), "Total")) {
                    foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                        if(ShouldIgnore(assembly))
                            IGNORED_ASSEMBLIES.Add(assembly);
                        else
                            Load(assembly);
                    }
                }

                isInitialized = true;
                initializePromise?.Complete();
            }
            catch { };
            //Debug.Log(TAG + $"{lookupTable.Count} different attributes cached\n{string.Join("\n", lookupTable.Keys.Select(x=>x.FullName))}");
        }

        private static void Load(Assembly assembly) {
            try {
                // Debug.Log("Checking assembly: " + assembly.FullName);
                foreach(var type in assembly.GetTypes())
                    Load(assembly, type);
            }
            catch { };
        }

        private static void Load(Assembly assembly, Type type) {
            try {
                foreach(var attribute in type.GetCustomAttributes<Attribute>()) {
                    if(TryGetStorage(attribute.GetType(), out var storage))
                        storage.Add(new ClassAttribute(assembly, type, attribute));
                }
            }
            catch { };
            try {
                foreach(var field in type.GetRuntimeFields()) {
                    Load(assembly, type, field);
                }
            }
            catch { };
            try {
                foreach(var property in type.GetRuntimeProperties()) {
                    Load(assembly, type, property);
                }
            }
            catch { };
            try {
                foreach(var method in type.GetRuntimeMethods()) {
                    Load(assembly, type, method);
                }
            }
            catch { };
        }

        private static void Load(Assembly assembly, Type type, FieldInfo field) {
            try {
                foreach(var attribute in field.GetCustomAttributes<Attribute>(true)) {
                    if(TryGetStorage(attribute.GetType(), out var storage))
                        storage.Add(new FieldAttribute(assembly, type, field, attribute));
                }
            }
            catch { };
        }

        private static void Load(Assembly assembly, Type type, PropertyInfo property) {
            try {
                foreach(var attribute in property.GetCustomAttributes<Attribute>(true)) {
                    if(TryGetStorage(attribute.GetType(), out var storage))
                        storage.Add(new PropertyAttribute(assembly, type, property, attribute));
                }
            }
            catch { };
        }

        private static void Load(Assembly assembly, Type type, MethodInfo method) {
            try {
                foreach(var attribute in method.GetCustomAttributes<Attribute>(true)) {
                    if(TryGetStorage(attribute.GetType(), out var storage))
                        storage.Add(new MethodAttribute(assembly, type, method, attribute));
                }
            }
            catch { };
        }

        #endregion

        #region Try Find

        public static bool TryFind<Attribute>(out Storage storage, bool waitUntilInitialized = false) where Attribute : System.Attribute {
            Validate(waitUntilInitialized);
            return lookupTable.TryGetValue(typeof(Attribute), out storage);
        }

        public static bool TryFind(Type type, out Storage storage, bool waitUntilInitialized = false) {
            Validate(waitUntilInitialized);
            return lookupTable.TryGetValue(type, out storage);
        }

        #endregion
    }
}
